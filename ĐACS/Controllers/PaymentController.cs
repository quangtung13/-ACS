
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VNPay;
using System.Configuration;
using ĐACS.Models;
using Newtonsoft.Json.Linq;
using MoMo;
using System.Security.Cryptography.Xml;

namespace ĐACS.Controllers
{
    public class PaymentController : Controller
    {
        DBContext data = new DBContext();
        public ActionResult paymentMomo()
        {
            var dataObject = TempData["data_payment"] as dynamic;
            int tongTien = int.Parse(dataObject.ttg + "");
            


            //return view thanh toan momo
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "test";
            string returnUrl = "https://localhost:44310/Payment/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            int amount = tongTien;
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";
            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount+"" },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        public ActionResult ConfirmPaymentClient()
        {
            SANPHAM sp = new SANPHAM();
            var dataObject = TempData["data_payment"] as dynamic;
            var response = Request.QueryString;
            string resultCode = response.Get("errorCode");
            string orderId = response.Get("orderId");          
            if (resultCode != null && int.Parse(resultCode)  != 0)
            {
                decimal maHD = dataObject.maHD;

                var hoaDon = data.HOADON.Where(h => h.MAHD == maHD).First();
                var chiTietHoaDons = data.CHITIETHOADON.Where(c => c.MAHD == maHD).ToList();
                foreach (var chiTietHoaDon in chiTietHoaDons)
                {
                    sp = data.SANPHAM.Single(n => n.MASP == chiTietHoaDon.MASP);
                    sp.SLT += chiTietHoaDon.SOLUONG;
                    data.CHITIETHOADON.Remove(chiTietHoaDon);
                    
                }

                data.HOADON.Remove(hoaDon);
                data.SaveChanges();
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId;
                return View("PaymentError");
            }
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        //Ngân hàng
        public ActionResult paymentBankCard()
        {

            var dataObject = TempData["data_payment"] as dynamic;

            int tongTien = int.Parse(dataObject.ttg + "");

            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", tongTien * 100 + ""); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", "127.0.0.1"); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult Confirm()
        {
            var dataObject = TempData["data_payment"] as dynamic;
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();
                SANPHAM sp = new SANPHAM();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        
                        return RedirectToAction("XacNhanDonHang", "GioHang");
                    }
                    else
                    {
                        decimal maHD = dataObject.maHD;
                        
                        var hoaDon = data.HOADON.Where(h => h.MAHD == maHD).FirstOrDefault();
                        var chiTietHoaDons = data.CHITIETHOADON.Where(c => c.MAHD == maHD).ToList();
                        foreach (var chiTietHoaDon in chiTietHoaDons)
                        {
                            sp = data.SANPHAM.Single(n => n.MASP == chiTietHoaDon.MASP);
                            sp.SLT += chiTietHoaDon.SOLUONG;
                            data.CHITIETHOADON.Remove(chiTietHoaDon);
                            data.SaveChanges();
                        }

                        data.HOADON.Remove(hoaDon);
                        data.SaveChanges();

                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        return View("PaymentError");
                    }
                }
                else
                {
                    decimal maHD = dataObject.maHD;
                    
                    var hoaDon = data.HOADON.Where(h => h.MAHD == maHD).FirstOrDefault();
                    var chiTietHoaDons = data.CHITIETHOADON.Where(c => c.MAHD == maHD).ToList();
                    foreach (var chiTietHoaDon in chiTietHoaDons)
                    {
                        data.CHITIETHOADON.Remove(chiTietHoaDon);
                        data.SaveChanges();
                    }

                    data.HOADON.Remove(hoaDon);
                    data.SaveChanges();

                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                    return View("PaymentError");
                }
            }
            ViewBag.Message = "Can ot found router";
            return View("PaymentError");
        }

        public ActionResult PaymentError()
        {
            return View();
        }
    }
}
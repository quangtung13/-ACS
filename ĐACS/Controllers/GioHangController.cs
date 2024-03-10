
using ĐACS.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace ĐACS.Controllers
{
    public class GioHangController : Controller
    {
        DBContext data = new DBContext();
        // GET: GioHang
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int id, string strURL)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.Find(n => n.MASP == id);
            if (sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGioHang.Add(sanpham);
            }
            else
            {
                sanpham.isoluong++;
            }
            return RedirectToAction("Home", "Store");
        }
        private int TongSoLuong()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tsl = lstGioHang.Sum(n => n.isoluong);
            }
            return tsl;
        }
        private int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tsl = lstGioHang.Count;
            }
            return tsl;
        }
        private decimal TongTien()
        {
            decimal tt = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tt = lstGioHang.Sum(n => n.dthanhtien);
            }
            return tt;
        }
        // [Authorize]
        public ActionResult GioHang()
        {
            if (Session["GioHang"] != null)
            {
                ViewBag.ThongBao = "Không có sản phẩm nào";
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            Session["Tongslsp"] = TongSoLuong();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }
        public ActionResult XoaGioHang(int id)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.MASP == id);
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.MASP == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGioHang(int id, FormCollection collection)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.MASP == id);
            if (sanpham != null)
            {
                sanpham.isoluong = int.Parse(collection["txtSoLg"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("GioHang");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TENDANGNHAP"] == null || Session["TENDANGNHAP"].ToString() == null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Store");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGioHang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            int paymentId = int.Parse(collection["payment"]);
            double tongTien = 0;

            if (paymentId == 1 || paymentId == 2 || paymentId == 3)
            {

                NGUOIDUNG kh = (NGUOIDUNG)Session["TENDANGNHAP"];
                SANPHAM s = new SANPHAM();
                HOADON hd = new HOADON();
                HINHTHUCTT tt = new HINHTHUCTT();
                List<GioHang> gh = LayGioHang();

                hd.TENDANGNHAP = kh.TENDANGNHAP;
                hd.THOIGIANDAT = DateTime.Now;
                hd.THOIGIANGIAODUKIEN = DateTime.Now.AddDays(3);
                hd.DIACHI = collection["DIACHI"];
                hd.SDTNHAN = kh.SDT;
                hd.MAHTTT = paymentId;
                data.HOADON.Add(hd);
                hd.MAHD = hd.MAHD;
                int ttg = 0;
                foreach (var item in gh)
                {
                    CHITIETHOADON cthd = new CHITIETHOADON();
                    cthd.SOLUONG = item.isoluong;
                    cthd.MAHD = hd.MAHD;
                    cthd.MASP = item.MASP;
                    cthd.GIA = (decimal?)item.GIA;
                    s = data.SANPHAM.Single(n => n.MASP == item.MASP);
                    s.SLT -= cthd.SOLUONG;
                    ttg += Convert.ToInt32(item.GIA*item.isoluong);
                    data.CHITIETHOADON.Add(cthd);                  
                }
                hd.THANHTIEN = ttg;
                data.HOADON.Add(hd);
                data.SaveChanges();
                Session["GioHang"] = null;

                var maHD = hd.MAHD;

                var dataPaymentObject = new { maHD, ttg };

                

               if (paymentId == 1)
                {
                    return RedirectToAction("XacnhanDonhang", "GioHang");
                }
                else if (paymentId == 2)
                {
                    TempData["data_payment"] = dataPaymentObject;
                    return RedirectToAction("paymentBankCard", "Payment");
                }
                else if (paymentId == 3)
                {
                    TempData["data_payment"] = dataPaymentObject;
                    return RedirectToAction("paymentMomo", "Payment");
                }

            }
             return RedirectToAction("Home", "Store");

        }
    

        public ActionResult XacnhanDonhang()
        {
            return View();
        }
    }
}
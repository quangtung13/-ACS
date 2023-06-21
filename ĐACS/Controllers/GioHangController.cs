
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
            DONHANG dh = new DONHANG();
            NGUOIDUNG kh = (NGUOIDUNG)Session["TENDANGNHAP"];
            SANPHAM s = new SANPHAM();
            HOADON hd = new HOADON();
            List<GioHang> gh = LayGioHang();

            dh.TENDANGNHAP = kh.TENDANGNHAP;
            dh.THOIGIANDAT = DateTime.Now;
            dh.THOIGIANGIAODUKIEN = DateTime.Now.AddDays(3);
            data.DONHANG.Add(dh);
            data.SaveChanges();
            hd.NGAYLAPHD = DateTime.Now;
            hd.MADH = dh.MADH;
            int tt = 0;
            foreach (var item in gh)
            {
                CHITIETDONHANG ctdh = new CHITIETDONHANG();
                ctdh.SOLUONG = item.isoluong;
                ctdh.MADH = dh.MADH;
                ctdh.MASP = item.MASP;
                ctdh.GIASP = (decimal?)item.GIA;
                s = data.SANPHAM.Single(n => n.MASP == item.MASP);
                s.SLT -= ctdh.SOLUONG;
                tt += Convert.ToInt32(item.GIA);
                data.CHITIETDONHANG.Add(ctdh);
                data.SaveChanges();
            }
            hd.THANHTIEN = tt;
            data.HOADON.Add(hd);
            data.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("Home", "Store");
        }

        public ActionResult XacnhanDonhang()
        {
            return View();
        }
    }
}
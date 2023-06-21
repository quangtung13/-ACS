using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ĐACS.Models;

using PagedList;

namespace ĐACS.Controllers
{
    public class AdminController : Controller
    {
        DBContext data = new DBContext();

        public ActionResult Index(int? page)
        {

            if (page == null) page = 1;
            var item = (from hh in data.SANPHAM select hh).Where(m => m.SLT > 0).OrderBy(m => m.MASP); ;
            int pageSize = 4;
            int pageNum = page ?? 1;
            return View(item.ToPagedList(pageNum, pageSize));
        }
        //[Authorize]
        public ActionResult ThemSP()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemSP(FormCollection collection, SANPHAM sp)
        {

            var E_tensp = collection["TENSP"];

            var E_giaban = Convert.ToDecimal(collection["GIA"]);
            var E_mota = collection["MOTA"];
            var E_hinh = collection["HINH"];
            var E_soluongton = Convert.ToInt32(collection["SLT"]);

            if (string.IsNullOrEmpty(E_tensp))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                sp.TENSP = E_tensp.ToString();
                sp.HINH = E_hinh.ToString();
                sp.GIA = E_giaban;
                sp.SLT = E_soluongton;

                data.SANPHAM.Add(sp);
                data.SaveChanges();

                return RedirectToAction("Index");
            }
            return this.ThemSP();
        }
        public ActionResult SuaSP(int id)
        {
            var S_sanpham = data.SANPHAM.First(m => m.MASP == id);
            return View(S_sanpham);
        }
        [HttpPost]
        public ActionResult SuaSP(int id, FormCollection collection)
        {
            var E_sanpham = data.SANPHAM.First(m => m.MASP == id);

            var E_tensp = collection["TENSP"];

            var E_giaban = Convert.ToDecimal(collection["GIA"]);
            var E_mota = collection["MOTA"];
            var E_hinh = collection["HINH"];
            var E_soluongton = Convert.ToInt32(collection["SLT"]);
            E_sanpham.MASP = id;
            if (string.IsNullOrEmpty(E_tensp))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {

                E_sanpham.TENSP = E_tensp;
                E_sanpham.HINH = E_hinh;
                E_sanpham.GIA = E_giaban;
                E_sanpham.MOTA = E_mota;
                E_sanpham.SLT = E_soluongton;
                UpdateModel(E_sanpham);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return this.SuaSP(id);

        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
        public ActionResult XoaSP(int id)
        {
            var D_sp = data.SANPHAM.First(m => m.MASP == id);
            return View(D_sp);
        }
        [HttpPost]
        public ActionResult XoaSP(int id, FormCollection collection)
        {
            var D_sp = data.SANPHAM.Where(m => m.MASP == id).First();
            data.SANPHAM.Remove(D_sp);
            data.SaveChanges();
            return RedirectToAction("Index");

        }
        //private double TongDoanhThu()
        //{
        //    double dt = 0;
        //    List<HOADON> tdt = Session["HOADON"] as List<HOADON>;
        //    if (tdt != null)
        //    {
        //        dt = Convert.ToDouble(tdt.Sum(n => n.THANHTIEN));
        //    }
        //    return dt;
        //}
        //public ActionResult DoanhThu(FormCollection collection)
        //{
        //    int dt = 0;
        //    List< HOADON> hd = new List<HOADON>();
        //    foreach (HOADON hoaDon in hd)
        //    {
        //        dt += Convert.ToInt32(hoaDon.THANHTIEN);
        //    }
        //    return PartialView(dt);
        //}
    }
}
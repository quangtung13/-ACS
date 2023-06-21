using ĐACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ĐACS.Controllers
{
    public class DonHangController : Controller
    {
        DBContext data = new DBContext();
        //GET: DonHang

        public ActionResult XemDonHang(int? id)
        {
            var donhang = (from dh in data.DONHANG select dh);
            return View(donhang);
        }
        public ActionResult XemChiTietDonHang(int id)
        {
            var ctdonhang = (from dh in data.CHITIETDONHANG select dh).Where(m => m.MADH == id).First();
            return View(ctdonhang);
        }
        //public ActionResult HuyDonHang(string id, FormCollection collection)
        //{
        //    var D_ctdh = data.CHITIETHOADON.Where(m => m.MAHD == id).First();
        //    data.CHITIETHOADON.Remove(D_ctdh);
        //    data.SaveChanges();
        //    return RedirectToAction("Index");

        //}
    }
}
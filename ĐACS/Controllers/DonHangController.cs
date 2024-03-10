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

            var donhang = (from dh in data.HOADON select dh).OrderBy(m => m.MAHD).ToList();
            return View(donhang);
        }
        public ActionResult XemChiTietDonHang(int id) { 
            var ctdonhang = (from dh in data.CHITIETHOADON select dh).Where(m => m.MAHD == id).ToList();
            ViewData["id"] = id;
            return View(ctdonhang);
        }
        public ActionResult HuyDonHang(decimal id)
        {
           var D_ctdh = data.CHITIETHOADON.Where(m => m.MAHD == id).ToList();
            foreach(var ctdh in D_ctdh)
            {
                data.CHITIETHOADON.Remove(ctdh);
            }
            var hd = data.HOADON.Where(h => h.MAHD == id).First();
            data.HOADON.Remove(hd);
            data.SaveChanges();
           return RedirectToAction("XemDonHang");
        }
    }
}
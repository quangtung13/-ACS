using ĐACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ĐACS.Controllers
{
    public class DanhMucController : Controller
    {

        DBContext data = new DBContext();
        // GET: Danhmuc
        public ActionResult DanhMuc()
        {
            var listdm = from dm in data.LOAISP select dm;
            return PartialView(listdm);
        }
        public ActionResult DanhMucAdmin()
        {
            var listdm = from dm in data.LOAISP select dm;
            return View(listdm);
        }
        public ActionResult DanhMucSP(int id)
        {

            var list = data.SANPHAM.Where(m => m.MALOAI == id).ToList();

            return View(list);
        }
        public ActionResult ThemDM()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemDM(FormCollection collection, LOAISP dm)
        {

            var S_TenDM = collection["TenDM"];
            if (string.IsNullOrEmpty(S_TenDM))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                dm.TENLOAI = S_TenDM.ToString();
                data.LOAISP.Add(dm);
                data.SaveChanges();
                return RedirectToAction("DanhMucAdmin");
            }
            return this.ThemDM();
        }
        public ActionResult XoaDM(int id, FormCollection collection)
        {
            var D_dm = data.LOAISP.Where(m => m.MALOAI == id).First();
            data.LOAISP.Remove(D_dm);
            data.SaveChanges();
            return RedirectToAction("DanhMucAdmin");

        }
    }
}
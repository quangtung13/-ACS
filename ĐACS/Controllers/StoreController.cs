using ĐACS.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Web.UI;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace ĐACS.Controllers
{
    public class StoreController : Controller
    {
        DBContext data = new DBContext();
        // GET: Store
        public ActionResult Home(int? page)
        {
            if (page == null) page = 1;
            var item = (from hh in data.SANPHAM select hh).Where(m => m.SLT > 0).OrderBy(m => m.MASP); ;
            int pageSize = 4;
            int pageNum = page ?? 1;
            return View(item.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Search(int? page, string search)
        {
            ViewBag.Keyword = search;
            if (page == null)
                page = 1;

            var items = data.SANPHAM.Where(hh => hh.SLT > 0 && hh.TENSP.ToUpper().Contains(search.ToUpper()))
                                    .OrderBy(hh => hh.MASP);

            int pageSize = 8;
            int pageNumber = page ?? 1;
            return View(items.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ThongTinSP(int? id)
        {
            var item = data.SANPHAM.Where(m => m.MASP == id).First();
            return View(item);
        }
    }
}
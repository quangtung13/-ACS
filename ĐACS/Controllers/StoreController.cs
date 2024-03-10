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
        public ActionResult Home(int? page, bool? lowPrHight)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNum = page ?? 1;

            if(lowPrHight == null)
            {
                var listOrder = (from hh in data.SANPHAM select hh).Where(m => m.SLT > 0).OrderBy(m => m.MASP).ToList();
                return View("Home", listOrder.ToPagedList(pageNum, pageSize));
            }
            if (lowPrHight == false)
            {
                var listOrder = (from hh in data.SANPHAM select hh).Where(m => m.SLT > 0).OrderBy(m => m.GIA).ToList();
                return View("Home", listOrder.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var listOrder = (from hh in data.SANPHAM select hh).Where(m => m.SLT > 0).OrderByDescending(m => m.GIA).ToList();

                return View("Home", listOrder.ToPagedList(pageNum, pageSize));
            }
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
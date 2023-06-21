using ĐACS.Models;
using ĐACS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace ĐACS.Controllers
{
    public class UserController : Controller
    {
        DBContext data = new DBContext();
        // GET: NguoiDung
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, NGUOIDUNG nd)
        {
            var regexPassword = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            var hoten = collection["TENND"];
            var tendangnhap = collection["TENDANGNHAP"];
            var matkhau = collection["MATKHAU"];
            var MatkhauXacNhan = collection["MatkhauXacNhan"];
            var email = collection["MAIL"];
            var diachi = collection["DIACHI"];
            var dienthoai = collection["SDT"];
            var ngaysinh = string.Format("{0:MM/dd/yyyy}", collection["NGAYSINH"]);
            if (tendangnhap == "admin")
            {
                //ViewData["Admin"] = "Không thể tạo tài ndoản admin!";
                return RedirectToAction("DangKy", "User");
            }
            else
            {
                if (!regexPassword.IsMatch(matkhau))
                {
                    ViewData["MATKHAU"] = "Mật khẩu không hợp lệ!";
                }
                else
                {
                    if (String.IsNullOrEmpty(MatkhauXacNhan))
                    {
                        ViewData["NhapMKXN"] = "Phải nhập mật khẩu xác nhận!";
                    }
                    else
                    {
                        if (!matkhau.Equals(MatkhauXacNhan))
                        {
                            ViewData["MatkhauGiongNhau"] = "Mật khẩu và mật khầu xác nhận phài giống nhau";
                        }
                        else
                        {
                            nd.TENND = hoten;
                            nd.TENDANGNHAP = tendangnhap;
                            nd.MATKHAU = matkhau;
                            nd.MAIL = email;
                            nd.DIACHI = diachi;
                            nd.SDT = dienthoai;
                            nd.NGAYSINH = DateTime.Parse(ngaysinh);
                            data.NGUOIDUNG.Add(nd);
                            data.SaveChanges();
                            return RedirectToAction("DangNhap");
                        }
                    }

                }
                return this.DangKy();
            }
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendangnhap = collection["TENDANGNHAP"];
            var matkhau = collection["MATKHAU"];
            NGUOIDUNG nd = data.NGUOIDUNG.SingleOrDefault(n => n.TENDANGNHAP == tendangnhap && n.MATKHAU == matkhau);
            if (nd != null)
            {
                if (tendangnhap == "admin")
                {
                    Session["TENDANGNHAP"] = nd;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    Session["TENDANGNHAP"] = nd;
                    return RedirectToAction("Home", "Store");
                }

            }
            else
            {
                ViewData["login"] = "Tên Đăng Nhập hoặc mật ndẩu sai!";
                return RedirectToAction("DangNhap", "NGUOIDUNG");
            }

        }
        public ActionResult LogOff()
        {
            Session["TENDANGNHAP"] = null;
            Session.Abandon();
            return RedirectToAction("Home", "Store");
        }
    }
}
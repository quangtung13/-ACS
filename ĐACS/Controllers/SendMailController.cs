using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using MimeKit;
using MailKit.Net.Smtp;
using System.Web.UI.WebControls;

namespace ĐACS.Controllers
{
    public class SendMailController : Controller
    {
        string[] str2 = new string[30];

        [HttpPost]//phương thức post
        public ActionResult LienHe(FormCollection lienhe)
        {
            try
            {
            var file = Request.Files.Get("imageFile");
            string TENND = lienhe["TENND"];
            string MAIL = lienhe["MAIL"];
            string DIACHI = lienhe["DIACHI"];
            string SDT = lienhe["SDT"];
            string NOIDUNG = lienhe["NOIDUNG"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Quang Tùng", "qtung101@gmail.com"));
            message.To.Add(new MailboxAddress("Recipient Name", "qtung101@gmail.com"));
            message.Subject = "Liên hệ khách hàng";

            var builder = new BodyBuilder();
            builder.TextBody = $"Tên khách hàng: {TENND}\n";
            builder.TextBody += $"Email: {MAIL}\n";
            builder.TextBody += $"Địa chỉ: {DIACHI}\n";
            builder.TextBody += $"Số điện thoại: {SDT}\n";
            builder.TextBody += $"Nội dung: {NOIDUNG}\n";


            if (file != null && file.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.InputStream.CopyTo(memoryStream);
                    var imageAttachment = builder.Attachments.Add("image.jpg", memoryStream.ToArray());
                    imageAttachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Vui lòng chọn một file hình ảnh!";
                return View();
            }

            message.Body = builder.ToMessageBody();


            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("qtung101@gmail.com", "wyzs qyqp yijc batw");
                client.Send(message);
                client.Disconnect(true);
            }
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
           

            return View(lienhe);
        }

        

        public ActionResult SendMail()
        {

            return View();
        }
    }
}
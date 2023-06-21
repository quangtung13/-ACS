using ĐACS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ĐACS.Models
{
    public class GioHang
    {
        DBContext data = new DBContext();
        public int MASP { get; set; }
        [Display(Name = "tên sản phẩm")]
        public string TENSP { get; set; }
        [Display(Name = "ảnh bìa")]
        public string HINH { get; set; }
        [Display(Name = "giá bán")]
        public decimal GIA { get; set; }
        [Display(Name = "số lượng")]
        public int isoluong { get; set; }
        [Display(Name = "thành tiền")]
        public decimal dthanhtien
        {
            get { return isoluong * GIA; }
        }
        public GioHang(int id)
        {
            MASP = id;
            SANPHAM sp = data.SANPHAM.Single(n => n.MASP == MASP);
            TENSP = sp.TENSP;
            HINH = sp.HINH;
            GIA = decimal.Parse(sp.GIA.ToString());
            isoluong = 1;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThietBiDienTu6.Models
{
    public class Cart
    {
        WebDienTuEntities da = new WebDienTuEntities();
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get { return UnitPrice * Quantity; } }
        public Cart(int id)
        {
            SANPHAM sp = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
            ProductID = sp.MaSP;
            ProductName = sp.TenSP;
            UnitPrice = (decimal)sp.Giaban;
            Quantity = 1;
        }

    }
}
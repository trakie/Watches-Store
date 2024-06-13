using Watches.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Watches.Controllers
{
    public class ListCartController : Controller
    {
        WatchesEntities da = new WatchesEntities();
        // GET: ListCart
        private List<CartModel> GetListCarts()
        {
            List<CartModel> carts = Session["CartModel"] as List<CartModel>; //ds cacs sp trong gio hang
            if (carts == null)//chua co sp nao trong gio hang
            {
                carts = new List<CartModel>();
                Session["CartModel"] = carts;
            }
            return carts;
        }
        public ActionResult ListCarts()
        {
            List<CartModel> carts = GetListCarts();
            ViewBag.TongSoLuong = TongSL();
            ViewBag.TongTien = TongTien();
            return View(carts);
        }
        public ActionResult AddCart(int id)
        {
            List<CartModel> carts = GetListCarts();
            //lay thong tin sp
            CartModel c = carts.Find(s => s.ProductID == id);
            SANPHAM sp = da.SANPHAMs.First(s=>s.MaSP==id);
          
                if (c == null)
                {
                    c = new CartModel(id);
                    carts.Add(c);
                }
                else
                    c.Quantity++;
            
           
            //Thiet lap thuoc tinh


            return RedirectToAction("ListCarts");
        }
        private int TongSL()
        {
            int SL = 0;
            List<CartModel> carts = Session["CartModel"] as List<CartModel>;
            if (carts != null)
            {
                SL = carts.Sum(c => c.Quantity);
            }
            return SL;
        }
        private double TongTien()
        {
            double Tong = 0;
            List<CartModel> carts = Session["CartModel"] as List<CartModel>;
            if (carts != null)
            {
                Tong = (double)carts.Sum(c => c.Total);
            }
            return Tong;
        }

        // GET: ListCart
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult XoaGioHang(int id)
        {
            List<CartModel> carts = GetListCarts();
            //lay thong tin sp
            CartModel c = carts.Single(s => s.ProductID == id);
            if (c != null)
            {
                carts.RemoveAll(n => n.ProductID == id);
                return RedirectToAction("ListCarts");
            }
            if (carts.Count == 0)
            {
                return RedirectToAction("Index", "User");
            }
            //Thiet lap thuoc tinh


            return RedirectToAction("ListCarts");
        }

        public ActionResult DatHang()
        {
            if (Session["UserID"] == null || Session["UserID"].ToString() == "")
                return RedirectToAction("Login", "User");
            if (Session["CartModel"] == null)
                return RedirectToAction("Index", "User");
            //lay gio hang tu session
            List<CartModel> carts = GetListCarts();
            Session["CartModel"] = carts;
            ViewBag.TongSoLuong = TongSL();
            ViewBag.TongTien = TongTien();
            return View(carts);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            using (TransactionScope tranScope=new TransactionScope() )
            {
                try
                {
                    DONDATHANG o = new DONDATHANG();
                    KHACHHANG c = (KHACHHANG)Session["UserID"];
                    List<CartModel> carts = GetListCarts();
                    //lay thong tin sp
                    o.MaKH = c.MaKH;
                    o.Ngaydat = DateTime.Now;
                   
                   

                    da.DONDATHANGs.Add(o);
                    da.SaveChanges();
                    //Them chi tiet don dat hang
                    foreach (var item in carts)
                    {
                        CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                        ctdh.MaDonHang = o.MaDonHang;
                        ctdh.MaSP = item.ProductID;
                        ctdh.Dongia = item.UnitPrice;
                        ctdh.Soluong = (short)item.Quantity;
                        da.CHITIETDONTHANGs.Add(ctdh);
                    }
                    da.SaveChanges();
                    tranScope.Complete();
                    Session["CartModel"] = null;
                }
                catch (Exception)
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListCarts", "ListCart");
                }
            }

            return RedirectToAction("XacNhanDonHang", "ListCart");
        }
        public ActionResult XacNhanDonHang()
        {

            return View();
        }

    }
}
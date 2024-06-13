using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Watches.Controllers
{
    public class HomeController : Controller
    {
        WatchesEntities da = new WatchesEntities();

        public ActionResult Index(int ?page, string searchString)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            IEnumerable<SANPHAM> ds;
            
            if (!String.IsNullOrEmpty(searchString)) // kiểm tra chuỗi tìm kiếm có rỗng/null hay không
            {
                
                ds = da.SANPHAMs.OrderByDescending(s => s.MaSP).Where(s=>s.TenSP.Contains(searchString)||s.LoaiSP.TenLoaiSP.Contains(searchString)).Take(15).ToList();
            }
            else
                ds = da.SANPHAMs.OrderByDescending(s => s.MaSP).Take(15).ToList();
            
            return View(ds.ToPagedList(pageNum,pageSize));
        }

        //Hiển thị chi tiết sp
        public ActionResult ProductDetail(int id)
        {
           
            SANPHAM p = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
            List<DanhGia> d = da.DanhGias.Where(s => s.MaSP == id).ToList();
            ViewData["DanhGia"] = d;
            return View(p);
        }

        //Lấy đánh giá và bình luận trong View của ProductDetail

        

        //Xem sản phẩm theo danh mục
        public ActionResult ProductType(int id)
        {
            ViewBag.ID = id;
         
            List<SANPHAM> ds = da.SANPHAMs.Where(s => s.MaLoai == id).ToList();
            return View(ds);
        }
    }
}
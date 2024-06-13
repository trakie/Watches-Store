using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Transactions;
using PagedList;
using PagedList.Mvc;

namespace Watches.Controllers
{
    public class UserController : Controller
    {
        WatchesEntities da = new WatchesEntities();
        private bool IsStrongPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUppercase = true;
                else if (char.IsLower(c))
                    hasLowercase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;

                if (hasUppercase && hasLowercase && hasDigit)
                    return true;
            }

            return false;
        }
        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
           
            return RedirectToAction("Index", "Home");
        }
        // GET: User
        
        public ActionResult Index(int ?page, string searchString)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.XinChao = Session["FullName"];
                int pageSize = 5;
                int pageNum = (page ?? 1);
                IEnumerable<SANPHAM> ds;
                if (!String.IsNullOrEmpty(searchString)) // kiểm tra chuỗi tìm kiếm có rỗng/null hay không
                {

                    ds = da.SANPHAMs.OrderByDescending(s => s.MaSP).Where(s => s.TenSP.Contains(searchString) || s.LoaiSP.TenLoaiSP.Contains(searchString)).Take(15).ToList();
                }
                else
                    ds = da.SANPHAMs.OrderByDescending(s => s.MaSP).Take(15).ToList();

                return View(ds.ToPagedList(pageNum, pageSize));
            }
        }
        //public ActionResult Dangnhap(FormCollection collection)
        //{
        //    var tenDN = collection["TenDN"];
        //    var matKhau = collection["MatKhau"];
        //    if (String.IsNullOrEmpty(tenDN))
        //        ViewData["Loi1"] = "Phai nhap ten dang nhap";
        //    else if (String.IsNullOrEmpty(matKhau))
        //        ViewData["Loi2"] = "Phai nhap mat khau";
        //    else
        //    {
        //        KHACHHANG c = da.KHACHHANGs.First(n => n.Taikhoan == tenDN && n.Matkhau == matKhau);
        //        if (c != null)
        //        {
        //            //ViewBag.ThongBao = "Chuc mung dang nhap thanh cong";
        //            Session["TaiKhoan"] = c;
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //            ViewBag.ThongBao = "Ten dang nhap hoac mk khong dung";
        //    }
        //    return View();
        //}
        public ActionResult Login(FormCollection collection)
        {
            if (Session["UserID"] != null)
                return RedirectToAction("Index");
            else
            {
                var tenDN = collection["TenDN"];
                var matKhau = collection["MatKhau"];
                if (tenDN != null && matKhau != null)
                {
                    if (ModelState.IsValid)
                    {


                        if (ValidateUser(tenDN, matKhau))
                        {
                            KHACHHANG k = da.KHACHHANGs.FirstOrDefault(n => n.Taikhoan == tenDN &&
                                                                        n.Matkhau == matKhau);
                            Session["UserID"] = k;
                            FormsAuthentication.SetAuthCookie(k.Taikhoan, false);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "");
                            ViewBag.ThongBao = "Sai tên đăng nhập hoặc mật khẩu";

                        }
                    }
                }    
               
                return View();
            }
           
        }

        private bool ValidateUser(string Email, string Password)
        {

            bool isValid = false;

            using (var db = new WatchesEntities())
            {
                var User = db.KHACHHANGs.FirstOrDefault(u => u.Taikhoan == Email);
                
              
                if (User != null)
                {
                    if (User.Matkhau == Password)
                    {
                        Session["UserID"] = User.Taikhoan;
                        //Session["Email"] = User.Taikhoan;
                        Session["FullName"] = User.HoTen;

                        isValid = true;
                       
                    }
              
                }
               
            }

            return isValid;
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            //if (Session["UserID"] == null)
            //    return RedirectToAction("Index");
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection,KHACHHANG kh)
        {
            //if (Session["UserID"] == null)
            //    return RedirectToAction("Index");
            //else
            //{
                var HoTen = collection["HoTen"];
                var TenDN = collection["TenDN"];
                var MK = collection["MK"];
                var MKLai = collection["MKLai"];
                var DiaChi = collection["DiaChi"];
                var Email = collection["Email"];
                var DienThoai = collection["DienThoai"];
                KHACHHANG k = da.KHACHHANGs.FirstOrDefault(s => s.Taikhoan == TenDN);
                KHACHHANG mail = da.KHACHHANGs.FirstOrDefault(s => s.Email == Email);
                var NgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
                if (String.IsNullOrEmpty(HoTen))
                {
                    ViewData["Loi1"] = "Ho ten khach hang khong duoc de trong";
                }
                else if (String.IsNullOrEmpty(TenDN) || k != null)
                {
                    ViewData["Loi2"] = "Tên đăng nhập không được để trống hoặc trùng";
                }
                else if (String.IsNullOrEmpty(MK)||!IsStrongPassword(MK))
                {
                    ViewData["Loi3"] = "Mật khẩu không được để trống và phải đủ 8 ký tự bao gồm chữ hoa, thường,số";
                }
                else if (String.IsNullOrEmpty(MKLai) || MKLai != MK)
                {
                    ViewData["Loi4"] = "Mật khẩu nhập lại không được để trống và phải giống mật khẩu";
                }
                else if (String.IsNullOrEmpty(DienThoai)|| DienThoai.Length<10|| DienThoai.Length > 10)
                {
                    ViewData["Loi5"] = "Số điện thoại không được để trống và phải đủ 10 ký tự ";
                }
                else if (String.IsNullOrEmpty(Email) || mail != null)
                {
                    ViewData["Loi6"] = "Email không được để trống hoặc trùng ";
                }
                else if (String.IsNullOrEmpty(NgaySinh))
                {
                    ViewData["Loi7"] = "Ngày sinh không được để trống ";
                }
                else if (String.IsNullOrEmpty(DiaChi))
                {
                    ViewData["Loi8"] = "Địa chỉ không được để trống ";
                }
                else
                {
                    kh.HoTen = HoTen;
                    kh.Taikhoan = TenDN;
                    kh.Matkhau = MK;
                    kh.Ngaysinh = DateTime.Parse(NgaySinh);
                    kh.Email = Email;
                    kh.DienthoaiKH = DienThoai;
                    kh.DiachiKH = DiaChi;
                    da.KHACHHANGs.Add(kh);
                    da.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                return View();
            
      
        }
        //Xem chi tiết SP
        public ActionResult ProductDetail(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index");
            SANPHAM p = da.SANPHAMs.FirstOrDefault(s => s.MaSP == id);
            List<DanhGia> d = da.DanhGias.Where(s => s.MaSP == id).ToList();
            ViewData["DanhGia"] = d;
            return View(p);
        }

        //Lấy đánh giá và bình luận trong View của ProductDetail

        [HttpPost]
        public ActionResult BinhLuan(int id, FormCollection collection)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index");
            string link = String.Format("ProductDetail/{0}",
                        id);
            try
            {
                //Tạo mới


                DanhGia dg = new DanhGia();

                //Thiết lập thuộc tính
                KHACHHANG kh = Session["UserID"] as KHACHHANG;
                dg.MaKH = kh.MaKH; //int.Parse(Membership.GetUser().ToString())
                dg.MaSP = id; //gán cứng sp
                dg.DiemDanhGia = int.Parse(collection["DiemSo"]);
                dg.NhanXet = collection["comment"];
                dg.NgayTao = DateTime.Now;

                // Thêm dg vào bảng DanhGia
                da.DanhGias.Add(dg);

                //Cập nhật thay đổi db
                da.SaveChanges();

            }
            catch (Exception)
            {

                return RedirectToAction(link);
            }

            return RedirectToAction(link);
        }

        //Xem sản phẩm theo danh mục
        public ActionResult ProductType(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index");
            ViewData["id"] = id; //lấy id sp để xem đánh giá - chưa lấy được
            List<SANPHAM> ds = da.SANPHAMs.Where(s => s.MaLoai == id).ToList();
            foreach (var item in ds)
                ViewBag.TenLoai = item.LoaiSP.TenLoaiSP;
            return View(ds);
        }
        public ActionResult XemDonHang()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index","Home");
            KHACHHANG k = (KHACHHANG)Session["UserID"];
            List<DONDATHANG> dh = da.DONDATHANGs.Where(s=>s.MaKH==k.MaKH).ToList();
           
            if(dh.Count > 0)
                ViewBag.SoLuongDH = "";
            else
                ViewBag.SoLuongDH = dh.Count;
            return View(dh);
        }
        public ActionResult XemCTDonHang(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Home");
            
            List<CHITIETDONTHANG> dh = da.CHITIETDONTHANGs.Where(s => s.MaDonHang == id).ToList();
            Session["dh"] = dh;

            ViewBag.TongSL = TongSL();
            ViewBag.TongTien = TongTien();
            return View(dh);
        }
        private int TongSL()
        {
            int SL = 0;
            List<CHITIETDONTHANG> carts = Session["dh"] as List<CHITIETDONTHANG>;
            if (carts != null)
            {
                SL = (int)carts.Sum(c => c.Soluong);
            }
            return SL;
        }
        private double TongTien()
        {
            double Tong = 0;
            List<CHITIETDONTHANG> carts = Session["dh"] as List<CHITIETDONTHANG>;
            if (carts != null)
            {
                Tong = (double)carts.Sum(c => c.Soluong*c.Dongia);
            }
            return Tong;
        }
    }
}
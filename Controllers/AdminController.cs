using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Data;
using System.Transactions;
using System.Data.SqlClient;

namespace Watches.Controllers
{
    public class AdminController : Controller
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
        public ActionResult DangNhap(FormCollection collection)
        {
            if (Session["AdminID"] != null)
                return RedirectToAction("Index");
            else
            {
                var tenDN = collection["TenDN"];
                var matKhau = collection["Matkhau"];
                if (tenDN != null && matKhau != null)
                {
                    if (ModelState.IsValid)
                    {

                        if (KTUser(tenDN, matKhau))
                        {
                            Admin k = da.Admins.FirstOrDefault(n => n.UserAdmin == tenDN &&
                                                                        n.PassAdmin == matKhau);

                            FormsAuthentication.SetAuthCookie(k.UserAdmin, false);

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
        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request

            return RedirectToAction("Index", "Home");
        }
        private bool KTUser(string Email, string Password)
        {

            bool isValid = false;
            Admin User = da.Admins.FirstOrDefault(u => u.UserAdmin == Email && u.PassAdmin == Password);
            if (User != null)
            {

                Session["AdminID"] = User.UserAdmin;
                Session["HoTenAd"] = User.HoTen;
                isValid = true;

            }

            return isValid;
        }

        public ActionResult EditOrder(int id)
        {
            try
            {
                if (Session["AdminID"] == null)
                    return RedirectToAction("Index", "Home");
                ViewData["KH"] = new SelectList(da.KHACHHANGs, "MaKH", "HoTen");

                DONDATHANG p = da.DONDATHANGs.FirstOrDefault(s => s.MaDonHang == id);

                return View(p);
            }
            catch (Exception)
            {
                return RedirectToAction("ListOrder");

            }
        }

        //Xử lý cập nhật SP
        [HttpPost]
        public ActionResult EditOrder(int id, FormCollection collection)
        {
            try
            {

                DONDATHANG d = da.DONDATHANGs.First(s => s.MaDonHang == id);

                var tt = Request.Form["tt"];
                var gh = Request.Form["gh"];
                if (tt.Contains("true"))
                    d.Dathanhtoan = true;
                else
                    d.Dathanhtoan = false;

                if (gh.Contains("true"))
                    d.Tinhtranggiaohang = true;
                else
                    d.Tinhtranggiaohang = false;
                var NgayGiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiaoHang"]);
                d.Ngaygiao = DateTime.Parse(NgayGiao);







                da.SaveChanges();

                return RedirectToAction("ListOrder");
            }
            catch
            {
                return View();
            }
        }
        //public ActionResult EditOrderDetail(int id)
        //{
        //    try
        //    {
        //        if (Session["AdminID"] == null)
        //            return RedirectToAction("Index", "Home");
        //        CHITIETDONTHANG p1 = Session["MaDH"] as CHITIETDONTHANG;
        //        ViewData["SP"] = new SelectList(da.SANPHAMs, "MaSP", "TenSP");
        //        CHITIETDONTHANG p = da.CHITIETDONTHANGs.Single(s => s.MaDonHang == p1.MaDonHang&&s.MaSP==id);

        //        return View(p);
        //    }
        //    catch (Exception)
        //    {
        //        return RedirectToAction("ListOrder");

        //    }
        //}
        //// GET: Admin
        //[HttpPost]
        //public ActionResult EditOrderDetail(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        if (Session["AdminID"] == null)
        //            return RedirectToAction("Index", "Home");

        //        CHITIETDONTHANG p1 = Session["MaDH"] as CHITIETDONTHANG;
        //        CHITIETDONTHANG p = da.CHITIETDONTHANGs.Single(s => s.MaSP == id&&s.MaDonHang==p1.MaDonHang);
            
        //        if(p!=null)
        //        {
        //            p.MaSP = int.Parse(collection["SP"]);
        //            p.Soluong=int.Parse(collection["SL"]);
        //            SANPHAM sp = da.SANPHAMs.Single(s => s.MaSP == p.MaSP);
        //            p.Dongia=sp.Giaban* int.Parse(collection["SL"]);
        //            da.SaveChanges();
        //        }    
        //        return View(p);
        //    }
        //    catch (Exception)
        //    {
        //        return RedirectToAction("ListOrder");

        //    }
        //}

        //Xử lý cập nhật SP
       
  
        public ActionResult DeleteOrder(int id, FormCollection collection)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            try
            {
                DONDATHANG c = da.DONDATHANGs.Single(s => s.MaDonHang == id);
                if (c != null)
                {
                    da.DONDATHANGs.Remove(c);
                    da.SaveChanges();
                    return RedirectToAction("ListOrder");
                }
                else
                {
                    return RedirectToAction("ListOrder");
                }
            }
            catch
            {
                return RedirectToAction("ListOrder");
            }
            // TODO: Add delete logic here
         


        }
        public ActionResult DeleteOD(int ProID,int OdID)
        {
            try
            {
                CHITIETDONTHANG c = da.CHITIETDONTHANGs.Single(s => s.MaSP == ProID&&s.MaDonHang==OdID);
                string link = string.Format("ListOrderDetail/{0}", OdID);
                if (c != null)
                {
                    da.CHITIETDONTHANGs.Remove(c);
                    da.SaveChanges();
                    return RedirectToAction(link);
                }
                else
                {
                    return RedirectToAction("ListOrder");
                }
            }
            catch
            {
                return View();
            }

        }

         public ActionResult Index()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            ViewBag.XinChaoAd = Session["HoTenAd"];
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {           
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection,Admin kh)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index");
            else
            {
                var HoTen = collection["HoTen"];
                var TenDN = collection["TenDN"];
                var MK = collection["MK"];
                var MKLai = collection["MKLai"];
               
             
                Admin k = da.Admins.FirstOrDefault(s => s.UserAdmin == TenDN);
             

                if (String.IsNullOrEmpty(HoTen))
                {
                    ViewData["Loi1"] = "Ho ten khong duoc de trong";
                }
                else if (String.IsNullOrEmpty(TenDN) || k != null)
                {
                    ViewData["Loi2"] = "Tên đăng nhập không được để trống hoặc trùng";
                }
                else if (String.IsNullOrEmpty(MK) || !IsStrongPassword(MK))
                {
                    ViewData["Loi3"] = "Mật khẩu không được để trống và phải đủ 8 ký tự bao gồm chữ hoa, thường,số";
                }
                else if (String.IsNullOrEmpty(MKLai) || MKLai != MK)
                {
                    ViewData["Loi4"] = "Mật khẩu nhập lại không được để trống và phải giống mật khẩu";
                }
             
                else
                {
                  
                    kh.HoTen = HoTen;
                    kh.UserAdmin = TenDN;
                    kh.PassAdmin = MK;
                
                    da.Admins.Add(kh);
                    da.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }

        }

        //Xem danh sách sp
        public ActionResult ListProducts()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            IEnumerable<SANPHAM> ds = da.SANPHAMs.OrderByDescending(s => s.MaSP);
            return View(ds);
        }
        public ActionResult ListOrderDetail(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            IEnumerable<CHITIETDONTHANG> ds = da.CHITIETDONTHANGs.Where(s=>s.MaDonHang==id);
           
            foreach (var item in ds)
            {
                Session["MaDH"] = item;
                if (item.DONDATHANG.Dathanhtoan == true)
                    ViewBag.ThanhToan = "Đã thanh toán";
                else
                    ViewBag.ThanhToan = "Chưa thanh toán";
                if (item.DONDATHANG.Tinhtranggiaohang == true)
                    ViewBag.GiaoHang = "Đã giao hàng";
                else
                    ViewBag.GiaoHang = "Chưa giao hàng";
            }
          
            return View(ds);
        }
        public ActionResult ListOrder()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");
            IEnumerable<DONDATHANG> ds = da.DONDATHANGs.OrderByDescending(s => s.MaDonHang);
             
            return View(ds);
        }
        public ActionResult ListCus()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            IEnumerable<KHACHHANG> ds = da.KHACHHANGs.OrderByDescending(s => s.MaKH);
            return View(ds);
        }
        public ActionResult EditCus(int id)
        {
           
                try
                {
                    if (Session["AdminID"] == null)
                        return RedirectToAction("Index", "Home");
                    KHACHHANG p = da.KHACHHANGs.First(s => s.MaKH == id);
                    //if(p!=null)
                        return View(p);
                    //else
                    //return RedirectToAction("ListCus", "Admin");
                }
                catch (Exception)
                {
                   
                    return RedirectToAction("ListCus", "Admin");
                }
            
        }
        [HttpPost]
        public ActionResult EditCus(int id, FormCollection collection)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    KHACHHANG p = da.KHACHHANGs.First(s => s.MaKH == id);
                    var HoTen = collection["HoTen"];

                    var MK = collection["MK"];
                    var mail = collection["mail"];
                    var DiaChi = collection["DC"];
                    var DienThoai = collection["DT"];
                    var NgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
                    //var TK = collection["TK"];
                    //KHACHHANG k = da.KHACHHANGs.First(s => s.Taikhoan == TK);

                    //if (k != null)
                    //{
                    //    ViewData["Loi2"] = "Tài khoản không được trùng";
                    //}
                    //else
                    //{
                    //    p.Taikhoan = TK;
                        p.HoTen = HoTen;
                        p.Matkhau = MK;
                        p.DiachiKH = DiaChi;
                        p.DienthoaiKH = DienThoai;
                        p.Ngaysinh = DateTime.Parse(NgaySinh);
                        p.Email = mail;
                        da.SaveChanges();
                        tranScope.Complete();
                    //}    
                 
                    return RedirectToAction("ListCus");
                }
                catch (Exception)
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListCus", "Admin");
                }


            }
           
        }
        public ActionResult ListAd()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            IEnumerable<Admin> ds = da.Admins.OrderByDescending(s => s.MaAdmin);
            return View(ds);
        }
        public ActionResult EditAd(int id)
        {
          
            try
                {
                    if (Session["AdminID"] == null)
                        return RedirectToAction("Index", "Home");

                    Admin p = da.Admins.First(s => s.MaAdmin == id);
                    return View(p);
                }
                catch (Exception)
                {
                    
                    return RedirectToAction("ListAd", "Admin");
                }
           
        }
        [HttpPost]
        public ActionResult EditAd(int id, FormCollection collection)
        {


            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {

                    Admin p = da.Admins.First(s => s.MaAdmin == id);
                    var HoTen = collection["HoTen"];

                    var MK = collection["MK"];

                    var TK = collection["TK"];
                    Admin k = da.Admins.FirstOrDefault(s => s.UserAdmin == TK);

                    if (k != null)
                    {
                        ViewData["Loi2"] = "Tài khoản không được trùng";
                    }
                    else
                    {
                        p.HoTen = HoTen;
                        p.PassAdmin = MK;
                        p.UserAdmin = TK;


                        da.SaveChanges();
                        tranScope.Complete();
                    }


                    return RedirectToAction("ListAd");
                }
                catch (Exception)
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListAd", "Admin");
                } 
            }


        }
        // POST: Product/Delete/5
        public ActionResult DeleteAd(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");

            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    Admin p1 = Session["AdminID"] as Admin;
                    
                    Admin p = da.Admins.First(s => s.MaAdmin == id&&s.MaAdmin!=p1.MaAdmin);
                    if (p != null)
                    {
                        da.Admins.Remove(p);
                        da.SaveChanges();
                        tranScope.Complete();
                        return RedirectToAction("ListAd");
                    }
                    else
                    {
                        return RedirectToAction("ListAd");
                    }
                    //Thiet lap thuoc tinh
                }
                catch (Exception)
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListAd");
                } 
            }






        }
       

		//Xử lý thêm SP
		//[HttpPost]
		//public ActionResult ListOrder()
  //      {
  //          if (Session["AdminID"] == null)
  //              return RedirectToAction("Index","Home");
  //          IEnumerable<DONDATHANG> ds = da.DONDATHANGs.OrderByDescending(s => s.MaDonHang);
             
  //          return View(ds);
  //      }



        public ActionResult DeleteCus(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    KHACHHANG c = da.KHACHHANGs.Single(s => s.MaKH == id);
                    if (c != null)
                    {
                        da.KHACHHANGs.Remove(c);
                        da.SaveChanges();
                        tranScope.Complete();
                        return RedirectToAction("ListCus");
                    }
                    else
                    {
                        return RedirectToAction("ListCus");
                    }
                    //Thiet lap thuoc tinh
                }
                catch (Exception)
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListCus");
                }
            }

          



        }
        //Giao diện thêm sp
        public ActionResult CreateProduct()
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index","Home");

            ViewData["NCC"] = new SelectList(da.NHACUNGCAPs, "MaNCC", "TenNCC");
            ViewData["LSP"] = new SelectList(da.LoaiSPs, "MaLoaiSP", "TenLoaiSP");
            return View();

        }

        //Xử lý thêm SP
        [HttpPost]
        public ActionResult CreateProduct(SANPHAM sanpham, FormCollection collection) //FormCollection collection: toàn bộ dữ liệu có trên View
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    //Tạo mới 1 SP
                    SANPHAM p = new SANPHAM();

                    //Thiết lập các thuộc tính cho SP
                    p = sanpham;

                    //Thêm ảnh
                    if (sanpham.ImageUpload != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(sanpham.ImageUpload.FileName); //lấy tên file ko có phần mở rộng
                        string extension = Path.GetExtension(sanpham.ImageUpload.FileName);//lấy phần mở rộng
                        fileName = fileName + extension;
                        p.AnhSP = fileName;
                        sanpham.ImageUpload.SaveAs(Path.Combine(Server.MapPath("../Content/Images/"), fileName));

                    }
                    else
                        p.AnhSP = "loi-hinh-anh.jpg";

                    //Chưa có NCC là LSP
                    //Gán giá trị MaNCC, MaLSP
                    p.MaLoai = int.Parse(collection["LSP"]);
                    p.MaNCC = int.Parse(collection["NCC"]);

                    // Thêm SP vào bảng SanPham
                    da.SANPHAMs.Add(p);

                    //Cập nhật thay đổi db
                    da.SaveChanges();
                    tranScope.Complete();
                    //Hiển thị DSSP
                    return RedirectToAction("ListProducts");

                }
                catch//chưa xử lý bắt lỗi
                {
                    tranScope.Dispose();
                    return View();
                } 
            }
        }

        //Giao diện SP muốn sửa
        public ActionResult EditProduct(int id)
        {     
            if (Session["AdminID"] == null)
               return RedirectToAction("Index", "Home");
            try
            {
                ViewData["NCC"] = new SelectList(da.NHACUNGCAPs, "MaNCC", "TenNCC");
                ViewData["LSP"] = new SelectList(da.LoaiSPs, "MaLoaiSP", "TenLoaiSP");
				ViewData["BH"] = new SelectList(da.SANPHAMs, "MaSP", "ThoiLuongBaoHanh");

				SANPHAM p = da.SANPHAMs.First(s => s.MaSP == id);
                return View(p);
            }
            catch (Exception)
            {

                return RedirectToAction("ListProducts");
            }

        }

        //Xử lý cập nhật SP
        [HttpPost]
        public ActionResult EditProduct(int id,HttpPostedFileBase ImageUpload,FormCollection collection)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    // Xác định SP cần sửa trong da
                    SANPHAM p = da.SANPHAMs.First(s => s.MaSP == id);

                    //Thực hiện cập nhật da
                    p.TenSP = collection["TenSP"];
                    p.Giaban = decimal.Parse(collection["GiaBan"]);
                    p.Soluongton = int.Parse(collection["SL"]);

                    //Chưa có NCC là LSP
                    //Gán giá trị MaNCC, MaLSP
                    p.MaLoai = int.Parse(collection["LSP"]);
                    p.MaNCC = int.Parse(collection["NCC"]);
					p.ThoiLuongBaoHanh = int.Parse(collection["BH"]);



					//Cập nhật ảnh
					if (ImageUpload != null)
                    {

                        string fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName); //lấy tên file ko có phần mở rộng
                        string extension = Path.GetExtension(ImageUpload.FileName);//lấy phần mở rộng


                        fileName = fileName + extension;
                        p.AnhSP = fileName;
                        ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));

                    }
                    //else
                    //    p.AnhSP = "loi-hinh-anh.jpg"; //giữ nguyên hình ảnh ban đầu

                    //Lưu xuống da
                    da.SaveChanges();
                    tranScope.Complete();

                    return RedirectToAction("ListProducts");
                }
                catch//chưa xử lý bắt lỗi
                {
                    tranScope.Dispose();
                    return View();
                } 
            }
        }

        //Giao diện SP muốn xóa
        public ActionResult DeleteProduct(int id, FormCollection collection)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    // Xác định SP cần xóa trong da
                    SANPHAM p = da.SANPHAMs.First(s => s.MaSP == id);
                    //Thực hiện xóa
                    da.SANPHAMs.Remove(p);
                    //Cập nhật thay đổi da
                    da.SaveChanges();
                    tranScope.Complete();
                    return RedirectToAction("ListProducts");
                }
                catch//chưa xử lý bắt lỗi
                {
                    tranScope.Dispose();
                    return RedirectToAction("ListProducts");
                } 
            }
        }
        public ActionResult ThongKe(FormCollection collection)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            try
            {
                List<DONDATHANG> DhTheoThang = da.DONDATHANGs.Where(s => s.Ngaydat.Value.Month == DateTime.Now.Month).ToList();

                List<CHITIETDONTHANG> CTDHTheoThang = da.CHITIETDONTHANGs.Where(s => s.DONDATHANG.Ngaydat.Value.Month == DateTime.Now.Month).ToList();
                List<int> CacMaSPTest = new List<int>();
                int SLSP = 0;
                ViewData["CTDH1"] = CTDHTheoThang;
                foreach (var item in DhTheoThang)
                {
                    SLSP += TongSLSP(item.MaDonHang);
                }
                int TongDH = 0;
                if (DhTheoThang != null)
                    TongDH = DhTheoThang.Count();
                int MaxSLSPThang = 0;
                foreach (var item in CTDHTheoThang)
                {
                    CacMaSPTest.Add(item.MaSP);
                }
                List<int> CacMaSP = CacMaSPTest.Distinct().ToList();
                int MaSPMax = 0;
                foreach (var item in CacMaSP)
                {
                    int sl = (int)CTDHTheoThang.Where(c => c.MaSP == item).Sum(c => c.Soluong);
                    if (sl > MaxSLSPThang)
                    {
                        MaxSLSPThang = sl;
                        MaSPMax = item;
                    }
                }
                int TongSLSPBanChay = (int)da.CHITIETDONTHANGs.Where(c => c.MaSP == MaSPMax).Sum(c => c.Soluong);
                double TongGiaTriThang = (double)da.CHITIETDONTHANGs.Where(s => s.DONDATHANG.Ngaydat.Value.Month == DateTime.Now.Month).Sum(c => c.Soluong * c.Dongia);
                SANPHAM SpChay = da.SANPHAMs.FirstOrDefault(c => c.MaSP == MaSPMax);
                ViewBag.TongGT = TongGiaTriThang;
                //ViewBag.TongDH = TongDH;
                //ViewBag.TongSLSP = SLSP;
                //ViewBag.SPChay = SpChay.TenSP;
                //ViewBag.SLSPChay = TongSLSPBanChay;
                //Thang
                var thang = int.Parse(collection["thang"]);

                List<DONDATHANG> dh1 = da.DONDATHANGs.Where(s => s.Ngaydat.Value.Month == thang).ToList();
                List<CHITIETDONTHANG> CTDH1 = da.CHITIETDONTHANGs.OrderBy(s=>s.SANPHAM.TenSP).Where(s => s.DONDATHANG.Ngaydat.Value.Month == thang&&s.DONDATHANG.Ngaydat.Value.Year==DateTime.Now.Year).ToList();
                List<int> CacMaSPTest1 = new List<int>();
                int SLSP1 = 0;
                if (CTDH1 != null)
                    ViewData["CTDH1"] = CTDH1;
                
                    
                foreach (var item in dh1)
                {
                    SLSP1 += TongSLSP(item.MaDonHang);
                }
                int TongDH1 = 0;
                if (dh1 != null)
                    TongDH1 = dh1.Count();
                int MaxSLSPThang1 = 0;
                foreach (var item in CTDH1)
                {
                    CacMaSPTest1.Add(item.MaSP);
                }
                List<int> CacMaSP1 = CacMaSPTest1.Distinct().ToList();
                int MaSPMax1 = 0;
                foreach (var item in CacMaSP1)
                {
                    int sl1 = (int)CTDH1.Where(c => c.MaSP == item).Sum(c => c.Soluong);
                    if (sl1 > MaxSLSPThang1)
                    {
                        MaxSLSPThang1 = sl1;
                        MaSPMax1 = item;
                    }
                }
                int TongSLSPBanChay1 = (int)da.CHITIETDONTHANGs.Where(c => c.MaSP == MaSPMax1&&c.DONDATHANG.Ngaydat.Value.Month == thang).Sum(c => c.Soluong);
                double TongGiaTriThang1 = (double)da.CHITIETDONTHANGs.Where(s => s.DONDATHANG.Ngaydat.Value.Month == thang).Sum(c => c.Soluong * c.Dongia);
                SANPHAM SpChay1 = da.SANPHAMs.FirstOrDefault(c => c.MaSP == MaSPMax1);
                ViewBag.TongGT1 = TongGiaTriThang1;
                ViewBag.TongDH1 = TongDH1;
                ViewBag.TongSLSP1 = SLSP1;
                ViewBag.SPChay1 = SpChay1.TenSP;
                ViewBag.SLSPChay1 = TongSLSPBanChay1;
                ViewData["CTDH1"] = CTDH1;
            

                return View();
            }
            catch (Exception)
            {

                return View();
            }
        }
        
        private int TongSLSP(int id)
        {
            int SL = 0;
            List<CHITIETDONTHANG> p = da.CHITIETDONTHANGs.Where(s => s.DONDATHANG.MaDonHang == id).ToList();
            if (p != null)
            {
                SL = (int)p.Sum(c => c.Soluong);
            }
            return SL;
        }
        private int TongDH(int id)
        {
            int SL = 0;
            List<DONDATHANG> p = da.DONDATHANGs.Where(s => s.MaDonHang == id).ToList();
            if (p != null)
            {
                SL = p.Count();
            }
            return SL;
        }
		//Xử lý thêm SP
		
		public ActionResult BaoHanh()
		{
			if (Session["AdminID"] == null)
				return RedirectToAction("Index", "Home");

			//ViewData["Product"] = new SelectList(da.SANPHAMs, "MaSP", "TenSP");
			//ViewData["Order"] = new SelectList(da.DONDATHANGs, "MaDonHang", "MaDonHang");
			return View();

		}
		[HttpPost]
		public ActionResult BaoHanh(FormCollection collection)
		{
			if (Session["AdminID"] == null)
				return RedirectToAction("Index", "Home");

			if (collection["Order"] != null)
			{


				int MaDon = int.Parse(collection["Order"]);

				Session["MaDon"] = MaDon;

				DONDATHANG dh = da.DONDATHANGs.FirstOrDefault(s => s.MaDonHang == MaDon);
				List<CHITIETDONTHANG> ctdh=da.CHITIETDONTHANGs.Where(s =>s.MaDonHang==MaDon).ToList();
				ViewData["ListSP"] = ctdh;
				int YearGiao = dh.Ngaygiao.Value.Year;
				int YearNow = DateTime.Now.Year;
				int YearProduct = DateTime.Now.Year;
				List<String> BaoHanh = new List<String>();
				foreach (var sp in ctdh)
				{

					if (sp.SANPHAM.ThoiLuongBaoHanh >= (YearNow - YearGiao))
					{
						BaoHanh.Add("Co");
					}
					else
						BaoHanh.Add("Khong");
				}
				ViewData["BaoHanh"] = BaoHanh;
			}
			if (collection["LyDo"] != null)
			{
				int MaDon = int.Parse(Session["MaDon"].ToString());
				DONDATHANG dh = da.DONDATHANGs.FirstOrDefault(s => s.MaDonHang == MaDon);
				dh.LyDoBaoHanh = collection["LyDo"].ToString();
				da.SaveChanges();

			}
			return View();
		}

        public ActionResult ListNCC()
		{
			if (Session["AdminID"] == null)
				return RedirectToAction("Index", "Home");
			IEnumerable<NHACUNGCAP> ds = da.NHACUNGCAPs.Select(s => s).ToList();
			return View(ds);
		}

        public ActionResult C_NCC()
        {
			if (Session["AdminID"] == null)
				return RedirectToAction("Index", "Home");

			
			return View();
		}

		[HttpPost]
		public ActionResult C_NCC(NHACUNGCAP ncc, FormCollection collection) //FormCollection collection: toàn bộ dữ liệu có trên View
		{
			using (TransactionScope tranScope = new TransactionScope())
			{
				try
				{
					//Tạo mới 1 SP
					NHACUNGCAP nhacungcap = new NHACUNGCAP();

                    //Thiết lập các thuộc tính cho SP
                    nhacungcap = ncc;

					

					// Thêm SP vào bảng SanPham
					da.NHACUNGCAPs.Add(nhacungcap);

					//Cập nhật thay đổi db
					da.SaveChanges();
					tranScope.Complete();
					//Hiển thị DSSP
					return RedirectToAction("ListNCC");

				}
				catch//chưa xử lý bắt lỗi
				{
					tranScope.Dispose();
					return View();
				}
			}
		}

        public ActionResult D_NCC(int id)
        {
			if (Session["AdminID"] == null)
				return RedirectToAction("Index", "Home");
			try
			{
				NHACUNGCAP c = da.NHACUNGCAPs.Single(s => s.MaNCC == id);
				if (c != null)
				{
					da.NHACUNGCAPs.Remove(c);
					da.SaveChanges();
					return RedirectToAction("ListNCC");
				}
				else
				{
					return RedirectToAction("ListNCC");
				}
			}
			catch
			{
				return RedirectToAction("ListNCC");
			}

		}

        public ActionResult E_NCC(int id)
        {
			try
			{
				if (Session["AdminID"] == null)
					return RedirectToAction("DS NCC", "ListNCC");
				NHACUNGCAP p = da.NHACUNGCAPs.First(s => s.MaNCC == id);

				return View(p);
			}
			catch (Exception)
			{
				return RedirectToAction("ListNCC");

			}

		}
		[HttpPost]
		public ActionResult E_NCC(NHACUNGCAP ncc,int id,FormCollection collection)
		{
			using (TransactionScope tranScope = new TransactionScope())
			{
				try
				{
					// Xác định SP cần sửa trong da
					NHACUNGCAP p = da.NHACUNGCAPs.First(s => s.MaNCC == id);

                    //Thực hiện cập nhật da
                    p.TenNCC = ncc.TenNCC;
                    p.Diachi = ncc.Diachi;
                    p.DienThoai = ncc.DienThoai;
					//else
					//    p.AnhSP = "loi-hinh-anh.jpg"; //giữ nguyên hình ảnh ban đầu

					//Lưu xuống da
					da.SaveChanges();
					tranScope.Complete();

					return RedirectToAction("ListNCC");
				}
				catch//chưa xử lý bắt lỗi
				{
					tranScope.Dispose();
					return View();
				}
			}
		}

        public ActionResult ListLSP()
        {
			if (Session["AdminID"] == null)
				return RedirectToAction("Index", "Home");
			IEnumerable<LoaiSP> ds = da.LoaiSPs.Select(s => s).ToList();
			return View(ds);
		}

        public ActionResult C_LSP()
        {
			if (Session["AdminID"] == null)
				return RedirectToAction("Index", "Home");
			return View();
		}

        [HttpPost]

        public ActionResult C_LSP(LoaiSP lsp, FormCollection collection)
        {
			using (TransactionScope tranScope = new TransactionScope())
			{
				try
				{
					//Tạo mới 1 SP
					LoaiSP loaisanpham = new LoaiSP();

					//Thiết lập các thuộc tính cho SP
					loaisanpham = lsp;



					// Thêm SP vào bảng SanPham
					da.LoaiSPs.Add(loaisanpham);

					//Cập nhật thay đổi db
					da.SaveChanges();
					tranScope.Complete();
					//Hiển thị DSSP
					return RedirectToAction("ListLSP");

				}
				catch//chưa xử lý bắt lỗi
				{
					tranScope.Dispose();
					return View();
				}
			}
		}

        public ActionResult E_LSP(int id)
        {
			try
			{
				if (Session["AdminID"] == null)
					return RedirectToAction("DS LSP", "ListLSP");
				LoaiSP p = da.LoaiSPs.First(s => s.MaLoaiSP == id);

				return View(p);
			}
			catch (Exception)
			{
				return RedirectToAction("ListLSP");

			}

		}
        [HttpPost]
        public ActionResult E_LSP(LoaiSP lsp, int id, FormCollection collection)
        {
			using (TransactionScope tranScope = new TransactionScope())
			{
				try
				{
					// Xác định SP cần sửa trong da
					LoaiSP p = da.LoaiSPs.First(s => s.MaLoaiSP == id);

					//Thực hiện cập nhật da
					p.TenLoaiSP = lsp.TenLoaiSP;
					//else
					//    p.AnhSP = "loi-hinh-anh.jpg"; //giữ nguyên hình ảnh ban đầu

					//Lưu xuống da
					da.SaveChanges();
					tranScope.Complete();

					return RedirectToAction("ListLSP");
				}
				catch//chưa xử lý bắt lỗi
				{
					tranScope.Dispose();
					return View();
				}
			}
		}

        public ActionResult D_LSP(int id)
        {
            if (Session["AdminID"] == null)
                return RedirectToAction("Index", "Home");
            try
            {
                LoaiSP lsp = da.LoaiSPs.Single(s => s.MaLoaiSP == id);
                if (lsp != null)
                {
                    da.LoaiSPs.Remove(lsp);
                    da.SaveChanges();
                    return RedirectToAction("ListLSP");
                }
                else
                {
                    return RedirectToAction("ListLSP");
                }
            }
            catch
            {
                return RedirectToAction("ListLSP");
            }
        }

        //public ActionResult D_LSP(int id, FormCollection collection)
        //{
        //	if (Session["AdminID"] == null)
        //		return RedirectToAction("Index", "Home");
        //	using (TransactionScope tranScope = new TransactionScope())
        //	{
        //		try
        //		{
        //			// Xác định SP cần xóa trong da
        //			LoaiSP p = da.LoaiSPs.First(s => s.MaLoaiSP == id);
        //			//Thực hiện xóa
        //			da.LoaiSPs.Remove(p);
        //			//Cập nhật thay đổi da
        //			da.SaveChanges();
        //			tranScope.Complete();
        //			return RedirectToAction("ListLSP");
        //		}
        //		catch//chưa xử lý bắt lỗi
        //		{
        //			tranScope.Dispose();
        //			return RedirectToAction("ListLSP");
        //		}
        //	}
        //}
    }
}

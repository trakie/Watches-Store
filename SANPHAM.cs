﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Watches
{
    using System;
    using System.Collections.Generic;
	using System.Web;
	public partial class SANPHAM
    {
        public SANPHAM()
        {
            this.CHITIETDONTHANGs = new HashSet<CHITIETDONTHANG>();
            this.DanhGias = new HashSet<DanhGia>();
        }
    
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public Nullable<decimal> Giaban { get; set; }
        public string AnhSP { get; set; }
        public Nullable<int> Soluongton { get; set; }
        public Nullable<int> ThoiLuongBaoHanh { get; set; }
        public Nullable<int> MaLoai { get; set; }
        public Nullable<int> MaNCC { get; set; }
    
        public virtual ICollection<CHITIETDONTHANG> CHITIETDONTHANGs { get; set; }
        public virtual ICollection<DanhGia> DanhGias { get; set; }
        public virtual LoaiSP LoaiSP { get; set; }
        public virtual NHACUNGCAP NHACUNGCAP { get; set; }
		public HttpPostedFileBase ImageUpload { get; set; }//thuộc tính đăng file hình ảnh
	}
}
//------------------------------------------------------------------------------
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
    
    public partial class NHACUNGCAP
    {
        public NHACUNGCAP()
        {
            this.SANPHAMs = new HashSet<SANPHAM>();
        }
    
        public int MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string Diachi { get; set; }
        public string DienThoai { get; set; }
    
        public virtual ICollection<SANPHAM> SANPHAMs { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLBENHNHAN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BAOCAO
    {
        public string MABC { get; set; }
        public string idTK { get; set; }
        public string TENBAOCAO { get; set; }
        public string NOIDUNG { get; set; }
        public Nullable<System.DateTime> NGAYTAO { get; set; }
    
        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}

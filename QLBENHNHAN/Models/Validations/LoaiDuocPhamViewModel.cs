using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class LoaiDuocPhamViewModel
    {
        public string MALDP { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENLOAIDUOCPHAM { get; set; }
        public DateTime NGAYTAO { get; set; }
        public virtual ICollection<DUOCPHAM> DUOCPHAMs { get; set; }
    }
}
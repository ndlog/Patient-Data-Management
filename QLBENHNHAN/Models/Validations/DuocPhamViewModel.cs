using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class DuocPhamViewModel
    {
        public string MADP { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string idLDP { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENDUOCPHAM { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string MOTA { get; set; }
        public DateTime NGAYTAO { get; set; }
        public DateTime NGAYCAPNHAT { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string XUATXU { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int GIA {  get; set; } 
        public virtual ICollection<CHITIETDUOCPHAM> CHITIETDUOCPHAMs { get; set; }
        public virtual LOAIDUOCPHAM LOAIDUOCPHAM { get; set; }
    }
}
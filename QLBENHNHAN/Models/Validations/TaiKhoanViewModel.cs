using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class TaiKhoanViewModel
    {
        public string MATK { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string USERNAME { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string PASSWORD { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [DataType(DataType.Password)]
        [Compare("PASSWORD", ErrorMessage = "Mật khẩu nhập lại không trùng khớp")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới nhập lại không trùng khớp.")]
        public string ConfirmNewPassword { get; set; } 
        public bool ACTIVE { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string VAITRO { get; set; }
        public DateTime NGAYTAO { get; set; }
        public DateTime NGAYTRUYCAP { get; set; }
        public string MACODE { get; set; }
        public DateTime NGAYCAPNHAT { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Định dạng email không hợp lệ.")]
        public string EMAIL { get; set; }
        public virtual ICollection<BACSI> BACSIs { get; set; }
        public virtual ICollection<BAOCAO> BAOCAOs { get; set; }
        public virtual ICollection<BENHNHAN> BENHNHANs { get; set; }
        public virtual ICollection<CHITIETDUOCPHAM> CHITIETDUOCPHAMs { get; set; }
        public virtual ICollection<CHITIETTHIETBI> CHITIETTHIETBIs { get; set; }
    }
}
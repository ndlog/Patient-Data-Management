using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class BenhNhanViewModel
    {
        public string MABN { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string HOTEN { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NGAYSINH { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string GIOITINH { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0]{1}[0-9]{9}$", ErrorMessage = "Số điện thoại phải dài 10 chữ số.")]
        public string SODIENTHOAI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string DIACHI { get; set; }
        public DateTime NGAYTAO { get; set; }
        public string idP { get; set; }
        public DateTime NGAYCAPNHAT { get; set; }
        public string idTK { get; set; }
        public virtual PHONG PHONG { get; set; }
        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
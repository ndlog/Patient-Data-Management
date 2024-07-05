using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class BacSiViewModel
    {
        public string MABS { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string HOTEN { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0]{1}[0-9]{9}$", ErrorMessage = "Số điện thoại phải dài 10 chữ số.")]
        public string SODIENTHOAI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string DIACHI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string GIOITINH { get; set; }
        public string GIOITHIEU { get; set; }
        public DateTime NGAYTAO { get; set; }
        public DateTime NGAYCAPNHAT { get; set; }
        public string idK { get; set; }
        public string idTK { get; set; }
        public virtual KHOA KHOA { get; set; }
        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
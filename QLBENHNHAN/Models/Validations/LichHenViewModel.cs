using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class LichHenViewModel
    {
        public string MALH { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string HOTEN { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0]{1}[0-9]{9}$", ErrorMessage = "Số điện thoại phải dài 10 chữ số.")]
        public string SODIENTHOAI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NGAY { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public TimeSpan GIO { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string GHICHU { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string idK { get; set; }
        public string idBN { get; set; }

        public virtual BENHNHAN BENHNHAN { get; set; }
        public virtual KHOA KHOA { get; set; }
    }
}
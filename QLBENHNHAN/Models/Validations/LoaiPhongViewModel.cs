using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class LoaiPhongViewModel
    {
        public string MALP { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENLOAIPHONG { get; set; }
        public DateTime NGAYTAO { get; set; }
        public virtual ICollection<PHONG> PHONGs { get; set; }
    }
}
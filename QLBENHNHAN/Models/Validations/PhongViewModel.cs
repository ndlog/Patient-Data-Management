using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class PhongViewModel
    {
        public string MAP { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENPHONG { get; set; }
        public DateTime NGAYTAO { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string idLP { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TANG { get; set; }
        public virtual ICollection<BENHNHAN> BENHNHANs { get; set; }
        public virtual LOAIPHONG LOAIPHONG { get; set; }
        public virtual ICollection<THIETBI> THIETBIs { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class ThietBiViewModel
    {
        public string MATB { get; set; }
        public string idP { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string idLTB { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENTHIETBI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string MOTA { get; set; }
        public DateTime NGAYTAO { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string XUATXU { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TINHTRANG { get; set; }
        public virtual ICollection<CHITIETTHIETBI> CHITIETTHIETBIs { get; set; }
        public virtual LOAITHIETBI LOAITHIETBI { get; set; }
        public virtual PHONG PHONG { get; set; }
    }
}
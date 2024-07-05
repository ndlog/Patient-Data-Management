using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class LoaiThietBiViewModel
    {
        public string MALTB { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENLOAITHIETBI { get; set; }
        public DateTime NGAYTAO { get; set; }
        public virtual ICollection<THIETBI> THIETBIs { get; set; }
    }
}
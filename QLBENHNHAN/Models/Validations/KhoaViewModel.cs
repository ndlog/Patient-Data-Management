using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class KhoaViewModel
    {
        public string MAK { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENKHOA { get; set; }
        public DateTime NGAYTAO { get; set; }
        public virtual ICollection<BACSI> BACSIs { get; set; }
    }
}
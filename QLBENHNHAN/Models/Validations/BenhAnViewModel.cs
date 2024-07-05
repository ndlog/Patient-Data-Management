using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class BenhAnViewModel
    {
        public string MABA { get; set; }
        [Required(ErrorMessage = "Bác sĩ là bắt buộc.")]
        public string idBS { get; set; }
        [Required(ErrorMessage = "Bệnh nhân là bắt buộc.")]
        public string idBN { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TENBENHAN { get; set; }
        public DateTime NGAY { get; set; }
        public TimeSpan GIO { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int MACH { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int THANNHIET { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int NHIPTHO { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int CHIEUCAO { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int CANNANG { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string MATTRAI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string MATPHAI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int NHANAPPHAI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public int NHANAPTRAI { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string CHUANDOANLAMSANG { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string CHUANDOANCUOICUNG { get; set; }
        public DateTime NGAYTAO { get; set; }
        [Required(ErrorMessage = "Thông tin này là bắt buộc.")]
        public string TRANGTHAI { get; set; }
        public string idTKHAM { get; set; }

        public virtual BACSI BACSI { get; set; }
        public virtual BENHNHAN BENHNHAN { get; set; }
    }
}
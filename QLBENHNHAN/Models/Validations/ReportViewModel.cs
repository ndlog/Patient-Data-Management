using QLBENHNHAN.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBENHNHAN.Models.Validations
{
    public class ReportViewModel
    {
        public List<BENHNHAN> BENHNHANs { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportType { get; set; }
        public string NOIDUNG { get; set; }
        public Dictionary<string, List<BENHNHAN>> GroupedBENHNHANs { get; set; }
        public Dictionary<string, List<CHITIETBENHAN>> GroupedBENHANs { get; set; }
        public Dictionary<string, List<HOADON>> GroupedHOADONs { get; set; }
        public Dictionary<string, List<PHIEUHEN>> GroupedLICHHENs { get; set; }
    }
}
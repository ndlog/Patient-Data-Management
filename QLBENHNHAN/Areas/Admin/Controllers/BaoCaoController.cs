using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using PagedList;
using QLBENHNHAN.Models;
using QLBENHNHAN.Models.Validations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using System.IO;
using System.Drawing;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class BaoCaoController : Controller
    {
        private QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        public ActionResult TaoBaoCaoBenhNhan()
        {
            var baoCaos = new ReportViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };
            return View(baoCaos);
        }
        [HttpPost]
        public ActionResult TaoBaoCaoBenhNhan(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var benhNhans = _db.BENHNHANs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .OrderBy(p => p.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<BENHNHAN>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = benhNhans
                        .GroupBy(p => p.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tuần":
                    groupedData = benhNhans
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tháng":
                    groupedData = benhNhans
                        .GroupBy(p => p.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Quý":
                    groupedData = benhNhans
                        .GroupBy(p => $"Q{((p.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Năm":
                    groupedData = benhNhans
                        .GroupBy(p => p.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
            }

            model.GroupedBENHNHANs = groupedData;

            return View(model);
        }
        public ActionResult TaoBaoCaoBenhAn()
        {
            var baoCaos = new ReportViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };
            return View(baoCaos);
        }
        [HttpPost]
        public ActionResult TaoBaoCaoBenhAn(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var benhAns = _db.CHITIETBENHANs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .OrderBy(p => p.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<CHITIETBENHAN>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = benhAns
                        .GroupBy(p => p.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tuần":
                    groupedData = benhAns
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tháng":
                    groupedData = benhAns
                        .GroupBy(p => p.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Quý":
                    groupedData = benhAns
                        .GroupBy(p => $"Q{((p.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Năm":
                    groupedData = benhAns
                        .GroupBy(p => p.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
            }

            model.GroupedBENHANs = groupedData;

            return View(model);
        }
        public ActionResult TaoBaoCaoLichHen()
        {
            var baoCaos = new ReportViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };
            return View(baoCaos);
        }
        [HttpPost]
        public ActionResult TaoBaoCaoLichHen(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var phieHens = _db.PHIEUHENs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .OrderBy(p => p.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<PHIEUHEN>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = phieHens
                        .GroupBy(p => p.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tuần":
                    groupedData = phieHens
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tháng":
                    groupedData = phieHens
                        .GroupBy(p => p.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Quý":
                    groupedData = phieHens
                        .GroupBy(p => $"Q{((p.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Năm":
                    groupedData = phieHens
                        .GroupBy(p => p.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
            }

            model.GroupedLICHHENs = groupedData;

            return View(model);
        }
        public ActionResult TaoBaoCaoHoaDon()
        {
            var baoCaos = new ReportViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };
            return View(baoCaos);
        }
        [HttpPost]
        public ActionResult TaoBaoCaoHoaDon(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var hoaDons = _db.HOADONs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .AsEnumerable()
                .Select(p => new
                {
                    HOADON = p,
                    TongTien = GetTongTienFromChiTietHoaDon(p.CHITIETHOADON)
                })
                .OrderBy(p => p.HOADON.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<HOADON>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = hoaDons
                        .GroupBy(p => p.HOADON.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Tuần":
                    groupedData = hoaDons
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.HOADON.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.HOADON.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Tháng":
                    groupedData = hoaDons
                        .GroupBy(p => p.HOADON.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Quý":
                    groupedData = hoaDons
                        .GroupBy(p => $"Q{((p.HOADON.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.HOADON.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Năm":
                    groupedData = hoaDons
                        .GroupBy(p => p.HOADON.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
            }

            model.GroupedHOADONs = groupedData;

            return View(model);
        }

        private int GetTongTienFromChiTietHoaDon(string chiTietHoaDonJson)
        {
            if (string.IsNullOrEmpty(chiTietHoaDonJson))
                return 0;

            var chiTietHoaDon = JsonConvert.DeserializeObject<List<dynamic>>(chiTietHoaDonJson);
            int totalTongTien = 0;
            foreach (var chiTiet in chiTietHoaDon)
            {
                totalTongTien += Convert.ToInt32(chiTiet["TongTien"]); // Adjust conversion as necessary
            }
            return totalTongTien;
        }
        public class ChiTietHoaDon
        {
            public decimal TONGTIEN { get; set; }
        }
        private int CalculateWeekOfYear(DateTime date)
        {
            // Tính số tuần dựa trên sự khác biệt giữa các ngày và các ngày trong tuần
            var daysOffset = DayOfWeek.Monday - date.DayOfWeek;
            var firstMonday = date.AddDays(daysOffset);
            var weekNumber = (int)(Math.Floor((date.Subtract(firstMonday).TotalDays / 7)) + 1);

            return weekNumber;
        }
        public ActionResult ExportExcelBenhNhan(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var benhNhans = _db.BENHNHANs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .OrderBy(p => p.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<BENHNHAN>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = benhNhans
                        .GroupBy(p => p.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tuần":
                    groupedData = benhNhans
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tháng":
                    groupedData = benhNhans
                        .GroupBy(p => p.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Quý":
                    groupedData = benhNhans
                        .GroupBy(p => $"Q{((p.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Năm":
                    groupedData = benhNhans
                        .GroupBy(p => p.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Dữ liệu bệnh nhân");

                // Add contact information
                string nameInfo = "Bệnh viện VLU Heart";
                ws.Cells["A1"].Value = nameInfo;
                int nameColumnMergeCount = (int)Math.Ceiling((double)nameInfo.Length / 15); 
                ws.Cells[1, 1, 1, nameColumnMergeCount].Merge = true;
                ws.Cells[1, 1, 1, nameColumnMergeCount].Style.WrapText = true;

                string addressInfo = "Địa chỉ: Cơ sở 1: 45 Nguyễn Khắc Nhu, Phường Cô Giang, Quận 1, Hồ Chí Minh - Cơ sở 2: 233A Đ. Phan Văn Trị, Phường 11, Bình Thạnh, Hồ Chí Minh - Cơ sở 3: 69/68 Đ. Đặng Thuỳ Trâm, Phường 13, Bình Thạnh, Hồ Chí Minh";
                ws.Cells["A2"].Value = addressInfo;
                int addressColumnMergeCount = (int)Math.Ceiling((double)addressInfo.Length / 10);
                ws.Cells[2, 1, 2, addressColumnMergeCount].Merge = true;
                ws.Cells[2, 1, 2, addressColumnMergeCount].Style.WrapText = true;

                string phoneInfo = "Số điện thoại: (+84) 395274446";
                ws.Cells["A3"].Value = phoneInfo;
                int phoneColumnMergeCount = (int)Math.Ceiling((double)phoneInfo.Length / 10); 
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Merge = true;
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Style.WrapText = true;

                string emailInfo = "Email: long.207ct40402@vanlanguni.vn";
                ws.Cells["A4"].Value = emailInfo;
                int emailColumnMergeCount = (int)Math.Ceiling((double)emailInfo.Length / 15); 
                ws.Cells[4, 1, 4, emailColumnMergeCount].Merge = true;
                ws.Cells[4, 1, 4, emailColumnMergeCount].Style.WrapText = true;

                string header = $"Báo cáo dữ liệu bệnh nhân theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}";
                ws.Cells["A6"].Value = header;
                int headerColumnMergeCount = (int)Math.Ceiling((double)header.Length / 10);
                ws.Cells[6, 1, 6, headerColumnMergeCount].Merge = true;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Size = 20;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Bold = true;

                // Apply style to contact information
                using (var range = ws.Cells["A1:A6"])
                {
                    range.Style.Font.Bold = true;
                }
                
                int rowStart = 8;
                foreach (var group in groupedData)
                {
                    ws.Cells[rowStart, 1].Value = "Thời gian:";
                    ws.Cells[rowStart, 3].Value = group.Key;
                    ws.Cells[rowStart, 1, rowStart, 2].Merge = true;
                    ws.Cells[rowStart, 1, rowStart, 3].Style.Font.Bold = true;
                    rowStart++;

                    // Add headers for patient data
                    ws.Cells[rowStart, 1].Value = "STT";
                    ws.Cells[rowStart, 2].Value = "Mã bệnh nhân";
                    ws.Cells[rowStart, 3].Value = "Họ tên";
                    ws.Cells[rowStart, 4].Value = "Ngày sinh";
                    ws.Cells[rowStart, 5].Value = "Số điện thoại";
                    ws.Cells[rowStart, 6].Value = "Giới tính";
                    ws.Cells[rowStart, 7].Value = "Địa chỉ";
                    ws.Cells[rowStart, 1, rowStart, 7].Style.Font.Bold = true;
                    rowStart++;

                    int rowNumber = 1;
                    foreach (var patient in group.Value)
                    {
                        ws.Cells[string.Format("A{0}", rowStart)].Value = rowNumber.ToString();
                        ws.Cells[string.Format("B{0}", rowStart)].Value = patient.MABN;
                        ws.Cells[string.Format("C{0}", rowStart)].Value = patient.HOTEN;
                        ws.Cells[string.Format("D{0}", rowStart)].Value = patient.NGAYSINH?.ToString("dd/MM/yyyy") ?? "";
                        ws.Cells[string.Format("E{0}", rowStart)].Value = patient.SODIENTHOAI;
                        ws.Cells[string.Format("F{0}", rowStart)].Value = patient.GIOITINH;
                        ws.Cells[string.Format("G{0}", rowStart)].Value = patient.DIACHI;

                        // Apply border to data
                        using (var range = ws.Cells[string.Format("A{0}:G{0}", rowStart)])
                        {
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        rowStart++;
                        rowNumber++;
                    }
                }

                // AutoFit columns
                ws.Cells["A:G"].AutoFitColumns();

                // Convert package to byte array
                byte[] excelData = pck.GetAsByteArray();

                string fileName = $"Báo cáo dữ liệu bệnh nhân theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public ActionResult ExportExcelBenhAn(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var benhAns = _db.CHITIETBENHANs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .OrderBy(p => p.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<CHITIETBENHAN>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = benhAns
                        .GroupBy(p => p.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tuần":
                    groupedData = benhAns
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tháng":
                    groupedData = benhAns
                        .GroupBy(p => p.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Quý":
                    groupedData = benhAns
                        .GroupBy(p => $"Q{((p.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Năm":
                    groupedData = benhAns
                        .GroupBy(p => p.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Dữ liệu bệnh án");

                // Add contact information
                string nameInfo = "Bệnh viện VLU Heart";
                ws.Cells["A1"].Value = nameInfo;
                int nameColumnMergeCount = (int)Math.Ceiling((double)nameInfo.Length / 15);
                ws.Cells[1, 1, 1, nameColumnMergeCount].Merge = true;
                ws.Cells[1, 1, 1, nameColumnMergeCount].Style.WrapText = true;

                string addressInfo = "Địa chỉ: Cơ sở 1: 45 Nguyễn Khắc Nhu, Phường Cô Giang, Quận 1, Hồ Chí Minh - Cơ sở 2: 233A Đ. Phan Văn Trị, Phường 11, Bình Thạnh, Hồ Chí Minh - Cơ sở 3: 69/68 Đ. Đặng Thuỳ Trâm, Phường 13, Bình Thạnh, Hồ Chí Minh";
                ws.Cells["A2"].Value = addressInfo;
                int addressColumnMergeCount = (int)Math.Ceiling((double)addressInfo.Length / 10);
                ws.Cells[2, 1, 2, addressColumnMergeCount].Merge = true;
                ws.Cells[2, 1, 2, addressColumnMergeCount].Style.WrapText = true;

                string phoneInfo = "Số điện thoại: (+84) 395274446";
                ws.Cells["A3"].Value = phoneInfo;
                int phoneColumnMergeCount = (int)Math.Ceiling((double)phoneInfo.Length / 10);
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Merge = true;
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Style.WrapText = true;

                string emailInfo = "Email: long.207ct40402@vanlanguni.vn";
                ws.Cells["A4"].Value = emailInfo;
                int emailColumnMergeCount = (int)Math.Ceiling((double)emailInfo.Length / 15);
                ws.Cells[4, 1, 4, emailColumnMergeCount].Merge = true;
                ws.Cells[4, 1, 4, emailColumnMergeCount].Style.WrapText = true;

                string header = $"Báo cáo dữ liệu bệnh án theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}";
                ws.Cells["A6"].Value = header;
                int headerColumnMergeCount = (int)Math.Ceiling((double)header.Length / 10);
                ws.Cells[6, 1, 6, headerColumnMergeCount].Merge = true;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Size = 20;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Bold = true;

                // Apply style to contact information
                using (var range = ws.Cells["A1:A6"])
                {
                    range.Style.Font.Bold = true;
                }

                int rowStart = 8;
                foreach (var group in groupedData)
                {
                    ws.Cells[rowStart, 1].Value = "Thời gian:";
                    ws.Cells[rowStart, 3].Value = group.Key;
                    ws.Cells[rowStart, 1, rowStart, 2].Merge = true;
                    ws.Cells[rowStart, 1, rowStart, 3].Style.Font.Bold = true;
                    rowStart++;

                    // Add headers for patient data
                    ws.Cells[rowStart, 1].Value = "STT";
                    ws.Cells[rowStart, 2].Value = "Mã bệnh án";
                    ws.Cells[rowStart, 3].Value = "Tiêu đề";
                    ws.Cells[rowStart, 4].Value = "Tên bệnh nhân";
                    ws.Cells[rowStart, 5].Value = "Tên bác sĩ";
                    ws.Cells[rowStart, 6].Value = "Trạng thái";
                    ws.Cells[rowStart, 7].Value = "Ngày";
                    ws.Cells[rowStart, 8].Value = "Giờ";
                    ws.Cells[rowStart, 9].Value = "Mạch";
                    ws.Cells[rowStart, 10].Value = "Thân nhiệt";
                    ws.Cells[rowStart, 11].Value = "Nhịp thở";
                    ws.Cells[rowStart, 12].Value = "Chiều cao";
                    ws.Cells[rowStart, 13].Value = "Cân nặng";
                    ws.Cells[rowStart, 14].Value = "Mắt trái";
                    ws.Cells[rowStart, 15].Value = "Mắt phải";
                    ws.Cells[rowStart, 16].Value = "Nhãn áp trái";
                    ws.Cells[rowStart, 17].Value = "Nhãn áp phải";
                    ws.Cells[rowStart, 18].Value = "Tiền sử bệnh";
                    ws.Cells[rowStart, 19].Value = "Chẩn đoán lâm sàng";
                    ws.Cells[rowStart, 20].Value = "Chẩn đoán cuối cùng";
                    ws.Cells[rowStart, 1, rowStart, 20].Style.Font.Bold = true;
                    rowStart++;

                    int rowNumber = 1;
                    foreach (var patient in group.Value)
                    {
                        ws.Cells[string.Format("A{0}", rowStart)].Value = rowNumber.ToString();
                        ws.Cells[string.Format("B{0}", rowStart)].Value = patient.MABA;
                        ws.Cells[string.Format("C{0}", rowStart)].Value = patient.TENBENHAN;
                        ws.Cells[string.Format("D{0}", rowStart)].Value = patient.idBN + " - " + patient.BENHNHAN.HOTEN;
                        ws.Cells[string.Format("E{0}", rowStart)].Value = patient.idBS + " - " + patient.BACSI.HOTEN;
                        ws.Cells[string.Format("F{0}", rowStart)].Value = patient.TRANGTHAI;
                        ws.Cells[string.Format("G{0}", rowStart)].Value = patient.NGAY?.ToString("dd/MM/yyyy") ?? "";
                        ws.Cells[string.Format("H{0}", rowStart)].Value = patient.GIO.Value.ToString(@"hh\:mm");
                        ws.Cells[string.Format("I{0}", rowStart)].Value = patient.MACH;
                        ws.Cells[string.Format("J{0}", rowStart)].Value = patient.THANNHIET;
                        ws.Cells[string.Format("K{0}", rowStart)].Value = patient.NHIPTHO;
                        ws.Cells[string.Format("L{0}", rowStart)].Value = patient.CHIEUCAO;
                        ws.Cells[string.Format("M{0}", rowStart)].Value = patient.CANNANG;
                        ws.Cells[string.Format("N{0}", rowStart)].Value = patient.MATPHAI;
                        ws.Cells[string.Format("O{0}", rowStart)].Value = patient.MATPHAI;
                        ws.Cells[string.Format("P{0}", rowStart)].Value = patient.NHANAPTRAI;
                        ws.Cells[string.Format("Q{0}", rowStart)].Value = patient.NHANAPPHAI;
                        ws.Cells[string.Format("R{0}", rowStart)].Value = patient.TIENSUBENH;
                        ws.Cells[string.Format("S{0}", rowStart)].Value = patient.CHUANDOANLAMSANG;
                        ws.Cells[string.Format("T{0}", rowStart)].Value = patient.CHUANDOANCUOICUNG;

                        // Apply border to data
                        using (var range = ws.Cells[string.Format("A{0}:T{0}", rowStart)])
                        {
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        rowStart++;
                        rowNumber++;
                    }
                }

                // AutoFit columns
                ws.Cells["A:T"].AutoFitColumns();

                // Convert package to byte array
                byte[] excelData = pck.GetAsByteArray();

                string fileName = $"Báo cáo dữ liệu bệnh án theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public ActionResult ExportExcelLichHen(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var phieHens = _db.PHIEUHENs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .OrderBy(p => p.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<PHIEUHEN>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = phieHens
                        .GroupBy(p => p.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tuần":
                    groupedData = phieHens
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Tháng":
                    groupedData = phieHens
                        .GroupBy(p => p.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Quý":
                    groupedData = phieHens
                        .GroupBy(p => $"Q{((p.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
                case "Năm":
                    groupedData = phieHens
                        .GroupBy(p => p.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.ToList());
                    break;
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Dữ liệu lịch hẹn");

                // Add contact information
                string nameInfo = "Bệnh viện VLU Heart";
                ws.Cells["A1"].Value = nameInfo;
                int nameColumnMergeCount = (int)Math.Ceiling((double)nameInfo.Length / 15);
                ws.Cells[1, 1, 1, nameColumnMergeCount].Merge = true;
                ws.Cells[1, 1, 1, nameColumnMergeCount].Style.WrapText = true;

                string addressInfo = "Địa chỉ: Cơ sở 1: 45 Nguyễn Khắc Nhu, Phường Cô Giang, Quận 1, Hồ Chí Minh - Cơ sở 2: 233A Đ. Phan Văn Trị, Phường 11, Bình Thạnh, Hồ Chí Minh - Cơ sở 3: 69/68 Đ. Đặng Thuỳ Trâm, Phường 13, Bình Thạnh, Hồ Chí Minh";
                ws.Cells["A2"].Value = addressInfo;
                int addressColumnMergeCount = (int)Math.Ceiling((double)addressInfo.Length / 10);
                ws.Cells[2, 1, 2, addressColumnMergeCount].Merge = true;
                ws.Cells[2, 1, 2, addressColumnMergeCount].Style.WrapText = true;

                string phoneInfo = "Số điện thoại: (+84) 395274446";
                ws.Cells["A3"].Value = phoneInfo;
                int phoneColumnMergeCount = (int)Math.Ceiling((double)phoneInfo.Length / 10);
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Merge = true;
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Style.WrapText = true;

                string emailInfo = "Email: long.207ct40402@vanlanguni.vn";
                ws.Cells["A4"].Value = emailInfo;
                int emailColumnMergeCount = (int)Math.Ceiling((double)emailInfo.Length / 15);
                ws.Cells[4, 1, 4, emailColumnMergeCount].Merge = true;
                ws.Cells[4, 1, 4, emailColumnMergeCount].Style.WrapText = true;

                string header = $"Báo cáo dữ liệu lịch hẹn theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}";
                ws.Cells["A6"].Value = header;
                int headerColumnMergeCount = (int)Math.Ceiling((double)header.Length / 10);
                ws.Cells[6, 1, 6, headerColumnMergeCount].Merge = true;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Size = 20;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Bold = true;

                // Apply style to contact information
                using (var range = ws.Cells["A1:A6"])
                {
                    range.Style.Font.Bold = true;
                }

                int rowStart = 8;
                foreach (var group in groupedData)
                {
                    ws.Cells[rowStart, 1].Value = "Thời gian:";
                    ws.Cells[rowStart, 3].Value = group.Key;
                    ws.Cells[rowStart, 1, rowStart, 2].Merge = true;
                    ws.Cells[rowStart, 1, rowStart, 3].Style.Font.Bold = true;
                    rowStart++;

                    // Add headers for patient data
                    ws.Cells[rowStart, 1].Value = "STT";
                    ws.Cells[rowStart, 2].Value = "Mã lịch hẹn";
                    ws.Cells[rowStart, 3].Value = "Tên bệnh nhân";
                    ws.Cells[rowStart, 4].Value = "Số điện thoại";
                    ws.Cells[rowStart, 5].Value = "Ngày";
                    ws.Cells[rowStart, 6].Value = "Giờ";
                    ws.Cells[rowStart, 7].Value = "Ghi chú";
                    ws.Cells[rowStart, 8].Value = "Tình trạng";
                    ws.Cells[rowStart, 1, rowStart, 8].Style.Font.Bold = true;
                    rowStart++;

                    int rowNumber = 1;
                    foreach (var patient in group.Value)
                    {
                        ws.Cells[string.Format("A{0}", rowStart)].Value = rowNumber.ToString();
                        ws.Cells[string.Format("B{0}", rowStart)].Value = patient.MALH;
                        ws.Cells[string.Format("C{0}", rowStart)].Value = patient.idBN + " - " + patient.BENHNHAN.HOTEN;
                        ws.Cells[string.Format("D{0}", rowStart)].Value = patient.SODIENTHOAI;
                        ws.Cells[string.Format("E{0}", rowStart)].Value = patient.NGAY?.ToString("dd/MM/yyyy") ?? "";
                        ws.Cells[string.Format("F{0}", rowStart)].Value = patient.GIO.Value.ToString(@"hh\:mm");
                        ws.Cells[string.Format("G{0}", rowStart)].Value = patient.GHICHU;
                        ws.Cells[string.Format("H{0}", rowStart)].Value = (bool)patient.ACTIVE ? "Đã duyệt" : "Chưa duyệt";

                        // Apply border to data
                        using (var range = ws.Cells[string.Format("A{0}:H{0}", rowStart)])
                        {
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        rowStart++;
                        rowNumber++;
                    }
                }

                // AutoFit columns
                ws.Cells["A:H"].AutoFitColumns();

                // Convert package to byte array
                byte[] excelData = pck.GetAsByteArray();

                string fileName = $"Báo cáo dữ liệu lịch hẹn theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public ActionResult ExportExcelHoaDon(ReportViewModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            string reportType = model.ReportType;

            var hoaDons = _db.HOADONs
                .Where(p => p.NGAYTAO >= startDate && p.NGAYTAO <= endDate)
                .AsEnumerable()
                .Select(p => new
                {
                    HOADON = p,
                    TongTien = GetTongTienFromChiTietHoaDon(p.CHITIETHOADON)
                })
                .OrderBy(p => p.HOADON.NGAYTAO)
                .ToList();

            var groupedData = new Dictionary<string, List<HOADON>>();

            switch (reportType)
            {
                case "Ngày":
                    groupedData = hoaDons
                        .GroupBy(p => p.HOADON.NGAYTAO.Value.ToString("dd/MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Tuần":
                    groupedData = hoaDons
                        .GroupBy(p => $"{CultureInfo.CurrentCulture.Calendar.GetWeekOfYear((DateTime)p.HOADON.NGAYTAO, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}/{p.HOADON.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Tháng":
                    groupedData = hoaDons
                        .GroupBy(p => p.HOADON.NGAYTAO.Value.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Quý":
                    groupedData = hoaDons
                        .GroupBy(p => $"Q{((p.HOADON.NGAYTAO.Value.Month - 1) / 3) + 1}/{p.HOADON.NGAYTAO.Value.Year}")
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
                case "Năm":
                    groupedData = hoaDons
                        .GroupBy(p => p.HOADON.NGAYTAO.Value.Year.ToString())
                        .ToDictionary(g => g.Key, g => g.Select(x => x.HOADON).ToList());
                    break;
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Dữ liệu hóa đơn");

                // Add contact information
                string nameInfo = "Bệnh viện VLU Heart";
                ws.Cells["A1"].Value = nameInfo;
                int nameColumnMergeCount = (int)Math.Ceiling((double)nameInfo.Length / 15);
                ws.Cells[1, 1, 1, nameColumnMergeCount].Merge = true;
                ws.Cells[1, 1, 1, nameColumnMergeCount].Style.WrapText = true;

                string addressInfo = "Địa chỉ: Cơ sở 1: 45 Nguyễn Khắc Nhu, Phường Cô Giang, Quận 1, Hồ Chí Minh - Cơ sở 2: 233A Đ. Phan Văn Trị, Phường 11, Bình Thạnh, Hồ Chí Minh - Cơ sở 3: 69/68 Đ. Đặng Thuỳ Trâm, Phường 13, Bình Thạnh, Hồ Chí Minh";
                ws.Cells["A2"].Value = addressInfo;
                int addressColumnMergeCount = (int)Math.Ceiling((double)addressInfo.Length / 10);
                ws.Cells[2, 1, 2, addressColumnMergeCount].Merge = true;
                ws.Cells[2, 1, 2, addressColumnMergeCount].Style.WrapText = true;

                string phoneInfo = "Số điện thoại: (+84) 395274446";
                ws.Cells["A3"].Value = phoneInfo;
                int phoneColumnMergeCount = (int)Math.Ceiling((double)phoneInfo.Length / 10);
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Merge = true;
                ws.Cells[3, 1, 3, phoneColumnMergeCount].Style.WrapText = true;

                string emailInfo = "Email: long.207ct40402@vanlanguni.vn";
                ws.Cells["A4"].Value = emailInfo;
                int emailColumnMergeCount = (int)Math.Ceiling((double)emailInfo.Length / 15);
                ws.Cells[4, 1, 4, emailColumnMergeCount].Merge = true;
                ws.Cells[4, 1, 4, emailColumnMergeCount].Style.WrapText = true;

                string header = $"Báo cáo dữ liệu hóa đơn theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}";
                ws.Cells["A6"].Value = header;
                int headerColumnMergeCount = (int)Math.Ceiling((double)header.Length / 10);
                ws.Cells[6, 1, 6, headerColumnMergeCount].Merge = true;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Size = 20;
                ws.Cells[6, 1, 6, headerColumnMergeCount].Style.Font.Bold = true;

                // Apply style to contact information
                using (var range = ws.Cells["A1:A6"])
                {
                    range.Style.Font.Bold = true;
                }

                int rowStart = 8;
                foreach (var group in groupedData)
                {
                    ws.Cells[rowStart, 1].Value = "Thời gian:";
                    ws.Cells[rowStart, 3].Value = group.Key;
                    ws.Cells[rowStart, 1, rowStart, 2].Merge = true;
                    ws.Cells[rowStart, 1, rowStart, 3].Style.Font.Bold = true;
                    rowStart++;

                    // Add headers for patient data
                    ws.Cells[rowStart, 1].Value = "STT";
                    ws.Cells[rowStart, 2].Value = "Mã hóa đơn";
                    ws.Cells[rowStart, 3].Value = "Tên bệnh nhân";
                    ws.Cells[rowStart, 4].Value = "Tiêu đề";
                    ws.Cells[rowStart, 5].Value = "Số tiền (VNĐ)";
                    ws.Cells[rowStart, 6].Value = "Tình trạng";
                    ws.Cells[rowStart, 1, rowStart, 6].Style.Font.Bold = true;
                    rowStart++;

                    int rowNumber = 1;
                    foreach (var patient in group.Value)
                    {
                        var chiTietHoaDon = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(patient.CHITIETHOADON);
                        var cultureInfo = new System.Globalization.CultureInfo("en-US");

                        // Tính tổng TongTien từ chiTietHoaDon
                        var totalTongTien = 0;
                        foreach (var chiTiet in chiTietHoaDon)
                        {
                            // Truy cập thuộc tính TongTien
                            if (chiTiet.ContainsKey("TongTien"))
                            {
                                totalTongTien += Convert.ToInt32(chiTiet["TongTien"]); //Điều chỉnh chuyển đổi loại
                            }
                        }
                        ws.Cells[string.Format("A{0}", rowStart)].Value = rowNumber.ToString();
                        ws.Cells[string.Format("B{0}", rowStart)].Value = patient.MAHD;
                        ws.Cells[string.Format("C{0}", rowStart)].Value = patient.idBN + " - " + patient.BENHNHAN.HOTEN;
                        ws.Cells[string.Format("D{0}", rowStart)].Value = patient.TENHOADON;
                        ws.Cells[string.Format("E{0}", rowStart)].Value = totalTongTien.ToString("N0", cultureInfo);
                        ws.Cells[string.Format("F{0}", rowStart)].Value = (bool)patient.TRANGTHAI ? "Đã thanh toán" : "Chưa thanh toán";

                        // Apply border to data
                        using (var range = ws.Cells[string.Format("A{0}:F{0}", rowStart)])
                        {
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        rowStart++;
                        rowNumber++;
                    }
                }

                // AutoFit columns
                ws.Cells["A:F"].AutoFitColumns();

                // Convert package to byte array
                byte[] excelData = pck.GetAsByteArray();

                string fileName = $"Báo cáo dữ liệu hóa đơn theo {reportType} - {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D5");

            while (_db.BAOCAOs.Any(item => item.MABC == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D5");
            }
            return newID;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
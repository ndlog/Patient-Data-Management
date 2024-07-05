using QLBENHNHAN.Models;
using QLBENHNHAN.Models.Validations;
using QLDULIEUBENHNAN;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["UserRole"] != null)
            {
                var totalPatients = _db.BENHNHANs.Count();
                ViewBag.TotalPatients = totalPatients;

                // Lấy số lượng bệnh nhân theo từng tháng
                var monthlyPatients = _db.BENHNHANs
                    .Where(b => b.NGAYTAO.HasValue) // Đảm bảo NGAYTAO có giá trị
                    .GroupBy(b => new { b.NGAYTAO.Value.Month, b.NGAYTAO.Value.Year })
                    .Select(g => new
                    {
                        Month = g.Key.Month,
                        Year = g.Key.Year,
                        PatientCount = g.Count()
                    })
                    .ToList();

                // Tạo mảng dữ liệu cho từng tháng
                var patientData = new int[12];
                foreach (var item in monthlyPatients)
                {
                    if (item.Year == DateTime.Now.Year) // Chỉ lấy dữ liệu của năm hiện tại
                    {
                        patientData[item.Month - 1] = item.PatientCount;
                    }
                }

                ViewBag.PatientData = patientData; // Truyền dữ liệu tới view

                // Lấy tổng số bệnh án theo từng trạng thái
                var totalBenhAn = new Dictionary<string, int>();

                // Đợi thăm khám
                totalBenhAn["Đợi thăm khám"] = _db.CHITIETBENHANs.Count(b => b.TRANGTHAI == "Đợi thăm khám");

                // Đang thăm khám
                totalBenhAn["Đang thăm khám"] = _db.CHITIETBENHANs.Count(b => b.TRANGTHAI == "Đang thăm khám");

                // Đã thăm khám
                totalBenhAn["Đã thăm khám"] = _db.CHITIETBENHANs.Count(b => b.TRANGTHAI == "Đã thăm khám");

                ViewBag.TotalBenhAn = totalBenhAn;

                var totalDoctors = _db.BACSIs.Count();
                ViewBag.TotalDoctors = totalDoctors;

                var totalLichHens = _db.PHIEUHENs.Count();
                ViewBag.TotalLichHens = totalLichHens;

                var totalHoaDons = _db.HOADONs.Count();
                ViewBag.TotalHoaDons = totalHoaDons;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }
        public ActionResult Loi()
        {
            return View();
        }

        public ActionResult Info()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["UserID"] != null && (string)Session["UserRole"] == "Doctor")
            {
                // Lấy ID của tài khoản từ Session
                string userID = Session["UserID"].ToString();

                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        // Truy vấn dữ liệu của bác sĩ từ cơ sở dữ liệu bằng cách sử dụng ID của tài khoản
                        var doctorData = (from bs in context.BACSIs
                                          where bs.idTK == userID 
                                          select bs).Include(bs => bs.TAIKHOAN).FirstOrDefault();

                        if (doctorData != null)
                        {
                            var doctorViewModel = new BacSiViewModel
                            {
                                MABS = doctorData.MABS,
                                HOTEN = doctorData.HOTEN,
                                SODIENTHOAI = doctorData.SODIENTHOAI,
                                DIACHI = doctorData.DIACHI,
                                GIOITINH = doctorData.GIOITINH,
                                GIOITHIEU = doctorData.GIOITHIEU,
                                NGAYCAPNHAT = (DateTime)doctorData.NGAYCAPNHAT,
                                idK = doctorData.idK,
                                idTK = doctorData.idTK,
                            };
                            return View(doctorViewModel);
                        }
                        else
                        {
                            // Nếu không tìm thấy dữ liệu hoặc MABS không khớp, chuyển hướng hoặc hiển thị thông báo lỗi
                            TempData["ErrorAdmin"] = "Bạn không có quyền truy cập vào trang này.";
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý trường hợp có lỗi xảy ra khi truy xuất dữ liệu từ cơ sở dữ liệu
                    TempData["ErrorAdmin"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            else if (Session["UserRole"] != null && (string)Session["UserRole"] == "Admin")
            {
                // Nếu người dùng có vai trò là "Admin", chuyển hướng họ đến trang chủ hoặc trang thông báo lỗi
                TempData["ErrorAdmin"] = "Bạn không cần truy cập vào trang này.";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                // Xử lý trường hợp người dùng chưa đăng nhập hoặc không phải là "Doctor" hoặc "Admin"
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(BacSiViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.BACSIs.Find(model.MABS);
                    if (editModel != null)
                    {
                        editModel.HOTEN = model.HOTEN;
                        editModel.SODIENTHOAI = model.SODIENTHOAI;
                        editModel.DIACHI = model.DIACHI;
                        editModel.GIOITINH = model.GIOITINH; 
                        editModel.GIOITHIEU = model.GIOITHIEU;
                        model.NGAYCAPNHAT = DateTime.Now;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật thông tin thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi", "Home", new {area = "Admin"});
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật thông tin thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật thông tin thất bại: " + ex.Message;
                return View(model);
            }
        }
        // Export to PDF
        public ActionResult ExportPdf()
        {
            var patients = _db.BENHNHANs.ToList();
            // Định nghĩa font tiếng Việt
            string fontPath = Server.MapPath("~/Content/font/Be_VietNam_Pro/BeVietNamPro-Regular.ttf"); 
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            // Sử dụng font chữ tiếng Việt cho bảng
            Font vietnameseFont = new Font(baseFont, 12);

            // Đặt kích thước trang là A4
            Document doc = new Document(PageSize.A4);

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                Paragraph title = new Paragraph("Thông tin dữ liệu Bệnh nhân", vietnameseFont);
                title.Alignment = Element.ALIGN_CENTER; // Căn giữa văn bản
                doc.Add(title);
                doc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(5);

                // Đặt phần trăm chiều rộng của bảng để vừa với trang A4
                table.WidthPercentage = 100;

                table.AddCell(new Phrase("Họ tên", vietnameseFont));
                table.AddCell(new Phrase("Ngày sinh", vietnameseFont));
                table.AddCell(new Phrase("Địa chỉ", vietnameseFont));
                table.AddCell(new Phrase("Giới tính", vietnameseFont));
                table.AddCell(new Phrase("Số điện thoại", vietnameseFont));

                foreach (var data in patients)
                {
                    table.AddCell(new Phrase(data.HOTEN, vietnameseFont));
                    table.AddCell(new Phrase(data.NGAYSINH?.ToString("dd/MM/yyyy") ?? "", vietnameseFont)); // Định dạng ngaysinh là dd/MM/yyyy
                    table.AddCell(new Phrase(data.DIACHI, vietnameseFont));
                    table.AddCell(new Phrase(data.GIOITINH, vietnameseFont));
                    table.AddCell(new Phrase(data.SODIENTHOAI, vietnameseFont));
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "PatientData.pdf");
            }
        }

        public ActionResult ExportExcel()
        {
            var patients = _db.BENHNHANs.ToList();

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("PatientData");

                ws.Cells["A1"].Value = "Họ tên";
                ws.Cells["B1"].Value = "Ngày sinh";
                ws.Cells["C1"].Value = "Địa chỉ";
                ws.Cells["D1"].Value = "Giới tính";
                ws.Cells["E1"].Value = "Số điện thoại";

                // Áp dụng đường viền cho các ô tiêu đề
                using (var range = ws.Cells["A1:E1"])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                int rowStart = 2;
                foreach (var data in patients)
                {
                    ws.Cells[string.Format("A{0}", rowStart)].Value = data.HOTEN;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = data.NGAYSINH?.ToString("dd/MM/yyyy") ?? "";
                    ws.Cells[string.Format("C{0}", rowStart)].Value = data.DIACHI;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = data.GIOITINH;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = data.SODIENTHOAI;

                    // Áp dụng đường viền cho các ô dữ liệu
                    using (var range = ws.Cells[string.Format("A{0}:E{0}", rowStart)])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    rowStart++;
                }

                // Tự động điều chỉnh cột
                ws.Cells["A:E"].AutoFitColumns();

                // Chuyển đổi gói thành một mảng byte
                byte[] excelData = pck.GetAsByteArray();

                // Trả về tệp Excel
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PatientData.xlsx");
            }
        }
        public ActionResult Logout()
        {
            // Xóa phiên và xác thực biểu mẫu
            Session.Remove("UserRole");
            return RedirectToAction("Login", "Home", new { area = "" });
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

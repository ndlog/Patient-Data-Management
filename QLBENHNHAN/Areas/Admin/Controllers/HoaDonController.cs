using PagedList;
using QLBENHNHAN.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using System.Windows.Media.Media3D;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/HoaDon
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (Session["UserRole"] != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.DateSort = sortOrder == "" ? "date_asc" : "";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var hoaDons = from s in _db.HOADONs
                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    hoaDons = hoaDons.Where(s => s.idBN.Contains(searchString) ||
                                                s.BENHNHAN.HOTEN.Contains(searchString)
                                                || s.MAHD.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        hoaDons = hoaDons.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        hoaDons = hoaDons.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(hoaDons.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }
        public ActionResult Details(string id)
        {
            var details = _db.HOADONs.Find(id);
            if (details == null)
            {
                return HttpNotFound();
            }
            return View(details);
        }
        
        // POST: Admin/DuocPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToanTienMat(string id)
        {
            var hoaDon = _db.HOADONs.Find(id); // Fetch the invoice by ID
            if (hoaDon != null && hoaDon.TRANGTHAI == false)
            {
                hoaDon.TRANGTHAI = true;
                TempData["SuccessAdmin"] = "Thanh toán hóa đơn thành công";
                _db.SaveChanges(); 
            }
            return RedirectToAction("Details", new { id = id }); 
        }
        public ActionResult Create()
        {
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN");

            var hoaDons = new HOADON
            {
                TENHOADON = "Hóa đơn ngày " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            };

            return View(hoaDons);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HOADON model, string ChiTietHoaDon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.MAHD = GenerateUniqueID("LH");
                    model.TRANGTHAI = false;
                    model.NGAYTAO = DateTime.Now;
                    // Lưu dữ liệu danh sách thuốc vào trường CHITIETHOADON
                    model.CHITIETHOADON = ChiTietHoaDon;
                        
                    _db.HOADONs.Add(model);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm hóa đơn thành công";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    // Nếu có lỗi xảy ra, hiển thị thông báo lỗi và hiển thị lại form
                    TempData["ErrorAdmin"] = "Thêm hóa đơn thất bại: " + ex.Message;
                    return View(model);
                }
            }
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", model.idBN);

            return View(model);
        }
        public ActionResult SearchProducts(string keyword)
        {
            try
            {
                // Tìm kiếm dược phẩm dựa trên keyword
                var matchedProducts = _db.DUOCPHAMs.Where(p => p.TENDUOCPHAM.Contains(keyword)).ToList();

                // Tạo HTML cho danh sách kết quả tìm kiếm
                string htmlResult = "";
                foreach (var product in matchedProducts)
                {
                    htmlResult += "<li class='list-group-item' data-product-price='" + product.GIA + "'>" + product.TENDUOCPHAM + "</li>";
                }

                // Trả về kết quả tìm kiếm dưới dạng HTML
                return Content(htmlResult, "text/html");
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu có lỗi xảy ra
                return Content("Error: " + ex.Message);
            }
        }
        public ActionResult GetPrescriptions(string id)
        {
            var prescriptions = _db.DONTHUOCs.Where(d => d.idBN == id).ToList();
            return PartialView("_PrescriptionsPartial", prescriptions);
        }
        public ActionResult Edit(string id)
        {
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();

            var editing = _db.HOADONs.Find(id);
            if (editing == null)
            {
                return HttpNotFound();
            }

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", editing.idBN);
            return View(editing);
        }

        // POST: Admin/DuocPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HOADON model, string CHITIETHOADON)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.HOADONs.Find(model.MAHD);
                    if (editModel != null)
                    {
                        // Giữ lại giá trị của NGAYTAO
                        model.NGAYTAO = editModel.NGAYTAO;
                        editModel.TENHOADON = model.TENHOADON;
                        editModel.CHITIETHOADON = CHITIETHOADON ?? editModel.CHITIETHOADON;
                        editModel.TRANGTHAI = model.TRANGTHAI;

                        _db.Entry(editModel).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật hóa đơn thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi");
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật hóa đơn thất bại.";
                    // Cần gán lại ViewBag.idBN nếu model state không hợp lệ
                    var benhNhanList = _db.BENHNHANs.Select(p => new
                    {
                        MABN = p.MABN,
                        HOTEN = p.MABN + " - " + p.HOTEN
                    }).ToList();
                    ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", model.idBN);

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật hóa đơn thất bại: " + ex.Message;

                // Cần gán lại ViewBag.idBN nếu có exception
                var benhNhanList = _db.BENHNHANs.Select(p => new
                {
                    MABN = p.MABN,
                    HOTEN = p.MABN + " - " + p.HOTEN
                }).ToList();
                ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", model.idBN);

                return View(model);
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D10");

            while (_db.HOADONs.Any(item => item.MAHD == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D10");
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
using Antlr.Runtime.Misc;
using Newtonsoft.Json;
using PagedList;
using QLBENHNHAN.Models;
using QLBENHNHAN.Models.Validations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class DonThuocController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/DonThuoc
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string id)
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

                var donThuocs = from s in _db.DONTHUOCs
                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    donThuocs = donThuocs.Where(s => s.MADT.Contains(searchString) 
                                                    || s.idBN.Contains(searchString)
                                                    || s.BENHNHAN.HOTEN.Contains(searchString)
                                                    || s.TENDONTHUOC.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        donThuocs = donThuocs.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        donThuocs = donThuocs.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                //donThuocs = donThuocs.Where(s => s.BENHNHAN.MABN == id);    

                return View(donThuocs.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }
        public ActionResult Details(string id)
        {

            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();

            var details = _db.DONTHUOCs.Find(id);
            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", details.idBN);
            return View(details);
        }
        public ActionResult Create()
        {
            var model = new DONTHUOC
            {
                TENDONTHUOC = "Đơn thuốc ngày " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                
            };
            ViewBag.idDP = new SelectList(_db.DUOCPHAMs, "MADP", "TENDUOCPHAM");

            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();
            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN");
            
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DONTHUOC model, string ChiTietDonThuoc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.MADT = GenerateUniqueID("DT");
                    model.NGAYTAO = DateTime.Now;
                    // Lưu dữ liệu danh sách thuốc vào trường CHITIETHOADON
                    model.CHITIETDONTHUOC = ChiTietDonThuoc;

                    _db.DONTHUOCs.Add(model);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm đơn thuốc thành công";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm đơn thuốc thất bại: " + ex.Message;
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
                    htmlResult += "<li class='list-group-item'>" + product.TENDUOCPHAM + "</li>";
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
        public ActionResult Edit(string id)
        {
            var donThuoc = _db.DONTHUOCs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }

            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();
            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", donThuoc.idBN);

            return View(donThuoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DONTHUOC model, string CHITIETDONTHUOC)
        {
            if (ModelState.IsValid)
            {
                var existingDonThuoc = _db.DONTHUOCs.Find(model.MADT);
                if (existingDonThuoc != null)
                {
                    existingDonThuoc.TENDONTHUOC = model.TENDONTHUOC;
                    existingDonThuoc.idBN = model.idBN;
                    existingDonThuoc.CHITIETDONTHUOC = CHITIETDONTHUOC ?? existingDonThuoc.CHITIETDONTHUOC;
                    model.NGAYTAO = existingDonThuoc.NGAYTAO;

                    _db.Entry(existingDonThuoc).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Cập nhật đơn thuốc thành công.";

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorAdmin"] = "Cập nhật đơn thuốc thất bại.";
                    return HttpNotFound();
                }
            }

            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.HOTEN
            }).ToList();
            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", model.idBN);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.DONTHUOCs.Find(id);
                if (deleteModel != null)
                {
                    _db.DONTHUOCs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa đơn thuốc thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa đơn thuốc thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D10");

            while (_db.DONTHUOCs.Any(item => item.MADT == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D10");
            }
            return newID;

        }
    }
}
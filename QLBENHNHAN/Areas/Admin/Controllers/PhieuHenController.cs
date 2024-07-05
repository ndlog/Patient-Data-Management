using PagedList;
using QLBENHNHAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class PhieuHenController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/PhieuHen
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

                var phieuHens = from s in _db.PHIEUHENs
                              select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    phieuHens = phieuHens.Where(s => s.idBN.Contains(searchString) 
                                                || s.SODIENTHOAI.Contains(searchString)
                                                || s.BENHNHAN.HOTEN.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        phieuHens = phieuHens.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        phieuHens = phieuHens.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(phieuHens.ToPagedList(pageNumber, pageSize));
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
            var details = _db.PHIEUHENs.Find(id);

            if (details == null)
            {
                return HttpNotFound();
            }

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", details.idBN);

            return View(details);
        }
        [HttpGet]
        public JsonResult GetPhoneNumber(string idBN)
        {
            var phoneNumber = _db.BENHNHANs.Where(bn => bn.MABN == idBN).Select(bn => bn.SODIENTHOAI).FirstOrDefault();
            return Json(phoneNumber, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PHIEUHEN model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.MALH = GenerateUniqueID("LH");
                    model.ACTIVE = true;
                    model.NGAYTAO = DateTime.Now;

                    var selectedBN = _db.BENHNHANs.FirstOrDefault(bn => bn.MABN == model.idBN);
                    if (selectedBN != null)
                    {
                        model.SODIENTHOAI = selectedBN.SODIENTHOAI;
                    }
                    _db.PHIEUHENs.Add(model);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm lịch hẹn thành công";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    // Nếu có lỗi xảy ra, hiển thị thông báo lỗi và hiển thị lại form
                    TempData["ErrorAdmin"] = "Thêm lịch hẹn thất bại: " + ex.Message;
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
        public ActionResult Edit(string id)
        {
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();
            var editing = _db.PHIEUHENs.Find(id);

            if (editing == null)
            {
                return HttpNotFound();
            }

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", editing.idBN);

            return View(editing);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PHIEUHEN model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.PHIEUHENs.Find(model.MALH);
                    if (editModel != null)
                    {
                        // Giữ lại giá trị của NGAYTAO
                        model.NGAYTAO = editModel.NGAYTAO;

                        _db.Entry(editModel).CurrentValues.SetValues(model);
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật lịch hẹn thành công.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorAdmin"] = "Cập nhật lịch hẹn thất bại.";
                        return HttpNotFound();
                    }
                }
                else
                {
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
                TempData["ErrorAdmin"] = "Cập nhật lịch hẹn thất bại: " + ex.Message;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.PHIEUHENs.Find(id);
                if (deleteModel != null)
                {
                    _db.PHIEUHENs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa lịch hẹn thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa lịch hẹn thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D10");

            while (_db.PHIEUHENs.Any(item => item.MALH == newID))
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
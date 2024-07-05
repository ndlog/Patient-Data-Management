using PagedList;
using QLBENHNHAN.Models;
using QLBENHNHAN.Models.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class BenhAnController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/BenhAn
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

                var benhAns = from s in _db.CHITIETBENHANs
                              select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    benhAns = benhAns.Where(s => s.TENBENHAN.Contains(searchString) 
                                            || s.BENHNHAN.HOTEN.Contains(searchString)
                                            || s.idBN.Contains(searchString)
                                            || s.MABA.Contains(searchString) 
                                            || s.BACSI.HOTEN.Contains(searchString)
                                            || s.idBS.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        benhAns = benhAns.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        benhAns = benhAns.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(benhAns.ToPagedList(pageNumber, pageSize));
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

            ViewBag.BenhNhanList = new SelectList(benhNhanList, "MABN", "HOTEN");

            var bacSiList = _db.BACSIs.Select(p => new
            {
                MABS = p.MABS,
                HOTEN = p.MABS + " - " + p.HOTEN
            }).ToList();
            ViewBag.BacSiList = new SelectList(bacSiList, "MABS", "HOTEN");

            var details = _db.CHITIETBENHANs.Find(id);
            
            return View(details);
        }
        public ActionResult Create()
        {
            var idBN = TempData["idBN"] as string;
            
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();

            if (string.IsNullOrEmpty(idBN) && !benhNhanList.Any())
            {
                // Xử lý trường hợp không có idBN (có thể hiển thị lỗi hoặc chuyển hướng)
                TempData["ErrorAdmin"] = "Không tìm thấy mã bệnh nhân. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", idBN);

            var bacSiList = _db.BACSIs.Select(p => new
            {
                MABS = p.MABS,
                HOTEN = p.MABS + " - " + p.HOTEN
            }).ToList();

            ViewBag.idBS = new SelectList(bacSiList, "MABS", "HOTEN");

            ViewBag.DefaultDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.DefaultTime = DateTime.Now.ToString("HH:mm");
            ViewBag.DefaultValue = 0;
            ViewBag.DefaultThiLuc = "10/10";

            var model = new CHITIETBENHAN
            {
                TENBENHAN = "Thăm khám ngày " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                TIENSUBENH = "Không",
                CHUANDOANLAMSANG = "Bình thường",
                CHUANDOANCUOICUNG = "Bình thường",
                NGAY = DateTime.Now,
                idBN = idBN
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MABA,idBS,idBN,TENBENHAN,NGAY,GIO,MACH,THANNHIET,NHIPTHO,CHIEUCAO,CANNANG,MATTRAI,MATPHAI,NHANAPPHAI,NHANAPTRAI,TIENSUBENH,CHUANDOANLAMSANG,CHUANDOANCUOICUNG,NGAYTAO,TRANGTHAI")] CHITIETBENHAN model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.MABA = GenerateUniqueID("BA");
                    model.TRANGTHAI = "Đợi thăm khám";
                    model.NGAYTAO = DateTime.Now;
                    _db.CHITIETBENHANs.Add(model);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm bệnh án thành công";
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    // Nếu có lỗi xảy ra, hiển thị thông báo lỗi và hiển thị lại form
                    TempData["ErrorAdmin"] = "Thêm bệnh án thất bại: " + ex.Message;
                    return View(model);
                }
            }
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();

            ViewBag.idBN = new SelectList(benhNhanList, "MABN", "HOTEN", model.idBN);

            var bacSiList = _db.BACSIs.Select(p => new
            {
                MABS = p.MABS,
                HOTEN = p.MABS + " - " + p.HOTEN
            }).ToList();

            ViewBag.idBS = new SelectList(bacSiList, "MABS", "HOTEN", model.idBS);

            return View(model);
        }
        public ActionResult Edit(string id)
        {
            var editing = _db.CHITIETBENHANs.Find(id);
            if (editing == null)
            {
                return HttpNotFound();
            }
            var benhNhanList = _db.BENHNHANs.Select(p => new
            {
                MABN = p.MABN,
                HOTEN = p.MABN + " - " + p.HOTEN
            }).ToList();
            ViewBag.BenhNhanList = new SelectList(benhNhanList, "MABN", "HOTEN", editing.idBN);

            var bacSiList = _db.BACSIs.Select(p => new
            {
                MABS = p.MABS,
                HOTEN = p.MABS + " - " + p.HOTEN
            }).ToList();
            ViewBag.BacSiList = new SelectList(bacSiList, "MABS", "HOTEN", editing.idBS);

            ViewBag.TrangThaiList = new SelectList(new List<string> { "Đợi thăm khám", "Đang thăm khám", "Đã thăm khám" });
            return View(editing);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CHITIETBENHAN model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.CHITIETBENHANs.Find(model.MABA);
                    if (editModel != null)
                    {
                        model.NGAY = editModel.NGAY;
                        model.GIO = editModel.GIO;
                        model.NGAYTAO = editModel.NGAYTAO;
                        _db.Entry(editModel).CurrentValues.SetValues(model);
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật bệnh án thành công.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorAdmin"] = "Cập nhật bệnh án thất bại.";
                        return HttpNotFound();
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật bệnh án thất bại: " + ex.Message;
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.CHITIETBENHANs.Find(id);
                if (deleteModel != null)
                {
                    _db.CHITIETBENHANs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa bệnh án thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa bệnh án thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D5");

            while (_db.CHITIETBENHANs.Any(item => item.MABA == newID))
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
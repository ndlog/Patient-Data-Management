using PagedList;
using QLBENHNHAN.Models;
using QLBENHNHAN.Models.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class ThietBiController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/ThietBi
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

                var thietBis = from s in _db.THIETBIs
                               select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    thietBis = thietBis.Where(s => s.TENTHIETBI.Contains(searchString)
                                           || s.XUATXU.Contains(searchString)
                                           || s.MATB.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        thietBis = thietBis.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        thietBis = thietBis.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(thietBis.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }

        // GET: Admin/ThietBi/Details/5
        public ActionResult Details(string id)
        {
            var phongList = _db.PHONGs.Select(p => new {
                MAP = p.MAP,
                TENPHONG = p.TENPHONG + "  -  " + p.TANG
            }).ToList();
            ViewBag.PhongList = new SelectList(phongList, "MAP", "TENPHONG");

            var details = _db.THIETBIs.Find(id);
            if (details == null)
            {
                return HttpNotFound();
            }
            return View(details);
        }

        // GET: Admin/ThietBi/Create
        public ActionResult Create()
        {
            ViewBag.IdLTBList = new SelectList(_db.LOAITHIETBIs, "MALTB", "TENLOAITHIETBI");
            return View();
        }

        // POST: Admin/ThietBi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThietBiViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var thietBi = new THIETBI
                    {
                        MATB = GenerateUniqueID("TB"),
                        idP = model.idP,
                        idLTB = model.idLTB,
                        TENTHIETBI = model.TENTHIETBI,
                        MOTA = model.MOTA,
                        XUATXU = model.XUATXU,
                        TINHTRANG = "Không hoạt động",
                        NGAYTAO = DateTime.Now,
                    };
                    _db.THIETBIs.Add(thietBi);
                    _db.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm thiết bị thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Thêm thiết bị thất bại: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Admin/ThietBi/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.IdLTBList = new SelectList(_db.LOAITHIETBIs, "MALTB", "TENLOAITHIETBI");
            var phongList = _db.PHONGs.Select(p => new {
                MAP = p.MAP,
                TENPHONG = p.TENPHONG + "  -  " + p.TANG
            }).ToList();
            ViewBag.PhongList = new SelectList(phongList, "MAP", "TENPHONG");

            var editing = _db.THIETBIs.Find(id);
            var viewModel = new ThietBiViewModel
            {
                MATB = editing.MATB,
                idP = editing.idP,
                idLTB = editing.idLTB,
                TENTHIETBI = editing.TENTHIETBI,
                MOTA = editing.MOTA,
                XUATXU = editing.XUATXU,
                TINHTRANG = editing.TINHTRANG,
                NGAYTAO = (DateTime)editing.NGAYTAO,
            };
            return View(viewModel);
        }

        // POST: Admin/ThietBi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(THIETBI model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.THIETBIs.Find(model.MATB);
                    if (editModel != null)
                    {
                        if (model.NGAYTAO == null)
                        {
                            // Nếu không có dữ liệu mới từ trường NGAYTAO, giữ lại ngày tạo cũ
                            model.NGAYTAO = editModel.NGAYTAO;
                        }
                        _db.Entry(editModel).CurrentValues.SetValues(model);
                        _db.SaveChanges();
                        TempData["SuccessMessage"] = "Cập nhật thiết bị thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi");
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorMessage"] = "Cập nhật thiết bị thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Cập nhật thiết bị thất bại: " + ex.Message;
                return View(model);
            }

        }

        // POST: Admin/ThietBi/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.THIETBIs.Find(id);
                if (deleteModel != null)
                {
                    _db.THIETBIs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessMessage"] = "Xóa thiết bị thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Xóa thiết bị thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D5");

            while (_db.THIETBIs.Any(item => item.MATB == newID))
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
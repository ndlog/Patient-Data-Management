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
    public class LoaiThietBiController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/LoaiThietBi
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

                var loaiThietBis = from s in _db.LOAITHIETBIs
                                   select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    loaiThietBis = loaiThietBis.Where(s => s.TENLOAITHIETBI.Contains(searchString) || s.MALTB.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        loaiThietBis = loaiThietBis.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        loaiThietBis = loaiThietBis.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(loaiThietBis.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }

        // GET: Admin/LoaiThietBi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiThietBi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiThietBiViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loaiThietBi = new LOAITHIETBI
                    {
                        MALTB = GenerateUniqueID("LTB"),
                        TENLOAITHIETBI = model.TENLOAITHIETBI,
                        NGAYTAO = DateTime.Now,
                    };
                    _db.LOAITHIETBIs.Add(loaiThietBi);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm loại thiết bị thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm loại thiết bị thất bại: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Admin/LoaiThietBi/Edit/5
        public ActionResult Edit(string id)
        {
            var editing = _db.LOAITHIETBIs.Find(id);
            var viewModel = new LoaiThietBiViewModel
            {
                MALTB = editing.MALTB,
                TENLOAITHIETBI = editing.TENLOAITHIETBI,
                NGAYTAO = (DateTime)editing.NGAYTAO,
            };
            return View(viewModel);
        }

        // POST: Admin/LoaiThietBi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiThietBiViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.LOAITHIETBIs.Find(model.MALTB);
                    if (editModel != null)
                    {
                        editModel.TENLOAITHIETBI = model.TENLOAITHIETBI;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật loại thiết bị thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi", "Home", new { area = "Admin" });
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật loại thiết bị thất bại: " + ex.Message;
                return View(model);
            }
        }

        // POST: Admin/LoaiThietBi/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.LOAITHIETBIs.Find(id);
                if (deleteModel != null)
                {
                    _db.LOAITHIETBIs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa loại thiết bị thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi", "Home", new { area = "Admin"});
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa loại thiết bị thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D3");

            while (_db.LOAITHIETBIs.Any(item => item.MALTB == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D3");
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
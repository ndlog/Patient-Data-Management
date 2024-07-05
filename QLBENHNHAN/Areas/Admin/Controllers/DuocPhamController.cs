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
    public class DuocPhamController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/DuocPham
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

                var duocPhams = from s in _db.DUOCPHAMs
                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    duocPhams = duocPhams.Where(s => s.TENDUOCPHAM.Contains(searchString)
                                           || s.XUATXU.Contains(searchString) 
                                           || s.MADP.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        duocPhams = duocPhams.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        duocPhams = duocPhams.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(duocPhams.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }

        // GET: Admin/DuocPham/Details/5
        public ActionResult Details(string id)
        {
            var details = _db.DUOCPHAMs.Find(id);
            if (details == null)
            {
                return HttpNotFound();
            }
            return View(details);
        }
        // GET: Admin/DuocPham/Create
        public ActionResult Create()
        {
            ViewBag.LoaiDuocPhamList = new SelectList(_db.LOAIDUOCPHAMs, "MALDP", "TENLOAIDUOCPHAM");
            return View();
        }

        // POST: Admin/DuocPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DuocPhamViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var duocPham = new DUOCPHAM
                    {
                        MADP = GenerateUniqueID("DP"),
                        idLDP = model.idLDP,
                        TENDUOCPHAM = model.TENDUOCPHAM,
                        MOTA = model.MOTA,
                        XUATXU = model.XUATXU,
                        GIA = model.GIA,
                        NGAYTAO = DateTime.Now,
                        NGAYCAPNHAT = DateTime.Now,
                    };
                    _db.DUOCPHAMs.Add(duocPham);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm Dược Phẩm thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm Dược Phẩm thất bại: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
            
        }

        // GET: Admin/DuocPham/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.LoaiDuocPhamList = new SelectList(_db.LOAIDUOCPHAMs, "MALDP", "TENLOAIDUOCPHAM");
            var editing = _db.DUOCPHAMs.Find(id);
            var viewModel = new DuocPhamViewModel
            {
                MADP = editing.MADP,
                idLDP = editing.idLDP,
                TENDUOCPHAM = editing.TENDUOCPHAM,
                MOTA = editing.MOTA,
                XUATXU = editing.XUATXU,
                GIA = (int)editing.GIA,
                NGAYTAO = (DateTime)editing.NGAYTAO,
                NGAYCAPNHAT = (DateTime)editing.NGAYCAPNHAT,
            };
            return View(viewModel);
        }

        // POST: Admin/DuocPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DuocPhamViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.DUOCPHAMs.Find(model.MADP);
                    if (editModel != null)
                    {
                        editModel.TENDUOCPHAM = model.TENDUOCPHAM;
                        editModel.idLDP = model.idLDP;
                        editModel.MOTA = model.MOTA;
                        editModel.XUATXU = model.XUATXU;
                        editModel.GIA = model.GIA;
                        editModel.NGAYCAPNHAT = DateTime.Now;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật Dược Phẩm thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi");
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật Dược Phẩm thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật Dược Phẩm thất bại: " + ex.Message;
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.DUOCPHAMs.Find(id);
                if (deleteModel != null)
                {
                    _db.DUOCPHAMs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa Dược Phẩm thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa Dược Phẩm thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D5");

            while (_db.DUOCPHAMs.Any(item => item.MADP == newID))
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
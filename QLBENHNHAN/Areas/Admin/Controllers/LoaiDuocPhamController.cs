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
    public class LoaiDuocPhamController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/LoaiDuocPham
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

                var loaiDuocPhams = from s in _db.LOAIDUOCPHAMs
                                    select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    loaiDuocPhams = loaiDuocPhams.Where(s => s.TENLOAIDUOCPHAM.Contains(searchString) || s.MALDP.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        loaiDuocPhams = loaiDuocPhams.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        loaiDuocPhams = loaiDuocPhams.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(loaiDuocPhams.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            
        }

        // GET: Admin/LoaiDuocPham/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiDuocPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiDuocPhamViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loaiDuocPham = new LOAIDUOCPHAM
                    {
                        MALDP = GenerateUniqueID("LDP"),
                        TENLOAIDUOCPHAM = model.TENLOAIDUOCPHAM,
                        NGAYTAO = DateTime.Now
                    };

                    _db.LOAIDUOCPHAMs.Add(loaiDuocPham);
                    _db.SaveChanges();

                    TempData["SuccessAdmin"] = "Thêm loại phòng thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm loại phòng thất bại: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Admin/LoaiDuocPham/Edit/5
        public ActionResult Edit(string id)
        {
            var editing = _db.LOAIDUOCPHAMs.Find(id);
            var viewModel = new LoaiDuocPhamViewModel
            {
                MALDP = editing.MALDP,
                TENLOAIDUOCPHAM = editing.TENLOAIDUOCPHAM,
                NGAYTAO = (DateTime)editing.NGAYTAO
            };
            return View(viewModel);
        }

        // POST: Admin/LoaiDuocPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiDuocPhamViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.LOAIDUOCPHAMs.Find(model.MALDP);
                    if (editModel != null)
                    {
                        editModel.TENLOAIDUOCPHAM = model.TENLOAIDUOCPHAM;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật Loại Dược Phẩm thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi");
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật Loại Dược Phẩm thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật Loại Dược Phẩm thất bại: " + ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.LOAIDUOCPHAMs.Find(id);
                if (deleteModel != null)
                {
                    _db.LOAIDUOCPHAMs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa Loại Dược Phẩm thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa Loại Dược Phẩm thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D3");

            while (_db.LOAIDUOCPHAMs.Any(item => item.MALDP == newID))
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
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
    public class LoaiPhongController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();

        // GET: Admin/LoaiPhong
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

                var loaiPhongs = from s in _db.LOAIPHONGs
                                 select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    loaiPhongs = loaiPhongs.Where(s => s.TENLOAIPHONG.Contains(searchString) || s.MALP.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        loaiPhongs = loaiPhongs.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        loaiPhongs = loaiPhongs.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(loaiPhongs.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            
        }

        // GET: Admin/LoaiPhong/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiPhong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiPhongViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loaiPhong = new LOAIPHONG
                    {
                        MALP = GenerateUniqueID("LP"),
                        TENLOAIPHONG = model.TENLOAIPHONG,
                        NGAYTAO = DateTime.Now
                    };

                    _db.LOAIPHONGs.Add(loaiPhong);
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

        // GET: Admin/LoaiPhong/Edit/5
        public ActionResult Edit(string id)
        {
            var editing = _db.LOAIPHONGs.Find(id);
            var viewModel = new LoaiPhongViewModel
            {
                MALP = editing.MALP,
                TENLOAIPHONG = editing.TENLOAIPHONG,
                NGAYTAO = (DateTime)editing.NGAYTAO
            };
            return View(viewModel);
        }

        // POST: Admin/LoaiPhong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiPhongViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.LOAIPHONGs.Find(model.MALP);
                    if (editModel != null)
                    {
                        editModel.TENLOAIPHONG = model.TENLOAIPHONG;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật loại phòng thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi", "Home", new {area = "Admin"});
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật loại phòng thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật loại phòng thất bại: " + ex.Message;
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.LOAIPHONGs.Find(id);
                if (deleteModel != null)
                {
                    _db.LOAIPHONGs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa loại phòng thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa loại phòng thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D2");

            while (_db.LOAIPHONGs.Any(item => item.MALP == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D2");
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
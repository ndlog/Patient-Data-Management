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
    public class KhoaController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/Khoa
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

                var Khoas = from s in _db.KHOAs
                            select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    Khoas = Khoas.Where(s => s.TENKHOA.Contains(searchString) || s.MAK.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        Khoas = Khoas.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        Khoas = Khoas.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(Khoas.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            
        }
        // GET: Admin/Khoa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Khoa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhoaViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var khoa = new KHOA
                    {
                        MAK = GenerateUniqueID("K"),
                        TENKHOA = model.TENKHOA,
                        NGAYTAO = DateTime.Now,
                    };
                    _db.KHOAs.Add(khoa);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm Khoa thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm Khoa thất bại: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Admin/Khoa/Edit/5
        public ActionResult Edit(string id)
        {
            var editing = _db.KHOAs.Find(id);
            var viewModel = new KhoaViewModel
            {
                MAK = editing.MAK,
                TENKHOA = editing.TENKHOA,
                NGAYTAO = (DateTime)editing.NGAYTAO,
            };
            return View(viewModel);
        }

        // POST: Admin/Khoa/Edit/5
        [HttpPost]
        public ActionResult Edit(KhoaViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.KHOAs.Find(model.MAK);
                    if (editModel != null)
                    {
                        editModel.TENKHOA = model.TENKHOA;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật Khoa thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi", "Home", new { area = "Admin"});
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật Khoa thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật Khoa thất bại: " + ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.KHOAs.Find(id);
                if (deleteModel != null)
                {
                    _db.KHOAs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa Khoa thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi", "Home", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa Khoa thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D2");

            while (_db.KHOAs.Any(item => item.MAK == newID))
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
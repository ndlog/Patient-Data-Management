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
    //[Authorize(Roles = "Admin")]
    public class BacSiController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/BacSi
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

                var bacsis = from s in _db.BACSIs
                             select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    bacsis = bacsis.Where(s => s.HOTEN.Contains(searchString)
                                           || s.SODIENTHOAI.Contains(searchString)
                                           || s.KHOA.TENKHOA.Contains(searchString)
                                           || s.MABS.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        bacsis = bacsis.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        bacsis = bacsis.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(bacsis.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }

        // GET: Admin/BacSi/Details/5
        public ActionResult Details(string id)
        {
            var details = _db.BACSIs.Find(id);
            if (details == null)
            {
                return HttpNotFound();
            }
            return View(details);
        }

        // GET: Admin/BacSi/Create
        public ActionResult Create()
        {
            ViewBag.KhoaList = new SelectList(_db.KHOAs, "MAK", "TENKHOA");
            return View();
        }

        // POST: Admin/BacSi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BacSiViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var bacSi = new BACSI
                    {
                        MABS = GenerateUniqueID("BS"),
                        HOTEN = model.HOTEN,
                        SODIENTHOAI = model.SODIENTHOAI,
                        DIACHI = model.DIACHI,
                        GIOITINH = model.GIOITINH,
                        GIOITHIEU = model.GIOITHIEU,
                        idK = model.idK,
                        idTK = model.idTK,
                        NGAYTAO = DateTime.Now,
                        NGAYCAPNHAT = DateTime.Now,
                    };
                    
                    _db.BACSIs.Add(bacSi);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm Bác Sĩ thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm Bác Sĩ thất bại: " + ex.Message;
                    return View(model);
                }

            }
            return View(model);
            
        }

        // GET: Admin/BacSi/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.KhoaList = new SelectList(_db.KHOAs, "MAK", "TENKHOA");
            var editing = _db.BACSIs.Find(id);
            var viewModel = new BacSiViewModel
            {
                MABS = editing.MABS,
                HOTEN = editing.HOTEN,
                SODIENTHOAI = editing.SODIENTHOAI,
                DIACHI = editing.DIACHI,
                GIOITINH = editing.GIOITINH,
                GIOITHIEU = editing.GIOITHIEU,
                idK = editing.idK,
                idTK = editing.idTK,
                NGAYTAO = (DateTime)editing.NGAYTAO,
                NGAYCAPNHAT = (DateTime)editing.NGAYCAPNHAT
            };
            return View(viewModel);
        }

        // POST: Admin/BacSi/Edit/5
        [HttpPost]
        public ActionResult Edit(BacSiViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.BACSIs.Find(model.MABS);
                    if (editModel != null)
                    {
                        editModel.HOTEN = model.HOTEN;
                        editModel.SODIENTHOAI = model.SODIENTHOAI;
                        editModel.DIACHI = model.DIACHI;
                        editModel.GIOITINH = model.GIOITINH;
                        editModel.GIOITHIEU = model.GIOITHIEU;
                        editModel.idK = model.idK;
                        editModel.idTK = model.idTK;
                        editModel.NGAYCAPNHAT = DateTime.Now;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật Bác Sĩ thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi", "Home", new {area = "Admin"});
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật Bác Sĩ thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật Bác Sĩ thất bại: " + ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.BACSIs.Find(id);
                if (deleteModel != null)
                {
                    _db.BACSIs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa Bác Sĩ thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi", "Home", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa Bác Sĩ thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            // Lấy giá trị bộ đếm cao nhất từ ID hiện có
            var lastID = _db.BACSIs
                .Where(b => b.MABS.StartsWith(prefix))
                .OrderByDescending(b => b.MABS)
                .Select(b => b.MABS)
                .FirstOrDefault();

            int counter = 1;

            if (lastID != null)
            {
                var numericPart = lastID.Substring(prefix.Length + 1);
                counter = int.Parse(numericPart) + 1;
            }
            // Định dạng ID mới với tiền tố và bộ đếm tăng dần
            string newID = $"{prefix}-{counter:D5}";

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
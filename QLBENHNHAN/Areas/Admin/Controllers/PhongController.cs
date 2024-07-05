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
    public class PhongController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/Phong
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

                var phongs = from s in _db.PHONGs
                             select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    phongs = phongs.Where(s => s.TENPHONG.Contains(searchString) 
                                            || s.TANG.Contains(searchString) 
                                            || s.MAP.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        phongs = phongs.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        phongs = phongs.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(phongs.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }

        // GET: Admin/Phong/Details/5
        public ActionResult Details(string id)
        {
            var details = _db.PHONGs.Find(id);
            if (details == null)
            {
                return HttpNotFound();
            }
            return View(details);
        }

        // GET: Admin/Phong/Create
        public ActionResult Create()
        {
            ViewBag.LoaiPhongList = new SelectList(_db.LOAIPHONGs, "MALP", "TENLOAIPHONG");
            return View();
        }

        // POST: Admin/Phong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhongViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var phong = new PHONG
                    {
                        MAP = GenerateUniqueID("P"),
                        TENPHONG = model.TENPHONG,
                        idLP = model.idLP,
                        TANG = model.TANG,
                        NGAYTAO = DateTime.Now,
                    };
                    
                    _db.PHONGs.Add(phong);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm phòng thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm phòng thất bại: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Admin/Phong/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.LoaiPhongList = new SelectList(_db.LOAIPHONGs, "MALP", "TENLOAIPHONG");
            var editing = _db.PHONGs.Find(id);
            var viewModel = new PhongViewModel
            {
                MAP = editing.MAP,
                TENPHONG = editing.TENPHONG,
                idLP = editing.idLP,
                TANG = editing.TANG,
                NGAYTAO = (DateTime)editing.NGAYTAO,
            };
            return View(viewModel);
        }

        // POST: Admin/Phong/Edit/5
        [HttpPost]
        public ActionResult Edit(PhongViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.PHONGs.Find(model.MAP);
                    if (editModel != null)
                    {
                        editModel.TENPHONG = model.TENPHONG;
                        editModel.idLP = model.idLP;
                        editModel.TANG = model.TANG;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật phòng thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi", "Home", new { area = "Admin" });
                }
                else
                {
                    // Nếu dữ liệu mô hình không hợp lệ, quay lại view chỉnh sửa và hiển thị các lỗi
                    TempData["ErrorAdmin"] = "Cập nhật phòng thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật phòng thất bại: " + ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.PHONGs.Find(id);
                if (deleteModel != null)
                {
                    _db.PHONGs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa phòng thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa phòng thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D4");

            while (_db.PHONGs.Any(item => item.MAP == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D4");
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
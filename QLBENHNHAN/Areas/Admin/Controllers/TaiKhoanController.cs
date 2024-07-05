using PagedList;
using QLBENHNHAN.Models;
using QLBENHNHAN.Models.Validations;
using QLDULIEUBENHNAN;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();

        // GET: Admin/TaiKhoan
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (Session["UserRole"] != null)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var taiKhoans = from s in _db.TAIKHOANs
                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    taiKhoans = taiKhoans.Where(s => s.USERNAME.Contains(searchString)
                                                || s.MATK.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        taiKhoans = taiKhoans.OrderBy(s => s.NGAYTAO);
                        break;
                    default:  // date descending 
                        taiKhoans = taiKhoans.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(taiKhoans.ToPagedList(pageNumber, pageSize)); ;
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
        }

        public ActionResult Authorization(string id)
        {
            var authorization = _db.TAIKHOANs.Find(id);
            var viewModel = new TaiKhoanViewModel
            {
                MATK = authorization.MATK,
                USERNAME = authorization.USERNAME,
                PASSWORD = authorization.PASSWORD,
                ConfirmPassword = authorization.PASSWORD,
                ACTIVE = (bool)authorization.ACTIVE,
                VAITRO = authorization.VAITRO,
                EMAIL = authorization.EMAIL,
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorization(TaiKhoanViewModel model)
        {
            try
            {
                // Chỉ cần xác thực các trường VAITRO và ACTIVE
                if (ModelState.IsValidField("VAITRO") && ModelState.IsValidField("ACTIVE"))
                {
                    var authorization = _db.TAIKHOANs.Find(model.MATK);
                    if (authorization != null)
                    {
                        // Chỉ cập nhật các trường VAITRO và ACTIVE
                        authorization.VAITRO = model.VAITRO;
                        authorization.ACTIVE = model.ACTIVE;
                        authorization.NGAYCAPNHAT = DateTime.Now;

                        _db.SaveChanges();

                        TempData["SuccessAdmin"] = "Phân quyền tài khoản thành công.";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Loi", "Home", new { area = "Admin" });
                }
                else
                {
                    // Hiển thị chi tiết các lỗi xác thực
                    TempData["ErrorAdmin"] = "Phân quyền tài khoản thất bại. Dữ liệu không hợp lệ!";
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        TempData["ErrorAdmin"] += " " + error.ErrorMessage;
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Phân quyền tài khoản thất bại: " + ex.Message;
                return View(model);
            }
        }

        // GET: Admin/TaiKhoan/Create
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

        // POST: Admin/TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaiKhoanViewModel model, string idBN)
        {
            if (ModelState.IsValidField("MATK") && ModelState.IsValidField("USERNAME") && ModelState.IsValidField("PASSWORD") && ModelState.IsValidField("ACTIVE") && ModelState.IsValidField("VAITRO") && ModelState.IsValidField("MACODE") && ModelState.IsValidField("EMAIL") && ModelState.IsValidField("NGAYTAO") && ModelState.IsValidField("USERNAME"))
            {
                var chkUser = _db.TAIKHOANs.FirstOrDefault(s => s.USERNAME == model.USERNAME);
                if (chkUser == null)
                {
                    var keyNew = Helper.GeneratePassword(10);
                    var password = Helper.EncodePassword(model.PASSWORD, keyNew);
                    var taiKhoan = new TAIKHOAN
                    {
                        MATK = GenerateUniqueIDTK("TK"),
                        USERNAME = model.USERNAME,
                        PASSWORD = password,
                        ACTIVE = true,
                        VAITRO = model.VAITRO,
                        MACODE = keyNew,
                        EMAIL = model.EMAIL,
                        NGAYTAO = DateTime.Now,
                    };
                    _db.TAIKHOANs.Add(taiKhoan);

                    if (!string.IsNullOrEmpty(idBN))
                    {
                        // Cập nhật idTK của bệnh nhân đã chọn bằng tài khoản MATK mới
                        var benhNhan = _db.BENHNHANs.FirstOrDefault(b => b.MABN == idBN);
                        if (benhNhan != null)
                        {
                            benhNhan.idTK = taiKhoan.MATK;
                        }
                    }
                    else
                    {
                        if (taiKhoan.VAITRO == "Customer")
                        {
                            //var benhNhan = new BENHNHAN
                            //{
                            //    MABN = GenerateUniqueIDBN("BN"),
                            //    idTK = taiKhoan.MATK,
                            //};
                            //_db.BENHNHANs.Add(benhNhan);
                        }
                        else if (taiKhoan.VAITRO == "Doctor")
                        {
                            var bacSi = new BACSI
                            {
                                MABS = GenerateUniqueIDBS("BS"),
                                idTK = taiKhoan.MATK
                            };
                            _db.BACSIs.Add(bacSi);
                        }
                    }

                    _db.SaveChanges();
                    ModelState.Clear();
                    TempData["SuccessAdmin"] = "Thêm tài khoản thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorAdmin"] = "Người dùng đã tồn tại!!";
                    return View(model);
                }
            }
            else
            {
                TempData["ErrorAdmin"] = "Dữ liệu không hợp lệ!";
                return View(model);
            }
        }

        // GET: Admin/TaiKhoan/Edit/5
        public ActionResult Edit(string id)
        {
            var editing = _db.TAIKHOANs.Find(id);
            var viewModel = new TaiKhoanViewModel
            {
                MATK = editing.MATK,
                USERNAME = editing.USERNAME,
                PASSWORD = editing.PASSWORD,
                MACODE = editing.MACODE,
                EMAIL = editing.EMAIL,
            };
            return View(viewModel);
        }

        // POST: Admin/TaiKhoan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaiKhoanViewModel model)
        {
            try
            {
                // Kiểm tra xem dữ liệu mô hình có hợp lệ không
                if (ModelState.IsValidField("MATK") && ModelState.IsValidField("PASSWORD") && ModelState.IsValidField("NewPassword") && ModelState.IsValidField("ConfirmNewPassword")
                    && ModelState.IsValidField("EMAIL") && ModelState.IsValidField("NGAYCAPNHAT") && ModelState.IsValidField("MACODE"))
                {
                    var editModel = _db.TAIKHOANs.Find(model.MATK);
                    if (editModel != null)
                    {
                        editModel.EMAIL = model.EMAIL;
                        editModel.NGAYCAPNHAT = DateTime.Now;
                        if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.ConfirmNewPassword)
                        {
                            var keyNew = Helper.GeneratePassword(10);
                            editModel.PASSWORD = Helper.EncodePassword(model.NewPassword, keyNew);
                            editModel.MACODE = keyNew;
                        }

                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật tài khoản thành công.";
                        return RedirectToAction("Index");
                    }
                    return HttpNotFound();
                }
                else
                {
                    TempData["ErrorAdmin"] = "Cập nhật tài khoản thất bại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Cập nhật tài khoản thất bại: " + ex.Message;
                return View(model);
            }
        }

        // GET: Admin/TaiKhoan/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.TAIKHOANs.Find(id);
                if (deleteModel != null)
                {
                    _db.TAIKHOANs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa tài khoản thành công.";
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa tài khoản thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueIDTK(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D5");

            while (_db.TAIKHOANs.Any(item => item.MATK == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D5");
            }

            return newID;

        }
        public string GenerateUniqueIDBN(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D5");

            while (_db.BENHNHANs.Any(item => item.MABN == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D5");
            }

            return newID;

        }
        public string GenerateUniqueIDBS(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D5");

            while (_db.BACSIs.Any(item => item.MABS == newID))
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
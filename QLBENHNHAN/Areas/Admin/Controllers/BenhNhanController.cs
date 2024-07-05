using PagedList;
using QLBENHNHAN.Models;
using QLBENHNHAN.Models.Validations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace QLBENHNHAN.Areas.Admin.Controllers
{
    public class BenhNhanController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        // GET: Admin/BenhNhan
        
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

                var benhNhans = from s in _db.BENHNHANs
                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    benhNhans = benhNhans.Where(s => s.HOTEN.Contains(searchString)
                                           || s.MABN.Contains(searchString)
                                           || s.SODIENTHOAI.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "date_asc":
                        benhNhans = benhNhans.OrderBy(s => s.NGAYTAO);
                        break;
                    default:
                        benhNhans = benhNhans.OrderByDescending(s => s.NGAYTAO);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(benhNhans.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }

        }
        // GET: Admin/BenhNhan/Details/5
        public ActionResult Details(string id)
        {
            ViewBag.PhongList = new SelectList(_db.PHONGs, "MAP", "TENPHONG");
            var details = _db.BENHNHANs.Find(id);
            return View(details);
        }
        // GET: Admin/BenhNhan/Create
        public ActionResult Create()
        {
           return View();
        }

        // POST: Admin/BenhNhan/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BenhNhanViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var benhNhan = new BENHNHAN
                    {
                        MABN = GenerateUniqueID("BN"),
                        HOTEN = model.HOTEN,
                        NGAYSINH = model.NGAYSINH, 
                        GIOITINH = model.GIOITINH, 
                        SODIENTHOAI = model.SODIENTHOAI,
                        DIACHI = model.DIACHI,
                        idP = model.idP,
                        idTK = model.idTK,
                        NGAYTAO = DateTime.Now,
                        NGAYCAPNHAT = DateTime.Now,
                    };
                    _db.BENHNHANs.Add(benhNhan);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Thêm bệnh nhân thành công.";
                    TempData["idBN"] = benhNhan.MABN;
                    return RedirectToAction("Create", "BenhAn", new { area = "Admin" });
                }
                catch (Exception ex)
                {
                    TempData["ErrorAdmin"] = "Thêm bệnh nhân thất bại: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
        }
        
        public ActionResult GetRoomsByFloor(string floor)
        {
            var rooms = _db.PHONGs.Where(p => p.TANG == floor).ToList();
            var roomList = rooms.Select(r => new { Value = r.MAP, Text = r.TENPHONG }).ToList();
            return Json(roomList, JsonRequestBehavior.AllowGet);
        }
        // GET: Admin/BenhNhan/Edit/5
        
        public ActionResult Edit(string id)
        {

            var editing = _db.BENHNHANs.Find(id);

            var viewModel = new BenhNhanViewModel
            {
                MABN = editing.MABN,
                HOTEN = editing.HOTEN,
                NGAYSINH = (DateTime)editing.NGAYSINH,
                GIOITINH = editing.GIOITINH,
                SODIENTHOAI = editing.SODIENTHOAI,
                DIACHI = editing.DIACHI,
                idP = editing.idP,
                idTK = editing.idTK,
                NGAYTAO = (DateTime)editing.NGAYTAO,
                NGAYCAPNHAT = (DateTime)editing.NGAYCAPNHAT,
            };
            ViewBag.PhongList = new SelectList(_db.PHONGs, "MAP", "TENPHONG", editing.idP);

            return View(viewModel);
        }
        // POST: Admin/BenhNhan/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BenhNhanViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var editModel = _db.BENHNHANs.Find(model.MABN);
                    if (editModel != null)
                    {
                        editModel.HOTEN = model.HOTEN;
                        editModel.NGAYSINH = model.NGAYSINH;
                        editModel.GIOITINH = model.GIOITINH;
                        editModel.SODIENTHOAI = model.SODIENTHOAI;
                        editModel.DIACHI = model.DIACHI;
                        editModel.idP = model.idP;
                        editModel.idTK = model.idTK;
                        editModel.NGAYCAPNHAT = DateTime.Now;
                        _db.SaveChanges();
                        TempData["SuccessAdmin"] = "Cập nhật bệnh nhân thành công.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorAdmin"] = "Cập nhật bệnh nhân thất bại.";
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
                TempData["ErrorAdmin"] = "Cập nhật bệnh nhân thất bại: " + ex.Message;
                return View(model);
            }
        }

        // POST: Admin/BenhNhan/Delete/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteModel = _db.BENHNHANs.Find(id);
                if (deleteModel != null)
                {
                    _db.BENHNHANs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["SuccessAdmin"] = "Xóa bệnh nhân thành công.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["ErrorAdmin"] = "Xóa bệnh nhân thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueID(string prefix)
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
using QLBENHNHAN.Models;
using QLDULIEUBENHNAN;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using QLBENHNHAN.Models.Validations;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.IO;

namespace QLBENHNHAN.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLDULIEUBENHNHANEntities _db = new QLDULIEUBENHNHANEntities();
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            try
            {
                using (var context = new QLDULIEUBENHNHANEntities())
                {
                    var getUser = (from s in context.TAIKHOANs where s.USERNAME == username select s).FirstOrDefault();
                    if (getUser != null)
                    {
                        if (getUser.ACTIVE == true) // Kiểm tra trường active
                        {
                            var hashCode = getUser.MACODE;
                            // Quy trình băm mật khẩu bằng class Helper  
                            var encodingPasswordString = Helper.EncodePassword(password, hashCode);
                            // Kiểm tra chi tiết đăng nhập username hoặc password 
                            var query = (from s in context.TAIKHOANs where (s.USERNAME == username) && s.PASSWORD.Equals(encodingPasswordString) select s).FirstOrDefault();
                            if (query != null)
                            {
                                // ghi đè ngày cuối cùng truy cập
                                query.NGAYTRUYCAP = DateTime.Now;
                                context.SaveChanges();

                                Session["UserID"] = query.MATK;
                                Session["UserRole"] = query.VAITRO;

                                if ((string)Session["UserRole"] == "Admin" || (string)Session["UserRole"] == "Doctor")
                                {
                                    // Nếu vai trò là Admin hoặc Doctor, chuyển hướng đến trang quản trị
                                    Session["UserNameAdmin"] = username;
                                    TempData["SuccessAdmin"] = "Đăng nhập thành công";
                                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                                }
                                else if ((string)Session["UserRole"] == "Customer")
                                {
                                    // Nếu vai trò là Customer, chuyển hướng đến trang khách hàng
                                    Session["UserNameCustomer"] = username;
                                    TempData["Success"] = "Đăng nhập thành công";
                                    return RedirectToAction("Index", "Home", new { area = "" });
                                }
                            }
                            TempData["Error"] = "Username hoặc Password không chính xác";
                            return View();
                        }
                        TempData["Error"] = "Tài khoản của bạn đã bị vô hiệu hóa. Liên hệ với quản trị viên để biết thêm chi tiết.";
                        return View();
                    }
                    TempData["Error"] = "Vui lòng kiểm tra thông tin!";
                    return View();
                }
            }
            catch (Exception e)
            {
                TempData["Error"] = "Đã có lỗi gì đó xảy ra, hãy liên hệ với Quản trị viên" + e.Message;
                return View();
            }
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TaiKhoanViewModel model)
        {
            if (ModelState.IsValidField("USERNAME") &&
                ModelState.IsValidField("PASSWORD") &&
                ModelState.IsValidField("ConfirmPassword"))
                {
                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        var chkUser = (from s in context.TAIKHOANs where s.USERNAME == model.USERNAME select s).FirstOrDefault();
                        if (chkUser == null)
                        {
                            var keyNew = Helper.GeneratePassword(10);
                            var password = Helper.EncodePassword(model.PASSWORD, keyNew);
                            var newUser = new TAIKHOAN
                            {
                                MATK = GenerateUniqueIDTK("TK"),
                                USERNAME = model.USERNAME,
                                PASSWORD = password,
                                EMAIL = model.EMAIL,
                                NGAYTAO = DateTime.Now,
                                MACODE = keyNew,
                                VAITRO = "Customer",
                                ACTIVE = false, // tạm thời để true - sau này sẽ check confirm
                            };

                            var newBenhNhan = new BENHNHAN
                            {
                                MABN = GenerateUniqueIDBN("BN"),
                                HOTEN = null,
                                SODIENTHOAI = null,
                                GIOITINH = null,
                                NGAYSINH = null,
                                DIACHI = null,
                                NGAYTAO = DateTime.Now,
                                idTK = newUser.MATK
                            };
                            // Lấy verificationLink từ hàm GenerateVerificationLink
                            string verificationLink = GenerateVerificationLink(newUser.EMAIL, newUser.MACODE);
                            // Gửi email xác thực
                            SendConfirmationEmail(newUser.EMAIL, newUser.USERNAME, verificationLink);

                            context.BENHNHANs.Add(newBenhNhan);
                            context.TAIKHOANs.Add(newUser);
                            context.SaveChanges();
                            ModelState.Clear();
                            TempData["Success"] = "Đăng ký thành công!";
                            return RedirectToAction("WaitingForVerification", "Home");
                        }
                        TempData["Error"] = "Người dùng đã tồn tại!!";
                        return View();
                    }
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Đã có lỗi xảy ra " + e;
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }
        private string GenerateVerificationLink(string email, string macode)
        {
            // Tạo ra đường dẫn xác nhận với email và mã code
            string baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";
            string verificationUrl = $"{baseUrl}/Home/VerifyEmail?email={email}&macode={macode}";
            return verificationUrl;
        }
        private void SendConfirmationEmail(string email, string username, string verificationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("VLU Heart", "vluheart@gmail.com"));
            message.To.Add(new MailboxAddress("Recipient Name", email));
            message.Subject = "Xác nhận đăng ký tài khoản";

            var emailModel = new EmailTemplateViewModel
            {
                Username = username,
                VerificationLink = verificationLink
            };

            string emailBody = RenderRazorViewToString("EmailTemplate", emailModel);

            var builder = new BodyBuilder { HtmlBody = emailBody };
            message.Body = builder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 587, false); // Connect to Gmail SMTP server
                    client.Authenticate("vluheart@gmail.com", "ugnv aeld dbbk oppd"); // Authenticate with your Gmail account
                    client.Send(message); // Send the email
                    client.Disconnect(true); // Disconnect from the server
                }
                catch (Exception ex)
                {
                    // Handle exceptions when sending email fails
                    throw new ApplicationException($"Đã xảy ra lỗi khi gửi email xác nhận: {ex.Message}");
                }
            }
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult VerifyEmail(string email, string macode)
        {
            using (var context = new QLDULIEUBENHNHANEntities())
            {
                // Tìm tài khoản có email và token tương ứng trong cơ sở dữ liệu
                var user = context.TAIKHOANs.SingleOrDefault(u => u.EMAIL == email && u.MACODE == macode);

                if (user != null)
                {
                    // Xác nhận email của người dùng và cập nhật trạng thái ACTIVE
                    user.ACTIVE = true;
                    context.SaveChanges();

                    TempData["Success"] = "Xác nhận email thành công!";
                    return RedirectToAction("Login", "Home", new { area = "" });
                }
                else
                {
                    TempData["Error"] = "Liên kết xác nhận không hợp lệ.";
                    return HttpNotFound(); // Hoặc chuyển hướng tới trang lỗi nếu không hợp lệ
                }
            }
        }
        public ActionResult ConfirmEmail(string email)
        {
            ViewBag.Email = email;
            return View();
        }
        public ActionResult WaitingForVerification()
        {
            return View();
        }
        private string GenerateResetPasswordLink(string email, string macode)
        {
            string baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";
            string resetPasswordUrl = $"{baseUrl}/Home/ResetPassword?email={email}&macode={macode}";
            return resetPasswordUrl;
        }
        private void SendResetPasswordEmail(string email, string username, string resetPasswordLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("VLU Heart", "vluheart@gmail.com"));
            message.To.Add(new MailboxAddress("Recipient Name", email));
            message.Subject = "Yêu cầu đặt lại mật khẩu";

            var emailModel = new EmailTemplateViewModel
            {
                Username = username,
                VerificationLink = resetPasswordLink
            };

            string emailBody = RenderRazorViewToString("ResetPasswordEmailTemplate", emailModel);

            var builder = new BodyBuilder { HtmlBody = emailBody };
            message.Body = builder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("vluheart@gmail.com", "ugnv aeld dbbk oppd");
                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Đã xảy ra lỗi khi gửi email đặt lại mật khẩu: {ex.Message}");
                }
            }
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            using (var context = new QLDULIEUBENHNHANEntities())
            {
                var user = context.TAIKHOANs.SingleOrDefault(u => u.EMAIL == email);

                if (user != null)
                {
                    var macode = Guid.NewGuid().ToString();
                    user.MACODE = macode;
                    user.NGAYCAPNHAT = DateTime.Now;
                    context.SaveChanges();

                    var resetPasswordLink = GenerateResetPasswordLink(user.EMAIL, macode);
                    SendResetPasswordEmail(user.EMAIL, user.USERNAME, resetPasswordLink);
                    TempData["Success"] = "Yêu cầu đặt lại mật khẩu đã được gửi vào email của bạn.";
                    return RedirectToAction("WaitingForResetPassword");
                }
                else
                {
                    TempData["Error"] = "Email không tồn tại trong hệ thống.";
                }
            }

            return RedirectToAction("ForgotPassword");
        }
        [HttpGet]
        public ActionResult ResetPassword(string email, string macode)
        {
            using (var context = new QLDULIEUBENHNHANEntities())
            {
                var user = context.TAIKHOANs.SingleOrDefault(u => u.EMAIL == email && u.MACODE == macode);

                if (user != null)
                {
                    var tokenAge = DateTime.Now - user.NGAYCAPNHAT;
                    if (tokenAge.HasValue && tokenAge.Value.TotalHours <= 24)
                    {
                        var model = new TAIKHOAN
                        {
                            EMAIL = email,
                            MACODE = macode
                        };
                        return View(model);
                    }
                }

                TempData["Error"] = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("ForgotPassword");
            }
        }
        [HttpPost]
        public ActionResult ResetPassword(TAIKHOAN model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new QLDULIEUBENHNHANEntities())
                {
                    var user = context.TAIKHOANs.SingleOrDefault(u => u.EMAIL == model.EMAIL && u.MACODE == model.MACODE);

                    if (user != null)
                    {
                        var keyNew = Helper.GeneratePassword(10);
                        string hashedPassword = Helper.EncodePassword(model.PASSWORD, keyNew);
                        user.PASSWORD = hashedPassword;
                        user.MACODE = keyNew;
                        user.NGAYCAPNHAT = DateTime.Now;
                        context.SaveChanges();

                        TempData["Success"] = "Mật khẩu của bạn đã được thay đổi thành công.";
                        return RedirectToAction("Login", "Home");
                    }
                    else
                    {
                        TempData["Error"] = "Liên kết đặt lại mật khẩu không hợp lệ.";
                    }
                }
            }

            return View(model);
        }
        public ActionResult WaitingForResetPassword()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UpdateInformation()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["UserID"] != null)
            {
                // Lấy ID của tài khoản từ Session
                string userID = Session["UserID"].ToString();

                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        // Truy vấn dữ liệu của bệnh nhân từ cơ sở dữ liệu bằng cách sử dụng ID của tài khoản
                        var patientData = (from bn in context.BENHNHANs
                                           where bn.idTK == userID
                                           select bn).Include(bn => bn.TAIKHOAN).FirstOrDefault();

                        if (patientData != null)
                        {
                            // Nếu tìm thấy dữ liệu của bệnh nhân, trả về view để chỉnh sửa
                            return View(patientData);
                        }
                        else
                        {
                            // Xử lý trường hợp không tìm thấy dữ liệu của bệnh nhân
                            TempData["Error"] = "Không tìm thấy thông tin của bệnh nhân.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý trường hợp có lỗi xảy ra khi truy xuất dữ liệu từ cơ sở dữ liệu
                    System.Diagnostics.Debug.WriteLine("Đã xảy ra lỗi: " + ex.ToString());
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Xử lý trường hợp người dùng chưa đăng nhập
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInformation(BENHNHAN model)
        {
            if (ModelState.IsValid)
            {
                // Lưu thông tin cập nhật vào cơ sở dữ liệu
                using (var context = new QLDULIEUBENHNHANEntities())
                {
                    try
                    {
                        // Truy vấn bệnh nhân cần cập nhật
                        var existingPatient = context.BENHNHANs.Find(model.MABN);

                        if (existingPatient != null)
                        {
                            // Cập nhật thông tin của bệnh nhân
                            existingPatient.HOTEN = model.HOTEN;
                            existingPatient.NGAYSINH = model.NGAYSINH;
                            existingPatient.GIOITINH = model.GIOITINH;
                            existingPatient.SODIENTHOAI = model.SODIENTHOAI;
                            existingPatient.DIACHI = model.DIACHI;

                            // Lấy ID của tài khoản từ Session
                            string userID = Session["UserID"].ToString();
                            // Truy vấn thông tin tài khoản
                            var user = context.TAIKHOANs.Find(userID);
                            if (user != null)
                            {
                                // Cập nhật trường EMAIL của mô hình TAIKHOAN
                                user.EMAIL = model.TAIKHOAN.EMAIL;
                            }

                            // Lưu thay đổi vào cơ sở dữ liệu
                            context.SaveChanges();

                            TempData["Success"] = "Cập nhật thông tin thành công.";
                            return View(existingPatient);
                        }
                        else
                        {
                            // Xử lý trường hợp không tìm thấy bệnh nhân
                            TempData["Error"] = "Không tìm thấy bệnh nhân";
                            return RedirectToAction("Index");
                        }
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                // Xử lý trường hợp ModelState không hợp lệ
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return View(model);
            }
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            TempData["Success"] = "Đăng xuất thành công";
            return RedirectToAction("Index", "Home", new {area = ""});
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
        public ActionResult DanhSachLichHen() 
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();

                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        var patient = context.BENHNHANs.FirstOrDefault(bn => bn.idTK == userID);

                        if (patient != null)
                        {
                            var lichHenList = context.PHIEUHENs.Where(mr => mr.idBN == patient.MABN).OrderByDescending(mr => mr.NGAYTAO).ToList();
                            return View(lichHenList);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin của bệnh nhân.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login");
            }
        }
        public ActionResult ThemLichHen()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemLichHen(PHIEUHEN model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra ngày phải lớn hơn ngày hiện tại
                    if (model.NGAY <= DateTime.Now.Date)
                    {
                        ModelState.AddModelError("NGAY", "Ngày phải lớn hơn ngày hiện tại.");
                        return View(model);
                    }

                    // Kiểm tra giờ phải từ 7:00 đến 11:30 hoặc từ 13:00 đến 17:00
                    TimeSpan gio = (TimeSpan)model.GIO;
                    if (!((gio >= TimeSpan.FromHours(7) && gio <= TimeSpan.FromHours(11) + TimeSpan.FromMinutes(30)) ||
                          (gio >= TimeSpan.FromHours(13) && gio <= TimeSpan.FromHours(17))))
                    {
                        ModelState.AddModelError("GIO", "Giờ phải từ 7:00 đến 11:30 hoặc từ 13:00 đến 17:00.");
                        return View(model);
                    }

                    string userID = Session["UserID"].ToString();
                    var patient = _db.BENHNHANs.FirstOrDefault(bn => bn.idTK == userID);

                    if (patient != null) {
                        model.idBN = patient.MABN;
                        model.MALH = GenerateUniqueIDLH("LH");
                        model.ACTIVE = false;
                        model.NGAYTAO = DateTime.Now;
                        _db.PHIEUHENs.Add(model);
                        _db.SaveChanges();
                        TempData["Success"] = "Đăng ký lịch hẹn thăm khám thành công";
                        return RedirectToAction("DanhSachLichHen", "Home");
                    }
                }
                catch (Exception)
                {
                    TempData["Error"] = "Đăng ký lịch hẹn không thành công";
                    return View(model);
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaLichHen(string id)
        {
            try
            {
                var deleteModel = _db.PHIEUHENs.Find(id);
                if (deleteModel != null)
                {
                    _db.PHIEUHENs.Remove(deleteModel);
                    _db.SaveChanges();
                    TempData["Success"] = "Xóa lịch hẹn thành công.";
                    return RedirectToAction("DanhSachLichHen");
                }
                return RedirectToAction("Loi");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Xóa lịch hẹn thất bại: " + ex.Message;
                return View();
            }
        }
        public string GenerateUniqueIDLH(string prefix)
        {
            int counter = 1;
            string newID = prefix + "-" + counter.ToString("D10");

            while (_db.PHIEUHENs.Any(item => item.MALH == newID))
            {
                counter++;
                newID = prefix + "-" + counter.ToString("D10");
            }

            return newID;

        }
        public ActionResult DanhSachThamKham()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();

                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        // Fetch the patient data based on the logged-in user's ID
                        var patient = context.BENHNHANs.FirstOrDefault(bn => bn.idTK == userID);

                        if (patient != null)
                        {
                            // Fetch the medical records for the patient
                            var medicalRecords = context.CHITIETBENHANs.Where(mr => mr.idBN == patient.MABN).OrderByDescending(mr => mr.NGAYTAO).ToList();

                            // Pass the medical records to the view
                            return View(medicalRecords);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin của bệnh nhân.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login");
            }
        }
        public ActionResult KetQuaKham(string id)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        // Fetch the medical record details based on the provided ID
                        var medicalRecord = context.CHITIETBENHANs.Include(mr => mr.BENHNHAN)
                            .FirstOrDefault(mr => mr.MABA == id);

                        if (medicalRecord != null)
                        {
                            // Create a view model to pass to the view
                            var viewModel = new CHITIETBENHAN
                            {
                                MABA = medicalRecord.MABA,
                                TENBENHAN = medicalRecord.TENBENHAN,
                                idBN = medicalRecord.BENHNHAN.HOTEN,
                                idBS = medicalRecord.BACSI.HOTEN + " - " + medicalRecord.BACSI.KHOA.TENKHOA,
                                NGAY = medicalRecord.NGAY,
                                GIO = medicalRecord.GIO,
                                MACH = medicalRecord.MACH,
                                THANNHIET = medicalRecord.THANNHIET,
                                NHIPTHO = medicalRecord.NHIPTHO,
                                CHIEUCAO = medicalRecord.CHIEUCAO,
                                CANNANG = medicalRecord.CANNANG,
                                MATPHAI = medicalRecord.MATPHAI,
                                MATTRAI = medicalRecord.MATTRAI,
                                NHANAPPHAI = medicalRecord.NHANAPPHAI,
                                NHANAPTRAI = medicalRecord.NHANAPTRAI,
                                CHUANDOANLAMSANG = medicalRecord.CHUANDOANLAMSANG,
                                CHUANDOANCUOICUNG = medicalRecord.CHUANDOANCUOICUNG,
                                TRANGTHAI = medicalRecord.TRANGTHAI,
                                NGAYTAO = medicalRecord.NGAYTAO,
                                TIENSUBENH = medicalRecord.TIENSUBENH,
                            };

                            return View(viewModel);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin bệnh án.";
                            return RedirectToAction("DanhSachThamKham");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("DanhSachThamKham");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult DanhSachHoaDon()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();

                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        var patient = context.BENHNHANs.FirstOrDefault(bn => bn.idTK == userID);

                        if (patient != null)
                        {
                            var hoaDons = context.HOADONs.Where(mr => mr.idBN == patient.MABN).OrderByDescending(mr => mr.NGAYTAO).ToList();
                            return View(hoaDons);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin của bệnh nhân.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult ChiTietHoaDon(string id)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        // Fetch the medical record details based on the provided ID
                        var hoaDons = context.HOADONs.Include(mr => mr.BENHNHAN)
                            .FirstOrDefault(mr => mr.MAHD == id);

                        if (hoaDons != null)
                        {
                            return View(hoaDons);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin hóa đơn.";
                            return HttpNotFound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("DanhSachHoaDon");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult DanhSachBienLai()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();

                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        var patient = context.BENHNHANs.FirstOrDefault(bn => bn.idTK == userID);

                        if (patient != null)
                        {
                            var bienLais = context.HOADONs.Where(mr => mr.idBN == patient.MABN).OrderByDescending(mr => mr.NGAYTAO).ToList();
                            return View(bienLais);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin của bệnh nhân.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult ChiTietBienLai(string id)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        // Fetch the medical record details based on the provided ID
                        var bienLais = context.HOADONs.Include(mr => mr.BENHNHAN)
                            .FirstOrDefault(mr => mr.MAHD == id);

                        if (bienLais != null)
                        {
                            return View(bienLais);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin biên lai.";
                            return HttpNotFound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("DanhSachBienLai");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult DanhSachDonThuoc()
        {
            // Check if the user is logged in
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();

                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        var patient = context.BENHNHANs.FirstOrDefault(bn => bn.idTK == userID);

                        if (patient != null)
                        {
                            var donThuocs = context.DONTHUOCs.Where(mr => mr.idBN == patient.MABN).OrderByDescending(mr => mr.NGAYTAO).ToList();
                            return View(donThuocs);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin của bệnh nhân.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult ChiTietDonThuoc(string id)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    using (var context = new QLDULIEUBENHNHANEntities())
                    {
                        // Fetch the medical record details based on the provided ID
                        var donThuocs = context.DONTHUOCs.Include(mr => mr.BENHNHAN)
                            .FirstOrDefault(mr => mr.MADT == id);

                        if (donThuocs != null)
                        {
                            return View(donThuocs);
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy thông tin đơn thuốc.";
                            return HttpNotFound();
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Đã xảy ra lỗi: " + ex.Message;
                    return RedirectToAction("DanhSachDonThuoc");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
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
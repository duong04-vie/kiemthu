using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Controllers
{
    public class UserController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        // ĐĂNG KÝ
        public ActionResult Dangky()
        {
            return View();
        }

        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(Nguoidung nguoidung)
        {
            try
            {
                // Thêm người dùng mới
                db.Nguoidungs.Add(nguoidung);
                db.SaveChanges();

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Dangnhap");
                }

                return View("Dangky");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangnhap(FormCollection userlog)
        {
            string userMail = userlog["userMail"].ToString();
            string password = userlog["password"].ToString();
            var islogin = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(userMail) && x.Matkhau.Equals(password));

            if (islogin != null)
            {
                if (userMail == "Admin@gmail.com")
                {
                    HttpCookie adminCookie = new HttpCookie("isAdmin", "true");
                    adminCookie.Expires = DateTime.Now.AddHours(1); // Cookie hết hạn sau 1 giờ
                    Response.Cookies.Add(adminCookie);

                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Admin/Home");
                }
                else
                {
                    // Thêm cookie "isUser" cho tài khoản người dùng thường
                    HttpCookie userCookie = new HttpCookie("isUser", "true");
                    userCookie.Expires = DateTime.Now.AddHours(1); // Cookie hết hạn sau 1 giờ
                    Response.Cookies.Add(userCookie);

                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Fail = "Đăng nhập thất bại";
                return View("Dangnhap");
            }
        }

        public ActionResult DangXuat()
        {
            // Xóa session của người dùng khi đăng xuất
            Session["use"] = null;

            // Xóa cookie "isAdmin" nếu tồn tại
            if (Request.Cookies["isAdmin"] != null)
            {
                HttpCookie adminCookie = new HttpCookie("isAdmin");
                adminCookie.Expires = DateTime.Now.AddDays(-1); // Đặt ngày hết hạn trong quá khứ
                Response.Cookies.Add(adminCookie);
            }

            // Xóa cookie "isUser" nếu tồn tại
            if (Request.Cookies["isUser"] != null)
            {
                HttpCookie userCookie = new HttpCookie("isUser");
                userCookie.Expires = DateTime.Now.AddDays(-1); // Đặt ngày hết hạn trong quá khứ
                Response.Cookies.Add(userCookie);
            }

            // Chuyển hướng người dùng về trang chủ sau khi đăng xuất
            return RedirectToAction("Index", "Home");
        }
    }
}
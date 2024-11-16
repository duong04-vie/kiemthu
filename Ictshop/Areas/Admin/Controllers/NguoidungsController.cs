using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Areas.Admin.Controllers
{
    public class NguoidungsController : Controller
    {
        private Qlbanhang db = new Qlbanhang();

        // Xem quản lý tất cả người dùng
        // GET: Admin/Nguoidungs
        public ActionResult Index()
        {
            var nguoidungs = db.Nguoidungs.Include(n => n.PhanQuyen);
            return View(nguoidungs.ToList());
        }

        // Xem chi tiết người dùng theo Mã người dùng
        // GET: Admin/Nguoidungs/Details/5
        public ActionResult Details(int? id)
        {
            // Kiểm tra cookie userCookie trước khi cho phép xem chi tiết
            if (!IsUserCookieValid())
            {
                return RedirectToAction("Dangnhap", "User");
            }

            // Nếu không có người dùng có mã được truyền vào thì trả về trang báo lỗi
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Khai báo một người dùng theo mã
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }

            // trả về trang chi tiết người dùng
            return View(nguoidung);
        }

        // Chỉnh sửa người dùng
        // GET: Admin/Nguoidungs/Edit/5
        public ActionResult Edit(int? id)
        {
            // Kiểm tra cookie isAdmin và userCookie trước khi cho phép chỉnh sửa
            if (id == null || !IsAdminCookieValid() || !IsUserCookieValid())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau,IDQuyen")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }

        // Xoá người dùng 
        // GET: Admin/Nguoidungs/Delete/5
        public ActionResult Delete(int? id)
        {
            // Kiểm tra cookie isAdmin và userCookie trước khi cho phép xóa
            if (id == null || !IsAdminCookieValid() || !IsUserCookieValid())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            db.Nguoidungs.Remove(nguoidung);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Kiểm tra tính hợp lệ của cookie isAdmin
        private bool IsAdminCookieValid()
        {
            HttpCookie adminCookie = Request.Cookies["isAdmin"];
            return adminCookie != null && adminCookie.Value == "true";
        }

        // Kiểm tra tính hợp lệ của cookie userCookie
        private bool IsUserCookieValid()
        {
            HttpCookie userCookie = Request.Cookies["userCookie"];
            return userCookie != null && !string.IsNullOrEmpty(userCookie["UserID"]);
        }

        // Kiểm tra quyền truy cập khi không có cookie hợp lệ
        private bool IsUserLoggedIn()
        {
            return Request.Cookies["userCookie"] != null;
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            // Xóa cookie userCookie và isAdmin
            HttpCookie userCookie = new HttpCookie("userCookie");
            userCookie.Expires = DateTime.Now.AddDays(-1);  // Đặt ngày hết hạn về quá khứ để xóa cookie
            Response.Cookies.Add(userCookie);

            HttpCookie adminCookie = new HttpCookie("isAdmin");
            adminCookie.Expires = DateTime.Now.AddDays(-1);  // Đặt ngày hết hạn về quá khứ để xóa cookie
            Response.Cookies.Add(adminCookie);

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Dangnhap", "User");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
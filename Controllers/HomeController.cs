using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

// ****************************************************** GET REQUEST ********************************************** //       
        // User Login & Reg Page //
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        // User personal dashboard //
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? LoggedUser = HttpContext.Session.GetInt32("UserId");
            if(LoggedUser == null)
            {
                return View("Index");
            }
            User ToView = dbContext.Users.Include(u => u.Wedders).ThenInclude(wed => wed.Wedding).FirstOrDefault(u => u.UserId == LoggedUser);
            ViewBag.Weddings = dbContext.Weddings.Include(w => w.Wedders).ThenInclude(ass => ass.User);
            return View("Dashboard", ToView);
        }

        // add new wedding page //
        [HttpGet("addnewwedding")]
        public IActionResult AddNewWedding()
        {
            int? LoggedUser = HttpContext.Session.GetInt32("UserId");
            if(LoggedUser == null)
            {
                return View("Index");
            }
            return View("AddNewWedding");
        }

        // wedding detail page //
        [HttpGet("viewdetail/{WeddingId}")]
        public IActionResult ViewDetail(int WeddingId)
        {
            int? LoggedUser = HttpContext.Session.GetInt32("UserId");
            if(LoggedUser == null)
            {
                return View("Index");
            }
            ViewBag.Wedding= dbContext.Weddings.Include(u => u.Wedders).ThenInclude(wed => wed.User).FirstOrDefault(wedding => wedding.WeddingId == WeddingId);
            return View("ViewDetail");
        }


// ****************************************************** POST REQUEST ********************************************** //  

        // Create a new User //
        [HttpPost("register")]
        public IActionResult Register(LoginReg FromForm)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == FromForm.UserForm.Email))
                {
                    ModelState.AddModelError("UserForm.Email", "Email is already in use!");
                    return Index();
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                FromForm.UserForm.Password = Hasher.HashPassword(FromForm.UserForm, FromForm.UserForm.Password);
                dbContext.Users.Add(FromForm.UserForm);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", FromForm.UserForm.UserId);
                return RedirectToAction("Dashboard");
            }
            return Index();
        }

        // login in existing User //
        [HttpPost("login")]
        public IActionResult Login(LoginReg FromForm)
        {
            if(ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == FromForm.LoggedUserForm.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoggedUserForm.Email", "Invalid Email/Password");
                    return Index();
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(FromForm.LoggedUserForm, userInDb.Password, FromForm.LoggedUserForm.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("LoggedUserForm.Password", "Wrong Password");
                    return Index();
                }
                else
                {
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return Index();
            }
        }

        // Add a new Wedding for the logged in User //
        [HttpPost("addnew")]
        public IActionResult AddNew()
        {
            if(ModelState.IsValid)
            {
                int? LoggedUser = HttpContext.Session.GetInt32("UserId");
                string Wedder1 = Request.Form["Wedder1"];
                string Wedder2 = Request.Form["Wedder2"];
                string Address = Request.Form["Address"];
                DateTime Date = Convert.ToDateTime(Request.Form["Date"]);
                Wedding wedding = new Wedding(){UserId = (int)LoggedUser, Wedder1 = Wedder1, Wedder2 = Wedder2, Date = Date, Address = Address};
                dbContext.Add(wedding);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("AddNewWedding");
        }

        // RSVP for other users wedding //
        [HttpPost("/action/{WeddingId}")]
        public IActionResult RSVP(int WeddingId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            int weddingId = WeddingId;
            Association rsvp = new Association() {UserId = (int)userId, WeddingId = weddingId};
            dbContext.Add(rsvp);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // un-RSVP for other users wedding //
        [HttpPost("/unrsvp/{WeddingId}")]
        public IActionResult UnRSVP(int WeddingId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            int weddingId = WeddingId;
            Association ToDelete = dbContext.Associations.FirstOrDefault(user => user.UserId == userId && user.WeddingId == WeddingId);
            dbContext.Remove(ToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("/delete/{WeddingId}")]
        public IActionResult Delete(int WeddingId)
        {
            Wedding ToDelete = dbContext.Weddings.FirstOrDefault(wed => wed.WeddingId == WeddingId);
            dbContext.Remove(ToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // Logout User //
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}

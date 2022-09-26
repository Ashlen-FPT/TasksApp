using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksApp.Data;
using TasksApp.Models;
using TasksApp.Services;

namespace TasksApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserService _userService;

        public UserController(ApplicationDbContext db , UserService userService)
        {
            _db = db;
            _userService = userService;
        }
        [Authorize]
        public IActionResult Index()
        {   
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var usersList = new List<ApplicationUser>();

            if (User.IsInRole(SD.Role_Admin.ToString()))
            {
                if (User.FindFirst("Organization")?.Value == "MCT")
                {
                    usersList = _db.ApplicationUsers.Where(x => x.Categories == "MCT").ToList();
                }
            }

            if (User.IsInRole(SD.Role_Admin.ToString()))
            {
                if (User.FindFirst("Organization")?.Value == "TLG")
                {
                    usersList = _db.ApplicationUsers.Where(x => x.Categories == "TLG").ToList();
                }
            }

            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in usersList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

            }

            return Json(new { data = usersList });
        }

        [HttpGet]
        public IActionResult GetCust()
        {
            var userList = _db.ApplicationUsers.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

            }
            var list = userList.Where(l => l.Role == SD.Role_Operator);
            return Json(new { data = list });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked, we will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful." });
        }

        #endregion

    }
}

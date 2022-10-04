using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using e_loanApp.Models;
using System.Net;
using System.Web.Security;

namespace e_loanApp.Controllers
{
    public class AdminController : Controller
    {
        private eLoan_ApplicationEntities1 db = new eLoan_ApplicationEntities1();
        // GET: Bank
        [AutorizedClass(Roles = "Administrator")]
        [ActionName("AdminPage")]
        public ActionResult AdminPage()
        {
            var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
            var bidTables = db.Transaction_Table.Where(a => a.User_Id == 1).Where(a => a.Receiver_Account_name != "Bank Admin");
            System.Diagnostics.Debug.WriteLine(userID);
           // var result = db.Transaction_Table.Sum(x => x.amount);
            ViewBag.Total = String.Format("{0:C}", db.Transaction_Table.Sum(x => x.amount));
            // bidTables.Add(new { Value = result, DisplayName = (object)"New Name" });

            return View(bidTables.ToList());
        }
        [HttpPost]
        public ActionResult ApproveLoan(string id, bool acceptLoanAnse, Transaction_Table transaction_)
        {
            System.Diagnostics.Debug.WriteLine(transaction_);
            System.Diagnostics.Debug.WriteLine(acceptLoanAnse);
            System.Diagnostics.Debug.WriteLine(id);
            //  transaction_.Txn_Id = id;
            transaction_.Transaction_Approved = acceptLoanAnse;
            System.Diagnostics.Debug.WriteLine(id);
            //Get Single course which need to update  
            var objCourse = db.Transaction_Table.Single(d => d.TXN_REF == id);
            //Field which will be update  
            objCourse.Transaction_Approved = true;
            objCourse.Transaction_status = "In Progress";

            db.SaveChanges();
            return RedirectToAction("ViewPendingLoan", "Admin");
        }
        [HttpPost]
        public ActionResult RejectLoan(string id, bool rejectLoanAnse, Transaction_Table transaction_)
        {
            transaction_.Transaction_Approved = rejectLoanAnse;
            var objCourse = db.Transaction_Table.Single(d => d.TXN_REF == id);
            //Field which will be update  
            objCourse.Transaction_Approved = rejectLoanAnse;
            objCourse.Transaction_status = "Rejected";
              db.SaveChanges();
            return RedirectToAction("ViewPendingLoan", "Admin");
        }
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser([Bind(Include = "UserId,User_First_Name,User_Last_Name,DateOfBirth,sortCode,acctNumber,Bank,Gender,Address,Phone_Number,Email_Address,Name_Of_College,Name_Of_Faculty,Programme_Of_Study")] UserTable user)
        {
            if (ModelState.IsValid)
            {
                db.UserTables.Add(user);
                db.SaveChanges();
                return RedirectToAction("ViewUserAccount");
            }
            ViewBag.AccountId = new SelectList(db.AccountTables, "Account_Id", "Account_name", user.AccountId);
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", user.RoleId);
            return View(user);
        }
        public ActionResult ViewPendingLoan()
        {
            var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
            // var userID = Context.USER.Where(user => user.UserName == login.userName).select(user => user.UserID);  .Where(a => a.User_Id == userID)
            var bidTables = db.Transaction_Table.Include(b => b.UserTable).Include(u => u.AccountTable).Where(a => a.Transaction_Approved == null).Where(a => a.Transaction_Purpose == "Request Loan");
            System.Diagnostics.Debug.WriteLine(userID);
            return View(bidTables.ToList());
        }
        public ActionResult ViewUserAccount()
        {
            return View(db.UserTables.ToList());
        }
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        public ActionResult ManageAccounts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTable user = db.UserTables.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
         [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageAccounts(UserTable user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewUserAccount");
            }
        ViewBag.AccountId = new SelectList(db.AccountTables, "Account_Id", "Account_name", user.AccountId);
        ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", user.RoleId);
        return View(user);
        }
    }
}
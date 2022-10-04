using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using e_loanApp.Models;
using System.Threading;
using System.Globalization;

namespace e_loanApp.Controllers
{
    public class BankController : Controller
    {
        private eLoan_ApplicationEntities1 db = new eLoan_ApplicationEntities1();
        // GET: Bank
     //   [Authorize(Roles = "Bank")]
        [AutorizedClass(Roles = "Bank")]
        [ActionName("BankPage")]
        public ActionResult BankPage()
        {
            var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
            var bidTables = db.Transaction_Table.Include(b => b.UserTable).Where(a => a.Transaction_Purpose != "Open Payment").Where(a => a.Transaction_status != null);
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            var firstamount = db.Transaction_Table.Where(a => a.User_Id == userID).Where(a => a.Transaction_status == "Paid").Sum(x => x.amount);
            var secondamount = db.Transaction_Table.Where(a => a.User_Id == userID).Where(a => a.Transaction_status == "Refunded").Sum(x => x.amount) * -1;
            var thirdamount = db.Transaction_Table.Where(a => a.User_Id == userID).Where(a => a.Receiver_Account_name == "Bank Admin").Sum(x => x.amount);
            //bidTables = bidTables.Select(i => Math.Abs(i)).ToList();
            var total = thirdamount - firstamount + secondamount;
            ViewBag.Total = string.Format(new CultureInfo("en-US"), "{0:c}", total);
          //  ViewBag.Total = String.Format("{0:C}", db.Transaction_Table.Where(a => a.User_Id == userID).Where(a => a.Transaction_status != "Rejected").Sum(x => x.amount));
            return View(bidTables.ToList());
        }
        public ActionResult PayLoan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ApproveLoan(string id, bool acceptLoanAnse, Transaction_Table transaction_)
        {
            //System.Diagnostics.Debug.WriteLine(transaction_);
            //System.Diagnostics.Debug.WriteLine(acceptLoanAnse);
            //System.Diagnostics.Debug.WriteLine(id);
            //  transaction_.Txn_Id = id;
            transaction_.Transaction_Approved = acceptLoanAnse;
           // System.Diagnostics.Debug.WriteLine(id);
            //Get Single course which need to update  
            var objCourse = db.Transaction_Table.Single(d => d.TXN_REF == id);
            //Field which will be update  
            objCourse.Transaction_Approved = true;
            objCourse.Transaction_status = "Paid";

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
        public ActionResult ViewPayLoan()
        {
            var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
            var bidTables = db.Transaction_Table.Include(b => b.UserTable).Where(a => a.Transaction_Purpose == "Request Loan").Where(a => a.Transaction_status == "In Progress");
            return View(bidTables.ToList());
        }
        public ActionResult ViewHistory()
        {
            var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
            var bidTables = db.Transaction_Table.Include(b => b.UserTable).Where(a => a.Transaction_Purpose != "Open Payment");
            return View(bidTables.ToList());
        }
    }
}
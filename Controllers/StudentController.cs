using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using e_loanApp.Models;

namespace e_loanApp.Controllers
{
    public class StudentController : Controller
    {
        private eLoan_ApplicationEntities1 db = new eLoan_ApplicationEntities1();
        // GET: Student
        public ActionResult OpenPayment()
        {
            var y = db.Transaction_Table.Where(a => a.Receiver_Email == System.Web.HttpContext.Current.User.Identity.Name).Where(a => a.Transaction_status != "Rejected").Where(a => a.Transaction_status != "Pending").Sum(x => x.amount);
            if (y < 0)
            {
                ViewBag.Total = "*** You do not have a positive balance, kindly request for loan from the school bank";
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                ViewBag.Total = "*** You can only transfer funds up to" + string.Format(new CultureInfo("en-US"), "{0:c}", y);
            }
           
            return View();
        }
        [HttpPost]
        public ActionResult OpenPayment(string Receiver_Account_name,string Receiver_SortCode,string Reciever_Accountnum,int Reciever_Amount, Transaction_Table transaction)
        {
            System.Diagnostics.Debug.WriteLine(Receiver_Account_name);
            if (db.UserTables.Any(o => o.acctNumber == Reciever_Accountnum && o.sortCode == Receiver_SortCode))
            {

                var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
                var receiverUserID = db.UserTables.FirstOrDefault(x => x.sortCode == Receiver_SortCode).UserId;
                var receiverEmail = db.UserTables.FirstOrDefault(x => x.sortCode == Receiver_SortCode).Email_Address;
                var p = Guid.NewGuid().ToString("N").Substring(0, 5);
                // Match!
                var sender = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).User_First_Name + ' ' + db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).User_Last_Name;
                transaction.Receiver_Account_name = sender;
                transaction.Receiver_SortCode = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).sortCode;
                transaction.Reciever_Accountnum = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).acctNumber;
                transaction.Transaction_Purpose = "Open Payment";
                transaction.Receiver_Email = System.Web.HttpContext.Current.User.Identity.Name;
                transaction.User_Id = userID;
                transaction.Transaction_Des = "Debit Transaction $" + Reciever_Amount + " From" + sender + " To " + Receiver_Account_name;
                transaction.TXN_REF = p;
                transaction.OpenPayUserId = userID;
                transaction.amount = -Reciever_Amount;
                db.Transaction_Table.Add(transaction);
              //  db.SaveChanges();
                db.Entry(transaction).State = EntityState.Added;
                db.SaveChanges();
                db.Entry(transaction).State = EntityState.Detached;

                transaction.Receiver_Account_name = Receiver_Account_name;
                transaction.Receiver_SortCode = Receiver_SortCode;
                transaction.Reciever_Accountnum = Reciever_Accountnum;
                transaction.Transaction_Purpose = "Open Payment";
                transaction.Receiver_Email = receiverEmail;
                transaction.User_Id = receiverUserID;
                transaction.Transaction_Des = "Credit Transaction $" + Reciever_Amount + " From" + sender + " To " + Receiver_Account_name;
                transaction.OpenPayUserId = userID;
                transaction.TXN_REF = p;
                transaction.amount = Reciever_Amount;
                db.Transaction_Table.Add(transaction);
                db.Entry(transaction).State = EntityState.Added;
                db.SaveChanges();
                db.Entry(transaction).State = EntityState.Detached;


                return RedirectToAction("LandingPage", "Student");
            }else
            {
                return Json(new { status = "error", message = "Account Does Not Match" });
            };
        }
        public ActionResult RequestLoan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RequestLoan(string Receiver_Account_name, string Receiver_SortCode, string Reciever_Accountnum, int Reciever_Amount, Transaction_Table transaction)
        {
            //string message = "";
            System.Diagnostics.Debug.WriteLine(Receiver_Account_name);
            if (db.UserTables.Any(o => o.acctNumber == Reciever_Accountnum && o.sortCode == Receiver_SortCode))
            {
                var p = Guid.NewGuid().ToString("N").Substring(0, 5);
                var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;

                transaction.Receiver_Account_name = Receiver_Account_name;
                transaction.Receiver_SortCode = Receiver_SortCode;
                transaction.Reciever_Accountnum = Reciever_Accountnum;
                transaction.Transaction_Purpose = "Request Loan";
                transaction.amount = Reciever_Amount; 
                transaction.OpenPayUserId = userID;
                transaction.Transaction_status = "Pending";
                transaction.TXN_REF = p;
                transaction.Transaction_Des = "Loan Request $"+ Reciever_Amount + " From Bank to " + Receiver_Account_name;
                transaction.Receiver_Email = System.Web.HttpContext.Current.User.Identity.Name;
                transaction.User_Id = 1;
                db.Transaction_Table.Add(transaction);
                db.SaveChanges();
             //   message = "Request has been sent already!";
                return RedirectToAction("LandingPage", "Student");
            }
            else
            {
              //  message = "Invalid credential provided";
                return Json(new { status = "error", message = "Account Does Not Match" });
            };
          //  ViewBag.Message = message;
            return View();
        }
        public ActionResult ReturnLoan()
        {
            var y = db.Transaction_Table.Where(a => a.Receiver_Email == System.Web.HttpContext.Current.User.Identity.Name).Where(a => a.Transaction_status != "Rejected").Where(a => a.Transaction_status != "Pending").Sum(x => x.amount);

            if (y < 0)
            {
                ViewBag.Total = "*** You do not have a positive balance, kindly request for loan from the school bank";
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                ViewBag.Total = "*** You can only transfer funds up to " + string.Format(new CultureInfo("en-US"), "{0:c}", y);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ReturnLoan(string Receiver_Account_name, string Receiver_SortCode, string Reciever_Accountnum, int Reciever_Amount, Transaction_Table transaction)
        {
            System.Diagnostics.Debug.WriteLine(Receiver_Account_name);
            if (db.UserTables.Any(o => o.acctNumber == Reciever_Accountnum && o.sortCode == Receiver_SortCode))
            {
                var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
                var p = Guid.NewGuid().ToString("N").Substring(0, 5);

                transaction.Receiver_Account_name = Receiver_Account_name;
                transaction.Receiver_SortCode = Receiver_SortCode;
                transaction.Reciever_Accountnum = Reciever_Accountnum;
                transaction.Transaction_Purpose = "Repay Loan";
                transaction.Receiver_Email = System.Web.HttpContext.Current.User.Identity.Name;
                transaction.amount = -Reciever_Amount;
                transaction.TXN_REF = p;
                transaction.OpenPayUserId = userID;
                transaction.Transaction_Des = "Loan Repayment $" + Reciever_Amount + " From " + Receiver_Account_name + " to Bank";
                transaction.Transaction_status = "Refunded";
                transaction.Transaction_Approved = false;
                transaction.User_Id = 1;
                db.Transaction_Table.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("LandingPage", "Student");
            }
            else
            {
                return Json(new { status = "error", message = "Account Does Not Match" });
            };
        }
        public ActionResult LoanRepayment()
        {
            var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
            // var userID = Context.USER.Where(user => user.UserName == login.userName).select(user => user.UserID);  .Where(a => a.User_Id == userID)
            var bidTables = db.Transaction_Table.Where(a => a.OpenPayUserId == userID).Where(a => a.Transaction_Purpose == "Repay Loan" || a.Transaction_Purpose == "Request Loan");
            System.Diagnostics.Debug.WriteLine(userID);
            return View(bidTables.ToList());

         //   return View();
        }
    }
}
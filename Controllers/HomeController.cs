using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using e_loanApp.Models;


namespace e_loanApp.Controllers
{
    public class HomeController : Controller
    {
        private eLoan_ApplicationEntities1 db = new eLoan_ApplicationEntities1();
        string generalEmail;
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Login()
        {

            return View();
        }

        //[Authorize]
        [AutorizedClass(Roles = "Student")]
        [ActionName("LandingPage")]
        public ActionResult LandingPage()
        {
            var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == System.Web.HttpContext.Current.User.Identity.Name).UserId;
            // var userID = Context.USER.Where(user => user.UserName == login.userName).select(user => user.UserID);  .Where(a => a.User_Id == userID)   .Where(a => a.OpenPayUserId == userID)
            var bidTables = db.Transaction_Table.Include(b => b.UserTable).Where(a => a.Receiver_Email == System.Web.HttpContext.Current.User.Identity.Name);
            System.Diagnostics.Debug.WriteLine(userID);
            //    ViewBag.Total = db.Transaction_Table.Where(a => a.User_Id == userID).Where(a => a.Transaction_status != "Rejected").Sum(x => x.amount);
            var y = db.Transaction_Table.Where(a => a.Receiver_Email == System.Web.HttpContext.Current.User.Identity.Name).Where(a => a.Transaction_status != "Rejected").Where(a => a.Transaction_status != "Pending").Sum(x => x.amount);
         //   var rejected = db.Transaction_Table.Where(a => a.Receiver_Email == System.Web.HttpContext.Current.User.Identity.Name).Where(a => a.Transaction_Purpose == "Request Loan").Sum(x => x.amount); 
          //  System.Diagnostics.Debug.WriteLine("total", y, rejected);
              // var total = y - rejected;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            ViewBag.Total = string.Format(new CultureInfo("en-US"), "{0:c}", y);
         //   System.Diagnostics.Debug.WriteLine("total", total, y, rejected);
            return View(bidTables.ToList());

           // return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            System.Diagnostics.Debug.WriteLine("1", login.Email_Address);
            string message = "";
            using (eLoan_ApplicationEntities1 dc = new eLoan_ApplicationEntities1())
            {
                var v = dc.UserTables.Where(a => a.Email_Address == login.Email_Address).FirstOrDefault();
                if (v != null)
                {
                    //if ((bool)!v.IsEmailVerified)
                    //{
                    //    ViewBag.Message = "Please verify your email first";
                    //    return View();
                    //}
                    System.Diagnostics.Debug.WriteLine("2",login.Email_Address);
                    if (string.Compare(login.UserPassword, v.UserPassword) == 0) 
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.Email_Address, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        //else if(User.IsInRole("Student"))
                        //{
                        //    System.Diagnostics.Debug.WriteLine("4", login.Email_Address);
                        //    //Session["UserId"] = login.;
                        //    Session["UserName"] = login.Email_Address;
                        //    return RedirectToAction("LandingPage", "Home", new { area = "Student" });  
                        //}
                        else if (login.Email_Address == "admin@gmail.com")
                        {
                            System.Diagnostics.Debug.WriteLine("5", login.Email_Address);
                            //Session["UserId"] = login.;
                            Session["UserName"] = login.Email_Address;
                            return RedirectToAction("AdminPage", "Admin", new { area = "Admin" });
                        }
                        else if (login.Email_Address == "bank@derby.ac.uk")
                        {
                            //Session["UserId"] = login.;
                            Session["UserName"] = login.Email_Address;
                            return RedirectToAction("BankPage", "Bank", new { area = "Bank" }); 
                        }
                        else 
                        {
                            Session["UserName"] = login.Email_Address;
                            return RedirectToAction("LandingPage", "Home", new { area = "Student" });
                        }
                    //    return View();
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public ActionResult Registration()
        {

            return View();
        }
        [HttpPost]
        public JsonResult AjaxGetCall(DateTime dateOfBirth, string studentNumber)     
        {
            System.Diagnostics.Debug.WriteLine("bn", studentNumber);
            #region //Password is already Exist 
            var isExist = IsEmailExist(studentNumber);
            if (isExist)
            {
                ModelState.AddModelError("EmailExist", "Email already exist");
               // return View();
            }
            #endregion
            eLoan_ApplicationEntities1 dc = new eLoan_ApplicationEntities1();

            System.Diagnostics.Debug.WriteLine(dateOfBirth);
            dc.Configuration.ProxyCreationEnabled = false;
            UserTable usertable = dc.UserTables.FirstOrDefault(s => s.Student_Number == studentNumber && s.UserPassword == null && s.DateOfBirth == dateOfBirth);
            //ModelState.AddModelError("EmailExist", "Email already exist");
            System.Diagnostics.Debug.WriteLine("bi", studentNumber);
           // System.Diagnostics.Debug.WriteLine(usertable.User_First_Name);
            return Json(usertable, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] UserTable userSubmitData, string dateOfBirth, string studentNumber, string userPassword, string userConfirmPassword)
        {

            bool Status = false;
            string message = "";
            System.Diagnostics.Debug.WriteLine(studentNumber);
            System.Diagnostics.Debug.WriteLine(userPassword);
            System.Diagnostics.Debug.WriteLine(dateOfBirth);
            System.Diagnostics.Debug.WriteLine(userSubmitData);
            // Model Validation 
           if (ModelState.IsValid)
           {

                #region //Email is already Exist 
                //var isExist = IsEmailExist(userSubmitData.Email_Address);
                //if (isExist)
                //{
                //    ModelState.AddModelError("EmailExist", "Email already exist");
                //    return View(userSubmitData);
                //}
                #endregion

                #region Generate Activation Code 
                userSubmitData.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
              //  userSubmitData.UserPassword = Crypto.Hash(userSubmitData.UserPassword);
              //  userSubmitData.ConfirmPassword = Crypto.Hash(userSubmitData.ConfirmPassword); //
                #endregion


                //var user = new User() { Id = userId, Password = password };
                //using (var db = new MyEfContextName())
                //{
                //    db.Users.Attach(user);
                //    db.Entry(user).Property(x => x.Password).IsModified = true;
                //    db.SaveChanges();
                //}

               //   userSubmitData.IsEmailVerified = false;
                #region Save to Database
                using (eLoan_ApplicationEntities1 dc = new eLoan_ApplicationEntities1())
                {
                    var objCourse = db.UserTables.Single(d => d.Student_Number == studentNumber);
                    objCourse.UserPassword = userSubmitData.UserPassword;
                    objCourse.IsEmailVerified = true;

                    db.SaveChanges();
                    return RedirectToAction("UploadImage");
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            //return View(userSubmitData);
            return View();
        }
        public ActionResult UploadImage()
        {
            //   ViewBag.UserId = new SelectList(db.Users, "UserID", "FirstName", item.UserId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImage([Bind(Include = "Email_Address,PictureToUpload")] UserTable user)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(user.PictureToUpload.FileName);
                string extension = Path.GetExtension(user.PictureToUpload.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                user.StudentPicture = "../StudentImages/" + fileName;
                fileName = Path.Combine(Server.MapPath("../StudentImages/"), fileName);
                user.PictureToUpload.SaveAs(fileName);
                System.Diagnostics.Debug.WriteLine("up", user.Email_Address);
                var objCourse = db.UserTables.Single(d => d.Email_Address == user.Email_Address);
                objCourse.StudentPicture = fileName;
                //  db.UserTables.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

         //   ViewBag.UserId = new SelectList(db.Users, "UserID", "FirstName", item.UserId);
            return View(user);
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (eLoan_ApplicationEntities1 dc = new eLoan_ApplicationEntities1())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = dc.UserTables.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        [NonAction]
        public bool IsPasswordExist(string emailID)
        {
            using (eLoan_ApplicationEntities1 dc = new eLoan_ApplicationEntities1())
            {
                var v = dc.UserTables.Where(a => a.Email_Address == emailID).Where(a => a.UserPassword == null).FirstOrDefault();
                return v != null;
            }
        }


        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (eLoan_ApplicationEntities1 dc = new eLoan_ApplicationEntities1())
            {
                var v = dc.UserTables.Where(a => a.Email_Address == emailID).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("bank@derby.ac.uk", "08087214877");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "08087214877"; // Replace with actual password
            string subject = "Your account is successfully created!";

            string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) ;
            // smtp.Send(message);
        }
    }
    public class Employee
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
    }
}
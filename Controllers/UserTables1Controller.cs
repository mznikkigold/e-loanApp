using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using e_loanApp.Models;

namespace e_loanApp.Controllers
{
    public class UserTables1Controller : Controller
    {
        private eLoan_ApplicationEntities1 db = new eLoan_ApplicationEntities1();

        // GET: UserTables1
        public ActionResult Index()
        {
            var userTables = db.UserTables.Include(u => u.AccountTable).Include(u => u.RoleTable).Where(a => a.RoleId != 1).Where(a => a.RoleId != 2);
            return View(userTables.ToList());
        }

        // GET: UserTables1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTable userTable = db.UserTables.Find(id);
            if (userTable == null)
            {
                return HttpNotFound();
            }
            return View(userTable);
        }

        // GET: UserTables1/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.AccountTables, "Account_Id", "Account_name");
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role");
            return View();
        }

        // POST: UserTables1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "UserPassword,IsEmailVerified,ActivationCode,Date_Created")] UserTable userTable, RoleMappingTable roleMappingTable)
        {
            if (ModelState.IsValid)
            {
                userTable.RoleId = 3;
               // userTable.AccountId = 
                db.UserTables.Add(userTable);
                db.SaveChanges();

                var userID = db.UserTables.FirstOrDefault(x => x.Email_Address == userTable.Email_Address).UserId;
                roleMappingTable.RoleId = 3;
                roleMappingTable.UserId = userID;
                db.RoleMappingTables.Add(roleMappingTable);
                db.SaveChanges();

              //  accountTable.UserId = userID;
                //accountTable.Account_SortCode = userTable.sortCode;
                //accountTable.Account_name = userTable.User_First_Name + ' ' + userTable.User_Last_Name;
                //accountTable.Account_number = userTable.acctNumber;
                //db.AccountTables.Add(accountTable);
                //db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.AccountTables, "Account_Id", "Account_name", userTable.AccountId);
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", userTable.RoleId);
            return View(userTable);
        }

        // GET: UserTables1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTable userTable = db.UserTables.Find(id);
            if (userTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.AccountTables, "Account_Id", "Account_name", userTable.AccountId);
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", userTable.RoleId);
            return View(userTable);
        }

        // POST: UserTables1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "UserPassword,IsEmailVerified,ActivationCode,Date_Created")] UserTable userTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userTable).State = EntityState.Modified;
                db.Entry(userTable).Property(x => x.UserPassword).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.AccountTables, "Account_Id", "Account_name", userTable.AccountId);
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", userTable.RoleId);
            return View(userTable);
        }

        // GET: UserTables1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTable userTable = db.UserTables.Find(id);
            if (userTable == null)
            {
                return HttpNotFound();
            }
            return View(userTable);
        }

        // POST: UserTables1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserTable userTable = db.UserTables.Find(id);
            db.UserTables.Remove(userTable);
            db.SaveChanges();
            return RedirectToAction("Index");
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

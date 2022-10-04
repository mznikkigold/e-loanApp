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
    public class AccountTablesController : Controller
    {
        private eLoan_ApplicationEntities1 db = new eLoan_ApplicationEntities1();

        // GET: AccountTables
        public ActionResult Index()
        {
            var accountTables = db.AccountTables.Include(a => a.UserTable);
            return View(accountTables.ToList());
        }

        // GET: AccountTables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountTable accountTable = db.AccountTables.Find(id);
            if (accountTable == null)
            {
                return HttpNotFound();
            }
            return View(accountTable);
        }

        // GET: AccountTables/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name");
            return View();
        }

        // POST: AccountTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Account_Id,Account_name,Account_SortCode,Account_number,UserId,Date_Created")] AccountTable accountTable)
        {
            if (ModelState.IsValid)
            {
                db.AccountTables.Add(accountTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name", accountTable.UserId);
            return View(accountTable);
        }

        // GET: AccountTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountTable accountTable = db.AccountTables.Find(id);
            if (accountTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name", accountTable.UserId);
            return View(accountTable);
        }

        // POST: AccountTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Account_Id,Account_name,Account_SortCode,Account_number,UserId,Date_Created")] AccountTable accountTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name", accountTable.UserId);
            return View(accountTable);
        }

        // GET: AccountTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountTable accountTable = db.AccountTables.Find(id);
            if (accountTable == null)
            {
                return HttpNotFound();
            }
            return View(accountTable);
        }

        // POST: AccountTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountTable accountTable = db.AccountTables.Find(id);
            db.AccountTables.Remove(accountTable);
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

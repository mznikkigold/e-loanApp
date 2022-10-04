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
    public class RoleMappingTablesController : Controller
    {
        private eLoan_ApplicationEntities1 db = new eLoan_ApplicationEntities1();

        // GET: RoleMappingTables
        public ActionResult Index()
        {
            var roleMappingTables = db.RoleMappingTables.Include(r => r.RoleTable).Include(r => r.UserTable);
            return View(roleMappingTables.ToList());
        }

        // GET: RoleMappingTables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMappingTable roleMappingTable = db.RoleMappingTables.Find(id);
            if (roleMappingTable == null)
            {
                return HttpNotFound();
            }
            return View(roleMappingTable);
        }

        // GET: RoleMappingTables/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role");
            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name");
            return View();
        }

        // POST: RoleMappingTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoleMappingId,UserId,RoleId")] RoleMappingTable roleMappingTable)
        {
            if (ModelState.IsValid)
            {
                db.RoleMappingTables.Add(roleMappingTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", roleMappingTable.RoleId);
            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name", roleMappingTable.UserId);
            return View(roleMappingTable);
        }

        // GET: RoleMappingTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMappingTable roleMappingTable = db.RoleMappingTables.Find(id);
            if (roleMappingTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", roleMappingTable.RoleId);
            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name", roleMappingTable.UserId);
            return View(roleMappingTable);
        }

        // POST: RoleMappingTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoleMappingId,UserId,RoleId")] RoleMappingTable roleMappingTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roleMappingTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.RoleTables, "RoleId", "Role", roleMappingTable.RoleId);
            ViewBag.UserId = new SelectList(db.UserTables, "UserId", "User_First_Name", roleMappingTable.UserId);
            return View(roleMappingTable);
        }

        // GET: RoleMappingTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMappingTable roleMappingTable = db.RoleMappingTables.Find(id);
            if (roleMappingTable == null)
            {
                return HttpNotFound();
            }
            return View(roleMappingTable);
        }

        // POST: RoleMappingTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoleMappingTable roleMappingTable = db.RoleMappingTables.Find(id);
            db.RoleMappingTables.Remove(roleMappingTable);
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

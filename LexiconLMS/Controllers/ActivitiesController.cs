using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LexiconLMS.Models;
using Microsoft.AspNet.Identity;

namespace LexiconLMS.Controllers
{
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Student")]
        public ActionResult Assignments()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
            Course UserCourse = currentUser.Course;
            ICollection<Module> Modules = UserCourse.Modules;
            ICollection<Activity> Activities = new List<Activity>();
            foreach (var module in Modules)
            {
                foreach (var activity in module.Activities)
                {
                    Activities.Add(activity);
                }
            }
            Activities = Activities.Where(a => a.ActivityType == ActivityType.Assignment).ToList();
            return View(Activities);
        }

        // GET: Activities
        public ActionResult Index()
        {
            var activities = db.activities.Include(a => a.Module);
            return View(activities.ToList());
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create(int id)
        {
            ViewBag.ModuleId = id;
            ViewBag.RedirectString = Request.UrlReferrer.ToString();
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create([Bind(Include = "ActivityId,ActivityType,ActivityName,ActivityStartDate,ActivityEndDate,ActivityDescription,ModuleId")] Activity activity, string redirectString)
        {
            if (ModelState.IsValid)
            {
                db.activities.Add(activity);
                db.SaveChanges();
                return Redirect(redirectString);
            }
            ViewBag.RedirectString = redirectString;
            ViewBag.ModuleId = new SelectList(db.modules, "ModuleId", "ModuleName", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.RedirectString = Request.UrlReferrer.ToString();
            ViewBag.ModuleId = new SelectList(db.modules, "ModuleId", "ModuleName", activity.ModuleId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit([Bind(Include = "ActivityId,ActivityType,ActivityName,ActivityStartDate,ActivityEndDate,ActivityDescription,ModuleId")] Activity activity, string RedirectString)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect(RedirectString);
            }
            ViewBag.RedirectString = RedirectString;
            ViewBag.ModuleId = new SelectList(db.modules, "ModuleId", "ModuleName", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.activities.Find(id);
            db.activities.Remove(activity);
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

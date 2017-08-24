using LexiconLMS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace LexiconLMS.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        [Authorize(Roles = "Teacher")]
        public ActionResult Index(string currentAction, string searchValue)
        {
            ViewBag.currentAction = currentAction;
            var Courses = db.courses.Select(c => c);
            if (currentAction != "Old")
            {
                Courses = Courses.Where(c => c.CourseEndDate > DateTime.Now);
            }
            else
            {
                Courses = Courses.Where(c => c.CourseEndDate < DateTime.Now);
            }
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                Courses = Courses.Where(c => c.CourseName.Contains(searchValue));
            }
            return View(Courses.OrderBy(c => c.CourseStartDate).ToList());
        }

        [Authorize(Roles = "Student")]
        public ActionResult Participants()
        {
            Course course = db.Users.Find(User.Identity.GetUserId()).Course;
            return View(course);
        }
        [Authorize(Roles = "Student")]
        public ActionResult MyCourse(int? week)
        {
            Course course = db.Users.Find(User.Identity.GetUserId()).Course; //<--- Test if null
            if (week != null)
            {
                ViewBag.Week = week;
            }
            return View(course);
        }
        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Teacher")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,CourseStartDate,CourseEndDate,CourseDescription")] Course course)
        {
            if (ModelState.IsValid)
            {
                if (db.courses.Where(c => c.CourseName == course.CourseName).Where(c => c.CourseEndDate > DateTime.Now).Any())
                {
                    ViewBag.Name = "There is already a course with that name.";
                }
                if (ViewBag.Name == null)
                {

                    db.courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.RedirectString = redirectCheck();

            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,CourseStartDate,CourseEndDate,CourseDescription")] Course course, string redirectString)
        {
            if (ModelState.IsValid)
            {
                if (db.modules.Where(m => m.CourseId == course.CourseId).Any())
                {
                    if (db.modules.Where(m => m.CourseId == course.CourseId).OrderBy(m => m.ModuleStartDate).FirstOrDefault().ModuleStartDate < course.CourseStartDate)
                    {
                        ViewBag.StartDate = "Course has a module starting before given start date of the course.";
                    }
                    if (db.modules.Where(m => m.CourseId == course.CourseId).OrderByDescending(m => m.ModuleEndDate).FirstOrDefault().ModuleEndDate > course.CourseEndDate)
                    {
                        ViewBag.EndDate = "Course has a module that ends after the given end date of the course.";
                    }
                }
                if (db.courses.Where(c => c.CourseName == course.CourseName).Where(c => c.CourseEndDate > DateTime.Now).Where(c => c.CourseId != course.CourseId).Any())
                {
                    ViewBag.Name = "There is already a course with that name.";
                }
                if (ViewBag.Name == null && ViewBag.EndDate == null && ViewBag.StartDate == null)
                {
                    db.Entry(course).State = EntityState.Modified;
                    db.SaveChanges();
                    if (redirectString != "Empty")
                    {
                        return Redirect(redirectString);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewBag.RedirectString = redirectString;
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.courses.Find(id);
            db.courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private string redirectCheck()
        {
            if (Request.UrlReferrer != null)
            {
                return Request.UrlReferrer.ToString();
            }
            else
            {
                return "Empty";
            }
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

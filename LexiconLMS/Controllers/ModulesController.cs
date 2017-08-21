using LexiconLMS.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace LexiconLMS.Controllers
{
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Modules
        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {
            var modules = db.Users.Find(User.Identity.GetUserId()).Course.Modules;
            return View(modules.OrderBy(m => m.ModuleStartDate).ToList());
        }

        // GET: Modules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // GET: Modules/Create
        public ActionResult Create(int Course)
        {
            ViewBag.Course = Course;
            ViewBag.RedirectString = redirectCheck();
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create([Bind(Include = "ModuleId,ModuleName,ModuleDescription,ModuleStartDate,ModuleEndDate,CourseId")] Module module, string redirectString)
        {
            if (ModelState.IsValid)
            {
                Validation(module);
                if (ViewBag.StartDate == null && ViewBag.EndDate == null && ViewBag.Name == null)
                {
                    db.modules.Add(module);
                    db.SaveChanges();
                    if (redirectString != "Empty")
                    {
                        return Redirect(redirectString);
                    }
                    else
                    {
                        return RedirectToAction("Details", "Courses", new { id = module.CourseId });
                    }

                }
            }
            ViewBag.RedirectString = redirectString;
            ViewBag.CourseId = new SelectList(db.courses, "CourseId", "CourseName", module.CourseId);
            ViewBag.Course = module.CourseId;
            return View(module);
        }

        // GET: Modules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            ViewBag.RedirectString = redirectCheck();
            ViewBag.CourseId = new SelectList(db.courses, "CourseId", "CourseName", module.CourseId);
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit([Bind(Include = "ModuleId,ModuleName,ModuleDescription,ModuleStartDate,ModuleEndDate,CourseId")] Module module, string redirectString)
        {
            if (ModelState.IsValid)
            {
                Validation(module);
                if (ViewBag.StartDate == null && ViewBag.EndDate == null && ViewBag.Name == null)
                {
                    db.Entry(module).State = EntityState.Modified;
                    db.SaveChanges();
                    if (redirectString != "Empty")
                    {
                        return Redirect(redirectString);
                    }
                    else
                    {
                        return RedirectToAction("Details", "Courses", new { id = module.CourseId });
                    }
                }
            }
            ViewBag.CourseId = new SelectList(db.courses, "CourseId", "CourseName", module.CourseId);
            ViewBag.RedirectString = redirectString;
            return View(module);
        }

        // GET: Modules/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.RedirectString = redirectCheck();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult DeleteConfirmed(int id, string redirectString)
        {
            Module module = db.modules.Find(id);
            db.modules.Remove(module);
            db.SaveChanges();
            if (redirectString != "Empty")
            {
                return Redirect(redirectString);
            }
            else
            {
                return RedirectToAction("Details", "Courses", new { id = module.CourseId });
            }
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

        private void Validation(Module module)
        {
            if (db.courses.Find(module.CourseId).CourseStartDate > module.ModuleStartDate)
            {
                ViewBag.StartDate = "Start Date of the Module can not be before the Start Date of the Course.";
            }
            if (db.courses.Find(module.CourseId).CourseEndDate < module.ModuleEndDate)
            {
                ViewBag.EndDate = "End Date of the Module can not be after the End Date of the Course.";
            }
            if (db.modules.Where(m => m.CourseId == module.CourseId).Where(m => m.ModuleId != module.ModuleId).Where(m => m.ModuleName == module.ModuleName).Any())
            {
                ViewBag.Name = "There is already a Module with that name in the Course.";
            }
            if (db.activities.Where(a => a.ModuleId == module.ModuleId).Any())
            {
                if (db.activities.Where(a => a.ModuleId == module.ModuleId).OrderBy(a => a.ActivityStartDate).FirstOrDefault().ActivityStartDate < module.ModuleStartDate)
                {
                    ViewBag.StartDate = "Module has an Activity starting before given start date of the Module.";
                }
                if (db.activities.Where(a => a.ModuleId == module.ModuleId).OrderByDescending(a => a.ActivityEndDate).FirstOrDefault().ActivityEndDate > module.ModuleEndDate)
                {
                    ViewBag.EndDate = "Module has an Activity that ends after the given end date of the Module.";
                }
                if (db.modules.Where(m => m.CourseId == module.CourseId).Where(m => m.ModuleId != module.ModuleId).Where(m => m.ModuleStartDate < module.ModuleStartDate).Where(m => m.ModuleEndDate > module.ModuleStartDate).Any())
                {
                    ViewBag.StartDate = "Module can not start before previous modules within the Course end.";
                }
                if (db.modules.Where(m => m.CourseId == module.CourseId).Where(m => m.ModuleId != module.ModuleId).Where(m => m.ModuleStartDate < module.ModuleEndDate).Where(m => m.ModuleEndDate > module.ModuleEndDate).Any()
                    || db.modules.Where(m => m.CourseId == module.CourseId).Where(m => m.ModuleId != module.ModuleId).Where(m => m.ModuleStartDate > module.ModuleStartDate).Where(m => m.ModuleEndDate < module.ModuleEndDate).Any())
                {
                    ViewBag.EndDate = "Module can not end after the next module within the Course starts.";
                }
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

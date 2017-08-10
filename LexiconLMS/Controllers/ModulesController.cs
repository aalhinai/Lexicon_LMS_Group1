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
            ViewBag.RedirectString = redirectString;
            ViewBag.CourseId = new SelectList(db.courses, "CourseId", "CourseName", module.CourseId);
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

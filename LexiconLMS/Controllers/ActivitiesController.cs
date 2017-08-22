using LexiconLMS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Details(int? id, string Message)
        {
            if (Message != null)
            {
                ViewBag.Message = Message.ToString();
            }

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
            ViewBag.RedirectString = redirectCheck();
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
                Validation(activity);
                if (ViewBag.StartDate == null && ViewBag.EndDate == null && ViewBag.Name == null)
                {
                    db.activities.Add(activity);
                    db.SaveChanges();
                    if (redirectString != "Empty")
                    {
                        return Redirect(redirectString);
                    }
                    else
                    {
                        return RedirectToAction("Details", "Modules", new { id = activity.ModuleId });
                    }
                }
            }
            ViewBag.RedirectString = redirectString;
            ViewBag.ModuleId = activity.ModuleId;
            //ViewBag.ModuleId = new SelectList(db.modules, "ModuleId", "ModuleName", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        [Authorize(Roles = "Teacher")]
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
            ViewBag.RedirectString = redirectCheck();
            ViewBag.ModuleId = new SelectList(db.modules, "ModuleId", "ModuleName", activity.ModuleId);
            return View(activity);
        }



        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit([Bind(Include = "ActivityId,ActivityType,ActivityName,ActivityStartDate,ActivityEndDate,ActivityDescription,ModuleId")] Activity activity, string redirectString)
        {
            if (ModelState.IsValid)
            {
                Validation(activity);
                if (ViewBag.StartDate == null && ViewBag.EndDate == null && ViewBag.Name == null)
                {
                    db.Entry(activity).State = EntityState.Modified;
                    db.SaveChanges();
                    if (redirectString != "Empty")
                    {
                        return Redirect(redirectString);
                    }
                    else
                    {
                        return RedirectToAction("Details", "Activities", new { id = activity.ActivityId });
                    }

                }
            }
            ViewBag.RedirectString = redirectString;
            ViewBag.ModuleId = new SelectList(db.modules, "ModuleId", "ModuleName", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        [Authorize(Roles = "Teacher")]
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
            ViewBag.RedirectString = redirectCheck();
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
            {
                return RedirectToAction("Details", "Modules", new { id = activity.ModuleId });
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

        private void Validation(Activity activity)
        {
            if (db.modules.Find(activity.ModuleId).ModuleStartDate > activity.ActivityStartDate)
            {
                ViewBag.StartDate = "Start Date of the Activity can not be before the Start Date of the Module.";
            }
            if (db.modules.Find(activity.ModuleId).ModuleEndDate < activity.ActivityEndDate)
            {
                ViewBag.EndDate = "End Date of the Activity can not be after the End Date of the Module.";
            }
            if (db.activities.Where(a => a.ModuleId == activity.ModuleId).Where(a => a.ActivityId != activity.ActivityId).Where(a => a.ActivityName == activity.ActivityName).Any())
            {
                ViewBag.Name = "There is already an Activity with that name in the Module.";
            }
            if (db.activities.Where(a => a.ModuleId == activity.ModuleId).Where(a => a.ActivityId != activity.ActivityId).Where(a => a.ActivityStartDate < activity.ActivityStartDate).Where(a => a.ActivityEndDate > activity.ActivityStartDate).Any())
            {
                ViewBag.StartDate = "Activity can not start before previous activities within the Module end.";
            }
            if (db.activities.Where(a => a.ModuleId == activity.ModuleId).Where(a => a.ActivityId != activity.ActivityId).Where(a => a.ActivityStartDate < activity.ActivityEndDate).Where(a => a.ActivityEndDate > activity.ActivityEndDate).Any()
                || db.activities.Where(a => a.ModuleId == activity.ModuleId).Where(a => a.ActivityId != activity.ActivityId).Where(a => a.ActivityStartDate > activity.ActivityStartDate).Where(a => a.ActivityEndDate < activity.ActivityEndDate).Any())
            {
                ViewBag.EndDate = "Activity can not end after the next activity within the Module starts.";
            }
        }



        // for Uploading single files:

        // GET: Students
        [HttpGet]
        public ActionResult uploadFile(int? activityId, int? courseID, int? moduleId)
        {

            ViewBag.activityId = activityId;
            ViewBag.courseId = courseID;
            ViewBag.moduleId = moduleId;
            ViewBag.RedirectString = redirectCheck();
            return View();


            // return View();

        }

        // POST: Students
        [HttpPost]
        public ActionResult uploadFile([Bind(Include = "DocId,DocName,DocDescription,DocTimestamp,ActivityId,CourseId,ModuleId, DocURL")] Document document, HttpPostedFileBase file, int? activityId, int? courseID, int? moduleId, string redirectString, string description)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                var timeStamp = DateTime.Now.Ticks;
                string path = Path.Combine(Server.MapPath("~/Upload"),
                                           Path.GetFileName(timeStamp + "_" + file.FileName));
                file.SaveAs(path);

                document.DocName = Path.GetFileName(file.FileName);
                document.DocDescription = description;
                document.DocTimestamp = DateTime.Now;
                if (activityId != null)
                {
                    document.DocDeadline = db.activities.Find(activityId).ActivityEndDate;
                }
                document.ActivityId = activityId;
                //rename the file
                 document.DocURL = Path.GetFileName("/Upload/" + timeStamp + "_" + file.FileName);
                 document.CourseId = courseID;
                 document.ModuleId = moduleId;


                document.UserId = User.Identity.GetUserId();
                db.documents.Add(document);
                db.SaveChanges();

                ViewBag.Message = "File uploaded successfully";
                return Redirect(redirectString);

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            ViewBag.Id = activityId;
            return View();

        }

        //download documents 
        public FileResult DownloadDocument(string docLink)
        {
          
            var FileVirtualPath = "/Upload/" + docLink;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }


        //Show documents
        public ActionResult DocumentList(string role, bool? ontime, int? activityId, int? courseId, int? moduleId) //Role of the user who uploaded the documents.
        {
            var documents = db.documents.Select(d => d);
            if (activityId.HasValue)
            {
                documents = db.documents.Where(d => d.ActivityId == activityId);

                if (User.IsInRole(role) && role == "Student")
                {
                    var userId = User.Identity.GetUserId();
                    documents = documents.Where(d => d.UserId == userId);
                }
                else
                {
                    documents = documents.Where(d => d.User.Roles.Where(r => r.RoleId == db.Roles.Where(x => x.Name == role).FirstOrDefault().Id).Any());
                }
                if (role == "Student")
                {
                    if (ontime.Value)
                    {
                        documents = documents.Where(d => d.DocTimestamp < d.DocDeadline);
                    }
                    else
                    {
                        documents = documents.Where(d => d.DocTimestamp > d.DocDeadline);
                    }
                }
                if (ontime.HasValue)
                {
                    ViewBag.OnTime = ontime.Value;
                }

                ViewBag.Role = role;

            }//end of activityId.HasValue
            else if (courseId.HasValue)
            {
                 documents = db.documents.Where(d => d.CourseId == courseId);
            }
            else {
                 documents = db.documents.Where(d => d.ModuleId == moduleId);
            }
            return PartialView(documents.ToList());
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

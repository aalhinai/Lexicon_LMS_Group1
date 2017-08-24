using LexiconLMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LexiconLMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult TeacherList()
        {
            List<ApplicationUser> fullUsers = db.Users.Where(u => u.Roles.Where(r => r.RoleId == db.Roles.Where(role => role.Name == "Teacher").FirstOrDefault().Id).Any()).ToList();
            List<DisplayUserViewModel> users = new List<DisplayUserViewModel>();
            foreach (var item in fullUsers)
            {
                users.Add(new DisplayUserViewModel { Email = item.Email, UserFirstName = item.UserFirstName, UserLastName = item.UserLastName, Id = item.Id });
            }
            users = users.OrderBy(u => u.UserFullName).ToList();
            return View(users);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        // GET: /Account/Delete/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser fullUser = db.Users.Find(id);
            DisplayUserViewModel user = new DisplayUserViewModel { Email = fullUser.Email, UserFirstName = fullUser.UserFirstName, UserLastName = fullUser.UserLastName };
            if (user == null)
            {
                return HttpNotFound();
            }
            if (Request.UrlReferrer != null) // Check if a redirect string can be created.
            {
                ViewBag.RedirectString = Request.UrlReferrer.ToString();
            }
            else // If a redirect string is not available we set the value to "Empty".
            {
                ViewBag.RedirectString = "Empty";
            }
            return View(user);
        }

        //POST: AccountCountroller/Delete/5
        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string redirectString)
        {
            ApplicationUser user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            if (redirectString != "Empty") // If the value is not set to "Empty" we can redirect based on our redirect string.
            {
                return Redirect(redirectString);
            }
            else // If there is no defined redirect string we presume the user came from TeacherList.
            {
                return RedirectToAction("TeacherList");
            }
        }

        // GET: Account/Edit/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(string id, string role)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser dbUser = db.Users.Find(id);
            DisplayUserViewModel user = new DisplayUserViewModel { Id = dbUser.Id, Email = dbUser.Email, UserFirstName = dbUser.UserFirstName, UserLastName = dbUser.UserLastName };
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RedirectString = redirectCheck();
            ViewBag.Role = role;
            return View(user);
        }


        //POST: Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit([Bind(Include = "Id, Email, UserLastName, UserFirstName")] DisplayUserViewModel user, string redirectString)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser dbUser = db.Users.FirstOrDefault(x => x.Id == user.Id);
                dbUser.UserFirstName = user.UserFirstName;
                dbUser.UserLastName = user.UserLastName;
                dbUser.Email = user.Email;
                dbUser.UserName = user.Email;

                db.Entry(dbUser).State = EntityState.Modified;
                db.SaveChanges();

                if (redirectString != "Empty")
                {
                    return Redirect(redirectString);
                }
                else
                {
                    return RedirectToAction("TeacherList", "Account");
                }
            }
            ViewBag.RedirectString = redirectString;
            return View(user);
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






        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(int? courseId, string role)
        {
            ViewBag.Role = role;
            ViewBag.Course = courseId;

            if (courseId != null) ViewBag.CourseName = "course " + db.courses.Where(ci => ci.CourseId == courseId).FirstOrDefault().CourseName;
            else ViewBag.CourseName = "all courses";

            if (Request.UrlReferrer != null)
            {
                ViewBag.RedirectString = Request.UrlReferrer.ToString();
            }
            else
            {
                ViewBag.RedirectString = "Empty";
            }
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, int? Course, string Role, string redirectString)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, UserFirstName = model.UserFirstName, UserLastName = model.UserLastName, UserStartDate = DateTime.Now, CourseId = Course };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, Role);
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    if (redirectString != "Empty")
                    {
                        return Redirect(redirectString);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                AddErrors(result);
            }
            ViewBag.RedirectString = redirectString;
            ViewBag.Role = Role;
            ViewBag.Course = Course;
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public ActionResult LoginPartial()
        {
            DisplayUserViewModel user = new DisplayUserViewModel();
            if (db.Users.Find(User.Identity.GetUserId()) != null){
                var dbUser = db.Users.Find(User.Identity.GetUserId());

                user = new DisplayUserViewModel
                {
                    Email = dbUser.Email,
                    UserFirstName = dbUser.UserFirstName,
                    UserLastName = dbUser.UserLastName
                };
            }
            return PartialView("_LoginPartial", user);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        //---------tillagt av Stefan
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Manage");
        }


        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult ParticipantList(int courseId = 0)
        {
            if (courseId > 0)
            {
                Course currentCourse = db.courses.Where(c => c.CourseId == courseId).FirstOrDefault();
                ViewBag.CourseId = courseId;
                ViewBag.CourseName = currentCourse.CourseName;
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult> ParticipantList(HttpPostedFileBase ParticipantListFile, int CourseId = 0)
        {
            try
            {
                string ParticipantFilePath = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(ParticipantListFile.FileName));

                //ParticipantListFile.SaveAs(Server.MapPath("~/App_Data/" + ParticipantListFile.FileName));
                ParticipantListFile.SaveAs(ParticipantFilePath);

                var _userStore = new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(db);
                var _userManager = new UserManager<ApplicationUser>(_userStore);

                //XDocument xdocFromFile = XDocument.Load(Server.MapPath("~/App_Data/" + ParticipantListFile.FileName));
                XDocument xdocFromFile = XDocument.Load(ParticipantFilePath);

                XElement studentXList = xdocFromFile.Root.Elements().ElementAt(4); //Excel 2003: ElementAt(3) 

                if (studentXList != null &&
                    studentXList.FirstAttribute.Value.ToUpper() == "PARTICIPANTS" &&
                    studentXList.Elements().First().Elements().ElementAt(3).Elements().First().Value.ToUpper() == "USERFIRSTNAME" &&
                    studentXList.Elements().First().Elements().ElementAt(3).Elements().ElementAt(1).Value.ToUpper() == "USERLASTNAME" &&
                    studentXList.Elements().First().Elements().ElementAt(3).Elements().ElementAt(2).Value.ToUpper() == "EMAIL")
                {
                    ApplicationUser student = null;

                    for (var i = 4; i < studentXList.Elements().First().Elements().Count(); i++)
                    {
                        student = new ApplicationUser();

                        student.UserFirstName = studentXList.Elements().First().Elements().ElementAt(i).Elements().First().Value;
                        student.UserLastName = studentXList.Elements().First().Elements().ElementAt(i).Elements().ElementAt(1).Value;
                        student.Email = studentXList.Elements().First().Elements().ElementAt(i).Elements().ElementAt(2).Value;
                        student.UserName = student.Email;
                        student.CourseId = CourseId;
                        student.UserStartDate = DateTime.Now;

                        var result = await _userManager.CreateAsync(student, "leXicon");
                        if (result.Succeeded)
                        {
                            _userManager.AddToRole(student.Id, "Student");
                        }
                    }
                    System.IO.File.Delete(ParticipantFilePath);
                    return RedirectToAction("Details", "Courses", new { Id = CourseId }); // Success!
                }
                else
                {
                    try
                    {
                        System.IO.File.Delete(ParticipantFilePath);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex.Message;
                        return View();
                    }
                    ViewBag.Message = "Uploaded file is not of the right standard for these accounts!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }

            //return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
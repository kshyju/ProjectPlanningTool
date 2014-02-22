using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TeamBins.Services;
using TechiesWeb.TeamBins.Infrastructure;
using TechiesWeb.TeamBins.ViewModels;

namespace TechiesWeb.TeamBins.Controllers
{
    public class AccountController : BaseController
    {
            
        UserService userService;
        public AccountController()
        {          
            userService = new UserService(repo);
        }

        public AccountController(IRepositary repositary): base(repositary)
        {            

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Join(string returnurl="")
        {
            return View(new AccountSignupVM { ReturnUrl = returnurl });
        }

        [HttpPost]
        public ActionResult Join(AccountSignupVM model)
        {
            try
            {               
                if (ModelState.IsValid)
                {
                    var user = repo.GetUser(model.Email);
                    if (user == null)
                    {
                        var newUser = new User { EmailAddress = model.Email, FirstName = model.Name, Password = model.Password };
                        // SecurityService.SetNewPassword(newUser, model.Password); 
                       
                        var result = repo.SaveUser(newUser);
                        if (result.Status)
                        {
                            var team = new Team { Name = newUser.FirstName.Replace(" ", "-"), CreatedByID = result.OperationID };
                            if (team.Name.Length > 19)
                                team.Name = team.Name.Substring(0, 19);

                            var res = repo.SaveTeam(team);

                            var teamMember = new TeamMember { MemberID = result.OperationID, TeamID = team.ID, CreatedByID = result.OperationID };
                            repo.SaveTeamMember(teamMember);
                            if (teamMember.ID > 0)
                            {
                                SetUserIDToSession(result.OperationID, team.ID, model.Name);
                            }

                            if (!String.IsNullOrEmpty(model.ReturnUrl))
                                return RedirectToAction("JoinMyTeam", "Users", new { id = model.ReturnUrl });

                            return RedirectToAction("AccountCreated");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Account already exists with this email address");
                    }
                }               
            }
            catch (Exception ex)
            {
                log.Error(ex);
            } 
            return View(model);
        }
       
        public ActionResult AccountCreated()
        {
            return View();
        }
        
        public ActionResult Login()
        {       
            return View("Login",new LoginVM());
        }
       
        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = repo.GetUser(model.Email);
                    if (user != null)
                    {
                        //string hashed = SecurityService.GetPasswordHash(model.Password);
                        // var s= PasswordHash.ValidatePassword(model.Password,user.HA);
                        if (user.Password == model.Password)
                        {
                            var teamMember = user.TeamMembers1.Where(s => s.MemberID == user.ID).FirstOrDefault();
                            SetUserIDToSession(user.ID, teamMember.TeamID, user.FirstName);
                            return RedirectToAction("Index", "Dashboard");
                        }
                    }
                }
                ModelState.AddModelError("", "Username/Password is incorrect!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Oops! Something went wrong :(");
                log.Error(ex);
            }
            return View(model);
        }

        public ActionResult reset(string id)
        {
            var vm = new ResetPasswordVM();
            return View(vm);
        }

        public ActionResult forgotPassword()
        {
            return View("forgotPassword",new ForgotPasswordVM());
        }
        public ActionResult forgotPasswordEmailSent()
        {
            return View();
        }
        [HttpPost]
        public ActionResult forgotpassword(ForgotPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = repo.GetUser(model.Email);
                    if (user != null)
                    {
                        var passwordResetRequest = new PasswordResetRequest { UserID = user.ID };
                        passwordResetRequest.ActivationCode = Guid.NewGuid().ToString("n") + user.ID;
                        repo.SavePasswordResetRequest(passwordResetRequest);

                        var resetRequest = repo.GetPasswordResetRequest(passwordResetRequest.ActivationCode);
                        userService = new UserService(repo,SiteBaseURL);
                        userService.SendResetPasswordEmail(resetRequest);

                        return RedirectToAction("forgotPasswordEmailSent");
                    }
                    ModelState.AddModelError("", "We do not see an account with that email address!");
                }
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error in processing the your request");
            }
            return View(model);
        }

        public ActionResult ResetPassword(string id)
        {
            //coming from the password reset link received in email
            var passwordResetRequest = repo.GetPasswordResetRequest(id);
            if (passwordResetRequest != null)
            {
                var user = repo.GetUser(passwordResetRequest.UserID);
                if (user != null)
                {
                    return View(new ResetPasswordVM { ActivationCode = passwordResetRequest.ActivationCode });
                }
            }
            return View("NotFound");
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var passwordRequest = repo.GetPasswordResetRequest(model.ActivationCode);
                if (passwordRequest != null)
                {
                    var user = repo.GetUser(passwordRequest.UserID);
                    if (user != null)
                    {
                        user.Password = model.Password;
                        var result = repo.SaveUser(user);
                        if (result.Status)
                        {
                            return RedirectToAction("PasswordUpdated");
                        }
                    }
                }

            }
            return View(model);

        }
        public ActionResult PasswordUpdated()
        {
            return View();
        }
        public ActionResult Profile()
        {            
            var user = repo.GetUser(UserID);
            if(user!=null)
            {
                var vm = new EditProfileVM { Name = user.FirstName, Email = user.EmailAddress };
                return View(vm);
            }
            return View("NotFound");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult EditProfile()
        {
            var user = repo.GetUser(UserID);
            if (user != null)
            {
                var vm = new EditProfileVM { Name = user.FirstName, Email = user.EmailAddress };
                
               
                return View(vm);
            }
            return View("NotFound");
        }
       

        [HttpPost]
        public ActionResult EditProfile(EditProfileVM model)
        {
            if(ModelState.IsValid)
            {
                var user = repo.GetUser(UserID);
                if(user!=null)
                {
                    user.FirstName = model.Name;                  
                    var result = repo.SaveUser(user);
                    if(result.Status)
                    {
                        var msg = new AlertMessageStore();
                        msg.AddMessage("success", "Profile updated successfully");
                        TempData["AlertMessages"] = msg;
                        return RedirectToAction("EditProfile");
                    }
                }
            }
            return View(model);
        }

        public ActionResult Password()
        {
            var vm = new ChangePasswordVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Password(ChangePasswordVM model)
        {
            if(ModelState.IsValid)
            {
                var user = repo.GetUser(UserID);
                if (user != null && user.Password==model.Password)
                {
                    user.Password = model.Password;
                    var result = repo.SaveUser(user);
                    if (result.Status)
                    {
                        var msg = new AlertMessageStore();
                        msg.AddMessage("success", "Password updated successfully");
                        TempData["AlertMessages"] = msg;
                    }
                }
            }
            return View(model);
        }

        public ActionResult Settings()
        {
            var vm = new DefaultIssueSettings { Projects = GetProjectListItem() };
            var userService = new UserService(repo, SiteBaseURL);
            vm.SelectedProject = userService.GetDefaultProjectForCurrentTeam(UserID, TeamID);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Settings(DefaultIssueSettings model)
        {
            if(ModelState.IsValid)
            {
                var result =userService.SaveDefaultProjectForTeam(UserID,TeamID, model.SelectedProject);                
                if (result)
                {
                    var msg = new AlertMessageStore();
                    msg.AddMessage("success", "Settings updated successfully");
                    TempData["AlertMessages"] = msg;
                    return RedirectToAction("Settings");
                }                
            }
            model.Projects = GetProjectListItem();
            return View(model);
        }

        private List<SelectListItem> GetProjectListItem()
        {           
            var projects = repo.GetProjects(TeamID).
                                Where(s=>s.TeamID==TeamID).
                                Select(c=> new SelectListItem { Value=c.ID.ToString(), Text=c.Name}).
                                ToList();

            return projects;
        }

    }
}

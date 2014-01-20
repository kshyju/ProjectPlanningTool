using TeamBins.Services;
using SmartPlan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.Infrastructure;
using TechiesWeb.TeamBins.ViewModels;


namespace TechiesWeb.TeamBins.Controllers
{
    public class AccountController : BaseController
    {
            IRepositary repo;
            UserService userService;
            public AccountController()
        {
            repo = new Repositary();
            userService = new UserService(repo);
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
            if (ModelState.IsValid)
            {                
                var user = repo.GetUser(model.Email);
                if (user == null)
                {
                    var newUser = new User { EmailAddress = model.Email, FirstName=model.Name, Password=model.Password };
                   // SecurityService.SetNewPassword(newUser, model.Password);                  
                   var result = repo.SaveUser(newUser);
                    if (result.Status)
                    {
                        var team = new Team { Name = newUser.FirstName.Replace(" ","-") };
                        if (team.Name.Length > 19)
                            team.Name = team.Name.Substring(0, 19);

                        var res = repo.SaveTeam(team);

                        var teamMember = new TeamMember { MemberID = result.OperationID, TeamID = team.ID, CreatedByID = result.OperationID };
                        repo.SaveTeamMember(teamMember);
                        if (teamMember.ID > 0)
                        {
                            SetUserIDToSession(result.OperationID, team.ID,model.Name);
                        }

                       /* var notificationVM = new NotificationVM { Title = "New User Joined", Message = model.Name + " joined" };
                        var context = GlobalHost.ConnectionManager.GetHubContext<UserHub>();
                        context.Clients.All.ShowNotificaion(notificationVM);*/

                        if(!String.IsNullOrEmpty(model.ReturnUrl))
                            return RedirectToAction("JoinMyTeam", "Users", new { id = model.ReturnUrl });

                        return RedirectToAction("AccountCreated");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Account already exists with this email address");
                }
            }
            return View(model);
        }
        public ActionResult AccountCreated()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View(new LoginVM { Email = "kshyju1@gmail.com", Password = "" });
        }
        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = repo.GetUser(model.Email);
                if (user != null)
                {
                    string hashed = SecurityService.GetPasswordHash(model.Password);
                  // var s= PasswordHash.ValidatePassword(model.Password,user.HA);
                   if (user.Password==model.Password)
                   {
                       var teamMember=user.TeamMembers1.Where(s=>s.MemberID==user.ID).FirstOrDefault();
                       SetUserIDToSession(user.ID, teamMember.TeamID, user.FirstName);
                       return RedirectToAction("Index", "Dashboard");
                   }
                }
            }
            ModelState.AddModelError("", "Username/Password is incorrect!");
            return View(model);
        }

        public ActionResult reset(string id)
        {
            var vm = new ResetPasswordVM();
            return View(vm);
        }

        public ActionResult forgotPassword()
        {
            return View(new ForgotPasswordVM());
        }
        [HttpPost]
        public ActionResult forgotpassword(ForgotPasswordVM model)
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
            var teamMember = repo.GetTeamMember(UserID, TeamID);            
            if (teamMember != null)
            {
                if (teamMember.DefaultProjectID.HasValue)
                    vm.SelectedProject = teamMember.DefaultProjectID.Value;
            }
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
            var projects = repo.GetProjects().
                                Where(s=>s.TeamID==TeamID).
                                Select(c=> new SelectListItem { Value=c.ID.ToString(), Text=c.Name}).
                                ToList();

            return projects;
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

using TeamBins.Common.ViewModels;
using TeamBins.Infrastrucutre.Filters;
using TeamBins.Infrastrucutre.Services;


namespace TeamBins.Controllers.Web
{
    [LoginCheckFilter]
    public class UsersController : BaseController
    {
        readonly ITeamManager _teamManager;
        public UsersController(ITeamManager teamManager)
        {
            this._teamManager = teamManager;
        }
        public async Task<ActionResult> Index()
        {
            var teamVm = await _teamManager.GetTeamInoWithMembers();
            return View(teamVm);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Add(AddTeamMemberRequestVM model)
        {
            try
            {
                //if (ModelState.IsValid)
                {
                    await _teamManager.AddNewTeamMember(model);
                    return Json(new { Status = "Success", Message = "Successfully added user to team" });
                }
                ////else
                //{
                //    var errors = ViewData.ModelState.Values.SelectMany(s => s.Errors.Select(g => g.ErrorMessage));
                //    return Json(new { Status = "Error", Message = "Error adding user to team", Errors= errors });
                //}
            }
            catch (Exception ex)
            {
                //Log
            }

            return Json(new { Status = "Error", Message = "Error adding user to team" });
        }


        public async Task<IActionResult> JoinMyTeam(string id)
        {
            // For users who received an email with the join link to join a team.
            // The user must have created an account by now and coming back to this link after registration

            try
            {
                var result = await _teamManager.ValidateAndAssociateNewUserToTeam(id);
                // TO DO : Add activity item
                if (result)
                {
                    return View("WelcomeToTeam");
                }
                return View("NotFound");
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }

    }
}

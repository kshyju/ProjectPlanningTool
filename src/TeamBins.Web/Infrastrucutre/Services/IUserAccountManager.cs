using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Infrastrucutre.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using TeamBins.DataAccessCore;

using TeamBins.DataAccess;
using TeamBins.Infrastrucutre;

namespace TeamBins.Services
{

    public interface IUserAccountManager
    {
        Task SaveNotificationSettings(UserEmailNotificationSettingsVM model);
        Task<DefaultIssueSettings> GetIssueSettingsForUser();
        Task<EditProfileVm> GetUserProfile();
        Task SetDefaultTeam(int userId, int teamId);
        Task<IEnumerable<TeamDto>> GetTeams(int userId);
        Task<UserAccountDto> GetUser(int id);
        
        Task UpdateProfile(EditProfileVm model);
        //void UpdatePassword(ChangePasswordVM model);
        Task SaveDefaultProjectForTeam(DefaultIssueSettings defaultIssueSettings);
        Task<UserAccountDto> GetUser(string email);
        Task<LoggedInSessionInfo> CreateAccount(UserAccountDto userAccount);
        Task<UserEmailNotificationSettingsVM> GetNotificationSettings();
        Task UpdateLastLoginTime(int userId);

        Task SavePasswordResetRequest(UserAccountDto user);
        Task<PasswordResetRequest> GetPasswordResetRequest(string activationCode);
        Task UpdatePassword(string password, int userId);
    }

    public class UserAccountManager : IUserAccountManager
    {
        readonly AppSettings _settings;

        private IEmailRepository emailRepository;
        private IEmailManager emailManager;
        private IProjectManager projectManager;
        private IUserRepository userRepository;
        private IUserAuthHelper userSessionHelper;
        private ITeamRepository teamRepository;
        public UserAccountManager(IUserRepository userRepository, IUserAuthHelper userSessionHelper, IProjectManager projectManager, ITeamRepository teamRepository, IEmailManager emailManager, IEmailRepository emailRepository, IOptions<AppSettings> settings)
        {
            this.emailManager = emailManager;
            this.userRepository = userRepository;
            this.userSessionHelper = userSessionHelper;
            this.projectManager = projectManager;
            this.teamRepository = teamRepository;
            this.emailRepository = emailRepository;
            this._settings = settings.Value;
        }

        public async Task SaveDefaultProjectForTeam(DefaultIssueSettings defaultIssueSettings)
        {
            defaultIssueSettings.TeamId = this.userSessionHelper.TeamId;
            defaultIssueSettings.UserId = this.userSessionHelper.UserId;
            await this.userRepository.SaveDefaultIssueSettings(defaultIssueSettings);
        }
        public async Task<EditProfileVm> GetUserProfile()
        {
            var vm = new EditProfileVm();
            var user = await this.userRepository.GetUser(this.userSessionHelper.UserId);
            if (user != null)
            {
                vm.Name = user.Name;
                vm.Email = user.EmailAddress;
            }
            return vm;
        }



        public async Task<DefaultIssueSettings> GetIssueSettingsForUser()
        {
            var vm = new DefaultIssueSettings();
            vm.Projects = this.projectManager.GetProjects(this.userSessionHelper.TeamId)
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToList();


            var tm = this.teamRepository.GetTeamMember(this.userSessionHelper.TeamId, this.userSessionHelper.UserId);
            vm.SelectedProject = tm.DefaultProjectId;
            return await Task.FromResult(vm);
        }

        public async Task UpdateLastLoginTime(int userId)
        {
            await this.userRepository.UpdateLastLoginTime(userId);
        }
        public async Task SetDefaultTeam(int userId, int teamId)
        {
            await this.userRepository.SetDefaultTeam(userId, teamId);
        }
        public async Task<UserAccountDto> GetUser(int id)
        {
            return await this.userRepository.GetUser(id);
        }
        public async Task<UserAccountDto> GetUser(string email)
        {
            return await this.userRepository.GetUser(email);
        }
        public async Task<IEnumerable<TeamDto>> GetTeams(int userId)
        {
            return await userRepository.GetTeams(userId);

        }

        public async Task<PasswordResetRequest> GetPasswordResetRequest(string activationCode)
        {
            return await this.userRepository.GetPasswordResetRequest(activationCode);
        }
        public async Task SavePasswordResetRequest(UserAccountDto user)
        {
            var passwordResetRequest = new PasswordResetRequest
            {
                UserId = user.Id,
                ActivationCode = string.Format("{0}{1}{2}", Guid.NewGuid().ToString().Split('-').First(), user.Id, Guid.NewGuid().ToString().Split('-').First())
            };
            await this.userRepository.SavePasswordResetRequest(passwordResetRequest);
            passwordResetRequest.User = user;
            //Email
            await SendEmailNotificaionForResetPassword(passwordResetRequest);
        }

        private async Task SendEmailNotificaionForResetPassword(PasswordResetRequest passwordResetRequest)
        {
            try
            {
                var emailTemplate = await emailRepository.GetEmailTemplate("ResetPassword");
                if (emailTemplate != null)
                {
                    var emailSubject = emailTemplate.Subject;
                    var emailBody = emailTemplate.EmailBody;
                    var email = new Email();

                    email.ToAddress.Add(passwordResetRequest.User.EmailAddress);

                    var url = this._settings.SiteUrl + "/Account/resetpassword/" + passwordResetRequest.ActivationCode;
                    var link = string.Format("<a href='{0}'>{0}</a>", url);

                    emailBody = emailBody.Replace("@resetLink", link);

                    email.Body = emailBody;
                    email.Subject = emailSubject;
                    await this.emailManager.Send(email);
                }

            }
            catch (Exception)
            {
                // Silently fail. We will log this. But we do not want to show an error to user because of this
            }
        }


        public async Task UpdatePassword(string password, int userId)
        {
            await userRepository.UpdatePassword(password, userId);
        }
        public async Task<LoggedInSessionInfo> CreateAccount(UserAccountDto userAccount)
        {
            var userId = await userRepository.CreateAccount(userAccount);
            var teamName = userAccount.Name.Replace(" ", "") + " Team";
            var teamId = await Task.FromResult(teamRepository.SaveTeam(new TeamDto { CreatedById = userId, Name = teamName }));
            await this.userRepository.SetDefaultTeam(userId, teamId);

            await this.emailManager.SendAccountCreatedEmail(new UserDto
            {
                Name = userAccount.Name,
                EmailAddress = userAccount.EmailAddress
            });

            return new LoggedInSessionInfo() { TeamId = teamId, UserId = userId, UserDisplayName = userAccount.Name };
        }
        public async Task UpdateProfile(EditProfileVm model)
        {
            model.Id = this.userSessionHelper.UserId;
            await userRepository.SaveUserProfile(model);
        }

        public async Task<UserEmailNotificationSettingsVM> GetNotificationSettings()
        {
            return new UserEmailNotificationSettingsVM
            {
                TeamId = userSessionHelper.TeamId,
                EmailSubscriptions = await userRepository.EmailSubscriptions(userSessionHelper.UserId, userSessionHelper.TeamId)
            };
        }
        public async Task SaveNotificationSettings(UserEmailNotificationSettingsVM model)
        {
            model.UserId = userSessionHelper.UserId;
            model.TeamId = userSessionHelper.TeamId;
            await userRepository.SaveNotificationSettings(model);

        }
    }

}
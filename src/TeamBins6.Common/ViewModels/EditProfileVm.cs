using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;

namespace TeamBins.Common.ViewModels
{
    public class SettingsVm
    {
        public EditProfileVm Profile { set; get; }
        public ChangePasswordVM PasswordChange { set; get; }

        public DefaultIssueSettings IssueSettings { set; get; }

        public UserEmailNotificationSettingsVM NotificationSettings { set; get; }

        public SettingsVm()
        {
            this.Profile = new EditProfileVm();
            PasswordChange = new ChangePasswordVM();
            IssueSettings = new DefaultIssueSettings();
            NotificationSettings = new UserEmailNotificationSettingsVM();
        }
    }

    public class EditProfileVm
    {
        [Required]
        [StringLength(20)]
        public string Name { set; get; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }

        public int Id { set; get; }

    }

    public class ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string Password { set; get; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string ConfirmPassword { set; get; }

        public string ActivationCode { set; get; }
    }

    public class ChangePasswordVM : ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string CurrentPassword { set; get; }

    }

    public class DefaultIssueSettings
    {
        public int UserId { set; get; }
        public int TeamId { set; get; }
        public List<SelectListItem> Projects { set; get; }
        public int? SelectedProject { set; get; }

        public DefaultIssueSettings()
        {
            Projects = new List<SelectListItem>();
        }
    }

    public class UserEmailNotificationSettingsVM
    {
        public int TeamID { set; get; }
        public List<EmailSubscriptionVM> EmailSubscriptions { set; get; }
        public UserEmailNotificationSettingsVM()
        {
            EmailSubscriptions = new List<EmailSubscriptionVM>();
        }
    }
    public class EmailSubscriptionVM
    {
        public int NotificationTypeID { set; get; }
        public string Name { set; get; }
        public bool IsSelected { set; get; }
    }
}

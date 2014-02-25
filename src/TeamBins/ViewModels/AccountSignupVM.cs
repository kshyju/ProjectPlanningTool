using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechiesWeb.TeamBins.ViewModels
{
    public class LoginVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }

        [Required]
        [DataType(DataType.Password)]        
        public string Password { set; get; }

    }
    public class AccountSignupVM
    {
        [Required]
        [StringLength(20)]
        public string Name { set; get; }

        [Required]
        [DataType(DataType.EmailAddress)]        
        [StringLength(100)]
        public string Email { set; get; }
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength=6,ErrorMessage="Password must have minimum of 6 characters")]
        public string Password { set; get; }

        public string ReturnUrl { set; get; }
    }
    public class ForgotPasswordVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }
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

    public class EditProfileVM
    {
        [Required]
        [StringLength(20)]
        public string Name { set; get; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }
        
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
        public List<SelectListItem> Projects { set; get; }
        public int SelectedProject { set; get; }

        public DefaultIssueSettings()
        {
            Projects = new List<SelectListItem>();
        }
    }

    public class UserEmailNotificationSettingsVM
    {
        public int TeamID { set;get;}
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

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

        public bool RememberMe { set; get; }

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
    




   


}

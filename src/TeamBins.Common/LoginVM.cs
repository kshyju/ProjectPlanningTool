using System;
using System.ComponentModel.DataAnnotations;

namespace TeamBins.Common.ViewModels
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
        [StringLength(25, MinimumLength = 6, ErrorMessage = "Password must have minimum of 6 characters")]
        public string Password { set; get; }

        public string ReturnUrl { set; get; }
    }
    public class ForgotPasswordVm
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }
    }

    public class PasswordResetRequest
    {
        public int Id { set; get; }
        public int UserId { set; get; }
        public string ActivationCode { set; get; }
        public UserDto User { set; get; }

        public DateTime CreatedDate { set; get; }
    }

}
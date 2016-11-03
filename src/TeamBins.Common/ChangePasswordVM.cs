using System.ComponentModel.DataAnnotations;

namespace TeamBins.Common.ViewModels
{
    public class ChangePasswordVM : ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string CurrentPassword { set; get; }

    }
}
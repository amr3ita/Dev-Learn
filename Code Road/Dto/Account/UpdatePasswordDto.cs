using System.ComponentModel.DataAnnotations;

namespace Code_Road.Dto.Account
{
    public class UpdatePasswordDto
    {
        [DataType(DataType.EmailAddress)]
        public string Gmail { get; set; }
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^*(?=.{7,})(?=.*[\\d])(?=.*[\\W]).*$", ErrorMessage = "Password Must contains at least 7 characters and at least one digit and at least one special character")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [RegularExpression("^*(?=.{7,})(?=.*[\\d])(?=.*[\\W]).*$", ErrorMessage = "Password Must contains at least 7 characters and at least one digit and at least one special character")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}

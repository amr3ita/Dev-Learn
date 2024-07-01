using System.ComponentModel.DataAnnotations;

namespace Code_Road.Dto.Account
{
    public class AddUserToRoleDto
    {
        [EmailAddress]
        public string UserEmail { get; set; }
        public string Role { get; set; }
    }
}

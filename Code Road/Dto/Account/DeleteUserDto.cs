using System.ComponentModel.DataAnnotations;

namespace Code_Road.Dto.Account
{
    public class DeleteUserDto
    {
        [DataType(DataType.EmailAddress)]
        public string AdminGmail { get; set; }
        [DataType(DataType.EmailAddress)]
        public string UserGmail { get; set; }
    }
}

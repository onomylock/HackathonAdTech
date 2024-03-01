using System.ComponentModel.DataAnnotations;

namespace HackathonAdTech.Domain.Entities.DTO.Auth
{
    public class UserForAuthDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}

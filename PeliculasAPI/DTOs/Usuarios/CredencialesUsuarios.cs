using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public sealed class CredencialesUsuarios
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

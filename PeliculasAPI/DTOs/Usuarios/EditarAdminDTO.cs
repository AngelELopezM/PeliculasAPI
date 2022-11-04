using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public sealed class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

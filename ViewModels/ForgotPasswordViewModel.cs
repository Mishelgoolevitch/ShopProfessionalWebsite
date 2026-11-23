using System.ComponentModel.DataAnnotations;

namespace ShopProfessionalWebsite.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}

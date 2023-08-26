using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class RegisterUserRequestViewModel
    {
        [Display(Name = "first name")]
        [Required(ErrorMessage = "please enter {0}")]
        [MaxLength(50, ErrorMessage = "{0} exceeds {1} characters")]
        public string FirstName { get; set; }
        [Display(Name = "last name")]
        [Required(ErrorMessage = "please enter {0}")]
        [MaxLength(50, ErrorMessage = "{0} exceeds {1} characters")]

        public string LastName { get; set; }
        [Display(Name = "email address")]
        [Required(ErrorMessage = "please enter {0}")]
        [EmailAddress(ErrorMessage = "{0} is invalid")]
        public string Email { get; set; }
        [Display(Name = "password")]
        [Required(ErrorMessage = "please enter {0}")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "{0} must contain at least 1 lowercase, 1 uppercase, 1 special character, and 1 digit")]
        public string Password { get; set; }
    }
}

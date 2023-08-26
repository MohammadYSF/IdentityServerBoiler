using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public record LoginRequestViewModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "{0} is invalid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string ClientId { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string ClientSecret { get; set; }
    }
}

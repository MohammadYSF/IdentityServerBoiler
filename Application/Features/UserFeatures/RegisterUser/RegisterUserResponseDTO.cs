using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserFeatures.RegisterUser
{
    public sealed class RegisterUserResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public bool Success { get; set; }
        public IDictionary<string, string> ErrorMessages { get; set; } = new Dictionary<string, string>();
    }
}

using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
    }
}

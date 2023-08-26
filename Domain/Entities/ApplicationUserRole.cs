using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
    }
}

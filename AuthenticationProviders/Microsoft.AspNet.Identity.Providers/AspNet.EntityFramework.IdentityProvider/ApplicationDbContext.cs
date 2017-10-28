using System.Data.Entity;
using AspNet.EntityFramework.IdentityProvider.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Provider.EntityFramework.Configurtion;

namespace AspNet.EntityFramework.IdentityProvider
{
    [DbConfigurationType(typeof(CustomDbConfiguration))]
    internal class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
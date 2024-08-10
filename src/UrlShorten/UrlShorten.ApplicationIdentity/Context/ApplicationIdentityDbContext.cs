using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.ApplicationIdentity.Manager;

namespace UrlShorten.ApplicationIdentity.Context
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationIdentityUser,IdentityRole<Guid>,Guid>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
        {
        }
    }
}

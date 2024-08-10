using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.Models;

namespace UrlShorten.DataAccess.Context
{
    public class UrlShortenDbContext : DbContext
    {
        public UrlShortenDbContext(DbContextOptions<UrlShortenDbContext> options) : base(options)
        {
        }

        public DbSet<Url> Urls { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

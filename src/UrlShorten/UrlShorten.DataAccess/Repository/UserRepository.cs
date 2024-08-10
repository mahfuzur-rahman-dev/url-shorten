using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.DataAccess.Context;
using UrlShorten.DataAccess.Repository.IRepository;
using UrlShorten.Models;

namespace UrlShorten.DataAccess.Repository
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        private readonly UrlShortenDbContext _context;
        public UserRepository(UrlShortenDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
    }
}

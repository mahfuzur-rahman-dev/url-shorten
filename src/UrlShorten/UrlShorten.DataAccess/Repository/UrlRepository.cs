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
    public class UrlRepository : Repository<Url, Guid>, IUrlRepository
    {
        private readonly UrlShortenDbContext _context;
        public UrlRepository(UrlShortenDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Url url)
        {
            _context.Urls.Update(url);
        }
    }
}

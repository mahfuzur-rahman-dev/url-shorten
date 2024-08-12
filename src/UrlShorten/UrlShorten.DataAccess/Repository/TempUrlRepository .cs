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
    public class TempUrlRepository : Repository<TempUrl, Guid>, ITempUrlRepository
    {
        private readonly UrlShortenDbContext _context;
        public TempUrlRepository(UrlShortenDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(TempUrl url)
        {
            _context.TempUrls.Update(url);
        }
    }
}

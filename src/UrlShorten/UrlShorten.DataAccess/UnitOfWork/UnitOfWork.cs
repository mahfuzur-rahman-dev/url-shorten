using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.DataAccess.Context;
using UrlShorten.DataAccess.Repository;
using UrlShorten.DataAccess.Repository.IRepository;

namespace UrlShorten.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUrlRepository Url {  get; set; }
        public IUserRepository User {  get; set; }


        private readonly UrlShortenDbContext _context;
        public UnitOfWork(UrlShortenDbContext context)
        {
            _context = context;
            Url = new UrlRepository(_context);
            User = new UserRepository(_context);

        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.DataAccess.Repository.IRepository;

namespace UrlShorten.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUrlRepository Url { get; }
        ITempUrlRepository TempUrl { get; }
        IUserRepository User { get; }

        Task SaveAsync();
    }
}

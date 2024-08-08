using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.Models;

namespace UrlShorten.DataAccess.Repository.IRepository
{
    public interface IUrlRepository: IRepository<Url,Guid>
    {
        void Update(Url url);

    }
}

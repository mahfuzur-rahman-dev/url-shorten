using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShorten.Models;

namespace UrlShorten.DataAccess.Repository.IRepository
{
    public interface ITempUrlRepository: IRepository<TempUrl,Guid>
    {
        void Update(TempUrl url);

    }
}

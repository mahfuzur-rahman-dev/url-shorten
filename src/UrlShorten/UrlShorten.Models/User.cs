using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShorten.Models
{
    public class User :IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string  Name { get; set; }
        public string Email { get; set; }
    }
}

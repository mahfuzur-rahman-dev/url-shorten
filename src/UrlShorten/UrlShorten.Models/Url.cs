using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShorten.Models
{
    public class Url : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string LongUrl { get; set; }
        public string Domain {  get; set; }
        public string ShortKeyword { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set;}
        public Guid? MemberId { get; set; }
    }
}

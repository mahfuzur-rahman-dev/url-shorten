using System.ComponentModel.DataAnnotations;

namespace UrlShorten.Web.Models
{
    public class CheckAvailabilityRequestModel
    {
        public string LongUrl { get; set; }

        [StringLength(15, MinimumLength = 4)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "ShortKeyword can only contain letters and numbers")]
        public string ShortKeyword { get; set; }
    }
}

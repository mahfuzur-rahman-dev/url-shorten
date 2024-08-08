namespace UrlShorten.Web.Models
{
    public class CheckAvailabilityRequestModel
    {
        public string LongUrl { get; set; }
        public string ShortKeyword { get; set; }
    }
}

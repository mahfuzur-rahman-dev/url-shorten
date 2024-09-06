namespace UrlShorten.Web.Models
{
    public class CreateShortUrlRequestModel
    {
        public string LongUrl { get; set; }
        public string Domain { get; set; }
        public string ShortKeyword { get; set; }
    }
}

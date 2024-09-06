using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using UrlShorten.DataAccess.UnitOfWork;
using UrlShorten.Models;

namespace UrlShorten.Web.Others
{
    public class CookieManagement
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public CookieManagement(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task SetUrlInCookie(string shortUrl, Guid? id)
        {
            string cookieName = string.Empty;
            CookieOptions options = new CookieOptions()
            {
                Domain = "localhost", // Set the domain for the cookie
                Path = "/", // Cookie is available within the entire application
                Secure = false, // Ensure the cookie is only sent over HTTPS (set to false for local development)
                HttpOnly = true, // Prevent client-side scripts from accessing the cookie
            };

            if (id == null)
            {
                var cookiesCount = await ReadCookieCount("FreeUrlShorten_");
                cookieName = $"FreeUrlShorten_{cookiesCount}";
            }
            else
            {
                cookieName = $"UrlShorten_{id}";
            }

            // Append UserId to the cookies
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, shortUrl, options);
        }

        public async Task<List<string>> ReadCookie(string prefix)
        {
            var cookies = _httpContextAccessor.HttpContext.Request.Cookies;
            var values = new List<string>();

            foreach (var cookie in cookies)
            {
                if (cookie.Key.StartsWith(prefix))
                {
                    values.Add(cookie.Value);
                }
            }

            return values;
        }

        public async Task<int> ReadCookieCount(string prefix)
        {
            var cookies = _httpContextAccessor.HttpContext.Request.Cookies;
            int count = 0;

            foreach (var cookie in cookies)
            {
                if (cookie.Key.StartsWith(prefix))
                {
                    count++;
                }
            }

            return count;
        }


        public async Task CopyTheCookieUrls(Guid userId, List<string> cookies)
        {
            foreach (var cookie in cookies)
            {
                var tempUrl = await _unitOfWork.TempUrl.Get(x => x.ShortUrl == cookie);
                if (tempUrl != null)
                {
                    try
                    {
                        var copyTempUrlToUrl = new Url
                        {
                            LongUrl = tempUrl.LongUrl,
                            Domain = tempUrl.Domain,
                            ShortKeyword = tempUrl.ShortKeyword.ToLower(),
                            ShortUrl = "https://localhost:7063/" + tempUrl.Domain + "/" + tempUrl.ShortKeyword.ToLower(),
                            CreatedDateTime = DateTime.Now,
                            UpdatedDateTime = DateTime.Now,
                            UserId = userId
                        };

                        DeleteUrlsFromCookies(cookie);

                        await _unitOfWork.Url.AddAsync(copyTempUrlToUrl);
                        await _unitOfWork.SaveAsync();


                         _unitOfWork.TempUrl.Remove(tempUrl);
                        await _unitOfWork.SaveAsync();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }

        }

        public async void DeleteUrlsFromCookies(string cookieValue)
        {
            if (!string.IsNullOrEmpty(cookieValue))
            {
                var cookies = _httpContextAccessor.HttpContext.Request.Cookies;

                // Iterate over each cookie and delete if it matches the value
                foreach (var cookie in cookies)
                {
                    // Check if the cookie's value matches the one we want to delete
                    if (cookie.Value.Contains(cookieValue))
                    {
                        // Perform the deletion of the cookie
                        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie.Key);
                    }
                }
            }
        }
    }
}

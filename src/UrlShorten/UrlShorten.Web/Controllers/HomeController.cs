using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UrlShorten.ApplicationIdentity.Manager;
using UrlShorten.DataAccess.UnitOfWork;
using UrlShorten.Models;
using UrlShorten.Web.Models;


namespace UrlShorten.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationIdentityUser> _userManager; // Inject UserManager

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<ApplicationIdentityUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (IsUserLoggedIn())
            {
                var userId = _userManager.GetUserId(User);
                var user = await _unitOfWork.User.GetByIdAsync(Guid.Parse(userId));

                var allUrls = await _unitOfWork.Url.GetAsync(x=>x.UserId ==user.Id);

                ViewBag.UserName = user.Name;
                ViewBag.AllUrls = allUrls;
                ViewBag.Cookies = null;

                return View();

            }

            var cookies = await ReadCookie("FreeUrlShorten_");

            ViewBag.AllUrls = null;
            ViewBag.Cookies = cookies;
            ViewBag.UserName = null;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckShortKeywordAvailability(CheckAvailabilityRequestModel request)
        {
            if (ModelState.IsValid)
            {
                var checkShortKeyword = await _unitOfWork.Url.GetCountAsync(x => x.ShortKeyword == request.ShortKeyword.ToLower());
                if (checkShortKeyword > 0)
                    return Json(new { Available = false });
                else
                    return Json(new { Available = true });
            }
            else
                return BadRequest(new { message = "There cannot be empty space and length should be between 4 and 15" });

        }


        [HttpPost]
        public async Task<IActionResult> CreateShortUrl(CreateShortUrlRequestModel request)
        {
            if (request is null)
                return BadRequest(new {massage ="No data found..!"});
            
            if (ModelState.IsValid)
            {                    
                try
                {
                    var cookiesCount = await ReadCookieCount("FreeUrlShorten_");

                    if (!IsUserLoggedIn() && cookiesCount<3)
                    {
                        var addTempUrl = new TempUrl
                        {
                            LongUrl = request.LongUrl,
                            Domain = request.Domain,
                            ShortKeyword = request.ShortKeyword.ToLower(),
                            ShortUrl = "https://localhost:7063/" + request.Domain + "/" + request.ShortKeyword.ToLower(),
                            CreatedDateTime = DateTime.Now,
                            UpdatedDateTime = DateTime.Now
                        };

                        SetUrlInCookie(addTempUrl.ShortUrl,null);
                        await _unitOfWork.TempUrl.AddAsync(addTempUrl);
                        await _unitOfWork.SaveAsync();

                        return Json(new { CreateUrlStatus = true });
                    }
                    if (!IsUserLoggedIn() && cookiesCount > 3)
                    {
                        return BadRequest(new {massage = "Please login to create more." });
                    }

                    var userId = _userManager.GetUserId(User);
                    var user = await _unitOfWork.User.GetByIdAsync(Guid.Parse(userId));

                    if (user is null)
                        return BadRequest(new {massage ="Internal Server error..."});

                    var addUrl = new Url
                    {
                        LongUrl = request.LongUrl,
                        Domain = request.Domain,
                        ShortKeyword = request.ShortKeyword.ToLower(),
                        ShortUrl = "https://localhost:7063/" + request.Domain + "/" + request.ShortKeyword.ToLower(),
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now,
                        UserId = user.Id
                    };
                    
                    await _unitOfWork.Url.AddAsync(addUrl);
                    await _unitOfWork.SaveAsync();

                    return Json(new { CreateUrlStatus = true }); // Return JSON with success status and short URL
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Json(new { CreateUrlStatus = false }); // Return JSON with failure status
                }
            }
            else
            {
                return BadRequest(new { message = "Model state error.." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> RedirectToLongUrl(string shortUrl)
        {
            var checkShortUrl = await _unitOfWork.Url.GetAsync(x => x.ShortUrl == shortUrl);
            if (checkShortUrl == null)
                return BadRequest(new { message = "Link broken" });

            var longUrl = checkShortUrl.Select(x => x.LongUrl).FirstOrDefault();

            // Return the long URL as JSON
            return Json(new { longUrl = longUrl });

        }

        private async Task SetUrlInCookie(string shortUrl,Guid? id)
        {
            string cookieName = string.Empty;
            CookieOptions options = new CookieOptions()
            {
                Domain = "localhost", // Set the domain for the cookie
                Path = "/", // Cookie is available within the entire application
                Secure = false, // Ensure the cookie is only sent over HTTPS (set to false for local development)
                HttpOnly = true, // Prevent client-side scripts from accessing the cookie

            };
            if(id == null)
            {
                var cookiesCount =await ReadCookieCount("FreeUrlShorten_");
                cookieName = $"FreeUrlShorten_{cookiesCount}";
            }            
            else
                cookieName = $"UrlShorten_{id}";


            // Append UserId to the cookies
            Response.Cookies.Append(cookieName, shortUrl, options);

        }

        public async Task<List<string>> ReadCookie(string prefix)
        {
            var cookies = Request.Cookies;
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
            var cookies = Request.Cookies;
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


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool IsUserLoggedIn()
        {
            return User.Identity?.IsAuthenticated ?? false;
        }
    }
}

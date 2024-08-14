using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UrlShorten.ApplicationIdentity.Manager;
using UrlShorten.DataAccess.UnitOfWork;
using UrlShorten.Models;
using UrlShorten.Web.Models;
using UrlShorten.Web.Others;


namespace UrlShorten.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly CookieManagement _cookieManagement;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<ApplicationIdentityUser> userManager,CookieManagement cookieManagement)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cookieManagement = cookieManagement;
        }

        public async Task<IActionResult> Index()
        {
            var cookies = await _cookieManagement.ReadCookie("FreeUrlShorten_");

            if (IsUserLoggedIn())
            {
                var userId = _userManager.GetUserId(User);
                var user = await _unitOfWork.User.GetByIdAsync(Guid.Parse(userId));

                if(cookies is not null)
                    await _cookieManagement.CopyTheCookieUrls(user.Id,cookies);

                var allUrls = await _unitOfWork.Url.GetAsync(x=>x.UserId ==user.Id);

                ViewBag.UserName = user.Name;
                ViewBag.AllUrls = allUrls;
                ViewBag.Cookies = null;

                return View();

            }

            

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
                var checkShortKeywordUrltable = await _unitOfWork.Url.GetCountAsync(x => x.ShortKeyword == request.ShortKeyword.ToLower());
                var checkShortKeywordTempUrlTable = await _unitOfWork.TempUrl.GetCountAsync(x => x.ShortKeyword == request.ShortKeyword.ToLower());
                if (checkShortKeywordUrltable > 0 || checkShortKeywordTempUrlTable > 0)
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
                    var cookiesCount = await _cookieManagement.ReadCookieCount("FreeUrlShorten_");

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

                        _cookieManagement.SetUrlInCookie(addTempUrl.ShortUrl,null);
                        await _unitOfWork.TempUrl.AddAsync(addTempUrl);
                        await _unitOfWork.SaveAsync();

                        return Json(new { CreateUrlStatus = true });
                    }
                    if (!IsUserLoggedIn() && cookiesCount > 2)
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
            var checkShortUrlInUrlTable = await _unitOfWork.Url.GetAsync(x => x.ShortUrl == shortUrl);
            var checkShortUrlInTempUrlTable = await _unitOfWork.TempUrl.GetAsync(x => x.ShortUrl == shortUrl);
            if (checkShortUrlInUrlTable == null && checkShortUrlInTempUrlTable==null)
                return BadRequest(new { message = "Link broken" });

            var longUrl = checkShortUrlInUrlTable.Select(x => x.LongUrl).FirstOrDefault();
            if(longUrl == null)
                longUrl = checkShortUrlInTempUrlTable.Select(x => x.LongUrl).FirstOrDefault();

            if(longUrl == null)
                return BadRequest(new { massage = "Internal server error" });

            return Json(new { longUrl = longUrl });

        }

        [Authorize]
        public async Task<IActionResult> MyUrls()
        {
            if (IsUserLoggedIn())
            {
                var userId = _userManager.GetUserId(User);
                //var userUrls = await _unitOfWork.Url.GetAsync(x=>x.UserId == Guid.Parse(userId));
                var userUrls = await _unitOfWork.Url.GetAsync();
                if (userUrls == null)
                    return View();
                
                ViewBag.UserUrls = userUrls;

                return View();

            }
            {
                ViewBag.UserUrls = null;
                return View();
            }
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

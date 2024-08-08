using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UrlShorten.DataAccess.UnitOfWork;
using UrlShorten.Web.Models;

namespace UrlShorten.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckShortKeywordAvailability(CheckAvailabilityRequestModel request)
        {
            var checkShortKeyword = await _unitOfWork.Url.GetCountAsync(x=>x.ShortKeyword == request.ShortKeyword);
            if(checkShortKeyword > 0)
                return Json(new { Available = true });
            else
                return Json(new { Available = false });
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
    }
}

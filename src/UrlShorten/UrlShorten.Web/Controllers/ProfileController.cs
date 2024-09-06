using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShorten.ApplicationIdentity.Manager;
using UrlShorten.DataAccess.UnitOfWork;
using UrlShorten.Web.Models;

namespace UrlShorten.Web.Controllers
{
    //[Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationIdentityUser> _userManager;

        public ProfileController(ILogger<ProfileController> logger, IUnitOfWork unitOfWork, UserManager<ApplicationIdentityUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _unitOfWork.User.GetByIdAsync(Guid.Parse(userId));

            ViewBag.Email = user.Email;
            ViewBag.Name = user.Name;
            return View();
        }

       
        public async Task<IActionResult> Update()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _unitOfWork.User.GetByIdAsync(Guid.Parse(userId));
            var model = new ProfileUpdateViewModel();
            model.Email = user.Email;
            model.Name = user.Name;
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(ProfileUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    var user = await _unitOfWork.User.GetByIdAsync(Guid.Parse(userId));
                    if (user == null)
                    {
                        TempData["Error"] = "Internal server error";
                        return View(model);
                    }

                    

                    if (user.Email == model.Email && user.Name == model.Name)
                    {
                        TempData["Error"] = "Update at least one field..!";
                        return View(model);

                    }

                    user.Email = model.Email;
                    user.Name = model.Name;

                    _unitOfWork.User.Update(user);
                    await _unitOfWork.SaveAsync();
                    TempData["Success"] = "Profile update successful.";

                    return RedirectToAction("Index");


                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            TempData["Error"] = "Sever error";
            return View();
        }
    }
}

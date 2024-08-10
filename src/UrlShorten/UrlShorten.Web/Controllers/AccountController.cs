using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShorten.ApplicationIdentity.Context;
using UrlShorten.ApplicationIdentity.Manager;
using UrlShorten.DataAccess.Context;
using UrlShorten.Models;
using UrlShorten.Web.Models;

namespace UrlShorten.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationIdentityDbContext _identityDbContext;
        private readonly UrlShortenDbContext _dbContext;
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly SignInManager<ApplicationIdentityUser> _signInManager;
        public AccountController(ApplicationIdentityDbContext identityDbContext, UrlShortenDbContext dbContext,UserManager<ApplicationIdentityUser>userManager, SignInManager<ApplicationIdentityUser> signInManager)
        {
            _dbContext = dbContext;
            _identityDbContext = identityDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            var registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				using (var transaction = await _identityDbContext.Database.BeginTransactionAsync())
				{
					try
					{
						var identityAppUser = new ApplicationIdentityUser
						{
							Email = model.Email,
							UserName = model.Email,
							NormalizedUserName = model.Email.ToUpper(),
							NormalizedEmail = model.Email.ToUpper(),
							EmailConfirmed = false,
						};

						var result = await _userManager.CreateAsync(identityAppUser, model.Password);
						if (result.Succeeded)
						{
							// Assign the same ID to the User entity
							var applicationUser = new User
							{
								Id = identityAppUser.Id, // Use the ID from identityAppUser
								Name = model.Name,
								Email = model.Email,
							};

							_dbContext.Users.Add(applicationUser);
							await _dbContext.SaveChangesAsync();

							await transaction.CommitAsync();

                            //await _signInManager.SignInAsync(identityAppUser, isPersistent: false);
                            TempData["Success"] = "Registration successful. Please log in.";
                            TempData["Email"] = model.Email; // Pass the email to TempData
							return RedirectToAction("Login", "Account");
						}

						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
							if (error.Code == "DuplicateUserName")
							{
								TempData["Error"] = "Email already in use.";
							}
						}
					}
					catch (Exception)
					{
						await transaction.RollbackAsync();
						ModelState.AddModelError(string.Empty, "An error occurred while creating the user. Please try again.");
						TempData["Error"] = "An error occurred while creating the user. Please try again.";
					}
				}
			}
			return View(model);
		}



		public IActionResult Login()
        {
            var loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					TempData["Success"] = "Login successful.";
					return RedirectToAction("Index", "Home");
				}
				if (result.IsLockedOut)
				{
					TempData["Error"] = "Account locked out.";
					return View("Lockout");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					TempData["Error"] = "Invalid login attempt.";
				}
			}

			return View(model);
		}

		[Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("Login");
        }
    }


}

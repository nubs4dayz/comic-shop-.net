using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain;
using Shop.Domain.DomainModels;
using Shop.Domain.DTO;
using Shop.Domain.Identity;
using System.Security.Claims;

namespace Shop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ShopApplicationUser> userManager;
        private readonly SignInManager<ShopApplicationUser> signInManager;
        public AccountController(UserManager<ShopApplicationUser> userManager, SignInManager<ShopApplicationUser> signInManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new ShopApplicationUser
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        PhoneNumber = request.PhoneNumber,
                        UserCart = new ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        //await userManager.AddToRoleAsync(user, "Standard");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult AddUserToRole()
        {
            AddToRoleModel model = new AddToRoleModel();
            model.Roles = new List<string>() { "Administrator", "Standard" };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddUserToRole(AddToRoleModel model)
        {
            var email = model.Email;
            var userTask = userManager.FindByEmailAsync(email);
            if (userTask == null)
            {
                throw new NullReferenceException();
            }

            ShopApplicationUser user = await userTask;

            await userManager.AddToRoleAsync(user, model.SelectedRole);

            return RedirectToAction("Index", "Comics");
        }
    }
}

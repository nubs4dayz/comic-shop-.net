using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain;
using Shop.Domain.DomainModels;
using Shop.Domain.DTO;
using Shop.Domain.Identity;
using Shop.Service.Interface;

namespace Shop.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IComicService _comicService;
        private readonly UserManager<ShopApplicationUser> _userManager;

        public AdminController(IOrderService orderService, IComicService comicService, UserManager<ShopApplicationUser> userManager)
        {
            _orderService = orderService;
            _comicService = comicService;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        public List<Order> GetOrders()
        {
            var result = this._orderService.getAllOrders();
            return result;
        }

        [HttpGet("[action]")]
        public List<Comic> GetComics()
        {
            var result = this._comicService.GetAllComics();
            return result;
        }

        [HttpPost("[action]")]
        public Order GetDetailsForOrder(BaseEntity model)
        {
            var result = this._orderService.getOrderDetails(model);
            return result;
        }

        [HttpPost("[action]")]
        public async Task<bool> ImportAllUsers(List<UserRegistrationDto> model)
        {
            bool status = true;
            foreach (var item in model)
            {
                var userCheck = await _userManager.FindByEmailAsync(item.Email);
                if (userCheck == null)
                {
                    var user = new ShopApplicationUser
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        PhoneNumber = item.PhoneNumber,
                        UserCart = new ShoppingCart()
                    };

                    var result = await _userManager.CreateAsync(user, item.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Standard");
                    }

                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }

            return status;
        }
    }
}

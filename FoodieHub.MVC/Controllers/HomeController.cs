using FoodieHub.MVC.Attributes;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService authService;
        private readonly IRecipeService recipeService;
        private readonly IUserService userService;

        public HomeController(IAuthService authService, IRecipeService recipeService, IUserService userService)
        {
            this.authService = authService;
            this.recipeService = recipeService;
            this.userService = userService;
        }
        
        public async Task<IActionResult> Index()
        {
            var token = Request.GetCookie("TokenUser");

            if (!string.IsNullOrEmpty(token))
            {
                var user = await authService.GetProfile();
                if (user != null)
                {
                    Response.SetCookie("UserID", user.Id);
                    Response.SetCookie("FullName", user.Fullname);
                    if (!string.IsNullOrEmpty(user?.Avatar))
                    {
                        Response.SetCookie("Avatar", user.Avatar);
                    }
                }
            }
            var query = new QueryRecipeModel
            {
                IsActive = true,
            };
            var result = await recipeService.GetAll(query);

            return View(result?.Items);
        }
     
    }
}

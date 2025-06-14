using FoodieHub.MVC.Models.Breadcrumb;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Helpers
{
    public static class BreadcrumbHelper
    {
        public static void SetBreadcrumb(Controller controller, string path, string title)
        {
            controller.ViewData["Breadcrumb"] = new BreadcrumbModel
            {
                Path = path,
                Title = title
            };
        }
    }
}

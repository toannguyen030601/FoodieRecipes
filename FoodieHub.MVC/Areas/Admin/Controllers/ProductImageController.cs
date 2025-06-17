using AspNetCoreHero.ToastNotification.Abstractions;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductImageController : Controller
    {
        private readonly IProductImageService _productImageService;
        private readonly INotyfService _notyf;

        public ProductImageController(IProductImageService productImageService, INotyfService notyf)
        {
            _productImageService = productImageService;
            _notyf = notyf;
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteImageInImg(int ImageID)
        //{
        //    var obj = await _productImageService.DeleteImage(ImageID);
        //    return Ok(obj);
        //}

        [HttpPost]
        public async Task<IActionResult> AddImg(ProductImageDTO productImage)
        {
            var result = await _productImageService.AddImageProduct(productImage);
            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        [ActionName("AddMultipleImages")]
        public async Task<IActionResult> AddMultipleImages(int ProductID, List<IFormFile> Images)
        {
            var result = await _productImageService.AddMultipleImages(ProductID, Images);
            if (result != null && result.Success)
            {
                _notyf.Success("Thêm ảnh chi tiết thành công!");
            }
            else
            {
                _notyf.Error("Thêm ảnh thất bại. Vui lòng thử lại.");
            }
            return RedirectToAction("Index", "Products");
        }
    }
}

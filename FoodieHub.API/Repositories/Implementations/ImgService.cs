using AutoMapper;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Data;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Product;
using FoodieHub.API.Repositories.Interfaces;
using FoodieHub.API.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class ImgService:IImgService
    {
         private readonly AppDbContext _appDbContext;
        IMapper _mapper;
        ImageExtentions _uploadImageHelper;
        public ImgService(AppDbContext appDbContext, IMapper mapper, ImageExtentions uploadImageHelper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _uploadImageHelper = uploadImageHelper;
        }


        public async Task<ServiceResponse> AddImage(ProductImageDTO img)
        {
            var uploadImageResult = await _uploadImageHelper.UploadImage(img.ImageURL, "ProductDetail");

            if (!uploadImageResult.Success || string.IsNullOrEmpty(uploadImageResult.FilePath))
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to upload image to Cloudinary.",
                    StatusCode = 400
                };
            }

            var productImage = _mapper.Map<ProductImage>(img);
            productImage.ProductID = img.ProductID;
            productImage.ImageURL = uploadImageResult.FilePath;

            await _appDbContext.ProductImages.AddAsync(productImage);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Image added successfully.",
                    StatusCode = 201
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to save image to database.",
                StatusCode = 500
            };
        }

        public async Task<ServiceResponse> AddMultipleImages(List<ProductImageDTO> imgs)
        {
            if (imgs == null || !imgs.Any())
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "No images provided.",
                    StatusCode = 400
                };
            }

            int successCount = 0;

            foreach (var img in imgs)
            {
                var uploadImageResult = await _uploadImageHelper.UploadImage(img.ImageURL, "ProductDetail");

                if (!uploadImageResult.Success || string.IsNullOrEmpty(uploadImageResult.FilePath))
                    continue;

                var productImage = _mapper.Map<ProductImage>(img);
                productImage.ImageURL = uploadImageResult.FilePath;

                await _appDbContext.ProductImages.AddAsync(productImage);
                successCount++;
            }

            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = successCount > 0,
                Message = successCount > 0 ? $"Added {successCount} image(s)." : "Failed to upload images.",
                StatusCode = successCount > 0 ? 201 : 500
            };
        }

        public async Task<ServiceResponse> DeleteImgByProductID(int id)
        {
            if (_appDbContext == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Database context is not available.",
                    StatusCode = 500
                };
            }

            // Lấy danh sách các ảnh có ProductID tương ứng
            var images = await _appDbContext.ProductImages
                                            .Where(img => img.ProductID == id)
                                            .ToListAsync();

            if (images == null || images.Count == 0)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"No images found for ProductID: {id}",
                    StatusCode = 404
                };
            }

            // Xóa các ảnh trong danh sách
            _appDbContext.ProductImages.RemoveRange(images);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Images deleted successfully.",
                StatusCode = 200
            };
        }


        public async Task<ServiceResponse> DeleteImage(int id)
        {
            var obj = await _appDbContext.ProductImages.FindAsync(id);
            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find image with ID: {id}",
                    StatusCode = 404
                };
            }

             _uploadImageHelper.DeleteImage(obj.ImageURL);
          

            _appDbContext.ProductImages.Remove(obj);

            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Image deleted successfully.",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to delete image record from database.",
                    StatusCode = 500
                };
            }
        }

        public async Task<List<ProductImage>> GetAllImg()
        {
            if (_appDbContext == null)
            {
                return null;
            }

            var imgs = await _appDbContext.ProductImages.ToListAsync();
            return imgs;
        }


    }
}

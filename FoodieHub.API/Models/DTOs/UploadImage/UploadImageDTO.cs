namespace FoodieHub.API.Models.DTOs.UploadImage
{
    public class UploadImageDTO
    {
        public IFormFile File { get; set; } = default!;
        public string Folder { get; set; } = default!;
    }
}

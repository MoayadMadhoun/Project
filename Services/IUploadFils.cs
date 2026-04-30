
namespace MyShope.Services
{
    public interface IUploadFils
    {
        string? UploadFile(IFormFile upload, string subFolder = "ProuctsImg");
    }
}
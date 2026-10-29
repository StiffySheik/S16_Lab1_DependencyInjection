using ZombieParty.Models.Data;

namespace ZombieParty.Models
{
    public interface IImageFileManager
    {
        string? UploadImage(IFormCollection form, bool edit, string? oldImagePath = null);
    }
}
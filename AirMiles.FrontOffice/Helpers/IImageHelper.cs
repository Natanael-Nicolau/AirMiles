using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AirMiles.FrontOffice.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder, int clientId);
    }
}

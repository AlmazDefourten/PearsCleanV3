using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PearsCleanV3.Domain.Common;

public interface IFileStorage
{
    Task<bool> UploadPicture(string url, IFormFile file);

    Task<byte[]> GetPicture(string url);
}

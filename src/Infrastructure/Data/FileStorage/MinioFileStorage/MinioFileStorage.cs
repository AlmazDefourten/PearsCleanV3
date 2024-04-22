using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using PearsCleanV3.Domain.Common;

namespace PearsCleanV3.Infrastructure.Data.FileStorage.MinioFileStorage;

public class MinioFileStorage(IMinioClient minioClient) : IFileStorage
{
    const string ContentType = "image/jpeg";

    public async Task<bool> UploadPicture(string url, IFormFile file)
    {
        const string bucketName = "profilepics";

        try
        {
            await using (var stream = file.OpenReadStream())
            {
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithStreamData(stream)
                    .WithObject(url)
                    .WithObjectSize(stream.Length)
                    .WithContentType(ContentType);
                
                minioClient.PutObjectAsync(putObjectArgs).Wait();
            }
            //TODO: logging
            Console.WriteLine("Successfully uploaded " + url);
            return true;
        }
        catch (MinioException e)
        {
            Console.WriteLine("File Upload Error: {0}", e.Message);
            return false;
        }
    }
    
    public async Task<byte[]> GetPicture(string url)
    {
        const string bucketName = "profilepics";

        try
        {
            var stream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithHeaders(new Dictionary<string, string>())
                .WithCallbackStream((s, t) => s.CopyToAsync(stream, t))
                .WithObject(url);
            await minioClient.GetObjectAsync(args).ConfigureAwait(false);
            Console.WriteLine($"Downloaded the file {url} in bucket {bucketName}");
            return stream.ToArray();
            //TODO: logging
        }
        catch (Exception e)
        {
            Console.WriteLine("File get Error: {0}", e.Message);
            return Array.Empty<byte>();
        }
    }
}

using Minio;

namespace PearsCleanV3.Infrastructure.Data.FileStorage.MinioFileStorage;

public static class MinioFactory
{
    public static IMinioClient CreateMinioClientWithDefaults()
    {
        // TODO: refactor secrets
        var endpoint  = "127.0.0.1:9000";
        var accessKey = "2DbTm7Irg6knrafN9dDk";
        var secretKey = "INV2fq3gLQi62Qx5xBsyJT9pm6T9mcOIgWJFzyYJ";
        try
        {
            return new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build();
        }
        catch (Exception ex)
        {
            // TODO: logging
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}

using Microsoft.AspNetCore.Http;

namespace PearsCleanV3.Application.FunctionalTests.Mocks;

public static class FileMocksFactory
{
    public static IFormFile CreateFormFileMock()
    {
        const string content = "Hello World from a Fake File";
        const string fileName = "test.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        return file;
    }
}

using System.IO;
using System.Net.Http;

namespace Documentor.Core.Models;

public sealed class DownloadFileResult : IAsyncDisposable
{
    private readonly HttpResponseMessage _response;

    public string FileName { get; }
    public Stream ContentStream { get; }

    public DownloadFileResult(HttpResponseMessage response, string fileName, Stream contentStream)
    {
        _response = response;
        FileName = fileName;
        ContentStream = contentStream;
    }

    public async ValueTask DisposeAsync()
    {
        await ContentStream.DisposeAsync();
        _response.Dispose();
    }
}
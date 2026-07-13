using System.Text;
using CharzPiexApi.Domain;

namespace CharzPiexApi;

public class FileService(IConfiguration config, ILogger<FileService> logger)
{
    public async Task<string> Write(string filename, string content)
    {
        var oneCConfig = config.GetSection(Constants.OneCPrefix);
        var path = Path.Join(oneCConfig[Constants.ExchangeDir], filename).Replace(" ", "-");
        logger.LogInformation("Writing content to {path}", path);
        var targetEncodingName = config.GetSection(Constants.File)[Constants.DefectEncoding];
        var encoding = Encoding.GetEncoding(targetEncodingName ?? Encoding.UTF8.EncodingName);
        await File.WriteAllTextAsync(path, content, encoding);
        return path;
    }
}
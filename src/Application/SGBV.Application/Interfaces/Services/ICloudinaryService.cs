namespace SGBV.Application.Interfaces.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageCloudinaryAsync(
        Stream fileStream,
        string imageName,
        CancellationToken cancellationToken);
}
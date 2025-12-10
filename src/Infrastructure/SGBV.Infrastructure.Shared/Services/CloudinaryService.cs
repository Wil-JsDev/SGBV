using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using SGBV.Application.Interfaces.Services;
using SGBV.Domain.Settings;

namespace SGBV.Infrastructure.Shared.Services;

public class CloudinaryService(IOptions<CloudinarySettings> cloudinaryOptions) : ICloudinaryService
{
    private CloudinarySettings Cloudinary { get; } = cloudinaryOptions.Value;

    public async Task<string> UploadImageCloudinaryAsync(
        Stream fileStream,
        string imageName,
        CancellationToken cancellationToken)
    {
        Cloudinary cloudinary = new(Cloudinary.CloudinaryUrl);
        ImageUploadParams image = new()
        {
            File = new FileDescription(imageName, fileStream),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true
        };

        ImageUploadResult uploadResult = await cloudinary.UploadAsync(image, cancellationToken);
        return uploadResult.SecureUrl.ToString();
    }
}
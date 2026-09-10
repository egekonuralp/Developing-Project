namespace TechStore.Helpers
{
    public static class ImageValidationHelper
    {
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        public const long MaxImageSizeBytes = 5 * 1024 * 1024;

        public static bool IsValidImage(IFormFile file, out string errorMessage)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(extension))
            {
                errorMessage = "Sadece .jpg, .jpeg, .png veya .webp uzantılı dosyalar yüklenebilir.";
                return false;
            }

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (!allowedContentTypes.Contains(file.ContentType))
            {
                errorMessage = "Geçersiz dosya türü.";
                return false;
            }

            if (file.Length > MaxImageSizeBytes)
            {
                errorMessage = "Dosya boyutu 5 MB'ı geçemez.";
                return false;
            }

            if (file.Length == 0)
            {
                errorMessage = "Boş dosya yüklenemez.";
                return false;
            }

            if (!HasValidImageSignature(file, extension))
            {
                errorMessage = "Dosya içeriği seçilen görsel türüyle eşleşmiyor.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static bool HasValidImageSignature(IFormFile file, string extension)
        {
            Span<byte> header = stackalloc byte[12];

            using var stream = file.OpenReadStream();
            var bytesRead = stream.Read(header);

            return extension switch
            {
                ".jpg" or ".jpeg" => bytesRead >= 3 &&
                    header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                ".png" => bytesRead >= 8 &&
                    header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                ".webp" => bytesRead >= 12 &&
                    header[..4].SequenceEqual("RIFF"u8) &&
                    header.Slice(8, 4).SequenceEqual("WEBP"u8),
                _ => false
            };
        }
    }
}
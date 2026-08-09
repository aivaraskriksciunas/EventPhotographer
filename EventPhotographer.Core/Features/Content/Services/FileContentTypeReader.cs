namespace EventPhotographer.Core.Features.Content.Services;

public record FileContentTypeInfo(
    string MimeType, 
    string Extension,
    bool IsAllowed = true
) {}

public class FileContentTypeReader
{
    private static readonly List<FileContentTypeInfo> _fileTypes = new List<FileContentTypeInfo>
    {
        new ("image/jpeg", ".jpeg"),
        new ("image/png", ".png"),
        new ("image/gif", ".gif"),
        new ("image/x-canon-crw", ".crw"),
        new ("video/mp4", ".mp4"),
        new ("audio/m4a", ".m4a"),
        new ("video/x-m4v", ".m4v"),
        new ("video/x-ms-wmv", ".wmv"),
        new ("video/x-msvideo", ".avi"),
        new ("audio/wav", ".wav"),
        new ("image/webp", ".webp"),
        new ("video/quicktime", ".mov"),
        new ("application/zip", ".zip", false),
    };

    private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
    {
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8 },
            }
        },
        { ".png", new List<byte[]>
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
            }
        },
        { ".gif", new List<byte[]>
            {
                new byte[] { 0x47, 0x49, 0x46, 0x38 },
            }
        },
        { ".crw", new List<byte[]>
            {
                new byte[] { 0x00, 0x00, 0x00, 0x14, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D },
                new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 },
                new byte[] { 0x00, 0x00, 0x00, 0x1C, 0x66, 0x74, 0x79, 0x70 },
                new byte[] { 0x66, 0x74, 0x79, 0x70, 0x33, 0x67, 0x70, 0x35 },
                new byte[] { 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56 },
                new byte[] { 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D },
            }
        },
        { ".mp4", new List<byte[]>
            {
                new byte[] { 0x49, 0x49, 0x1A, 0x00, 0x00, 0x00, 0x48, 0x45 },
            }
        },
        { ".m4a", new List<byte[]>
            {
                new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41 },
            }
        },
        { ".m4v", new List<byte[]>
            {
                new byte[] { 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32 },
            }
        },
        { ".wmv", new List<byte[]>
            {
                new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 },
            }
        },
        { ".avi", new List<byte[]>
            {
                new byte[] { 0x41, 0x56, 0x49, 0x20, 0x4C, 0x49, 0x53, 0x54 },
            }
        },
        { ".wav", new List<byte[]>
            {
                new byte[] { 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20 },
            }
        },
        { ".webp", new List<byte[]>
            {
                new byte[] { 0x57, 0x45, 0x42, 0x50 },
            }
        },
        { ".mov", new List<byte[]>
            {
                new byte[] { 0x66, 0x74, 0x79, 0x70, 0x71, 0x74, 0x20, 0x20 },
                new byte[] { 0x6D, 0x6F, 0x6F, 0x76 },
                new byte[] { 0x66, 0x72, 0x65, 0x65 },
                new byte[] { 0x6D, 0x64, 0x61, 0x74 },
                new byte[] { 0x77, 0x69, 0x64, 0x65 },
                new byte[] { 0x70, 0x6E, 0x6F, 0x74 },
                new byte[] { 0x73, 0x6B, 0x69, 0x70 },
            }
        },
    };

    private static readonly int _maxSignatureLength = _fileSignature.Values.SelectMany(v => v).Max(m => m.Length);

    public FileContentTypeInfo? DetermineFileExtension(Stream stream)
    {
        using (var reader = new BinaryReader(stream, System.Text.Encoding.Default, true))
        {
            var headerBytes = reader.ReadBytes(_maxSignatureLength);

            foreach (var ext in _fileSignature.Keys)
            {
                var signatures = _fileSignature[ext];

                if (signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature)))
                {
                    return _fileTypes.FirstOrDefault(ft => ft.Extension == ext);
                }
            }
        }

        stream.Position = 0;
        return null;
    }

    public static FileContentTypeInfo? GetFileTypeFromExtension(string extension)
    {
        extension = extension.ToLower();
        return _fileTypes.FirstOrDefault(ft => ft.Extension == extension);
    }

    public static FileContentTypeInfo? GetFileTypeFromMimeType(string mimeType)
    {
        mimeType = mimeType.ToLower();
        return _fileTypes.FirstOrDefault(ft => ft.MimeType == mimeType);
    }

    public static bool IsAllowedMimeType(string mimeType)
    {
        mimeType = mimeType.ToLower();
        return _fileTypes.Any(ft => ft.MimeType == mimeType && ft.IsAllowed);
    }
}

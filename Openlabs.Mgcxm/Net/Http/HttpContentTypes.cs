// Copr. (c) Nexus 2023. All rights reserved.

using System.Reflection;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Net;

public enum HttpContentTypes
{
    [CommonMimeType("unknown", "unknown")]
    Invalid,
    
    [CommonMimeType("audio", "aac", "aac")]
    AacAudio,
    [CommonMimeType("audio", "x-abiword", "abw")]
    AbiWord,
    [CommonMimeType("audio", "x-freearc", "arc")]
    ArchiveDocument,
    [CommonMimeType("image", "avif", "avif")]
    AvifImage,
    [CommonMimeType("video", "x-msvideo", "avi")]
    Avi,
    [CommonMimeType("application", "octet-stream", "bin")]
    BinaryData,
    [CommonMimeType("image", "bmp", "bmp")]
    WinOs2Bitmap,
    [CommonMimeType("application", "x-bzip", "bz")]
    BzipArchive,
    [CommonMimeType("application", "x-bzip2", "bz2")]
    Bzip2Archive,
    [CommonMimeType("application", "x-cdf", "cda")]
    CdAudio,
    [CommonMimeType("application", "x-csh", "csh")]
    CshellScript,
    [CommonMimeType("text", "x-css", "css")]
    CssSheet,
    [CommonMimeType("application", "msword", "doc")]
    MsWordDocument,
    [CommonMimeType("application", "vnd.openxmlformats-officedocument.wordprocessingml.document", "docx")]
    MsWordDocumentXml,
    [CommonMimeType("application", "vnd.ms-fontobject", "eot")]
    MsEmbeddedOpenTypeFont,
    [CommonMimeType("application", "epub+zip", "epub")]
    ElectronicPublication,
    [CommonMimeType("application", "gzip", "gz")]
    GzipCompressArchive,
    [CommonMimeType("image", "gif", "gif")]
    GraphicsInterchange,
    [CommonMimeType("text", "html", "html")]
    HtmlFile,
    [CommonMimeType("image", "vnd.microsoft.icon", "ico")]
    IconFile,
    [CommonMimeType("text", "calendar", "ics")]
    Calender,
    [CommonMimeType("application", "java-archive", "jar")]
    JavaArchive,
    [CommonMimeType("image", "image/jpeg", "jpeg")]
    JpegImage,
    [CommonMimeType("text", "javascript", "js")]
    Javascript,
    [CommonMimeType("application", "json", "json")]
    Json,
    [CommonMimeType("application", "ld+json", "jsonld")]
    JsonLd,
    [CommonMimeType("audio", "mpeg", "mp3")]
    Mp3Audio,
    [CommonMimeType("video", "mp4", "mp4")]
    Mp4Video,
    [CommonMimeType("video", "mpeg", "mpeg")]
    MpegVideo,
    [CommonMimeType("audio", "ogg", "ogg")]
    OggAudio,
    [CommonMimeType("video", "ogg", "ogv")]
    OgvAudio,
    [CommonMimeType("audio", "opus", "opus")]
    OpusAudio,
    [CommonMimeType("font", "otf", "ttf")]
    OpenTypeFont,
    [CommonMimeType("image", "png", "png")]
    PngImage,
    [CommonMimeType("text", "plain", "txt")]
    PlainText,
    [CommonMimeType("audio", "wav", "wav")]
    WavAudio,
    [CommonMimeType("audio", "webm", "weba")]
    WebmAudio,
    [CommonMimeType("video", "webm", "webm")]
    WebmVideo,
    [CommonMimeType("image", "webp", "webp")]
    WebpImage,
    [CommonMimeType("application", "xml", "xml")]
    Xml,
    [CommonMimeType("application", "zip", "zip")]
    ZipArchive,
    [CommonMimeType("application", "x-www-form-urlencoded")]
    UrlencodedForm,
    [CommonMimeType("multipart", "form-data")]
    MultipartForm
}

/// <summary>
/// A helper class to resolve HTTP content type values.
/// </summary>
public static class HttpContentTypeHelper
{
    /// <summary>
    /// Resolves the MIME type associated with the specified <see cref="HttpContentTypes"/>.
    /// </summary>
    /// <param name="contentType">The <see cref="HttpContentTypes"/> value.</param>
    /// <returns>The resolved MIME type.</returns>
    public static string ResolveValue(HttpContentTypes contentType)
    {
        try
        {
            var type = typeof(HttpContentTypes);
            var values = type.GetMember(contentType.ToString());
            var enumValueMemberInfo = values.FirstOrDefault(m => m.DeclaringType == type);

            CommonMimeTypeAttribute mimeTypeAttribute;
            if ((mimeTypeAttribute = enumValueMemberInfo.GetCustomAttribute<CommonMimeTypeAttribute>()) == null)
                throw new InvalidDataException("A common mime type is not attached to this enum.");

            return mimeTypeAttribute.MimeType;
        }
        catch
        {
            return ResolveValue(HttpContentTypes.Invalid);
        }
    }

    /// <summary>
    /// Resolves the <see cref="HttpContentTypes"/> value associated with the given MIME type.
    /// </summary>
    /// <param name="contentType">The MIME type to resolve.</param>
    /// <returns>The <see cref="HttpContentTypes"/> value.</returns>
    public static HttpContentTypes ResolveValue(string contentType)
    {
        var type = typeof(HttpContentTypes);
        var values = type.GetMembers();

        foreach (var value in values)
        {
            CommonMimeTypeAttribute mimeTypeAttribute = value.GetCustomAttribute<CommonMimeTypeAttribute>();
            if (mimeTypeAttribute != null && mimeTypeAttribute.MimeType == contentType)
                return Enum.Parse<HttpContentTypes>(value.Name);
        }

        return HttpContentTypes.Invalid;
    }
}
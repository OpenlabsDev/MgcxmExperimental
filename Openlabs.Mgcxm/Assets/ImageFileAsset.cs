using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// Represents an asset stored in a file, as an image. (.png, jpg)
/// </summary>
public sealed class ImageFileAsset : FileAsset
{
    /// <inheritdoc/>
    public override async Task Load(object[] arguments)
    {
        if (arguments.Length == 0)
            throw new ArgumentException("Cannot accept 0 arguments");
        var path = (string)arguments[0];
        var extension = System.IO.Path.GetExtension(path).TrimStart('.');

        bool validExtension = false;
        foreach (var ext in _allAcceptableImageExtensions)
        {
            if (extension.Equals(ext))
            {
                validExtension = true;
            }
        }

        if (!validExtension)
            throw new ArgumentException(string.Format("Cannot accept a non-acceptable image format. Given = '{0}'", extension));

        await base.Load(arguments);
        _image = Image.Load(ReadImage());

        return;
    }
    
    /// <summary>
    /// Reads the image data as a byte array.
    /// </summary>
    /// <returns>An array of bytes, the image data.</returns>
    /// <exception cref="Exception">No data was found in the image file.</exception>
    public byte[] ReadImage()
    {
        byte[] data = ReadBytes();
        if (data == null)
            throw new Exception("No data found!");

        return data;
    }

    public int Width => _image.Width;
    public int Height => _image.Height;
    public Image Image => _image;

    private Image _image;

    static string[] _allAcceptableImageExtensions = new string[]
    {
        "png",
        "jpg",
        "bmp"
    };
}

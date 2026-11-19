using System;
using System.IO;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Avalonia.Browser
{
    internal class CssCursor : ICursorImpl
    {
        public const string Default = StandardCursorTypeCssKeywords.Default;
        public string? Value { get; set; }
        
        public CssCursor(StandardCursorType type)
        {
            Value = ToKeyword(type);
        }

        /// <summary>
        /// Create a cursor from base64 image
        /// </summary>
        public CssCursor(string base64, string format, PixelPoint hotspot, StandardCursorType fallback)
        {
            Value = FormattableString.Invariant($"url(\"data:image/{format};base64,{base64}\") {hotspot.X} {hotspot.Y}, {ToKeyword(fallback)}");
        }

        /// <summary>
        /// Create a cursor from url to *.cur file.
        /// </summary>
        public CssCursor(string url, StandardCursorType fallback)
        {
            Value = $"url('{url}'), {ToKeyword(fallback)}";
        }

        /// <summary>
        /// Create a cursor from png/svg and hotspot position
        /// </summary>
        public CssCursor(string url, PixelPoint hotSpot, StandardCursorType fallback)
        {
            Value = FormattableString.Invariant($"url('{url}') {hotSpot.X} {hotSpot.Y}, {ToKeyword(fallback)}");
        }

        private static string ToKeyword(StandardCursorType type) =>
            StandardCursorTypeCssKeywords.GetKeyword(type);

        public void Dispose() {}
    }

    internal class CssCursorFactory : ICursorFactory
    {
        public ICursorImpl CreateCursor(Avalonia.Media.Imaging.Bitmap cursor, PixelPoint hotSpot)
        {
            using var imageStream = new MemoryStream();
            cursor.Save(imageStream, PngBitmapEncoderOptions.Default);

            //not memory optimized because CryptoStream with ToBase64Transform is not supported in the browser.
            var base64String = Convert.ToBase64String(imageStream.ToArray());
            return new CssCursor(base64String, "png", hotSpot, StandardCursorType.Arrow);
        }

        ICursorImpl ICursorFactory.GetCursor(StandardCursorType cursorType)
        {
            return new CssCursor(cursorType);
        }
    }
}


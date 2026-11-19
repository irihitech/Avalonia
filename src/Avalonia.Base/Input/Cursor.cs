using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

#nullable enable

namespace Avalonia.Input
{
    public enum StandardCursorType
    {
        Arrow,
        Ibeam,
        Wait,
        Cross,
        UpArrow,
        SizeWestEast,
        SizeNorthSouth,
        SizeAll,
        No,
        Hand,
        AppStarting,
        Help,
        TopSide,
        BottomSide,
        LeftSide,
        RightSide,
        TopLeftCorner,
        TopRightCorner,
        BottomLeftCorner,
        BottomRightCorner,
        DragMove,
        DragCopy,
        DragLink,
        None,

        // CSS cursor / XDG cursor-names standard shapes.
        SizeNorthWestSouthEast,
        SizeNorthEastSouthWest,
        Grab,
        Grabbing,
        ZoomIn,
        ZoomOut,
        Cell,
        VerticalText,
        ContextualMenu,
        NoDrop,
        AllScroll,
        ColResize,
        RowResize,
    }

    public class Cursor : IDisposable
    {
        public static readonly Cursor Default = new Cursor(StandardCursorType.Arrow);
        private readonly string _name;

        private Cursor(ICursorImpl platformImpl, string name)
        {
            PlatformImpl = platformImpl;
            _name = name;
        }

        public Cursor(StandardCursorType cursorType)
            : this(GetCursorFactory().GetCursor(cursorType), cursorType.ToString())
        {
        }

        public Cursor(Bitmap cursor, PixelPoint hotSpot)
            : this(GetCursorFactory().CreateCursor(cursor, hotSpot), "BitmapCursor")
        {
        }

        internal ICursorImpl PlatformImpl { get; }

        public void Dispose() => PlatformImpl.Dispose();

        public static Cursor Parse(string s)
        {
            return Enum.TryParse<StandardCursorType>(s, true, out var t) ?
                new Cursor(t) :
                throw new ArgumentException($"Unrecognized cursor type '{s}'.");
        }

        private static ICursorFactory GetCursorFactory()
        {
            return AvaloniaLocator.Current.GetRequiredService<ICursorFactory>();
        }

        public override string ToString()
        {
            return _name;
        }
    }
}

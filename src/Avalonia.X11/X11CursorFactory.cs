using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Internal;

namespace Avalonia.X11
{
    internal partial class X11CursorFactory : ICursorFactory
    {
        private static readonly byte[] NullCursorData = new byte[] { 0 };

        private static IntPtr _nullCursor;

        private readonly IntPtr _display;
        private Dictionary<StandardCursorType, IntPtr> _cursors;

        internal static readonly Dictionary<StandardCursorType, CursorFontShape> s_mapping =
            new Dictionary<StandardCursorType, CursorFontShape>
        {
                {StandardCursorType.Arrow, CursorFontShape.XC_left_ptr},
                {StandardCursorType.Cross, CursorFontShape.XC_cross},
                {StandardCursorType.Hand, CursorFontShape.XC_hand2},
                {StandardCursorType.Help, CursorFontShape.XC_question_arrow},
                {StandardCursorType.Ibeam, CursorFontShape.XC_xterm},
                {StandardCursorType.No, CursorFontShape.XC_X_cursor},
                {StandardCursorType.Wait, CursorFontShape.XC_watch},
                {StandardCursorType.AppStarting, CursorFontShape.XC_watch},
                {StandardCursorType.BottomSide, CursorFontShape.XC_bottom_side},
                {StandardCursorType.DragCopy, CursorFontShape.XC_center_ptr},
                {StandardCursorType.DragLink, CursorFontShape.XC_fleur},
                {StandardCursorType.DragMove, CursorFontShape.XC_diamond_cross},
                {StandardCursorType.LeftSide, CursorFontShape.XC_left_side},
                {StandardCursorType.RightSide, CursorFontShape.XC_right_side},
                {StandardCursorType.SizeAll, CursorFontShape.XC_sizing},
                {StandardCursorType.TopSide, CursorFontShape.XC_top_side},
                {StandardCursorType.UpArrow, CursorFontShape.XC_sb_up_arrow},
                {StandardCursorType.BottomLeftCorner, CursorFontShape.XC_bottom_left_corner},
                {StandardCursorType.BottomRightCorner, CursorFontShape.XC_bottom_right_corner},
                {StandardCursorType.SizeNorthSouth, CursorFontShape.XC_sb_v_double_arrow},
                {StandardCursorType.SizeWestEast, CursorFontShape.XC_sb_h_double_arrow},
                {StandardCursorType.TopLeftCorner, CursorFontShape.XC_top_left_corner},
                {StandardCursorType.TopRightCorner, CursorFontShape.XC_top_right_corner},
                // Font-cursor fallbacks for cursors primarily loaded from the theme
                {StandardCursorType.SizeNorthWestSouthEast, CursorFontShape.XC_top_left_corner},
                {StandardCursorType.SizeNorthEastSouthWest, CursorFontShape.XC_top_right_corner},
                {StandardCursorType.Grab, CursorFontShape.XC_hand1},
                // No closed-hand font glyph, hand2 is the closest approximation
                {StandardCursorType.Grabbing, CursorFontShape.XC_hand2},
                {StandardCursorType.ZoomIn, CursorFontShape.XC_plus},
                {StandardCursorType.Cell, CursorFontShape.XC_cross},
                {StandardCursorType.VerticalText, CursorFontShape.XC_xterm},
                {StandardCursorType.NoDrop, CursorFontShape.XC_X_cursor},
                {StandardCursorType.AllScroll, CursorFontShape.XC_fleur},
                {StandardCursorType.ColResize, CursorFontShape.XC_sb_h_double_arrow},
                {StandardCursorType.RowResize, CursorFontShape.XC_sb_v_double_arrow},
                // ZoomOut and ContextualMenu have no font-glyph equivalent, they fall back to the arrow
            };

        /// <summary>
        /// Theme cursor names tried in order: the CSS/XDG standard name first, then
        /// legacy aliases that predate it in older themes. Font-cursor fallbacks for
        /// these live in <see cref="s_mapping"/>.
        /// </summary>
        internal static readonly Dictionary<StandardCursorType, string[]> s_libraryCursors = new()
        {
            {StandardCursorType.DragCopy, ["dnd-copy"]},
            {StandardCursorType.DragLink, ["dnd-link"]},
            {StandardCursorType.DragMove, ["dnd-move"]},
            {StandardCursorType.AppStarting, ["progress"]},
            {StandardCursorType.SizeNorthWestSouthEast, ["nwse-resize", "size_fdiag"]},
            {StandardCursorType.SizeNorthEastSouthWest, ["nesw-resize", "size_bdiag"]},
            {StandardCursorType.Grab, ["grab", "openhand", "hand1"]},
            {StandardCursorType.Grabbing, ["grabbing", "closedhand", "hand2"]},
            {StandardCursorType.ZoomIn, ["zoom-in"]},
            {StandardCursorType.ZoomOut, ["zoom-out"]},
            {StandardCursorType.Cell, ["cell"]},
            {StandardCursorType.VerticalText, ["vertical-text"]},
            {StandardCursorType.ContextualMenu, ["context-menu"]},
            {StandardCursorType.NoDrop, ["no-drop", "forbidden", "crossed_circle"]},
            {StandardCursorType.AllScroll, ["all-scroll", "fleur"]},
            {StandardCursorType.ColResize, ["col-resize", "sb_h_double_arrow"]},
            {StandardCursorType.RowResize, ["row-resize", "sb_v_double_arrow"]},
        };

        public X11CursorFactory(IntPtr display)
        {
            _display = display;
            _nullCursor = GetNullCursor(display);
            
            _cursors = new Dictionary<StandardCursorType, IntPtr>();
        }

        // We don't have a "DragNo" standard cursor type, but Xcursor provides one
        public IntPtr DragNoDropCursorHandle
        {
            get
            {
                if (field == 0)
                    field = LoadDragNoDropCursor();
                return field;
            }
        }

        public ICursorImpl GetCursor(StandardCursorType cursorType)
        {
            var handle = GetCursorHandle(cursorType);
            return new CursorImpl(handle);
        }

        public IntPtr GetCursorHandle(StandardCursorType cursorType)
        {
            return cursorType == StandardCursorType.None ? _nullCursor : GetCursorHandleCached(cursorType);
        }

        public unsafe ICursorImpl CreateCursor(Bitmap cursor, PixelPoint hotSpot)
        {
            return new XImageCursor(_display, cursor, hotSpot);
        }

        private static IntPtr GetNullCursor(IntPtr display)
        {
            XColor color = new XColor();
            IntPtr window = XLib.XRootWindow(display, 0);
            IntPtr pixmap = XLib.XCreateBitmapFromData(display, window, NullCursorData, 1, 1);
            return XLib.XCreatePixmapCursor(display, pixmap, pixmap, ref color, ref color, 0, 0);
        }

        private IntPtr LoadDragNoDropCursor()
        {
            var handle = XLib.XcursorLibraryLoadCursor(_display, "dnd-no-drop");

            if (handle == 0)
                handle = GetCursorHandleCached(StandardCursorType.No);
            return handle;
        }

        private unsafe class XImageCursor : CursorImpl, IPlatformHandle
        {
            private readonly IntPtr _display;
            private readonly PixelSize _pixelSize;
            private readonly UnmanagedBlob _blob;

            public XImageCursor(IntPtr display, Bitmap bitmap, PixelPoint hotSpot)
            {
                var size = Marshal.SizeOf<XcursorImage>() +
                    (bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4);
                var platformRenderInterface = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();

                _display = display;
                _pixelSize = bitmap.PixelSize;
                _blob = new UnmanagedBlob(size);
                
                var image = (XcursorImage*)_blob.Address;
                image->version = 1;
                image->size = Marshal.SizeOf<XcursorImage>();
                image->width = bitmap.PixelSize.Width;
                image->height = bitmap.PixelSize.Height;
                image->xhot = hotSpot.X;
                image->yhot = hotSpot.Y;
                image->pixels = (IntPtr)(image + 1);

                bitmap.CopyPixels(new LockedFramebuffer(
                    _blob.Address + Marshal.SizeOf<XcursorImage>(),
                    _pixelSize, _pixelSize.Width * 4,
                    new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul, null));
                
                Handle = XLib.XcursorImageLoadCursor(display, _blob.Address);
            }

            public string HandleDescriptor => "XCURSOR";

            public override void Dispose()
            {
                XLib.XFreeCursor(_display, Handle);
                _blob.Dispose();
            }
        }
        
        private nint GetCursorHandleCached(StandardCursorType type) =>
            GetOrLoadCursorHandle(type, _cursors,
                name => XLib.XcursorLibraryLoadCursor(_display, name),
                shape => XLib.XCreateFontCursor(_display, shape),
                _nullCursor);

        /// <summary>
        /// Resolution chain for a standard cursor: theme cursor names in order, then the
        /// font-cursor fallback from <see cref="s_mapping"/>, then the arrow cursor.
        /// Static with injectable loaders so tests can simulate themes missing the
        /// CSS/XDG names and assert the actual fallback result.
        /// </summary>
        internal static nint GetOrLoadCursorHandle(
            StandardCursorType type,
            Dictionary<StandardCursorType, nint> cache,
            Func<string, nint> loadLibraryCursor,
            Func<CursorFontShape, nint> loadFontCursor,
            nint nullCursor)
        {
            if (cache.TryGetValue(type, out var handle))
                return handle;

            if (s_libraryCursors.TryGetValue(type, out var names))
            {
                foreach (var name in names)
                {
                    handle = loadLibraryCursor(name);
                    if (handle != 0)
                        break;
                }
            }

            if (handle == 0 && s_mapping.TryGetValue(type, out var cursorShape))
                handle = loadFontCursor(cursorShape);

            if (handle == IntPtr.Zero)
            {
                if (type != StandardCursorType.Arrow)
                    handle = GetOrLoadCursorHandle(StandardCursorType.Arrow, cache, loadLibraryCursor, loadFontCursor, nullCursor);
                else
                    handle = nullCursor;
            }

            cache[type] = handle;

            return handle;
        }
    }

    internal class CursorImpl : ICursorImpl
    {
        public CursorImpl() { }
        public CursorImpl(IntPtr handle) => Handle = handle;
        public IntPtr Handle { get; protected set; }
        public virtual void Dispose() { }
    }
}

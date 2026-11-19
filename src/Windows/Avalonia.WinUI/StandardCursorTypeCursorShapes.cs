using System.Collections.Generic;
using Avalonia.Input;

namespace Avalonia.WinUI
{
    /// <summary>
    /// Maps <see cref="StandardCursorType"/> to Microsoft.UI.Input.InputSystemCursorShape values.
    /// The values are identical to the WinRT enum (stable public API since Windows App SDK 1.0);
    /// this file must stay free of WinUI dependencies so unit tests can compile the same source
    /// via a Compile Include link and validate the table.
    /// </summary>
    internal static class StandardCursorTypeCursorShapes
    {
        // Microsoft.UI.Input.InputSystemCursorShape values
        public const int Arrow = 0;
        public const int Cross = 1;
        public const int Hand = 3;
        public const int Help = 4;
        public const int IBeam = 5;
        public const int SizeAll = 6;
        public const int SizeNortheastSouthwest = 7;
        public const int SizeNorthSouth = 8;
        public const int SizeNorthwestSoutheast = 9;
        public const int SizeWestEast = 10;
        public const int UniversalNo = 11;
        public const int UpArrow = 12;
        public const int Wait = 13;

        internal static readonly Dictionary<StandardCursorType, int> Mapping = new Dictionary
            <StandardCursorType, int>
        {
            {StandardCursorType.Arrow, Arrow},
            {StandardCursorType.Ibeam, IBeam},
            {StandardCursorType.Wait, Wait},
            {StandardCursorType.AppStarting, Wait},
            {StandardCursorType.Cross, Cross},
            {StandardCursorType.Help, Help},
            {StandardCursorType.Hand, Hand},
            {StandardCursorType.No, UniversalNo},
            {StandardCursorType.SizeAll, SizeAll},
            {StandardCursorType.SizeNorthSouth, SizeNorthSouth},
            {StandardCursorType.SizeWestEast, SizeWestEast},
            {StandardCursorType.UpArrow, UpArrow},
            {StandardCursorType.TopSide, SizeNorthSouth},
            {StandardCursorType.BottomSide, SizeNorthSouth},
            {StandardCursorType.LeftSide, SizeWestEast},
            {StandardCursorType.RightSide, SizeWestEast},
            {StandardCursorType.TopLeftCorner, SizeNorthwestSoutheast},
            {StandardCursorType.BottomRightCorner, SizeNorthwestSoutheast},
            {StandardCursorType.TopRightCorner, SizeNortheastSouthwest},
            {StandardCursorType.BottomLeftCorner, SizeNortheastSouthwest},
            // No built-in drag cursors — fall back to Arrow; OLE drag operations on
            // Win32 traditionally use ole32.dll resources we can't reach from here.
            {StandardCursorType.DragMove, Arrow},
            {StandardCursorType.DragCopy, Arrow},
            {StandardCursorType.DragLink, Arrow},

            // CSS cursor / XDG cursor-names standard shapes
            {StandardCursorType.SizeNorthWestSouthEast, SizeNorthwestSoutheast},
            {StandardCursorType.SizeNorthEastSouthWest, SizeNortheastSouthwest},
            {StandardCursorType.NoDrop, UniversalNo},
            {StandardCursorType.AllScroll, SizeAll},
            {StandardCursorType.ColResize, SizeWestEast},
            {StandardCursorType.RowResize, SizeNorthSouth},
            // No built-in WinUI shape — mapped to the closest system cursor
            {StandardCursorType.Grab, Hand},
            {StandardCursorType.Grabbing, Hand},
            {StandardCursorType.VerticalText, IBeam},
            {StandardCursorType.ZoomIn, Cross},
            {StandardCursorType.ZoomOut, Cross},
            {StandardCursorType.Cell, Cross},
            {StandardCursorType.ContextualMenu, Arrow},
        };

        /// <summary>Returns the cursor shape for the cursor, or the arrow shape for future enum members.</summary>
        public static int GetMapping(StandardCursorType cursorType) =>
            Mapping.TryGetValue(cursorType, out var shape) ? shape : Arrow;
    }
}

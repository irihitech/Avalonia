using System.Collections.Generic;
using Avalonia.Input;

namespace Avalonia.Android
{
    /// <summary>
    /// Maps <see cref="StandardCursorType"/> to android.view.PointerIcon type constants.
    /// The values are identical to AndroidX PointerIconCompat.Type_* (stable public API
    /// since API 24); this file must stay free of Android dependencies so unit tests can
    /// compile the same source via a Compile Include link and validate the table.
    /// On Android versions that don't know a type, PointerIconCompat.GetSystemIcon
    /// falls back to the arrow cursor on its own.
    /// </summary>
    internal static class StandardCursorTypePointerIcons
    {
        // android.view.PointerIcon.TYPE_* constants
        public const int Null = 0;
        public const int Arrow = 1000;
        public const int ContextMenu = 1001;
        public const int Hand = 1002;
        public const int Help = 1003;
        public const int Wait = 1004;
        public const int Cell = 1006;
        public const int Crosshair = 1007;
        public const int Text = 1008;
        public const int VerticalText = 1009;
        public const int Alias = 1010;
        public const int Copy = 1011;
        public const int NoDrop = 1012;
        public const int AllScroll = 1013;
        public const int HorizontalDoubleArrow = 1014;
        public const int VerticalDoubleArrow = 1015;
        public const int TopRightDiagonalDoubleArrow = 1016;
        public const int TopLeftDiagonalDoubleArrow = 1017;
        public const int ZoomIn = 1018;
        public const int ZoomOut = 1019;
        public const int Grab = 1020;
        public const int Grabbing = 1021;

        internal static readonly Dictionary<StandardCursorType, int> Mapping = new Dictionary
            <StandardCursorType, int>
        {
            {StandardCursorType.Arrow, Arrow},
            {StandardCursorType.Ibeam, Text},
            {StandardCursorType.Wait, Wait},
            {StandardCursorType.Cross, Crosshair},
            {StandardCursorType.UpArrow, Arrow},
            {StandardCursorType.SizeWestEast, HorizontalDoubleArrow},
            {StandardCursorType.SizeNorthSouth, VerticalDoubleArrow},
            {StandardCursorType.SizeAll, AllScroll},
            {StandardCursorType.No, NoDrop},
            {StandardCursorType.Hand, Hand},
            {StandardCursorType.AppStarting, Wait},
            {StandardCursorType.Help, Help},
            {StandardCursorType.TopSide, VerticalDoubleArrow},
            {StandardCursorType.BottomSide, VerticalDoubleArrow},
            {StandardCursorType.LeftSide, HorizontalDoubleArrow},
            {StandardCursorType.RightSide, HorizontalDoubleArrow},
            {StandardCursorType.TopLeftCorner, TopLeftDiagonalDoubleArrow},
            {StandardCursorType.TopRightCorner, TopRightDiagonalDoubleArrow},
            {StandardCursorType.BottomLeftCorner, TopRightDiagonalDoubleArrow},
            {StandardCursorType.BottomRightCorner, TopLeftDiagonalDoubleArrow},
            // Android has no dnd-move icon; all-scroll is the move shape (same as SizeAll)
            {StandardCursorType.DragMove, AllScroll},
            {StandardCursorType.DragCopy, Copy},
            {StandardCursorType.DragLink, Alias},
            {StandardCursorType.None, Null},

            // CSS cursor / XDG cursor-names standard shapes
            {StandardCursorType.SizeNorthWestSouthEast, TopLeftDiagonalDoubleArrow},
            {StandardCursorType.SizeNorthEastSouthWest, TopRightDiagonalDoubleArrow},
            {StandardCursorType.Grab, Grab},
            {StandardCursorType.Grabbing, Grabbing},
            {StandardCursorType.ZoomIn, ZoomIn},
            {StandardCursorType.ZoomOut, ZoomOut},
            {StandardCursorType.Cell, Cell},
            {StandardCursorType.VerticalText, VerticalText},
            {StandardCursorType.ContextualMenu, ContextMenu},
            {StandardCursorType.NoDrop, NoDrop},
            {StandardCursorType.AllScroll, AllScroll},
            // Column/row resize have no dedicated Android shape, the double arrows are the closest fit
            {StandardCursorType.ColResize, HorizontalDoubleArrow},
            {StandardCursorType.RowResize, VerticalDoubleArrow},
        };

        /// <summary>Returns the pointer icon type for the cursor, or the arrow icon for future enum members.</summary>
        public static int GetMapping(StandardCursorType cursorType) =>
            Mapping.TryGetValue(cursorType, out var iconType) ? iconType : Arrow;
    }
}

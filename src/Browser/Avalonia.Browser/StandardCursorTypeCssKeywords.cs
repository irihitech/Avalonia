using System.Collections.Generic;
using Avalonia.Input;

namespace Avalonia.Browser
{
    /// <summary>
    /// Maps <see cref="StandardCursorType"/> to CSS cursor keywords.
    /// This file must stay free of browser dependencies so unit tests can compile
    /// the same source via a Compile Include link and validate the table.
    /// </summary>
    internal static class StandardCursorTypeCssKeywords
    {
        public const string Default = "default";

        internal static readonly Dictionary<StandardCursorType, string> Mapping = new Dictionary
            <StandardCursorType, string>
        {
            {StandardCursorType.Arrow, Default},
            {StandardCursorType.Hand, "pointer"},
            {StandardCursorType.Cross, "crosshair"},
            {StandardCursorType.Help, "help"},
            {StandardCursorType.Ibeam, "text"},
            {StandardCursorType.No, "not-allowed"},
            {StandardCursorType.None, "none"},
            {StandardCursorType.Wait, "progress"},
            {StandardCursorType.AppStarting, "wait"},

            {StandardCursorType.DragMove, "move"},
            {StandardCursorType.DragCopy, "copy"},
            {StandardCursorType.DragLink, "alias"},

            {StandardCursorType.UpArrow, Default},/*not found matching one*/
            {StandardCursorType.SizeWestEast, "ew-resize"},
            {StandardCursorType.SizeNorthSouth, "ns-resize"},
            {StandardCursorType.SizeAll, "move"},

            {StandardCursorType.TopSide, "n-resize"},
            {StandardCursorType.BottomSide, "s-resize"},
            {StandardCursorType.LeftSide, "w-resize"},
            {StandardCursorType.RightSide, "e-resize"},
            {StandardCursorType.TopLeftCorner, "nw-resize"},
            {StandardCursorType.TopRightCorner, "ne-resize"},
            {StandardCursorType.BottomLeftCorner, "sw-resize"},
            {StandardCursorType.BottomRightCorner, "se-resize"},

            // CSS cursor / XDG cursor-names standard shapes
            {StandardCursorType.SizeNorthWestSouthEast, "nwse-resize"},
            {StandardCursorType.SizeNorthEastSouthWest, "nesw-resize"},
            {StandardCursorType.Grab, "grab"},
            {StandardCursorType.Grabbing, "grabbing"},
            {StandardCursorType.ZoomIn, "zoom-in"},
            {StandardCursorType.ZoomOut, "zoom-out"},
            {StandardCursorType.Cell, "cell"},
            {StandardCursorType.VerticalText, "vertical-text"},
            {StandardCursorType.ContextualMenu, "context-menu"},
            {StandardCursorType.NoDrop, "no-drop"},
            {StandardCursorType.AllScroll, "all-scroll"},
            {StandardCursorType.ColResize, "col-resize"},
            {StandardCursorType.RowResize, "row-resize"},
        };

        /// <summary>Returns the CSS cursor keyword, or "default" for future enum members.</summary>
        public static string GetKeyword(StandardCursorType cursorType) =>
            Mapping.TryGetValue(cursorType, out var keyword) ? keyword : Default;
    }
}

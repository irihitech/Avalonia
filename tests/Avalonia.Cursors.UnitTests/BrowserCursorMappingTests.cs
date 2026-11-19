using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Browser;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Cursors.UnitTests;

/// <summary>
/// Validates the Browser CSS keyword table (compiled into this assembly from the same
/// source file Avalonia.Browser uses, see the Compile Include link in the project).
/// Keywords are asserted against the CSS cursor keyword set so a typo cannot validate
/// itself, and every mapping is pinned so entries cannot silently swap.
/// </summary>
public class BrowserCursorMappingTests
{
    private static readonly HashSet<string> CssCursorKeywords = new HashSet<string>
    {
        // General
        "auto", "default", "none", "context-menu", "help", "pointer", "progress", "wait",
        // Selection and drag
        "cell", "crosshair", "text", "vertical-text", "alias", "copy", "move", "no-drop",
        "not-allowed", "grab", "grabbing",
        // Resizing and scrolling
        "e-resize", "n-resize", "ne-resize", "nw-resize", "s-resize", "se-resize",
        "sw-resize", "w-resize", "ew-resize", "ns-resize", "nesw-resize", "nwse-resize",
        "col-resize", "row-resize", "all-scroll", "zoom-in", "zoom-out",
    };

    public static TheoryData<StandardCursorType> CursorTypes =>
        new(Enum.GetValues<StandardCursorType>().Cast<StandardCursorType>());

    public static TheoryData<StandardCursorType, string> Mappings => new()
    {
        { StandardCursorType.Arrow, "default" },
        { StandardCursorType.Hand, "pointer" },
        { StandardCursorType.Cross, "crosshair" },
        { StandardCursorType.Help, "help" },
        { StandardCursorType.Ibeam, "text" },
        { StandardCursorType.No, "not-allowed" },
        { StandardCursorType.None, "none" },
        { StandardCursorType.Wait, "progress" },
        { StandardCursorType.AppStarting, "wait" },

        { StandardCursorType.DragMove, "move" },
        { StandardCursorType.DragCopy, "copy" },
        { StandardCursorType.DragLink, "alias" },

        // No matching CSS keyword
        { StandardCursorType.UpArrow, "default" },
        { StandardCursorType.SizeWestEast, "ew-resize" },
        { StandardCursorType.SizeNorthSouth, "ns-resize" },
        { StandardCursorType.SizeAll, "move" },

        { StandardCursorType.TopSide, "n-resize" },
        { StandardCursorType.BottomSide, "s-resize" },
        { StandardCursorType.LeftSide, "w-resize" },
        { StandardCursorType.RightSide, "e-resize" },
        { StandardCursorType.TopLeftCorner, "nw-resize" },
        { StandardCursorType.TopRightCorner, "ne-resize" },
        { StandardCursorType.BottomLeftCorner, "sw-resize" },
        { StandardCursorType.BottomRightCorner, "se-resize" },

        // CSS cursor / XDG cursor-names standard shapes
        { StandardCursorType.SizeNorthWestSouthEast, "nwse-resize" },
        { StandardCursorType.SizeNorthEastSouthWest, "nesw-resize" },
        { StandardCursorType.Grab, "grab" },
        { StandardCursorType.Grabbing, "grabbing" },
        { StandardCursorType.ZoomIn, "zoom-in" },
        { StandardCursorType.ZoomOut, "zoom-out" },
        { StandardCursorType.Cell, "cell" },
        { StandardCursorType.VerticalText, "vertical-text" },
        { StandardCursorType.ContextualMenu, "context-menu" },
        { StandardCursorType.NoDrop, "no-drop" },
        { StandardCursorType.AllScroll, "all-scroll" },
        { StandardCursorType.ColResize, "col-resize" },
        { StandardCursorType.RowResize, "row-resize" },
    };

    [Theory]
    [MemberData(nameof(CursorTypes))]
    public void Maps_To_A_Valid_Css_Cursor_Keyword(StandardCursorType type)
        => Assert.Contains(StandardCursorTypeCssKeywords.Mapping[type], CssCursorKeywords);

    [Theory]
    [MemberData(nameof(Mappings))]
    public void Maps_To_The_Expected_Keyword(StandardCursorType type, string expectedKeyword)
        => Assert.Equal(expectedKeyword, StandardCursorTypeCssKeywords.Mapping[type]);
}

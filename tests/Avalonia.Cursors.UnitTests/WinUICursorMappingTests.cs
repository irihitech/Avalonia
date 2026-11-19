using System;
using System.Linq;
using Avalonia.Input;
using Avalonia.WinUI;
using Xunit;

namespace Avalonia.Cursors.UnitTests;

/// <summary>
/// Validates the WinUI cursor shape table (compiled into this assembly from the same
/// source file Avalonia.WinUI uses, see the Compile Include link in the project).
/// Shape values are asserted as literals so a typo in the shared constants cannot
/// validate itself.
/// </summary>
public class WinUICursorMappingTests
{
    public static TheoryData<StandardCursorType> CursorTypesExceptNone =>
        new(Enum.GetValues<StandardCursorType>().Cast<StandardCursorType>()
            .Where(t => t != StandardCursorType.None));

    public static TheoryData<StandardCursorType, int> Mappings => new()
    {
        { StandardCursorType.Arrow, StandardCursorTypeCursorShapes.Arrow },
        { StandardCursorType.Ibeam, StandardCursorTypeCursorShapes.IBeam },
        { StandardCursorType.Wait, StandardCursorTypeCursorShapes.Wait },
        { StandardCursorType.AppStarting, StandardCursorTypeCursorShapes.Wait },
        { StandardCursorType.Cross, StandardCursorTypeCursorShapes.Cross },
        { StandardCursorType.Help, StandardCursorTypeCursorShapes.Help },
        { StandardCursorType.Hand, StandardCursorTypeCursorShapes.Hand },
        { StandardCursorType.No, StandardCursorTypeCursorShapes.UniversalNo },
        { StandardCursorType.SizeAll, StandardCursorTypeCursorShapes.SizeAll },
        { StandardCursorType.SizeNorthSouth, StandardCursorTypeCursorShapes.SizeNorthSouth },
        { StandardCursorType.SizeWestEast, StandardCursorTypeCursorShapes.SizeWestEast },
        { StandardCursorType.UpArrow, StandardCursorTypeCursorShapes.UpArrow },
        { StandardCursorType.TopSide, StandardCursorTypeCursorShapes.SizeNorthSouth },
        { StandardCursorType.BottomSide, StandardCursorTypeCursorShapes.SizeNorthSouth },
        { StandardCursorType.LeftSide, StandardCursorTypeCursorShapes.SizeWestEast },
        { StandardCursorType.RightSide, StandardCursorTypeCursorShapes.SizeWestEast },
        { StandardCursorType.TopLeftCorner, StandardCursorTypeCursorShapes.SizeNorthwestSoutheast },
        { StandardCursorType.BottomRightCorner, StandardCursorTypeCursorShapes.SizeNorthwestSoutheast },
        { StandardCursorType.TopRightCorner, StandardCursorTypeCursorShapes.SizeNortheastSouthwest },
        { StandardCursorType.BottomLeftCorner, StandardCursorTypeCursorShapes.SizeNortheastSouthwest },
        // No built-in drag cursors, the arrow is the closest fallback
        { StandardCursorType.DragMove, StandardCursorTypeCursorShapes.Arrow },
        { StandardCursorType.DragCopy, StandardCursorTypeCursorShapes.Arrow },
        { StandardCursorType.DragLink, StandardCursorTypeCursorShapes.Arrow },

        // CSS cursor / XDG cursor-names standard shapes
        { StandardCursorType.SizeNorthWestSouthEast, StandardCursorTypeCursorShapes.SizeNorthwestSoutheast },
        { StandardCursorType.SizeNorthEastSouthWest, StandardCursorTypeCursorShapes.SizeNortheastSouthwest },
        { StandardCursorType.NoDrop, StandardCursorTypeCursorShapes.UniversalNo },
        { StandardCursorType.AllScroll, StandardCursorTypeCursorShapes.SizeAll },
        { StandardCursorType.ColResize, StandardCursorTypeCursorShapes.SizeWestEast },
        { StandardCursorType.RowResize, StandardCursorTypeCursorShapes.SizeNorthSouth },
        // No built-in WinUI shape, mapped to the closest system cursor
        { StandardCursorType.Grab, StandardCursorTypeCursorShapes.Hand },
        { StandardCursorType.Grabbing, StandardCursorTypeCursorShapes.Hand },
        { StandardCursorType.VerticalText, StandardCursorTypeCursorShapes.IBeam },
        { StandardCursorType.ZoomIn, StandardCursorTypeCursorShapes.Cross },
        { StandardCursorType.ZoomOut, StandardCursorTypeCursorShapes.Cross },
        { StandardCursorType.Cell, StandardCursorTypeCursorShapes.Cross },
        { StandardCursorType.ContextualMenu, StandardCursorTypeCursorShapes.Arrow },
    };

    [Theory]
    [MemberData(nameof(CursorTypesExceptNone))]
    public void Every_Standard_Cursor_Type_Has_An_Explicit_WinUI_Mapping(StandardCursorType type)
        => Assert.True(StandardCursorTypeCursorShapes.Mapping.ContainsKey(type),
            $"{type} is missing from the WinUI cursor shape table");

    [Theory]
    [MemberData(nameof(Mappings))]
    public void Maps_To_The_Expected_InputSystemCursorShape(StandardCursorType type, int expectedShape)
        => Assert.Equal(expectedShape, StandardCursorTypeCursorShapes.Mapping[type]);

    [Fact]
    public void Shape_Constants_Match_InputSystemCursorShape()
    {
        // Microsoft.UI.Input.InputSystemCursorShape values, stable public API since
        // Windows App SDK 1.0; asserted as literals so a typo in the shared constants
        // file cannot silently validate itself.
        Assert.Equal(0, StandardCursorTypeCursorShapes.Arrow);
        Assert.Equal(1, StandardCursorTypeCursorShapes.Cross);
        Assert.Equal(3, StandardCursorTypeCursorShapes.Hand);
        Assert.Equal(4, StandardCursorTypeCursorShapes.Help);
        Assert.Equal(5, StandardCursorTypeCursorShapes.IBeam);
        Assert.Equal(6, StandardCursorTypeCursorShapes.SizeAll);
        Assert.Equal(7, StandardCursorTypeCursorShapes.SizeNortheastSouthwest);
        Assert.Equal(8, StandardCursorTypeCursorShapes.SizeNorthSouth);
        Assert.Equal(9, StandardCursorTypeCursorShapes.SizeNorthwestSoutheast);
        Assert.Equal(10, StandardCursorTypeCursorShapes.SizeWestEast);
        Assert.Equal(11, StandardCursorTypeCursorShapes.UniversalNo);
        Assert.Equal(12, StandardCursorTypeCursorShapes.UpArrow);
        Assert.Equal(13, StandardCursorTypeCursorShapes.Wait);
    }
}

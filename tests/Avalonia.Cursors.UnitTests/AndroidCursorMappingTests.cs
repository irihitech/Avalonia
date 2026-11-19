using System;
using System.Linq;
using Avalonia.Android;
using Avalonia.Input;
using Xunit;

namespace Avalonia.Cursors.UnitTests;

/// <summary>
/// Validates the Android cursor table (compiled into this assembly from the same source
/// file Avalonia.Android uses, see the Compile Include link in the project). Icon values
/// are asserted as literals so a typo in the shared constants cannot validate itself.
/// </summary>
public class AndroidCursorMappingTests
{
    public static TheoryData<StandardCursorType> CursorTypes =>
        new(Enum.GetValues<StandardCursorType>().Cast<StandardCursorType>());

    public static TheoryData<StandardCursorType, int> Mappings => new()
    {
        // CSS cursor / XDG cursor-names standard shapes
        { StandardCursorType.SizeNorthWestSouthEast, StandardCursorTypePointerIcons.TopLeftDiagonalDoubleArrow },
        { StandardCursorType.SizeNorthEastSouthWest, StandardCursorTypePointerIcons.TopRightDiagonalDoubleArrow },
        { StandardCursorType.Grab, StandardCursorTypePointerIcons.Grab },
        { StandardCursorType.Grabbing, StandardCursorTypePointerIcons.Grabbing },
        { StandardCursorType.ZoomIn, StandardCursorTypePointerIcons.ZoomIn },
        { StandardCursorType.ZoomOut, StandardCursorTypePointerIcons.ZoomOut },
        { StandardCursorType.Cell, StandardCursorTypePointerIcons.Cell },
        { StandardCursorType.VerticalText, StandardCursorTypePointerIcons.VerticalText },
        { StandardCursorType.ContextualMenu, StandardCursorTypePointerIcons.ContextMenu },
        { StandardCursorType.NoDrop, StandardCursorTypePointerIcons.NoDrop },
        { StandardCursorType.AllScroll, StandardCursorTypePointerIcons.AllScroll },
        // No dedicated Android shape for column/row resize, the double arrows are the closest fit
        { StandardCursorType.ColResize, StandardCursorTypePointerIcons.HorizontalDoubleArrow },
        { StandardCursorType.RowResize, StandardCursorTypePointerIcons.VerticalDoubleArrow },
    };

    public static TheoryData<StandardCursorType, int> LegacyMappings => new()
    {
        { StandardCursorType.Arrow, StandardCursorTypePointerIcons.Arrow },
        { StandardCursorType.Ibeam, StandardCursorTypePointerIcons.Text },
        { StandardCursorType.Wait, StandardCursorTypePointerIcons.Wait },
        { StandardCursorType.AppStarting, StandardCursorTypePointerIcons.Wait },
        { StandardCursorType.Cross, StandardCursorTypePointerIcons.Crosshair },
        { StandardCursorType.UpArrow, StandardCursorTypePointerIcons.Arrow },
        { StandardCursorType.SizeWestEast, StandardCursorTypePointerIcons.HorizontalDoubleArrow },
        { StandardCursorType.SizeNorthSouth, StandardCursorTypePointerIcons.VerticalDoubleArrow },
        { StandardCursorType.SizeAll, StandardCursorTypePointerIcons.AllScroll },
        { StandardCursorType.No, StandardCursorTypePointerIcons.NoDrop },
        { StandardCursorType.Hand, StandardCursorTypePointerIcons.Hand },
        { StandardCursorType.Help, StandardCursorTypePointerIcons.Help },
        { StandardCursorType.TopSide, StandardCursorTypePointerIcons.VerticalDoubleArrow },
        { StandardCursorType.BottomSide, StandardCursorTypePointerIcons.VerticalDoubleArrow },
        { StandardCursorType.LeftSide, StandardCursorTypePointerIcons.HorizontalDoubleArrow },
        { StandardCursorType.RightSide, StandardCursorTypePointerIcons.HorizontalDoubleArrow },
        { StandardCursorType.TopLeftCorner, StandardCursorTypePointerIcons.TopLeftDiagonalDoubleArrow },
        { StandardCursorType.TopRightCorner, StandardCursorTypePointerIcons.TopRightDiagonalDoubleArrow },
        { StandardCursorType.BottomLeftCorner, StandardCursorTypePointerIcons.TopRightDiagonalDoubleArrow },
        { StandardCursorType.BottomRightCorner, StandardCursorTypePointerIcons.TopLeftDiagonalDoubleArrow },
        { StandardCursorType.DragMove, StandardCursorTypePointerIcons.AllScroll },
        { StandardCursorType.DragCopy, StandardCursorTypePointerIcons.Copy },
        { StandardCursorType.DragLink, StandardCursorTypePointerIcons.Alias },
        { StandardCursorType.None, StandardCursorTypePointerIcons.Null },
    };

    [Theory]
    [MemberData(nameof(CursorTypes))]
    public void Every_Standard_Cursor_Type_Has_An_Explicit_Android_Mapping(StandardCursorType type)
        => Assert.True(StandardCursorTypePointerIcons.Mapping.ContainsKey(type),
            $"{type} is missing from the Android pointer icon table");

    [Theory]
    [MemberData(nameof(Mappings))]
    [MemberData(nameof(LegacyMappings))]
    public void Maps_To_The_Expected_Pointer_Icon(StandardCursorType type, int expectedIcon)
        => Assert.Equal(expectedIcon, StandardCursorTypePointerIcons.Mapping[type]);

    [Fact]
    public void Pointer_Icon_Constants_Match_android_view_PointerIcon()
    {
        // Stable public API since API 24; asserted as literals so a typo in the
        // shared constants file cannot silently validate itself.
        Assert.Equal(0, StandardCursorTypePointerIcons.Null);
        Assert.Equal(1000, StandardCursorTypePointerIcons.Arrow);
        Assert.Equal(1001, StandardCursorTypePointerIcons.ContextMenu);
        Assert.Equal(1002, StandardCursorTypePointerIcons.Hand);
        Assert.Equal(1003, StandardCursorTypePointerIcons.Help);
        Assert.Equal(1004, StandardCursorTypePointerIcons.Wait);
        Assert.Equal(1006, StandardCursorTypePointerIcons.Cell);
        Assert.Equal(1007, StandardCursorTypePointerIcons.Crosshair);
        Assert.Equal(1008, StandardCursorTypePointerIcons.Text);
        Assert.Equal(1009, StandardCursorTypePointerIcons.VerticalText);
        Assert.Equal(1010, StandardCursorTypePointerIcons.Alias);
        Assert.Equal(1011, StandardCursorTypePointerIcons.Copy);
        Assert.Equal(1012, StandardCursorTypePointerIcons.NoDrop);
        Assert.Equal(1013, StandardCursorTypePointerIcons.AllScroll);
        Assert.Equal(1014, StandardCursorTypePointerIcons.HorizontalDoubleArrow);
        Assert.Equal(1015, StandardCursorTypePointerIcons.VerticalDoubleArrow);
        Assert.Equal(1016, StandardCursorTypePointerIcons.TopRightDiagonalDoubleArrow);
        Assert.Equal(1017, StandardCursorTypePointerIcons.TopLeftDiagonalDoubleArrow);
        Assert.Equal(1018, StandardCursorTypePointerIcons.ZoomIn);
        Assert.Equal(1019, StandardCursorTypePointerIcons.ZoomOut);
        Assert.Equal(1020, StandardCursorTypePointerIcons.Grab);
        Assert.Equal(1021, StandardCursorTypePointerIcons.Grabbing);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Wayland.Server.Transient;
using Xunit;

namespace Avalonia.Cursors.UnitTests;

/// <summary>
/// Validates the Wayland cursor name chains and the fallback decision when the theme
/// is missing the CSS/XDG cursor names: candidates in order, then the arrow cursor,
/// and only hiding the cursor when even the arrow failed to load.
/// </summary>
public class WaylandCursorFallbackTests
{
    public static readonly StandardCursorType[] NewCursorTypes =
    [
        StandardCursorType.SizeNorthWestSouthEast,
        StandardCursorType.SizeNorthEastSouthWest,
        StandardCursorType.Grab,
        StandardCursorType.Grabbing,
        StandardCursorType.ZoomIn,
        StandardCursorType.ZoomOut,
        StandardCursorType.Cell,
        StandardCursorType.VerticalText,
        StandardCursorType.ContextualMenu,
        StandardCursorType.NoDrop,
        StandardCursorType.AllScroll,
        StandardCursorType.ColResize,
        StandardCursorType.RowResize,
    ];

    /// <summary>Cursor names that predate the CSS/XDG set and ship with every X cursor theme.</summary>
    private static readonly HashSet<string> s_classicThemeNames =
    [
        "left_ptr", "xterm", "watch", "cross", "sb_up_arrow", "sb_h_double_arrow", "sb_v_double_arrow",
        "fleur", "crossed_circle", "hand1", "hand2", "top_left_corner", "top_right_corner",
    ];

    /// <summary>Shapes with no classic-name equivalent — the arrow cursor is the accepted fallback.</summary>
    private static readonly HashSet<StandardCursorType> s_arrowFallbackOnly =
    [
        StandardCursorType.ZoomIn, StandardCursorType.ZoomOut,
        StandardCursorType.VerticalText, StandardCursorType.ContextualMenu,
    ];

    [Theory]
    [MemberData(nameof(NewCursorTypeData))]
    public void Chain_Ends_In_A_Name_Every_Theme_Ships_Or_Is_Allowed_To_Fall_Back_To_Arrow(
        StandardCursorType type)
    {
        var names = WaylandCursorManager.CursorNames[type];

        Assert.NotEmpty(names);
        Assert.True(s_classicThemeNames.Contains(names[^1]) || s_arrowFallbackOnly.Contains(type),
            $"{type} chain [{string.Join(", ", names)}] ends in '{names[^1]}' which a legacy theme " +
            "may not ship, and the type is not in the arrow-fallback allow list");
    }

    [Theory]
    [MemberData(nameof(NewCursorTypeData))]
    public void Chain_Has_No_Duplicate_Names(StandardCursorType type)
    {
        var names = WaylandCursorManager.CursorNames[type];

        Assert.Equal(names.Length, names.Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(NewCursorTypeData))]
    public void X11_And_Wayland_Prefer_The_Same_Primary_Theme_Name(StandardCursorType type)
    {
        Assert.Equal(
            Avalonia.X11.X11CursorFactory.s_libraryCursors[type][0],
            WaylandCursorManager.CursorNames[type][0]);
    }

    [Fact]
    public void Loaded_Cursor_Is_Returned_As_Is()
        => Assert.Equal(StandardCursorType.Hand,
            WaylandCursorManager.ResolveCursorType(_ => true, StandardCursorType.Hand));

    [Fact]
    public void Missing_Cursor_Falls_Back_To_Arrow()
        => Assert.Equal(StandardCursorType.Arrow,
            WaylandCursorManager.ResolveCursorType(t => t == StandardCursorType.Arrow, StandardCursorType.ZoomOut));

    [Fact]
    public void Missing_Arrow_Hides_The_Cursor()
    {
        Assert.Null(WaylandCursorManager.ResolveCursorType(_ => false, StandardCursorType.Arrow));
        Assert.Null(WaylandCursorManager.ResolveCursorType(_ => false, StandardCursorType.ZoomOut));
    }

    [Fact]
    public void None_Hides_The_Cursor()
        => Assert.Null(WaylandCursorManager.ResolveCursorType(_ => true, StandardCursorType.None));

    public static TheoryData<StandardCursorType> NewCursorTypeData
    {
        get
        {
            var data = new TheoryData<StandardCursorType>();
            foreach (var type in NewCursorTypes)
                data.Add(type);
            return data;
        }
    }
}

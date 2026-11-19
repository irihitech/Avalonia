using System;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.X11;
using Xunit;

namespace Avalonia.Cursors.UnitTests;

/// <summary>
/// Drives <see cref="X11CursorFactory.GetOrLoadCursorHandle"/> with fake loaders to verify
/// the actual fallback results when a theme is missing the CSS/XDG cursor names:
/// theme names in order, then the font-cursor fallback, then the arrow cursor.
/// </summary>
public class X11CursorFallbackTests
{
    private static readonly string[] s_noThemeNames = [];

    /// <summary>Records every request; theme names return 0x100 + i, font shapes 0x200 + shape.</summary>
    private sealed class RecordingLoaders(string[] availableThemeNames)
    {
        public List<string> LibraryNames { get; } = [];
        public List<CursorFontShape> FontShapes { get; } = [];

        public nint LoadLibrary(string name)
        {
            LibraryNames.Add(name);
            var index = Array.IndexOf(availableThemeNames, name);
            return index >= 0 ? 0x100 + index : 0;
        }

        // Font cursors are part of the core protocol and always available
        public nint LoadFont(CursorFontShape shape)
        {
            FontShapes.Add(shape);
            return 0x200 + (int)shape;
        }
    }

    private static nint Resolve(
        StandardCursorType type, RecordingLoaders loaders,
        Dictionary<StandardCursorType, nint>? cache = null) =>
        X11CursorFactory.GetOrLoadCursorHandle(type, cache ?? new Dictionary<StandardCursorType, nint>(),
            loaders.LoadLibrary, loaders.LoadFont, 0xDEAD);

    // expectedShape is a CursorFontShape value passed as int because the enum is
    // internal to Avalonia.X11 and cannot appear in a public test method signature.
    [Theory]
    [InlineData(StandardCursorType.Cell, (int)CursorFontShape.XC_cross)]
    [InlineData(StandardCursorType.NoDrop, (int)CursorFontShape.XC_X_cursor)]
    [InlineData(StandardCursorType.ColResize, (int)CursorFontShape.XC_sb_h_double_arrow)]
    [InlineData(StandardCursorType.RowResize, (int)CursorFontShape.XC_sb_v_double_arrow)]
    [InlineData(StandardCursorType.AllScroll, (int)CursorFontShape.XC_fleur)]
    [InlineData(StandardCursorType.Grab, (int)CursorFontShape.XC_hand1)]
    [InlineData(StandardCursorType.Grabbing, (int)CursorFontShape.XC_hand2)]
    [InlineData(StandardCursorType.VerticalText, (int)CursorFontShape.XC_xterm)]
    [InlineData(StandardCursorType.ZoomIn, (int)CursorFontShape.XC_plus)]
    [InlineData(StandardCursorType.SizeNorthWestSouthEast, (int)CursorFontShape.XC_top_left_corner)]
    [InlineData(StandardCursorType.SizeNorthEastSouthWest, (int)CursorFontShape.XC_top_right_corner)]
    public void Theme_Missing_Css_Name_Falls_Back_To_The_Font_Cursor(
        StandardCursorType type, int expectedShape)
    {
        var loaders = new RecordingLoaders(s_noThemeNames);

        var handle = Resolve(type, loaders);

        Assert.Equal(0x200 + expectedShape, handle);
        Assert.Equal([(CursorFontShape)expectedShape], loaders.FontShapes);
    }

    [Fact]
    public void NoDrop_Tries_Legacy_Aliases_Before_The_Font_Cursor()
    {
        var loaders = new RecordingLoaders(["forbidden"]);

        var handle = Resolve(StandardCursorType.NoDrop, loaders);

        // "no-drop" missing, "forbidden" available: stop there, no font request
        Assert.Equal(["no-drop", "forbidden"], loaders.LibraryNames);
        Assert.Equal(0x100, handle);
        Assert.Empty(loaders.FontShapes);
    }

    [Fact]
    public void ColResize_Tries_The_Legacy_XDG_Name_Before_The_Font_Cursor()
    {
        var loaders = new RecordingLoaders(["sb_h_double_arrow"]);

        var handle = Resolve(StandardCursorType.ColResize, loaders);

        Assert.Equal(["col-resize", "sb_h_double_arrow"], loaders.LibraryNames);
        Assert.Equal(0x100, handle);
        Assert.Empty(loaders.FontShapes);
    }

    [Fact]
    public void Grab_Tries_All_Aliases_Before_The_Font_Cursor()
    {
        var loaders = new RecordingLoaders(s_noThemeNames);

        Resolve(StandardCursorType.Grab, loaders);

        Assert.Equal(["grab", "openhand", "hand1"], loaders.LibraryNames);
    }

    [Theory]
    [InlineData(StandardCursorType.ZoomOut)]
    [InlineData(StandardCursorType.ContextualMenu)]
    public void No_Font_Equivalent_Falls_Back_To_The_Arrow_Cursor(StandardCursorType type)
    {
        var loaders = new RecordingLoaders(s_noThemeNames);

        var handle = Resolve(type, loaders);

        // Resolved through the Arrow cursor, which itself loads from the font cursor
        Assert.Equal([CursorFontShape.XC_left_ptr], loaders.FontShapes);
        Assert.Equal(0x200 + (int)CursorFontShape.XC_left_ptr, handle);
    }

    [Fact]
    public void Resolved_Handle_Is_Cached()
    {
        var loaders = new RecordingLoaders(s_noThemeNames);
        var cache = new Dictionary<StandardCursorType, nint>();

        var first = Resolve(StandardCursorType.Cell, loaders, cache);
        var second = Resolve(StandardCursorType.Cell, loaders, cache);

        Assert.Equal(first, second);
        Assert.Single(loaders.LibraryNames);
        Assert.Single(loaders.FontShapes);
    }
}

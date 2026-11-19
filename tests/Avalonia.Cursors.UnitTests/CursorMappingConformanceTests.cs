using System;
using Avalonia.Input;
using Avalonia.Wayland.Server.Transient;
using Xunit;

namespace Avalonia.Cursors.UnitTests;

/// <summary>
/// Keeps the per-backend cursor mapping tables in sync with <see cref="StandardCursorType"/>.
/// Win32 throws KeyNotFoundException for unmapped members, while X11, Wayland, Browser and
/// WinUI silently fall back to a default cursor — this makes every failure mode visible
/// on every OS. The Browser and WinUI tables are compiled into this assembly from the same
/// sources their backends use (see the Compile Include links in the project).
/// </summary>
public class CursorMappingConformanceTests
{
    public static TheoryData<StandardCursorType> AllCursorTypes
    {
        get
        {
            var data = new TheoryData<StandardCursorType>();
            foreach (var type in Enum.GetValues<StandardCursorType>())
            {
                data.Add(type);
            }

            return data;
        }
    }

    public static TheoryData<StandardCursorType> CursorTypesExceptNone
    {
        get
        {
            var data = new TheoryData<StandardCursorType>();
            foreach (var type in Enum.GetValues<StandardCursorType>())
            {
                if (type != StandardCursorType.None)
                {
                    data.Add(type);
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(CursorTypesExceptNone))]
    public void Win32_Has_Mapping_For_Every_Standard_Cursor_Type(StandardCursorType type)
        => Assert.True(Avalonia.Win32.StandardCursorTypeIds.Mapping.ContainsKey(type),
            $"{type} is missing from the Win32 cursor mapping table");

    [Theory]
    [MemberData(nameof(CursorTypesExceptNone))]
    public void X11_Has_Mapping_For_Every_Standard_Cursor_Type(StandardCursorType type)
        => Assert.True(Avalonia.X11.X11CursorFactory.s_mapping.ContainsKey(type)
                       || Avalonia.X11.X11CursorFactory.s_libraryCursors.ContainsKey(type),
            $"{type} is missing from both X11 cursor mapping tables");

    [Theory]
    [MemberData(nameof(CursorTypesExceptNone))]
    public void Wayland_Has_Mapping_For_Every_Standard_Cursor_Type(StandardCursorType type)
        => Assert.True(WaylandCursorManager.CursorNames.ContainsKey(type),
            $"{type} is missing from the Wayland cursor name table");

    [Theory]
    [MemberData(nameof(AllCursorTypes))]
    public void Browser_Has_Mapping_For_Every_Standard_Cursor_Type(StandardCursorType type)
        => Assert.True(Avalonia.Browser.StandardCursorTypeCssKeywords.Mapping.ContainsKey(type),
            $"{type} is missing from the Browser CSS keyword table");

    [Theory]
    [MemberData(nameof(CursorTypesExceptNone))]
    public void WinUI_Has_Mapping_For_Every_Standard_Cursor_Type(StandardCursorType type)
        => Assert.True(Avalonia.WinUI.StandardCursorTypeCursorShapes.Mapping.ContainsKey(type),
            $"{type} is missing from the WinUI cursor shape table");
}

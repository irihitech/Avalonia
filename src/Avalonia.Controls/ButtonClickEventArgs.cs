using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class ButtonClickEventArgs(KeyModifiers? keyModifiers = null) : RoutedEventArgs
{
    public KeyModifiers? KeyModifiers { get;  } = keyModifiers;
}

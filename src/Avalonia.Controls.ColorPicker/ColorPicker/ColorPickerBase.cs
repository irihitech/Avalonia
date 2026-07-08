using System;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Avalonia.Controls
{
    /// <summary>
    /// Abstract base class for <see cref="ColorView"/> and <see cref="ColorPicker"/>,
    /// providing all shared color properties and property-synchronization logic.
    /// </summary>
    public abstract partial class ColorPickerBase : TemplatedControl
    {
        /// <summary>
        /// Event for when the selected color changes.
        /// </summary>
        public event EventHandler<ColorChangedEventArgs>? ColorChanged;

        protected bool _ignorePropertyChanged = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPickerBase"/> class.
        /// </summary>
        protected ColorPickerBase() : base()
        {
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (_ignorePropertyChanged)
            {
                base.OnPropertyChanged(change);
                return;
            }

            // Always keep the two color properties in sync
            if (change.Property == ColorProperty)
            {
                _ignorePropertyChanged = true;

                SetCurrentValue(HsvColorProperty, Color.ToHsv());

                OnColorChanged(new ColorChangedEventArgs(
                    change.GetOldValue<Color>(),
                    change.GetNewValue<Color>()));

                _ignorePropertyChanged = false;
            }
            else if (change.Property == HsvColorProperty)
            {
                _ignorePropertyChanged = true;

                SetCurrentValue(ColorProperty, HsvColor.ToRgb());

                OnColorChanged(new ColorChangedEventArgs(
                    change.GetOldValue<HsvColor>().ToRgb(),
                    change.GetNewValue<HsvColor>().ToRgb()));

                _ignorePropertyChanged = false;
            }
            else if (change.Property == PaletteProperty)
            {
                IColorPalette? palette = Palette;

                // Any custom palette change must be automatically synced with the
                // bound properties controlling the palette grid
                if (palette != null)
                {
                    SetCurrentValue(PaletteColumnCountProperty, palette.ColorCount);

                    List<Color> newPaletteColors = new List<Color>();
                    for (int shadeIndex = 0; shadeIndex < palette.ShadeCount; shadeIndex++)
                    {
                        for (int colorIndex = 0; colorIndex < palette.ColorCount; colorIndex++)
                        {
                            newPaletteColors.Add(palette.GetColor(colorIndex, shadeIndex));
                        }
                    }

                    SetCurrentValue(PaletteColorsProperty, newPaletteColors);
                }
            }
            else if (change.Property == IsAlphaEnabledProperty)
            {
                // Manually coerce the HsvColor value
                // (Color will be coerced automatically if HsvColor changes)
                SetCurrentValue(HsvColorProperty, OnCoerceHsvColor(HsvColor));
            }

            base.OnPropertyChanged(change);
        }

        /// <summary>
        /// Raises the <see cref="ColorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ColorChangedEventArgs"/> defining old/new colors.</param>
        protected virtual void OnColorChanged(ColorChangedEventArgs e)
        {
            ColorChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Called when the <see cref="Color"/> property has to be coerced.
        /// </summary>
        /// <param name="value">The value to coerce.</param>
        protected virtual Color OnCoerceColor(Color value)
        {
            if (IsAlphaEnabled == false)
            {
                return new Color(255, value.R, value.G, value.B);
            }

            return value;
        }

        /// <summary>
        /// Called when the <see cref="HsvColor"/> property has to be coerced.
        /// </summary>
        /// <param name="value">The value to coerce.</param>
        protected virtual HsvColor OnCoerceHsvColor(HsvColor value)
        {
            if (IsAlphaEnabled == false)
            {
                return new HsvColor(1.0, value.H, value.S, value.V);
            }

            return value;
        }

        /// <summary>
        /// Coerces/validates the <see cref="Color"/> property value.
        /// </summary>
        /// <param name="instance">The <see cref="ColorPickerBase"/> instance.</param>
        /// <param name="value">The value to coerce.</param>
        /// <returns>The coerced/validated value.</returns>
        private static Color CoerceColor(AvaloniaObject instance, Color value)
        {
            if (instance is ColorPickerBase colorPickerBase)
            {
                return colorPickerBase.OnCoerceColor(value);
            }

            return value;
        }

        /// <summary>
        /// Coerces/validates the <see cref="HsvColor"/> property value.
        /// </summary>
        /// <param name="instance">The <see cref="ColorPickerBase"/> instance.</param>
        /// <param name="value">The value to coerce.</param>
        /// <returns>The coerced/validated value.</returns>
        private static HsvColor CoerceHsvColor(AvaloniaObject instance, HsvColor value)
        {
            if (instance is ColorPickerBase colorPickerBase)
            {
                return colorPickerBase.OnCoerceHsvColor(value);
            }

            return value;
        }
    }
}

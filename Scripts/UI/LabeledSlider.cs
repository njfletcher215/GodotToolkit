using Godot;
using System;

/// <summary>
/// A mediator between a slider and its title, current value, and tick labels.
/// </summary>
// TODO figure out if I can make this generic between extending HSlider and VSlider (remember to update the README if I can).
// TODO I forgot to add the title!
public partial class LabeledSlider : HSlider {
    /// <summary>
    /// The simple format string label for the current value of the slider.
    /// </summary>
    /// <value>Use "currentValue" in the format string.</value>
    [Export]
    private SimpleFormatStringLabel currentValue;
    /// <summary>
    /// The container for the tick labels.
    /// In order for the tick labels to line up with the ticks,
    /// this container should be the (1 + 1/slider.TickCount)x larger than the slider itself,
    /// and offset to the left by (1/slider.TickCount) / 2.
    [Export]
    private HBoxContainer tickLabels;

    /// <inheritdoc />
    public override void _Ready() {
        if (this.tickLabels != null) this.InitializeTickLabels();
        if (this.currentValue != null) {
            this.currentValue.SetValue("currentValue", this.Value);
            this.ValueChanged += (value) =>
                this.currentValue.SetValue("currentValue", this.Value);
        }
    }

    /// <summary>
    /// Create the tick labels.
    /// </summary>
    private void InitializeTickLabels() {
        double min = this.MinValue;
        double max = this.MaxValue;
        for (int i = 0; i < this.TickCount; i++) {
            double tickValue = min + (i * (max - min) / (this.TickCount - 1));
            Label tickLabel = new Label {
                Text = tickValue.ToString("0.##"),
                HorizontalAlignment = HorizontalAlignment.Center,
                SizeFlagsHorizontal = Control.SizeFlags.Expand | Control.SizeFlags.Fill
            };
            this.tickLabels.AddChild(tickLabel);
        }
    }
}

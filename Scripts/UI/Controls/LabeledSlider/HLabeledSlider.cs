using Godot;
using System;

/// <summary>
/// A mediator between a horizontal slider and its title, current value, and tick labels.
/// </summary>
public partial class HLabeledSlider : HSlider {
    /// <summary>
    /// The simple format string label for the current value of the slider.
    /// </summary>
    /// <value>Use "currentValue" in the format string.</value>
    [Export]
    private SimpleFormatStringLabel currentValue;
    [Export]
    private bool labelTicks;

    private Label[] tickLabels;

    /// <inheritdoc />
    public override void _Ready() {
        if (this.labelTicks) this.InitializeTickLabels();

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
        this.tickLabels = new Label[this.TickCount];

        double valMin = this.MinValue;
        double valMax = this.MaxValue;
        double valOffset = (valMax - valMin) / (this.TickCount - 1);

        float posMin = 0;
        float posMax = this.Size.X;
        float posOffset = (posMax - posMin) / (this.TickCount - 1);

        for (int i = 0; i < this.TickCount; i++) {
            double val = valMin + (i * valOffset);
            float pos = posMin + (i * posOffset);
            Label tickLabel = new Label {
                Text = val.ToString("0.##"),
                Position = new Vector2(pos, this.Size.Y),
            };

            // horizontally center the tickLabel on pos
            tickLabel.Position -= new Vector2(tickLabel.Size.X / 2, 0);

            this.tickLabels[i] = tickLabel;
            this.AddChild(tickLabel);
        }
    }
}

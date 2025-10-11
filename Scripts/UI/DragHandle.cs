using Godot;
using System;

/// <summary>
/// A handle by which to drag a DragGroup.
/// DragHandles automatically attach to the closest DragGroup parent.
/// </summary>
[Tool]
public partial class DragHandle : Area2D {
    private bool isDragging = false;

    private DragGroup _dragTarget;

    public DragGroup DragTarget { set { this._dragTarget = value; } }

    public override void _UnhandledInput(InputEvent @event) {
        // NOTE: drag stop is handled on UnhandledInput, meaning it can occur even if the mouse is *not in the area*
        //       likewise, the motion does not stop if the mouse moves so fast it leaves the area
        if (@event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed) this.isDragging = false;
        if (this.isDragging && @event is InputEventMouseMotion motionEvent) this._dragTarget.Position += motionEvent.Relative;
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        // NOTE: drag start is only handled on InputEvent, meaning it only occurs if the mouse is *in the area*
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) this.isDragging = true;
    }
}

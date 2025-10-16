using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A draggable node.
/// </summary>
[Tool]
public partial class DragGroup : Node2D {
    private const string NO_DRAG_HANDLE_WARNING = "This node has no DragHandle, so it cannot be dragged. Add a DragHandle child to enable this behavior.";

    [Export] private bool enabled = true;

    private List<DragHandle> dragHandles;

    private Vector2 _dragDelta;

    public Vector2 Drag { set { this._dragDelta = value; } }

    public bool IsDragged { get => this.enabled && this.dragHandles.Any(dragHandle => dragHandle.IsDragging); }

    public override string[] _GetConfigurationWarnings() {
        this.dragHandles = DragGroup.GetDragHandles(this);
        if (this.dragHandles.Count == 0) return [DragGroup.NO_DRAG_HANDLE_WARNING];
        return [];
    }

    public override void _Ready() {
        this.dragHandles = DragGroup.GetDragHandles(this);
        foreach (DragHandle dragHandle in dragHandles) dragHandle.DragTarget = this;
        this.Drag = Vector2.Zero;
    }

    public override void _Process(double delta) {
        if (this.enabled) this.Position += this._dragDelta;
        this.Drag = Vector2.Zero;
    }

    public void Enable() {
        this.enabled = true;
    }

    public void Disable() {
        this.enabled = false;
    }

    public void Toggle() {
        this.enabled = !this.enabled;
    }

    /// <summary>
    /// Get all DragHandle children (recursively).
    /// Does not search through other DragGroup children, so DragGroups can be nested.
    /// </summary>
    /// <param name="parent">The node from which to find DragHandle children.</param>
    private static List<DragHandle> GetDragHandles(Node parent) {
        List<DragHandle> dragHandles = new List<DragHandle>();

        foreach (Node child in parent.GetChildren()) {
            if (child is DragHandle dragHandle) dragHandles.Add(dragHandle);

            // search recursively, except in nested DragGroups (handles are expected to apply only to their nearest DragGroup ancestor)
            else if (child is not DragGroup) dragHandles.AddRange(DragGroup.GetDragHandles(child));
        }

        return dragHandles;
    }
}

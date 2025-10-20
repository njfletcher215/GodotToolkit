using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A draggable Node2D.
/// </summary>
[Tool]
public partial class DragGroup : Node2D {
    private const string NO_DRAG_HANDLE_WARNING = "This node has no DragHandle, so it cannot be dragged. Add a DragHandle child to enable this behavior.";

    [Export] private bool enabled = true;

    private List<DragHandle> _dragHandles;
    /// <summary>
    /// All DragHandles which can be used to drag this object.
    /// </summary>
    protected List<DragHandle> DragHandles {
        get => this._dragHandles ??= DragGroup.GetDragHandles(this);
        set => this._dragHandles = value;
    }

    private Vector2 _dragDelta;

    /// <summary>
    /// The amount by which to move this object on the next _Process frame.
    ///
    /// NOTE: Should typically only be set by the DragHandle.
    /// Any other classes which wish to move the DragGroup may do so directly.
    /// </summary>
    public Vector2 Drag { set { this._dragDelta = value; } }

    /// <summary>
    /// Whether the DragGroup is currently being dragged by any DragHandle.
    ///
    /// NOTE: This does not necessarily mean this object is *moving*,
    /// just that one of its DragHandles has been activated (clicked on).
    /// </summary>
    public bool IsBeingDragged { get => this.enabled && this.DragHandles.Any(dragHandle => dragHandle.IsDragging); }

    /// <inheritdoc />
    public override string[] _GetConfigurationWarnings() {
        if (this.DragHandles.Count == 0) return [DragGroup.NO_DRAG_HANDLE_WARNING];
        return [];
    }

    /// <inheritdoc />
    public override void _Ready() {
        foreach (DragHandle dragHandle in this.DragHandles) dragHandle.DragTarget = this;
        this.Drag = Vector2.Zero;
    }

    /// <inheritdoc />
    public override void _Process(double delta) {
        if (this.enabled) this.Position += this._dragDelta;
        this.Drag = Vector2.Zero;
    }

    /// <summary>
    /// Allow the DragGroup to be dragged.
    /// </summary>
    public void Enable() {
        this.enabled = true;
    }

    /// <summary>
    /// Disallow the DragGroup to be dragged.
    /// </summary>
    public void Disable() {
        this.enabled = false;
    }

    /// <summary>
    /// Toggle whether the DragGroup is allowed to be dragged.
    /// </summary>
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

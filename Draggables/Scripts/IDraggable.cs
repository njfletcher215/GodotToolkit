/// <summary>
/// A draggable.
/// Typically, a node wrapped in a DragGroup.
/// Provides a default property for getting if the DragGroup is currently being dragged,
/// as well as default functions for enabling/disabling/toggling the DragGroup.
/// </summary>
public interface IDraggable {
    /// <summary>
    /// The DragGroup associated with the draggable.
    ///
    /// NOTE: Typically, this is the parent (or an ancestor) of the draggable.
    DragGroup DragGroup { get; }

    /// <summary>
    /// Whether the draggable is *currently* being dragged.
    ///
    /// Note: This does not necessarily mean the draggable is moving,
    /// just that one of the DragHandles for its DragGroup has been activated (clicked on).
    /// </summary>
    bool IsBeingDragged { get => this.DragGroup.IsBeingDragged; }

    /// <summary>
    /// Allow the associated DragGroup to be dragged.
    /// </summary>
    void EnableDragging() {
        this.DragGroup.Enable();
    }

    /// <summary>
    /// Disallow the associated DragGroup to be dragged.
    /// </summary>
    void DisableDragging() {
        this.DragGroup.Disable();
    }

    /// <summary>
    /// Toggle whether the associated DragGroup is allowed to be dragged.
    /// </summary>
    void ToggleDragging() {
        this.DragGroup.Toggle();
    }
}

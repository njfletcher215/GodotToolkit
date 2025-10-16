/// <summary>
/// A draggable.
/// Typically, a node wrapped in a DragGroup.
/// Provides a default property for getting if the DragGroup is currently being dragged,
/// as well as default functions for enabling/disabling/toggling the DragGroup.
/// </summary>
public interface IDraggable {
    DragGroup DragGroup { get; }

    bool IsBeingDragged { get => this.DragGroup.IsBeingDragged; }

    void EnableDragging() {
        this.DragGroup.Enable();
    }

    void DisableDragging() {
        this.DragGroup.Disable();
    }

    void ToggleDragging() {
        this.DragGroup.Toggle();
    }
}

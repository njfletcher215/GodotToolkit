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

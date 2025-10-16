public interface IDraggable {
    DragGroup DragGroup { get; }

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

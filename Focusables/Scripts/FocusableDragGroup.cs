using Godot;
using System;

/// <summary>
/// A drag group that only moves when focused.
/// </summary>
public partial class FocusableDragGroup : DragGroup, IFocusable {
    public event FocusableStateChangedHandler? OnFocusChanged;
    public event FocusableStateChangedHandler? OnHoldFocusChanged;
    public event FocusableStateChangedHandler? OnSoleFocusChanged;

    [Export] private bool _isFocused;
    bool IFocusable.IsFocused {
        get => this._isFocused;
        set {
            this._isFocused = value;
            this.OnFocusChanged?.Invoke(this);
        }
    }

    [Export] private bool _holdFocus;
    bool IFocusable.HoldFocus {
        get => this._holdFocus;
        set {
            this._holdFocus = value;
            this.OnHoldFocusChanged?.Invoke(this);
        }
    }

    [Export] private bool _soleFocus;
    bool IFocusable.SoleFocus {
        get => this._soleFocus;
        set {
            this._soleFocus = value;
            this.OnSoleFocusChanged?.Invoke(this);
        }
    }

    [Export] private bool _focusLocked;
    bool IFocusable.FocusLocked { get => this._focusLocked; set => this._focusLocked = value; }

    /// <inheritdoc />
    public override void _Process(double delta) {
        if ((this as IFocusable).IsFocused) base._Process(delta);

        if (!(this as IFocusable).FocusLocked && !Input.IsMouseButtonPressed(MouseButton.Left)) (this as IFocusable).Unfocus();
    }

    /// <summary>
    /// Register this node to the given FocusRouter.
    /// This is required because this.DragHandles is not publicly exposed.
    /// </summary>
    public void RegisterTo(FocusRouter router) {
        foreach (DragHandle dragHandle in this.DragHandles) router.RegisterRoute(dragHandle, this);
    }
}

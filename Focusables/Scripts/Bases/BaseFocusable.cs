using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A base implementation of IFocusable.
/// </summary>
public partial class BaseFocusable : IFocusable {
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
}

using System;
using System.Collections.Generic;

public delegate void FocusableStateChangedHandler(IFocusable focusable);

/// <summary>
/// A focusable.
/// Typically, a node that either extends or contains an Area2D or other interactable component.
/// Provides a default property for getting if the implementer is currently focused,
/// as well as default functions for focusing/unfocusing/toggling the implementer.
///
/// The effect(s) of being focused are up to the implementer.
/// Typically, implementers will enable/disable input in some way depending on their focused state.
/// (This could mean performing/skipping their own input function, enabling/disabling a child Area2D's CollsionShape, etc.)
/// </summary>
public interface IFocusable {
    /// <summary>
    /// Whether the focusable is focused.
    /// </summary>
    public bool IsFocused { get; set; }

    /// <summary>
    /// Whether the focusable should attempt to hold its focus.
    /// External classes should typically not unfocus focusables when IsFocused and HoldFocus.
    /// </summary>
    public bool HoldFocus { get; set; }

    /// <summary>
    /// Whether the focusable should attempt to make itself the *sole* focus.
    /// External classes should typically not focus other focusables when IsFocused and SoleFocus.
    /// </summary>
    public bool SoleFocus { get; set; }

    /// <summary>
    /// Whether the focusable is being continuously focused by an external class, like a router.
    /// Focusables should typically not unfocus themselves when IsFocused and FocusLocked.
    ///
    /// NOTE: FocusLocked purposefully does not provide an event handle, as its set function may be called by a router
    /// repeatedly with no change in value. If an implementation wishes to include side effects in its set function (*which is not advised*),
    /// it may be beneficial to add a guard clause so the side effects only trigger if the value has actually changed.
    public bool FocusLocked { get; set; }

    /// <summary>
    /// The event handle for when the focusable is focused/unfocused.
    /// It is up to the implementer to actually invoke on IsFocused.Set.
    /// The implementer will be passed as an IFocusable as context.
    /// </summary>
    public event FocusableStateChangedHandler? OnFocusChanged;

    /// <summary>
    /// The event handle when the focusable's HoldFocus property is set.
    /// It is up to the implementer to actually invoke on HoldFocus.Set.
    /// The implementer will be passed as an IFocusable as context.
    /// </summary>
    public event FocusableStateChangedHandler? OnHoldFocusChanged;

    /// <summary>
    /// The event handle for when the focusable's SoleFocus property is set.
    /// It is up to the implementer to actually invoke on SoleFocus.Set.
    /// The implementer will be passed as an IFocusable as context.
    /// </summary>
    public event FocusableStateChangedHandler? OnSoleFocusChanged;

    /// <summary>
    /// Focus this.
    /// </summary>
    void Focus() {
        this.IsFocused = true;
    }

    /// <summary>
    /// Unfocus this.
    /// </summary>
    void Unfocus() {
        this.IsFocused = false;
    }

    /// <summary>
    /// Toggle this between focused and unfocused.
    /// </summary>
    void Toggle() {
        this.IsFocused = !this.IsFocused;
    }
}

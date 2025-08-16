using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// The logic-handling component tied to an overlay menu.
/// Typically, the screen the overlay menu is on.
/// </summary>
public abstract partial class OverlayMenuHost : Node2D {
    /// <summary>
    /// Map of eventName strings -> handler methods.
    /// By convention, eventName strings should be in the form "MenuName_ButtonName".
    /// By convention, handler methods should be named in the form "OnMenuName_ButtonName".
    /// </summary>
    protected abstract IDictionary<string, Action> overlayMenuEventHandlers { get; }

    /// <summary>
    /// Handles an event passed from an overlay menu.
    /// </summary>
    /// <param name="eventName">
    /// The name of the event (which should be used by the host to determine which action to take).
    /// By convention, eventName should be in the form "MenuName_ButtonName".
    /// </param>
    /// <exception cref="ArgumentException">Thrown if the host does not have a method to handle the eventName.</exception>
    public void OnOverlayMenuEvent(string eventName) {
        if (!this.overlayMenuEventHandlers.TryGetValue(eventName, out Action handler))
            throw new ArgumentException(message: $"Unknown event: '{eventName}'", paramName: nameof(eventName));
        handler();
    }
}

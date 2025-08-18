using Godot;
using System;

/// <summary>
/// A menu which is overlayed on another screen.
/// The actual logic is handled by another (arbitrary) class, which may or may not be the screen on which this is overlaid.
/// </summary>
public partial class OverlayMenu : Node2D {
    /// <summary>
    /// The logic-handling component which events are passed to.
    /// Typically, this will be the screen containing the overlay.
    /// </summary>
    [Export]
    private OverlayMenuHost overlayMenuHost;

    /// <summary>
    /// Pass the given event to the overlay menu host, and close this menu if requested.
    /// </summary>
    /// <param name="eventName">
    /// The name of the event.
    /// By convention, should be in the form "MenuName_ButtonName".
    /// </param>
    /// <param name="close">Whether or not to close the menu after passing the event.</param>
    public void OnEvent(string eventName, bool close) {
        this.overlayMenuHost.OnOverlayMenuEvent(eventName);
        if (close) this.Close();
    }

    /// <summary>
    /// Close the overlay menu by setting its visibility to 'false'.
    /// </summary>
    private void Close() {
        this.Hide();
    }
}

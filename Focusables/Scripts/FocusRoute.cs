using Godot;
using System;

/// <summary>
/// A Node route for a FocusRouter.
/// </summary>
[GlobalClass]
public partial class FocusRoute : Resource {
    /// <summary>
    /// The CollisionObject2D which serves to detect the mouse.
    /// </summary>
    [Export(PropertyHint.NodePathValidTypes, "CollisionObject2D")]
    public NodePath activatorPath;

    /// <summary>
    /// The Node which will be focused when the mouse is detected.
    /// It is expected that this Node implements IFocusable.
    /// (due to technical limitations, IFocusables cannot be directly serialized).
    /// </summary>
    [Export(PropertyHint.NodePathValidTypes, "Node")]
    public NodePath endpointNodePath;
}

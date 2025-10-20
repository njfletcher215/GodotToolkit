using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A router for routing focus between different IFocusables.
/// </summary>
public partial class FocusRouter : Node2D, IFocusable {
    private const string ROUTE_NOT_FOCUSABLE_WARNING_TEMPLATE = "Route node {0} does not implement IFocusable. Route nodes must implement IFocusable, or they cannot be routed to.";

    public event FocusableStateChangedHandler? OnFocusChanged;
    public event FocusableStateChangedHandler? OnHoldFocusChanged;
    public event FocusableStateChangedHandler? OnSoleFocusChanged;

    /// <summary>
    /// Whether the router is focused (and thus can pass focus to routes).
    /// Typically, if this is the root router, this should always be set to <c>true</c>.
    /// </summary>
    [Export] private bool _isFocused = true;
    bool IFocusable.IsFocused {
        get => this._isFocused;
        set {
            this._isFocused = value;
            this.OnFocusChanged?.Invoke(this);

            if (!value) this.RemoveAllFocusLocks();
        }
    }

    /// <summary>
    /// Whether the router is holding focus.
    /// Will be set to <c>true</c> while any focused route is holding focus (and <c>false</c> when no route is).
    /// NOTE: this.HoldFocus.Set is a noop -- this is purposeful to prevent external manipulation.
    /// </summary>
    private bool _holdFocus = false;
    bool IFocusable.HoldFocus { get => this._holdFocus; set { ; } }

    /// <summary>
    /// Whether the router is requesting *sole* focus.
    /// Will be set to <c>true</c> while any focused route is requesting sole focus (and <c>false</c> when no route is).
    /// NOTE: this.SoleFocus.Set is a noop -- this is purposeful to prevent external manipulation.
    /// </summary>
    private bool _soleFocus = false;
    bool IFocusable.SoleFocus { get => this._soleFocus; set { ; } }

    // TODO does this need to do anything? Ill have to play around with nested routers. For now, I never give up focus so its fine
    //      I think how i implemented hold focus and sole focus should make this irrelevant, as long as i remember to make draggables hold focus on click event and release it on release event
    /// <summary>
    /// Whether the router is being focuslocked by another router.
    /// </summary>
    [Export] private bool _focusLocked;
    bool IFocusable.FocusLocked {
        get => this._focusLocked;
        set {
            this._focusLocked = value;

            if (!value) this.RemoveAllFocusLocks();
        }
    }

    // TEMP switch back to private -- maybe I should add a function that allows readonly access
    public Dictionary<CollisionObject2D, IFocusable> _routes = new Dictionary<CollisionObject2D, IFocusable>();

    /// <summary>
    /// The (Node) routes this router serves.
    /// Routes may also be registered at runtime via the RegisterRoute function.
    /// The router may also serve routes that focus non-Node IFocusables,
    /// but due to technical limitations, they cannot be serialized and must be registered at runtime.
    /// </summary>
    [Export] private Godot.Collections.Array<FocusRoute> nodeRoutes;

    /// <summary>
    /// The collision mask on which to check for route activators.
    /// </summary>
    [Export(PropertyHint.Layers2DPhysics)]
    private uint collisionMask = uint.MaxValue;

    /// <summary>
    /// The maximum number of collisions to detect when searching for route activators.
    /// Setting this too high can cause major performance issues.
    /// </summary>
    [Export] private int maxCollisions = 1028;

    /// <inheritdoc />
    public override void _Input(InputEvent @event) {
        if (!this._isFocused) return;
        if (@event is not InputEventMouseMotion mouseEvent) return;

        List<CollisionObject2D> collisionObjects = this.PollPosition(this.GetViewport().GetMousePosition());
        CollisionObject2D topRouteKey = collisionObjects.FirstOrDefault(c => this._routes.ContainsKey(c));

        // unfocus every route endpoint except for the topRoute's (unless it has HoldFocus set)
        // (being mindful that the Unfocus function may have side effects, so only calling it if necessary)
        foreach (CollisionObject2D key in this._routes.Keys.Where(key => key != topRouteKey)) {
            if (this._routes[key].IsFocused && !this._routes[key].HoldFocus) this._routes[key].Unfocus();
            this._routes[key].FocusLocked = false;
        }

        // focus the topRoute's endpoint (as long as no route endpoint is requesting sole focus)
        // (again, being mindful of possible side effects)
        if (!this._soleFocus && topRouteKey != null && !this._routes[topRouteKey].IsFocused) {
            this._routes[topRouteKey].Focus();
            this._routes[topRouteKey].FocusLocked = true;
        }
    }

    /// <inheritdoc />
    public override void _Ready() {
        foreach (FocusRoute focusRoute in this.nodeRoutes) {
            if (this.GetNode<Node>(focusRoute.endpointNodePath) is IFocusable endpoint)
                this.RegisterRoute(this.GetNode<CollisionObject2D>(focusRoute.activatorPath), endpoint);
            else GD.PushWarning(string.Format(FocusRouter.ROUTE_NOT_FOCUSABLE_WARNING_TEMPLATE, this.GetNode<Node>(focusRoute.endpointNodePath)));
        }
    }

    /// <summary>
    /// Update this.HoldFocus and this.SoleFocus based on the newly updated focusable's state.
    /// </summary>
    private void OnRouteFocusableStateChanged(IFocusable context) {
        this.UpdateHoldFocus(context);
        this.UpdateSoleFocus(context);
    }

    /// <summary>
    /// Register a route.
    /// </summary>
    /// <param name="activator">The CollisionObject2D which will trigger the route.</param>
    /// <param name="endpoint">The IFocusable which will be focused when the route is triggered.</param>
    public void RegisterRoute(CollisionObject2D activator, IFocusable endpoint) {
        this._routes.Add(activator, endpoint);
        endpoint.OnFocusChanged += this.OnRouteFocusableStateChanged;
        endpoint.OnHoldFocusChanged += this.OnRouteFocusableStateChanged;
        endpoint.OnSoleFocusChanged += this.OnRouteFocusableStateChanged;
    }

    /// <summary>
    /// Remove focus lock from all routes.
    /// </summary>
    private void RemoveAllFocusLocks() {
        foreach (IFocusable endpoint in this._routes.Values) endpoint.FocusLocked = false;
    }

    /// <summary>
    /// Update the state of this.HoldFocus based on the state of the given focusable.
    /// NOTE: setting to true only depends on the state of the given focusable,
    /// but setting to false depends on the state of all routes' focusables.
    /// </summary>
    /// <param name="context">The most recently updated focusable (of this's routes).</param>
    private void UpdateHoldFocus(IFocusable context) {
        if (context.IsFocused && context.HoldFocus) {
            this._holdFocus = true;
            this.OnHoldFocusChanged?.Invoke(this);
        } else if (!this._routes.Values.Any(focusable => focusable.IsFocused && focusable.HoldFocus)) {
            this._holdFocus = false;
            this.OnHoldFocusChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// Update the state of this.SoleFocus based on the state of the given focusable.
    /// NOTE: setting to true only depends on the state of the given focusable,
    /// but setting to false depends on the state of all routes' focusables.
    /// </summary>
    /// <param name="context">The most recently updated focusable (of this's routes).</param>
    private void UpdateSoleFocus(IFocusable context) {
        if (context.IsFocused && context.SoleFocus) {
            this._soleFocus = true;
            this.OnSoleFocusChanged?.Invoke(this);
        } else if (!this._routes.Values.Any(focusable => focusable.IsFocused && focusable.SoleFocus)) {
            this._soleFocus = false;
            this.OnSoleFocusChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// Poll a position for all CollisionObject2Ds at the position, ordered by ZIndex.
    /// </summary>
    /// <param name="pos">The position to poll.</param>
    /// <returns>A list of CollisionObject2Ds found at the given position, ordered by ZIndex.</returns>
    private List<CollisionObject2D> PollPosition(Vector2 pos) {
        PhysicsPointQueryParameters2D query = new PhysicsPointQueryParameters2D();
        query.Position = pos;
        query.CollisionMask = this.collisionMask;
        query.CollideWithAreas = true;
        query.CollideWithBodies = true;

        // find all collisions at the given point
        Godot.Collections.Array<Godot.Collections.Dictionary> collisions = this.GetWorld2D().DirectSpaceState.IntersectPoint(query, this.maxCollisions);

        // return the colliders ordered by ZIndex
        return collisions
            .Select(c => c["collider"].AsGodotObject() as CollisionObject2D)
            .OrderByDescending(c => FocusRouter.GetAbsoluteZIndex(c))
            .ToList();
    }

    /// <summary>
    /// Calculate a CanvasItem's absolute ZIndex by walking up the tree
    /// and adding ZIndices until a non-CanvasItem or non-ZAsRelative node is encountered.
    /// </summary>
    public static int GetAbsoluteZIndex(CanvasItem canvasItem) {
        int z = 0;
        CanvasItem? current = canvasItem;

        while (current != null) {
            z += current.ZIndex;
            if (!current.ZAsRelative) break;
            current = current.GetParent() as CanvasItem;
        }

        return z;
    }
}

using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// An arranger for a Deck of Cards.
/// Handles moving cards to thier respective locations.
/// The cards in the library are moved to the library marker,
/// and the cards in the graveyard are moved to the graveyard marker.
/// Automatically arranges the hand along the curve of the hand path.
/// Also attempts to order the cards along the Z-axis.
///
/// Cards may be added as children of the DeckArranger directly,
/// or they may be wrapped in one or more other nodes,
/// but it is expected that all cards's nearest common ancestor is the DeckArranger
/// (ie you should not have a single node with multiple card children as a child of this).
///
/// If the card is wrapped in a DragGroup, this will not attempt to move the card while it is being dragged.
/// It will also move the card to the front of the Z-axis, so it appears above the other cards.
///
/// NOTE: this class does not enable/disable DragGroups, so if you want cards which are in the library or graveyard
/// to not be draggable, while cards in the hand are, that logic must be performed elsewhere.
/// </summary>
public partial class DeckArranger<T> : Node where T : Card {
    private const string CARD_NOT_DESCENDANT_WARNING_TEMPLATE = "Card ({0}) is not a descendant of the DeckArranger. The DeckArranger is unable to arrange cards which are not its children.";
    private const string MULTIPLE_CARD_DESCENDANTS_WARNING_TEMPLATE = "The added child node ({0}) has multiple Card descendants. ZIndex ordering may not function properly when the DeckArranger is not the Cards' nearest common ancestor.";
    private const string NO_CARD_DESCENDANT_ADDED_WARNING_TEMPLATE = "The added child node ({0}) does not contain a Card descendant. Typically, children of the DeckArranger are Cards or Node2D wrappers around a Card. If {0} is not supposed to be a card, this warning can be safely ignored.";
    private const string NON_NODE2D_ADDED_WARNING_TEMPLATE = "The added child node ({0}) is not a Node2D. Typically, children of the DeckArranger are Cards or Node2D wrappers around a Card. If {0} is not supposed to be a card, this warning can be safely ignored.";

    [Export] bool warnOnNoCardDescendantAdded = true;
    [Export] bool warnOnNonNode2DAdded = true;

    /// <summary>
    /// Cards which are found in this._deck and have been attempted to be moved,
    /// but which are not descendants of this. Warnings will be issued
    /// only the first time a non-descendant card is attempted to be moved.
    /// </summary>
    private HashSet<T> nonDescendantCards = new HashSet<T>();

    [Export] private Marker2D libraryMarker;
    [Export] private Path2D handPath;
    [Export] private Marker2D graveyardMarker;

    [Export] private float maxStep;

    /// <summary>
    /// The absolute spacing between cards in the hand.
    /// </summary>
    /// <value>The spacing. Values less than the width of a card will cause the cards to overlap in the hand.</value>
    [Export] private float defaultHandSpacing;

    private Vector2[] handPathCurvePoints;

    private Deck<T> _deck;
    public Deck<T> Deck { set { this._deck = value; } }

    /// <summary>
    /// A mapping of cards (which may or may not be currently in this._deck)
    /// -> the immediate child of this which contains the card.
    /// </summary>
    private Dictionary<T, Node2D> cardRoots = new Dictionary<T, Node2D>();

    /// <inheritdoc />
    public override void _Process(double delta) {
        // XXX each of these takes a snapshot of their respective zone
        //     I am very worried that will get expensive fast
        this.ArrangeLibrary();
        this.ArrangeHand();
        this.ArrangeGraveyard();
    }

    /// <inheritdoc />
    public override void _Ready() {
        // process each child (which has already been added to this manually, in-editor)
        foreach (Node child in this.GetChildren()) this.ProcessChild(child);
    }

    /// <inheritdoc />
    public new void AddChild(Node child, bool forceReadableName = false, InternalMode internalMode = InternalMode.Disabled) {
        base.AddChild(child, forceReadableName, internalMode);

        this.ProcessChild(child);
    }

    /// <summary>
    /// Move all cards in the library towards this.libraryMarker.Position.
    /// </summary>
    private void ArrangeLibrary() {
        if (this._deck == null) return;

        T[] library = this._deck.LibrarySnapshot.ToArray();

        for (int i = 0; i < library.Length; i++) {
            if (this.cardRoots.TryGetValue(library[i], out Node2D cardRoot)) {
                // if the card is being focused or dragged, move it above the rest of the cards
                if (library[i] is IDraggable draggable && draggable.IsBeingDragged ||
                    library[i] is IFocusable focusable && focusable.IsFocused)
                    cardRoot.ZIndex = this.handPathCurvePoints.Length;
                else {
                    cardRoot.Position = cardRoot.Position.MoveToward(this.libraryMarker.Position, this.maxStep);
                    cardRoot.ZAsRelative = true;
                    cardRoot.ZIndex = -1 * i - 1;  // the top of the stack is the 0th card, -1 to place this below the hand (where ZIndices start at 0 and ascend)
                }
            } else if (this.nonDescendantCards.Add(library[i])) GD.PushWarning(string.Format(DeckArranger<T>.CARD_NOT_DESCENDANT_WARNING_TEMPLATE, library[i]));
        }
    }

    /// <summary>
    /// Move all cards in the hand towards their position on this.handPath.Curve.
    /// </summary>
    private void ArrangeHand() {
        if (this._deck == null) return;

        T[] hand = this._deck.HandSnapshot.ToArray();

        if (this.handPathCurvePoints == null || this.handPathCurvePoints.Length != hand.Length)
            this.CalculateHandCurvePoints();

        for (int i = 0; i < hand.Length; i++) {
            if (this.cardRoots.TryGetValue(hand[i], out Node2D cardRoot)) {
                // if the card is being dragged, move it above the rest of the cards
                if (hand[i] is IDraggable draggable && draggable.IsBeingDragged ||
                    hand[i] is IFocusable focusable && focusable.IsFocused)
                    cardRoot.ZIndex = this.handPathCurvePoints.Length;
                else {
                    cardRoot.Position = cardRoot.Position.MoveToward(this.handPathCurvePoints[i], this.maxStep);
                    cardRoot.ZAsRelative = true;
                    cardRoot.ZIndex = i;  // to place cards on the right on top
                }
            } else if (this.nonDescendantCards.Add(hand[i])) GD.PushWarning(string.Format(DeckArranger<T>.CARD_NOT_DESCENDANT_WARNING_TEMPLATE, hand[i]));
        }

    }

    /// <summary>
    /// Move all cards in the graveyard towards this.graveyardMarker.Position.
    /// </summary>
    private void ArrangeGraveyard() {
        if (this._deck == null) return;

        T[] graveyard = this._deck.GraveyardSnapshot.ToArray();

        for (int i = 0; i < graveyard.Length; i++) {
            if (this.cardRoots.TryGetValue(graveyard[i], out Node2D cardRoot)) {
                // if the card is being dragged, move it above the rest of the cards
                if (graveyard[i] is IDraggable draggable && draggable.IsBeingDragged ||
                    graveyard[i] is IFocusable focusable && focusable.IsFocused)
                    cardRoot.ZIndex = this.handPathCurvePoints.Length;
                else {
                    cardRoot.Position = cardRoot.Position.MoveToward(this.graveyardMarker.Position, this.maxStep);
                    cardRoot.ZAsRelative = true;
                    cardRoot.ZIndex = -1 * i - 1;  // the top of the stack is the 0th card, -1 to place this below the hand (where ZIndices start at 0 and ascend)
                }
            } else if (this.nonDescendantCards.Add(graveyard[i])) GD.PushWarning(string.Format(DeckArranger<T>.CARD_NOT_DESCENDANT_WARNING_TEMPLATE, graveyard[i]));
        }
    }

    /// <summary>
    /// Calculate this.handPathCurvePoints, centering the points on this.handPath and shrinking the spacing if necessary.
    /// </summary>
    private void CalculateHandCurvePoints() {
        this.handPathCurvePoints = new Vector2[this._deck.HandSnapshot.Count];

        float spacing;
        float offset;

        // if the cards will not use the full curve when placed at the default spacing
        if (this.defaultHandSpacing * this.handPathCurvePoints.Length < this.handPath.Curve.GetBakedLength()) {
            // use the default spacing
            spacing = this.defaultHandSpacing;

            // set the offset to half the difference between the used and total Lengths (centering the points)
            offset = (this.handPath.Curve.GetBakedLength() - (this.defaultHandSpacing * (this.handPathCurvePoints.Length - 1))) / 2.0f;
        } else {  // else the cards would overflow the full curve when placed at the default spacing
            // set the spacing to the total Length divided by the number of cards, evenly spacing the cards along the total Length
            spacing = this.handPath.Curve.GetBakedLength() / this.handPathCurvePoints.Length;

            // don't use an offset
            offset = 0.0f;
        }

        for (int i = 0; i < this.handPathCurvePoints.Length; i++)
            this.handPathCurvePoints[i] = this.handPath.Position + this.handPath.Curve.SampleBaked((i * spacing) + offset);
    }

    /// <summary>
    /// Process a child of this, capturing its rootChild.
    /// </summary>
    /// <param name="child">The child to process.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="child" /> is not a child of this.</exception>
    private void ProcessChild(Node child) {
        if (child.GetParent() != this) throw new ArgumentException(message: $"{child} is not a child of this.");

        // this.libraryMarker, this.handPath, and this.graveyardMarker are not required to be children of this,
        // but it is expected that they usually will be -- return early so a warning is not raised
        if (child == this.libraryMarker || child == this.handPath || child == this.graveyardMarker) return;

        if (child is Node2D node2D) {
            // find Card and DragGroup descendants
            List<T> cards = new List<T>();
            Queue<Node> nodeQueue = new Queue<Node>(new Node[] { node2D });
            while (nodeQueue.TryDequeue(out Node node)) {
                if (node is T card) cards.Add(card);
                foreach (Node nodeChild in node.GetChildren()) nodeQueue.Enqueue(nodeChild);
            }

            // if there are multiple Card descendants, warn the user that ordering may not work properly
            if (cards.Count > 1) GD.PushWarning(string.Format(DeckArranger<T>.MULTIPLE_CARD_DESCENDANTS_WARNING_TEMPLATE, node2D));

            // if there is at least one Card descendant, add them to the card root map
            if (cards.Count > 0) foreach (T card in cards) this.cardRoots[card] = node2D;
            // else there are no Card descendants, warn the user that they probably intended to add a Card descendant
            else if (this.warnOnNoCardDescendantAdded) GD.PushWarning(string.Format(DeckArranger<T>.NO_CARD_DESCENDANT_ADDED_WARNING_TEMPLATE, node2D));

            // set the initial position of the newly added Node2D to this.libraryMarker.Position
            node2D.Position = this.libraryMarker.Position;
        }
        // else the new child is not a Node2D, warn the user that they probably intended to add a Node2D descendant
        else if (this.warnOnNonNode2DAdded) GD.PushWarning(string.Format(DeckArranger<T>.NON_NODE2D_ADDED_WARNING_TEMPLATE, child));
    }
}

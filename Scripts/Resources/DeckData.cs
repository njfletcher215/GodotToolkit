using Godot;
using System;

/// <summary>
/// A deck of cards.
/// </summary>
public partial class DeckData : Resource {
    /// <summary>
    /// The array of cards in the deck. If multiple copies of the same card are required, include its ID multiple times in the array.
    /// </summary>
    [Export] private string[] _cardIds;

    public string[] CardIds { get => this._cardIds; }
}

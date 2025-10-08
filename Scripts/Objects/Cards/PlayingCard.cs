using Godot;
using System;

/// <summary>
/// A playing card (for a standard 52-card deck).
/// NOTE: there is currently no system to automatically determine the correct face from the suit and rank.
/// </summary>
public partial class PlayingCard : Card {
    // ordered for bridge
    public enum FrenchSuit {
        CLUBS,
        DIAMONDS,
        HEARTS,
        SPADES
    }

    // ordered for bridge
    // NOTE: indices do NOT correspond to the number on the card
    public enum BridgeRank {
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING,
        ACE
    }

    [Export] protected PlayingCard.FrenchSuit _suit;
    [Export] protected PlayingCard.BridgeRank _rank;

    public PlayingCard.FrenchSuit Suit { get => this._suit; }
    public PlayingCard.BridgeRank Rank { get => this._rank; }

    public PlayingCard() : base() {}

    public PlayingCard(Node2D face, Node2D back, PlayingCard.FrenchSuit suit, PlayingCard.BridgeRank rank) : base(face, back) {
        this._suit = suit;
        this._rank = rank;
    }
}

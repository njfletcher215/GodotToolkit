using Godot;
using System;

/// <summary>
/// A very basic card. Consists of a front face and a back face. Can be flipped to reveal the front or back.
/// </summary>
public partial class Card : Node2D {
    [Export] protected Node2D face;
    [Export] protected Node2D back;

    public Card() {}

    public Card(Node2D face, Node2D back) {
        this.face = face;
        this.back = back;
    }

    /// <summary>
    /// Flip the card.
    /// </summary>
    public void Flip() {
        this.face.Visible = !this.face.Visible;
        this.back.Visible = !this.face.Visible;
    }

    /// <summary>
    /// Flip the card to face up.
    /// </summary>
    public void SetFaceDown() {
        this.face.Visible = false;
        this.back.Visible = true;
    }

    /// <summary>
    /// Flip the card to face down.
    /// </summary>
    public void SetFaceUp() {
        this.face.Visible = true;
        this.back.Visible = false;
    }
}

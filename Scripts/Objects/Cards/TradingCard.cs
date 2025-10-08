using Godot;
using System;

/// <summary>
/// A generic trading card.
/// </summary>
public partial class TradingCard : Card {
    // NOTE: typically, these will be children of `this.face`
    [Export] protected SimpleFormatStringLabel titleLabel;
    [Export] protected SimpleFormatStringLabel descriptionLabel;
    [Export] protected Sprite2D artwork;
    [Export] protected Sprite2D template;

    protected string _title;
    protected string _description;
    protected string _artworkPath;
    protected string _templatePath;

    public string Title { get => this._title; set { this._title = value; this.titleLabel.SetValue("title", this._title); } }

    protected string Description { get => this._description; set { this._description = value; this.descriptionLabel.SetValue("description", this._description); } }
    protected string ArtworkPath { get => this._artworkPath; set { this._artworkPath = value; this.artwork.Texture = ResourceLoader.Load<Texture2D>(Constants.Manifest.Assets.CARD_ARTWORK_DIRECTORY_PATH + this._artworkPath); } }
    protected string TemplatePath { get => this._templatePath; set { this._templatePath = value; this.template.Texture = ResourceLoader.Load<Texture2D>(Constants.Manifest.Assets.CARD_ARTWORK_DIRECTORY_PATH + this._templatePath); } }

    public TradingCard() : base() {}

    public TradingCard(Node2D face, Node2D back, string title, string description, string artworkPath, string templatePath) : base(face, back) {
        this.Title = title;
        this.Description = description;
        this.ArtworkPath = artworkPath;
        this.TemplatePath = templatePath;
    }
}

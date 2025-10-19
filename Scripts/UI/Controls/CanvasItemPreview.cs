using Godot;
using System;

/// <summary>
/// Preview of a canvas item (and each of its child canvas items), displayed as a texture.
/// <summary>
[Tool]
public partial class CanvasItemPreview : TextureRect {
    /// <summary>
    /// Preview the given canvas item.
    /// </summary>
    /// <param name="previewRoot">The canvas item to be previewed.</param>
    /// <param name="loadTimeSeconds">The amount of time to wait for the texture to properly load before destroying the previewRoot.</param>
    public void Preview(CanvasItem previewRoot, double loadTimeSeconds = 1.0) {
        CallDeferred(nameof(CaptureAndApplyTexture), previewRoot, loadTimeSeconds);
    }

    /// <summary>
    /// Preview the given canvas item.
    /// </summary>
    /// <param name="previewRootScene">The scene with the canvas item to be previewed as its root.</param>
    /// <param name="loadTimeSeconds">The amount of time to wait for the texture to properly load before destroying the previewRoot.</param>
    public void Preview(PackedScene previewRootScene, double loadTimeSeconds = 1.0) {
        this.Preview(previewRootScene.Instantiate<CanvasItem>(), loadTimeSeconds);
    }

    /// <summary>
    /// Generate and apply the preview texture. Must be done during the game loop.
    /// </summary>
    /// <param name="previewRoot">The canvas item to be captured.</param>
    /// <param name="loadTimeSeconds">The amount of time to wait for the texture to properly load before destroying the previewRoot.</param>
    private void CaptureAndApplyTexture(CanvasItem previewRoot, double loadTimeSeconds) {
        var viewport = new SubViewport {
            Size = (Godot.Vector2I)this.Size,
            TransparentBg = true,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Once,
            RenderTargetClearMode = SubViewport.ClearMode.Always
        };

        this.AddChild(viewport);
        viewport.AddChild(previewRoot);

        // Scale and center
        Rect2? visibleCanvasItemBounds = CanvasItemPreview.GetVisibleCanvasItemBounds(previewRoot);
        if (visibleCanvasItemBounds is Rect2 bounds) {
            GD.Print($"bounds: {visibleCanvasItemBounds}");
            Vector2 scale = this.Size / bounds.Size;
            float uniformScale = Mathf.Min(scale.X, scale.Y);
            Transform2D transform = Transform2D.Identity;
            transform = transform.Scaled(Vector2.One * uniformScale);
            transform.Origin = (this.Size / 2) - ((bounds.Size * uniformScale) / 2) - (bounds.Position * uniformScale);
            previewRoot.Set("transform", transform);

            this.Texture = viewport.GetTexture();

            // after a short delay (to make sure the texture renders properly),
            // cleanup the previewRoot to prevent slowdown
            this.DestroyCanvasItem(previewRoot, loadTimeSeconds);
        } else GD.Print($"{previewRoot} has no visible canvas items to display.");
    }

    /// <summary>
    /// Destroy the given canvas item after the given delay.
    /// </summary>
    /// <param name="canvasItem">The canvas item to be destroyed.</param>
    /// <param name="delay">The delay (in seconds) before destroying the canvas item.</param>
    private async void DestroyCanvasItem(CanvasItem canvasItem, double delay) {
        await ToSignal(this.GetTree().CreateTimer(delay), SceneTreeTimer.SignalName.Timeout);
        canvasItem.QueueFree();
    }

    /// <summary>
    /// Calculate the bounds for a canvas item (including its children).
    /// </summary>
    /// <param name="canvasItem">The canvas item for which the bounds will be calculated.</param>
    /// <returns>The bounds for the canvas item, or null if there are no visible canvas items.</returns>
    private static Rect2? GetVisibleCanvasItemBounds(CanvasItem canvasItem) {
        if (!canvasItem.Visible) return null; // ignore invisible canvasItems

        Rect2? bounds = null;

        if (canvasItem is Sprite2D sprite && sprite.Texture != null) {
                Vector2 size = sprite.Texture.GetSize() * sprite.Scale;
                Vector2 offset = sprite.Centered ? size / 2 : Vector2.Zero;
                bounds = new Rect2(canvasItem.GetGlobalTransform().Origin - offset, size);
        }
        else if (canvasItem is AnimatedSprite2D anim && anim.SpriteFrames != null) {
            Texture2D frameTex = anim.SpriteFrames.GetFrameTexture(anim.Animation, 0);
            if (frameTex != null) {
                Vector2 size = frameTex.GetSize() * anim.Scale;
                Vector2 offset = anim.Centered ? size / 2 : Vector2.Zero;
                bounds = new Rect2(canvasItem.GetGlobalTransform().Origin - offset, size);
            }
        }
        else if (canvasItem is Polygon2D poly) {
            bounds = poly.Polygon.Length > 0 ? new Rect2(canvasItem.GetGlobalTransform().Origin + poly.Polygon[0], Vector2.Zero) : null;
            foreach (Vector2 pt in poly.Polygon)
                bounds = bounds?.Expand(canvasItem.GetGlobalTransform().Origin + pt);
        }
        else if (canvasItem is Line2D line && line.GetPointCount() > 0) {
            bounds = new Rect2(canvasItem.GetGlobalTransform().Origin + line.GetPointPosition(0), Vector2.Zero);
            for (int i = 1; i < line.GetPointCount(); i++)
                bounds = bounds?.Expand(canvasItem.GetGlobalTransform().Origin + line.GetPointPosition(i));
        }
        else if (canvasItem is TileMapLayer tilemapLayer) {
            Rect2 usedRect = tilemapLayer.GetUsedRect();
            Vector2 tileSize = tilemapLayer.TileSet.TileSize;
            bounds = new Rect2(
                canvasItem.GetGlobalTransform().Origin + (Vector2)(usedRect.Position * tileSize),
                (Vector2)(usedRect.Size * tileSize)
            );
        }
        else if (canvasItem is Control ctrl) {
            bounds = new Rect2(canvasItem.GetGlobalTransform().Origin + ctrl.Position, ctrl.Size);
        }

        // recursively get the bounds of each child and merge them into bounds
        foreach (CanvasItem child in canvasItem.GetChildren()) {
            Rect2? childBounds = CanvasItemPreview.GetVisibleCanvasItemBounds(child);
            if (bounds == null) bounds = childBounds;
            else if (childBounds != null) bounds = bounds.Value.Merge(childBounds.Value);
        }

        return bounds;
    }

}

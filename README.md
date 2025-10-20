# GodotToolkit

Scenes and scripts of generic elements for Godot projects.

## Installation

Clone the repo into `res://addons/` (these are simply library files, so you can place them anywhere in your project folder -- but `addons/GodotToolkit` is the recommended location; some scenes may break if a non-default folder name is used).
```bash
# from your project's root directory
git clone git@github.com:njfletcher215/GodotToolkit.git addons/GodotToolkit/
```

## Draggables
A collection of classes enabling click-and-drag movement behavior for arbitrary components.

### DragGroup/DragHandle
A wrapper which allows itself (and by extension all descendants) to be dragged by any descendant DragHandles, and the handle by which the DragGroup can be dragged (using the mouse).

#### Usage
Wrap any node(s) you wish to make draggable in a DragGroup, and add one or more descendant DragHandles. Clicking and dragging on any of the DragHandles will update the position of the DragGroup (and by extension all descendants).

### IDraggable
A draggable node, i.e. a node wrapped in a DragGroup. Technically, there is no requirement that the node be *wrapped* in a DragGroup (or even be a node at all), it simply must be able to *reference* a DragGroup.

#### Usage
Implement the DragGroup property. Typically, this means returning this.GetParent() (assuming the implementer is a node and a direct child of the DragGroup). The implementer can then be cast as an IDraggable to expose the drag control methods (EnableDragging, DisableDragging, and ToggleDragging).

## Focusables
A collection of classes enabling the passing of "focus" to one or more components.

### AbstractFocusable
A base implementation of IFocusable. Most implementers of IFocusable should implement it in the same way, but if another base class is required the implementation will have to be copied. See [`AbstractFocusableNode`](#AbstractFocusableNode) and [`AbstractFocusableNode2D`](#AbstractFocusableNode2D) for base implementations that extend Node and Node2D.

### AbstractFocusableNode
**Inherits**: `Node`
A base implementation of IFocusable. Most implementers of IFocusable should implement it in the same way, but if a base class beyond Node is required the implementation will have to be copied. See [`AbstractFocusableNode2D`](#AbstractFocusableNode2D) for a base implementation that extends Node2D, or [`AbstractFocusable`](#AbstractFocusable) for a base implementation with no base class.

### AbstractFocusableNode2D
A base implementation of IFocusable. Most implementers of IFocusable should implement it in the same way, but if a base class beyond Node2D is required the implementation will have to be copied. See [`AbstractFocusableNode`](#AbstractFocusableNode`) for a base implementation that extends Node, or [`AbstractFocusable`](#AbstractFocusable) for a base implementation with no base class.

### FocusableDragGroup
**Inherits**: [`DragGroup`](#DragGroup/DragHandle)
A focusable DragGroup.

#### Usage
See [`DragGroup`](#DragGroup/DragHandle). Additionally, the DragGroup will only be draggable while it is focused. Note that the DragGroup will not be *enabled*/*disabled* when it is focused/unfocused, rather it will only allow itself to be dragged when it is both enabled and focused.

### FocusRoute
A route for the FocusRouter. Consists of an activator and an endpoint. Note that a FocusRoute is a *Node* route, which means the endpoint is a Node which is expected at runtime to implement IFocusable, since IFocusables cannot be directly serialized.

#### Usage
It is not expected that routes be directly saved to resource files. Rather, this class allows the FocusRouter to serialize its NodeRoutes while remaining as strongly-typed as possible.

### FocusRouter
A router that focuses its route endpoints when their respective activators are hovered by the mouse. Note that the FocusRouter is itself an IFocusable, so FocusRouters may be nested.

#### Usage
Attach this script to a Node2D and add one or more routes. Node routes (routes to a Node which is expected at runtime to implement IFocusable) can be serialized via the `nodeRoutes` property. Routes may also be added at runtime -- non-Node routes *must* be added at runtime.

### IFocusable
A focusable.

#### Usage
The effect(s) of being focused are up to the implementer. Typically, focusables will enable/disable input in some way depending on their focused state.

## Additional Provided Classes

### CanvasItemPreview
A preview of a canvas item (and each of its child canvas items), displayed as a texture.

#### Usage
Attach this script to a TextureRect you want to preview the canvas item on. Call the CanvasItemPreview's `Preview(CanvasItem previewRoot, double loadTimeSeconds = 1.0)` (or `Preview(PackedScene previewRootScene, double loadTimeSeconds = 1.0)`) method preview the given CanvasItem (or PackedScene containing a CanvasItem) as a texture on the CanvasItemPreview.

As part of the process to capture the texture, the CanvasItem must be loaded/instantiated temporarily; it is destroyed after loadTimeSeconds to save on resource usage. If the preview is not what you expect, ensure loadTimeSeconds is long enough for the CanvasItem to fully load.

### Card
A basic card, with a face and a back. Can be flipped to reveal each side.

#### Usage
Attach this script to a Node2D. Set the `face` and `back` Node2Ds, with any components of the "face" set as descendants of `face` and any components of the "back" as descendants of `back`. Typically, either the face node or the back node should then be set to `Visible = false`, depending on which side you wish to show by default.

### DeckArranger/CardArranger
An arranger for a deck of cards. Automatically arranges the library, hand, and graveyard of a deck in physical space. This includes the cards' 2D coordinates as well as their z indices.

#### Usage
The DeckArranger script cannot be attached to a node directly, as it is a generic type. Create a closed generic using the same type parameter as the deck you wish to arrange, and attach that to a node. If your deck is simply a `Deck<Card>`, you can use the CardArranger class.

Create two Marker2Ds, one to mark the library position and one to mark the graveyard position. Create one Path2D to mark the hand path, and set the curve (typically, this will be to a straight line or a simple arc).

When you initialize the deck you want to track, set the DeckArranger's `Deck` property to reference it (do NOT set the `Deck` property to a *new* deck -- the deck arranger does not provide any read access to the deck after it is set). Any cards in the deck should be descendants of the deck arranger -- they do not have to be direct children, as long as each child of the deck arranger has at most 1 card child (so you can, for example, wrap the cards in DragGroups and make the DragGroups children of the deck arranger).

The cards in the deck's library will now be moved toward the `libraryMarker`'s position each frame. Same for the deck's graveyard (which will be moved to the `graveyardMarker`'s position) and its hand (which will be spread across the `handPath`'s curve). They will also be ordered on the Z axis. If the cards implement IDraggable, they will not be moved by the deck arranger while they are being dragged (they will, however, be brought to the front of the Z axis). If the cards implement IFocusable, they will also be brought to the front of the Z axis while focused.

### EnumDropdown
An OptionButton with a specific enum as its options.

#### Usage
Attach this script to an OptionButton. Then, just set its `enumTypeName` to the assembly name of the enum, ex. "MyNamespace+MyEnum, myassembly".

### HLabeledSlider/VLabeledSlider
A slider with labels for its title, current value, and ticks.

#### Usage
Attach this script to an HSlider (if using HLabeledSlider) or VSlider (if using VLabeledSlider). Create a `SimpleFormatStringLabel` for the current value if you want to display it. This must be configured with a format string containing the key `currentValue`: ex. `My Option: {currentValue}`.

### OverlayMenu/OverlayMenuHost
A menu which is overlayed on another screen, and the screen on which it is overlayed.

#### Usage
Create your overlay menus (as .tscns). Make the root node of the scene a Node2D, and attach the OverlayMenu script to it. Then, attach a script which extends OverlayMenuHost to your desired host (usually the scene or menu over which the OverlayMenu is to be shown).

Each event-generating control in the menu should send its signals to the OverlayMenu's `OnEvent(string eventName, bool close)` -- with `eventName` being the name of the event (in the format MenuName_ButtonName, ex. "RoundPass_Continue", by convention), and `close` being whether the event should close the overlay menu.

The OverlayMenuHost should define an `IDictionary<string, Action> overlayMenuEventHandlers` which maps these `eventName`s to actual methods (again, in the format OnMenuName_ButtonName, ex. "OnRoundPass_Continue", by convention). Passing parameters to these methods is not currently supported.

### PlayingCard
**Inherits**: [`Card`](#card)
A card from a standard 52-card deck. Has `Suit` and `Rank` values.

#### Usage
See [`Card`](#card). The exposed enums `FrenchSuit` and `BridgeRank` can be used to perform comparisons between cards -- the suits and ranks are ordered according to the rules of bridge. Note that the index values of `BridgeRank` do not match the number on the card: for example, `BridgeRank.TWO` is at index 0, not 2, and `BridgeRank.JACK` is at index 9, not 11.

### SimpleFormatStringLabel
A simple label that displays arbitrary variables injected into a format string.

#### Usage
Attach this script to a Label. Then, set its `formatString`. To set/update a key, use the SimpleFormatStringLabel's `SetValue(string key, object val = null)` method.

`formatString` is based on a standard C# format string, but there are some differences. If a key is not set, or its value is `null`, "null" will be printed. If a value is IEnumerable (and not a string), it will be printed as a comma-separated list of the string values of its items (this is *not* done recursively).

### TradingCard
**Inherits**: [`Card`](#card)
A generic trading card.

#### Usage
See [`Card`](#card). Create SimpleFormatStringLabels for the `titleLabel` (containing the key 'title') and `descriptionLabel` (containing the key 'description'). Create Sprite2Ds for the `artwork` and `template`. The artwork and template textures will be automatically loaded upon setting the `ArtworkPath` and `TemplatePath` properties.

## Additional Provided Resources

### DeckData
Data for a deck of cards.

#### Usage
Add each card you want in the deck to the Card Ids array. If multiple copies of the same card are required, you can include its ID multiple times. There is currently to built-in mechanism to instantiate a card from a card id, that implementation is up to the user.

## Questions or Issues?

Feel free to open an issue on GitHub if something's not working or not clear.
Alternatively, you can contact the developer at njfletcher215@gmail.com


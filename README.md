# GodotToolkit

Scenes and scripts of generic elements for Godot projects.

## Installation

Clone the repo into `res://addons/` (these are simply library files, so you can place them anywhere in your project folder -- but `addons/GodotToolkit` is the recommended location; some scenes may break if a non-default folder name is used).
```bash
# from your project's root directory
git clone git@github.com:njfletcher215/GodotToolkit.git addons/GodotToolkit/
```

## Provided Classes

### CanvasItemPreview
A preview of a canvas item (and each of its child canvas items), displayed as a texture.

#### Usage
Attach this script to a TextureRect you want to preview the canvas item on. Call the CanvasItemPreview's `Preview(CanvasItem previewRoot, double loadTimeSeconds = 1.0)` (or `Preview(PackedScene previewRootScene, double loadTimeSeconds = 1.0)`) method preview the given CanvasItem (or PackedScene containing a CanvasItem) as a texture on the CanvasItemPreview.

As part of the process to capture the texture, the CanvasItem must be loaded/instantiated temporarily; it is destroyed after loadTimeSeconds to save on resource usage. If the preview is not what you expect, ensure loadTimeSeconds is long enough for the CanvasItem to fully load.

### Card
A basic card, with a face and a back. Can be flipped to reveal each side.

#### Usage
Attach this script to a Node2D. Set the `face` and `back` Node2Ds, with any components of the "face" set as descendants of `face` and any components of the "back" as descendants of `back`. Typically, either the face node or the back node should then be set to `Visible = false`, depending on which side you wish to show by default.

### DragGroup/DragHandle
A wrapper which allows itself (and by extension all descendants) to be dragged by any descendant DragHandles, and the handle by which the DragGroup can be dragged (using the mouse).

#### Usage
Wrap any node(s) you wish to make draggable in a DragGroup, and add one or more descendant DragHandles. Clicking and dragging on any of the DragHandles will update the position of the DragGroup (and by extension all descendants).

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

// TODO I just added the stuff for Scripts/Objects/Cards/*
        I need to go down the list for the rest of the classes and add or update them here

## Provided Interfaces

### IDraggable
A draggable node, i.e. a node wrapped in a DragGroup. Technically, there is no requirement that the node be *wrapped* in a DragGroup (or even be a node at all), it simply must be able to *reference* a DragGroup.

#### Usage
Implement the DragGroup property. Typically, this means returning this.GetParent() (assuming the implementer is a node and a direct child of the DragGroup). The implementer can then be cast as an IDraggable to expose the drag control methods (EnableDragging, DisableDragging, and ToggleDragging).

## Provided Resources

### DeckData
Data for a deck of cards.

#### Usage
Add each card you want in the deck to the Card Ids array. If multiple copies of the same card are required, you can include its ID multiple times. There is currently to built-in mechanism to instantiate a card from a card id, that implementation is up to the user.

## Questions or Issues?

Feel free to open an issue on GitHub if something's not working or not clear.
Alternatively, you can contact the developer at njfletcher215@gmail.com


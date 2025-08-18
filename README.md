# CommonGodotUI

Scenes and scripts of generic UI elements for Godot projects.

## Installation

Clone the repo into `res://addons/` (these are simply library files, so you can place them anywhere in your project folder -- but `addons/CommonGodotUI` is the recommended location).
```bash
# from your project's root directory
git clone git@github.com:njfletcher215/CommonGodotUI.git addons/CommonGodotUI/
```

## Provided Classes


### CanvasItemPreview
A preview of a canvas item (and each of its child canvas items), displayed as a texture.

#### Usage
Attach this script to a TextureRect you want to preview the canvas item on. Call the CanvasItemPreview's `Preview(CanvasItem previewRoot, double loadTimeSeconds = 1.0)` (or `Preview(PackedScene previewRootScene, double loadTimeSeconds = 1.0)`) method preview the given CanvasItem (or PackedScene containing a CanvasItem) as a texture on the CanvasItemPreview.

As part of the process to capture the texture, the CanvasItem must be loaded/instantiated temporarily; it is destroyed after loadTimeSeconds to save on resource usage. If the preview is not what you expect, ensure loadTimeSeconds is long enough for the CanvasItem to fully load.

### EnumDropdown
An OptionButton with a specific enum as its options.

#### Usage
Attach this script to an OptionButton. Then, just set its `enumTypeName` to the assembly name of the enum, ex. "MyNamespace+MyEnum, myassembly".

### LabeledSlider
A slider with labels for its title, current value, and ticks.

#### Usage
Attach this script to an HSlider. Create a `SimpleFormatStringLabel` for the current value if you want to display it. Create an `HBoxContainer` for the tick labels if you want to display them. Each of these must be manually placed (remember to account for the size of the labels when placing the `HBoxContainer`).

The `SimpleFormatStringLabel` must be configured with a format string containing the key `currentValue`: ex. `My Option: {currentValue}`.

### OverlayMenu/OverlayMenuHost
A menu which is overlayed on another screen, and the screen on which it is overlayed.

#### Usage
Create your overlay menus (as .tscns). Make the root node of the scene a Node2D, and attach the OverlayMenu script to it. Then, attach a script which extends OverlayMenuHost to your desired host (usually the scene or menu over which the OverlayMenu is to be shown).

Each event-generating control in the menu should send its signals to the OverlayMenu's `OnEvent(string eventName, bool close)` -- with `eventName` being the name of the event (in the format MenuName_ButtonName, ex. "RoundPass_Continue", by convention), and `close` being whether the event should close the overlay menu.

The OverlayMenuHost should define an `IDictionary<string, Action> overlayMenuEventHandlers` which maps these `eventName`s to actual methods (again, in the format OnMenuName_ButtonName, ex. "OnRoundPass_Continue", by convention). Passing parameters to these methods is not currently supported.

### SimpleFormatStringLabel
A simple label that displays arbitrary variables injected into a format string.

#### Usage
Attach this script to a Label. Then, set its `formatString`. To set/update a key, use the SimpleFormatStringLabel's `SetValue(string key, object val = null)` method.

`formatString` is based on a standard C# format string, but there are some differences. If a key is not set, or its value is `null`, "null" will be printed. If a value is IEnumerable (and not a string), it will be printed as a comma-separated list of the string values of its items (this is *not* done recursively).

## Questions or Issues?

Feel free to open an issue on GitHub if something's not working or not clear.
Alternatively, you can contact the developer at njfletcher215@gmail.com


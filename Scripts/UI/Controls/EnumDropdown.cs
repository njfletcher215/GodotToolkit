using Godot;
using System;

/// <summary>
/// An OptionButton with a specific enum as its options.
/// </summary>
public partial class EnumDropdown : OptionButton {
    [Signal]
    public delegate void ValueChangedEventHandler(string value);

    /// <summary>
    /// The name of the enum to use as the options.
    /// </summary>
    /// <value>The assembly name of the enum, ex. "MyNamespace+MyEnum, myassembly"</value>
    [Export]
    private string enumTypeName = "";

    /// <summary>
    /// The enum that is being used as options.
    /// Due to limitations with the Godot editor, this type must be determined AT RUNTIME.
    /// </summary>
    private Type enumType;
    private bool initialized = false;

    public override void _Ready() {
        this.Initialize();
    }

    /// <summary>
    /// Get the currently selected value as an enum value.
    /// </summary>
    /// <typeparam name="TEnum">
    /// The Enum type to cast the value to.
    /// Due to compile-time typing, this must be passed every call.
    /// However, it is only checked against enumType at runtime.
    /// </typeparam>
    /// <exception cref="ArgumentException">
    /// Thrown if the requested type does not match enumType.
    /// </exception>
    public TEnum GetValue<TEnum>() where TEnum : Enum {
        if (this.Selected < 0) return default(TEnum);
        if (typeof(TEnum) != this.enumType) throw new ArgumentException($"Requested type must be {this.enumType.Name}");
        return (TEnum)Enum.GetValues(this.enumType).GetValue(this.Selected);
    }

    /// <summary>
    /// Parse the enumType from the enumTypeName string,
    /// and replace any existing items in the dropdown list with the items of enumType.
    /// </summary>
    public void Initialize() {
        if (this.initialized) return;
        // parse the enumType from enumTypeName
        this.enumType = Type.GetType(this.enumTypeName);
        if (this.enumType == null || !this.enumType.IsEnum) {
            GD.PrintErr($"Invalid enum type: {this.enumTypeName}");
            return;
        }

        // replace any existing items in the list with the enum options
        this.Clear();
        foreach (string name in Enum.GetNames(this.enumType)) {
            this.AddItem(name);
        }

        this.ItemSelected += OnItemSelected;

        this.initialized = true;
    }

    /// <summary>
    /// Map an ItemSelected event to a ValueChanged event
    /// by mapping the Selected value to the corresponding enumType value.
    /// Unfortunately, due to limitations with Godot's signaling system,
    /// the emitted value must be passed as a string,
    /// which then must be parsed to the proper type by any listeners.
    /// </summary>
    /// <param name="index">The Selected index.</param>
    private void OnItemSelected(long index) {
        EmitSignal(SignalName.ValueChanged, Enum.GetNames(this.enumType)[index]);
    }
}

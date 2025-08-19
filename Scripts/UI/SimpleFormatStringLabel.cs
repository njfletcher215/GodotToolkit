using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// A simple label that displays arbitrary variables injected into a format string.
/// </summary>
[Tool]
public partial class SimpleFormatStringLabel : Label {
    private static readonly Regex formatKeyPattern = new Regex(@"\{(?<key>[^:}]+)(:(?<format>[^}]+))?\}", RegexOptions.Compiled);

    /// <summary>
    /// Whether to revert to the default/placeholder text when all values are null.
    /// </summary>
    [Export]
    private bool defaultTextOnNull = true;
    [Export(PropertyHint.MultilineText)]
    private string formatString = "";

    private string defaultText = "";

    /// <summary>
    /// The key-value pairs which will be injected into the format string.
    /// </summary>
    /// <value>
    /// The keys are the same keys present in the format string.
    /// The values are the values to be injected for each key.
    /// </value>
    private IDictionary<string, object> values = new Dictionary<string, object>();

    /// <summary>
    /// Replace named placeholders in the format string with values from the dictionary.
    /// If a key is missing or the value is null, "null" is substituted.
    /// Supports optional format specifiers for IFormattable values.
    /// </summary>
    /// <param name="format">The format string containing {key} tokens.</param>
    /// <param name="values">Dictionary mapping keys to objects.</param>
    /// <returns>The formatted string.</returns>
    private static string FormatWith(string format, IDictionary<string, object> values) {
        // Replace each placeholder match with the corresponding dictionary value (or "null")
        return formatKeyPattern.Replace(format, match => {
            string key = match.Groups["key"].Value;
            bool hasFormat = match.Groups["format"].Success;
            string formatSpec = hasFormat ? match.Groups["format"].Value : null;

            if (!values.TryGetValue(key, out object val) || val == null) return "null";

            if (val is IEnumerable<object> enumerable && val is not string) {
                List<string> outputs = new List<string>();
                foreach (object item in enumerable) {
                    if (hasFormat && item is IFormattable formattableItem)
                        outputs.Add(formattableItem.ToString(formatSpec, CultureInfo.CurrentCulture));
                    else outputs.Add(item?.ToString() ?? "");
                }
                return string.Join(", ", outputs);
            }

            // If a format specifier is present and the object supports IFormattable, apply it
            if (hasFormat && val is IFormattable formattable) {
                return formattable.ToString(formatSpec, CultureInfo.CurrentCulture);
            }

            return val.ToString()!;
        });
    }

    public override void _Ready() {
        this.defaultText = this.Text;
    }

    /// <summary>
    /// Set a key-value pair.
    /// </summary>
    /// <param name="key">The key used in the formatString.</param>
    /// <param name="val">The value to be injected.</param>
    public void SetValue(string key, object val = null) {
        this.values[key] = val;
        if (this.values.Values.All(val => val == null ||
                (val is IEnumerable<object> enumerable &&
                 val is not string &&
                 enumerable.All(enumerableVal => enumerableVal == null))))
            this.Text = this.defaultText;
        else this.Text = SimpleFormatStringLabel.FormatWith(this.formatString, this.values);
    }
}

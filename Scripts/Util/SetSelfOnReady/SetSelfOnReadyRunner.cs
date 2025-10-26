using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;

/// <summary>
/// The runner for the SelfSetOnReadyAttribute.
/// </summary>
public static class SetSelfOnReadyRunner {
    /// <summary>
    /// Cache to avoid repeated reflection scans per Type.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _cache = new();

    /// <summary>
    /// Invoke each [SetSelfOnReady] property setter with its current value.
    /// </summary>
    public static void Run(object target) {
        if (target == null) return;
        var props = _cache.GetOrAdd(target.GetType(), GetSetSelfOnReadyProps);
        if (props.Length == 0) return;

        foreach (var p in props) {
            var val = p?.GetValue(target);
            p?.SetMethod?.Invoke(target, new object[] { val });
        }
    }

    /// <summary>
    /// Get all properties marked with the SetSelfOnReadyAttribute.
    /// </summary
    private static PropertyInfo[] GetSetSelfOnReadyProps(Type t) {
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return t.GetProperties(Flags)
                .Where(p => p.GetIndexParameters().Length == 0
                            && p.CanRead
                            && p.CanWrite
                            && p.IsDefined(typeof(SetSelfOnReadyAttribute), inherit: true)
                            && p.GetSetMethod(nonPublic: true) != null)
                .ToArray();
    }

    /// <summary>
    /// Quick check used by the autoload to avoid wiring signals for types with no marked props.
    /// </summary>
    public static bool HasMarkedProps(Type t) => _cache.GetOrAdd(t, GetSetSelfOnReadyProps).Length > 0;
}

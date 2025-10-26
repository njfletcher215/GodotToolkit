using Godot;

/// <summary>
/// Autoload singleton that initializes any Node with [SetSelfOnReady] properties as it becomes ready.
/// </summary>
public partial class SetSelfOnReadyService : Node {
    public override void _EnterTree() {
        GetTree().NodeAdded += OnNodeAdded;
    }

    public override void _ExitTree() {
        GetTree().NodeAdded -= OnNodeAdded;
    }

    private void OnNodeAdded(Node n) {
        // Skip quickly if this type has no marked properties
        if (!SetSelfOnReadyRunner.HasMarkedProps(n.GetType())) {
            GD.Print($"No [SetSelfOnReady]s: {n.Name}");
            return;
        }

        GD.Print($"Yes [SetSelfOnReady]s: {n.Name}");
        // Run after the node is ready (ensures exported paths/children exist)
        n.Ready += () => SetSelfOnReadyRunner.Run(n);
    }
}

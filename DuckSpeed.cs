using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace duckspeed;

[PluginMetadata(
    Id = "vcs2.duckspeed",
    Version = "1.0.0",
    Name = "Duck Speed",
    Author = "ViTamin",
    Description = "Sets DuckSpeed to remove crouch delay."
)]
public sealed class Plugin : BasePlugin
{
    public Plugin(ISwiftlyCore core) : base(core) { }

    private const float DuckSpeedValue = 6.023437f;

    public override void Load(bool hotReload)
    {
        Core.Event.OnMovementServicesRunCommandHook -= OnRunCommand;
        Core.Event.OnMovementServicesRunCommandHook += OnRunCommand;
    }

    public override void Unload()
    {
        Core.Event.OnMovementServicesRunCommandHook -= OnRunCommand;
    }

    private void OnRunCommand(IOnMovementServicesRunCommandHookEvent e)
    {
        var ms = e.MovementServices;
        if (ms == null)
            return;

        if (!ms.DesiresDuck && ms.DuckAmount <= 0.0f)
            return;

        if (ms.DuckSpeed != DuckSpeedValue)
            ms.DuckSpeed = DuckSpeedValue;
    }
}


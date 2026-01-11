using System;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.ProtobufDefinitions;
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
    private const bool ResetDuckCooldown = true;

    private const float Eps = 0.0001f;

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
        if (ms is null)
            return;

        var pawn = ms.Pawn;
        if (pawn is null)
            return;

        var player = pawn.ToPlayer();
        if (player is null || !player.IsValid)
            return;

        if (player.IsFakeClient)
            return;

        var cmd = e.UserCmdPB;
        var buttonsPb = cmd?.Base?.ButtonsPb;
        if (buttonsPb is null)
            return;

        ulong mask = buttonsPb.Buttonstate1 | buttonsPb.Buttonstate2 | buttonsPb.Buttonstate3;

        bool duckDown = (mask & (ulong)GameButtonFlags.Ctrl) != 0;
        if (!duckDown)
            return;

        float cur = ms.DuckSpeed;
        if (MathF.Abs(cur - DuckSpeedValue) > Eps)
        {
            ms.DuckSpeed = DuckSpeedValue;
            ms.DuckSpeedUpdated(); 
        }

        if (!ResetDuckCooldown)
            return;

        if (ms.DuckTimeMsecs != 0)
        {
            ms.DuckTimeMsecs = 0;
            ms.DuckTimeMsecsUpdated();
        }

        if (ms.DuckJumpTimeMsecs != 0)
        {
            ms.DuckJumpTimeMsecs = 0;
            ms.DuckJumpTimeMsecsUpdated();
        }

        if (ms.LastDuckTime > -1000.0f)
        {
            ms.LastDuckTime = -9999.0f;
            ms.LastDuckTimeUpdated();
        }
    }
}

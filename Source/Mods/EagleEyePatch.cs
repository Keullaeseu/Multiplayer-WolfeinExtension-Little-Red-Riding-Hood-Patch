using System.Reflection.Emit;
using HarmonyLib;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerWolfeinExtensionLittleRedRidingHoodPatch.Source.Mods;

/// <summary>
///     Eagle_Eye.dll sync.
///     <list type="bullet">
///         <item>
///             <c>GameComponent_EagleEye.ScheduleTrigger</c> consumes <c>Rand.RangeInclusive</c>
///             from the MomoTalk option handler. Registered as a sync method so the delay ticks
///             match on every client (also covered when the Mirrored patch replays the event).
///         </item>
///         <item>
///             <c>Comp_UAVSubmit.SubmitTo</c> is a float-menu action that destroys the UAV
///             item and sets a story marker. Registered as a sync method (ThingComp instance
///             is serialized by Multiplayer).
///         </item>
///         <item>
///             <c>SpawnWreckage</c> falls back to <c>Find.CurrentMap</c>, which is the
///             locally selected map. The transpiler below replaces that fallback with
///             <c>Find.AnyPlayerHomeMap</c> so every client targets the same map.
///         </item>
///     </list>
/// </summary>
internal static class EagleEyePatch
{
    internal static void RegisterSyncMethods()
    {
        WEReflection.RegisterSyncMethodSafe("Eagle_Eye.GameComponent_EagleEye:ScheduleTrigger");
        WEReflection.RegisterSyncMethodSafe("Eagle_Eye.Comp_UAVSubmit:SubmitTo");
    }

    [MpCompatTranspiler("Eagle_Eye.GameComponent_EagleEye", "SpawnWreckage")]
    private static IEnumerable<CodeInstruction> ReplaceCurrentMapFallback(IEnumerable<CodeInstruction> instructions)
    {
        var currentMapGetter = AccessTools.PropertyGetter(typeof(Find), nameof(Find.CurrentMap));
        var homeMapGetter = AccessTools.PropertyGetter(typeof(Find), nameof(Find.AnyPlayerHomeMap));

        foreach (var instruction in instructions)
        {
            if (currentMapGetter != null && homeMapGetter != null
                                         && instruction.opcode == OpCodes.Call &&
                                         currentMapGetter.Equals(instruction.operand))
                instruction.operand = homeMapGetter;

            yield return instruction;
        }
    }
}
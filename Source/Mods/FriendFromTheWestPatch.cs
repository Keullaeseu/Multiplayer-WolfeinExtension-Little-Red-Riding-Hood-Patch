using System.Reflection.Emit;
using HarmonyLib;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerWolfeinExtensionLittleRedRidingHoodPatch.Source.Mods;

/// <summary>
///     Friend_From_the_West.dll sync.
///     <list type="bullet">
///         <item>
///             <c>GameComponent_FriendFromTheWest.GameComponentTick</c> reads
///             <c>Find.CurrentMap</c> for the raid target and the A10 airstrike sequence.
///             The transpiler replaces it with <c>Find.AnyPlayerHomeMap</c> so the incident
///             target, projectile spawn, explosions and exit-map orders run on the same map
///             for every client.
///         </item>
///         <item>
///             <c>StartAirstrike</c> is invoked from the raid letter dialog close action
///             (interface context). Registered as a sync method so closing the dialog on one
///             client starts the synced airstrike state machine everywhere.
///         </item>
///         <item>
///             <c>ScheduleRaid</c> is registered as well; it is normally called from the
///             synced tick, but syncing keeps debug/manual calls safe.
///         </item>
///     </list>
///     The raid incident's auto <c>OpenLetter</c> is intentionally left as-is: it fires
///     in synced incident context on all clients, and the resulting close action is
///     synced via <c>StartAirstrike</c>.
/// </summary>
internal static class FriendFromTheWestPatch
{
    internal static void RegisterSyncMethods()
    {
        WEReflection.RegisterSyncMethodSafe("Friend_From_the_West.GameComponent_FriendFromTheWest:StartAirstrike");
        WEReflection.RegisterSyncMethodSafe("Friend_From_the_West.GameComponent_FriendFromTheWest:ScheduleRaid");
    }

    [MpCompatTranspiler("Friend_From_the_West.GameComponent_FriendFromTheWest", "GameComponentTick")]
    private static IEnumerable<CodeInstruction> ReplaceCurrentMapWithHomeMap(IEnumerable<CodeInstruction> instructions)
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
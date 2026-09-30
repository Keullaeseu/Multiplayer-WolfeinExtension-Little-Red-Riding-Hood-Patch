using Multiplayer.Compat;
using Verse;

namespace MultiplayerWolfeinExtensionLittleRedRidingHoodPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for Wolfein extension:"Little Red Riding Hood" by heimu,
///     Last Update: 14 Sep @ 6:06pm 2026
///     <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=3698641606" />
///     The mod ships 12 assemblies. Most are deterministic in synced context (ticks,
///     incidents, quest-gen). This patch syncs the handful of interface-to-sim writes
///     and removes per-client map selection:
///     <see cref="DoorToDoorPatch" />, <see cref="EagleEyePatch" />,
///     <see cref="FriendFromTheWestPatch" />, <see cref="HomecomingPatch" />,
///     <see cref="AbilityReloadPatch" />.
///     Dialogue option events (RimMomotalk.MomoOptionEvents) are relayed by the
///     separate Grand Library of Mirrored compat patch; the mutators below are
///     additionally registered as sync methods so they stay correct even if that
///     patch is absent or loads in a different order.
/// </summary>
[MpCompatFor("heimu.LittleRedRidingHood")]
public class WELittleRedRidingHood
{
    internal const string LogPrefix = "[Multiplayer Wolfein extension:\"Little Red Riding Hood\" Patch]";

    public WELittleRedRidingHood(ModContentPack content)
    {
        LongEventHandler.ExecuteWhenFinished(LatePatch);
    }

    private static void LatePatch()
    {
        Log.Message($"{LogPrefix} Initializing...");

        WEReflection.ResolveAll();

        DoorToDoorPatch.RegisterSyncMethods();
        EagleEyePatch.RegisterSyncMethods();
        FriendFromTheWestPatch.RegisterSyncMethods();
        HomecomingPatch.RegisterSyncMethods();
        AbilityReloadPatch.RegisterSyncMethods();

        MpCompatPatchLoader.LoadPatch(typeof(EagleEyePatch));
        MpCompatPatchLoader.LoadPatch(typeof(FriendFromTheWestPatch));

        Log.Message($"{LogPrefix} Initialized.");
    }
}
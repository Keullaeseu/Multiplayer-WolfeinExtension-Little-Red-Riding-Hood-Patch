namespace MultiplayerWolfeinExtensionLittleRedRidingHoodPatch.Source.Mods;

/// <summary>
///     RW.dll (RW_Firearm_Loading) sync.
///     <c>CompAbilityReload.StartReload</c> is chosen from a gizmo and an ammo float
///     menu, then issues <c>pawn.jobs.TryTakeOrderedJob</c>. The job call itself is
///     already synced by Multiplayer core, but registering <c>StartReload</c> ensures
///     the ammo validation and charge math also execute identically on all clients.
///     The menu construction and ammo scan stay local (interface reads).
///     The <c>JobDriver_ReloadAbility</c> toils are deterministic and need no patch.
/// </summary>
internal static class AbilityReloadPatch
{
    internal static void RegisterSyncMethods()
    {
        WEReflection.RegisterSyncMethodSafe("RW_Firearm_Loading.CompAbilityReload:StartReload");
    }
}
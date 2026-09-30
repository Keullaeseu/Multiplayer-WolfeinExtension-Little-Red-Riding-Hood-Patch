using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace MultiplayerWolfeinExtensionLittleRedRidingHoodPatch.Source.Mods;

/// <summary>
///     Shared reflection helpers for the 12 Wolfein assemblies.
///     All lookups are string-based so the patch never needs a compile-time
///     reference to the mod DLLs. Missing types are logged and skipped so a
///     single renamed class cannot break the whole patch.
/// </summary>
internal static class WEReflection
{
    internal static void ResolveAll()
    {
        // Touch each assembly's key types once so load problems surface early with a clear log.
        // Individual patches re-resolve what they need and tolerate nulls.
        Touch("Door_to_Door_Recovery.GameComponent_DoorToDoorRecovery");
        Touch("Eagle_Eye.GameComponent_EagleEye");
        Touch("Eagle_Eye.Comp_UAVSubmit");
        Touch("Friend_From_the_West.GameComponent_FriendFromTheWest");
        Touch("Friend_From_the_West.ChoiceLetter_RaidAirstrike");
        Touch("Friend_From_the_West.IncidentWorker_RaidFriendFromTheWest");
        Touch("Homecoming.GameComponent_Homecoming");
        Touch("Homecoming.ChoiceLetter_Homecoming");
        Touch("Homecoming.IncidentWorker_Homecoming");
        Touch("RW_Firearm_Loading.CompAbilityReload");
    }

    private static void Touch(string typeName)
    {
        try
        {
            if (AccessTools.TypeByName(typeName) == null)
                Log.Warning(
                    $"{WELittleRedRidingHood.LogPrefix} Optional type not found (skipping related patches): {typeName}");
        }
        catch (Exception exception)
        {
            Log.Warning($"{WELittleRedRidingHood.LogPrefix} Error resolving type {typeName}: {exception}");
        }
    }

    internal static Type TypeOrNull(string typeName)
    {
        try
        {
            return AccessTools.TypeByName(typeName);
        }
        catch (Exception exception)
        {
            Log.Warning($"{WELittleRedRidingHood.LogPrefix} Error resolving type {typeName}: {exception}");
            return null;
        }
    }

    internal static MethodBase MethodOrNull(string typeColonMethod)
    {
        try
        {
            return AccessTools.DeclaredMethod(typeColonMethod);
        }
        catch (Exception exception)
        {
            Log.Warning($"{WELittleRedRidingHood.LogPrefix} Error resolving method {typeColonMethod}: {exception}");
            return null;
        }
    }

    internal static void RegisterSyncMethodSafe(string typeColonMethod)
    {
        try
        {
            var method = AccessTools.DeclaredMethod(typeColonMethod) ?? AccessTools.Method(typeColonMethod);
            if (method == null)
            {
                Log.Warning(
                    $"{WELittleRedRidingHood.LogPrefix} Could not find method {typeColonMethod} to sync, skipping.");
                return;
            }

            MP.RegisterSyncMethod(method);
        }
        catch (Exception exception)
        {
            Log.Error(
                $"{WELittleRedRidingHood.LogPrefix} Failed to register sync method {typeColonMethod}: {exception}");
        }
    }

    internal static void RegisterSyncMethodSafe(MethodInfo method, string label)
    {
        try
        {
            if (method == null)
            {
                Log.Warning($"{WELittleRedRidingHood.LogPrefix} Could not find method {label} to sync, skipping.");
                return;
            }

            MP.RegisterSyncMethod(method);
        }
        catch (Exception exception)
        {
            Log.Error($"{WELittleRedRidingHood.LogPrefix} Failed to register sync method {label}: {exception}");
        }
    }
}
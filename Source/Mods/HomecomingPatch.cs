using HarmonyLib;
using Multiplayer.API;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerWolfeinExtensionLittleRedRidingHoodPatch.Source.Mods;

/// <summary>
///     Homecoming.dll sync.
///     <list type="bullet">
///         <item>
///             <c>IncidentWorker_Homecoming.TryExecuteFromDialogue</c> is fired from the
///             MomoTalk option handler with a home map argument. Registered as a sync method
///             so the letter creation runs in synced context on all clients.
///         </item>
///         <item>
///             <c>ChoiceLetter_Homecoming.Choices</c> accept/reject actions mutate sim
///             state (faction change, spawn, world-pawn cleanup, cross-mod saved-pawn clear).
///             Both option lambdas are registered as sync methods with the second marked as
///             the default letter choice, following the MoreFactionInteraction pattern.
///         </item>
///     </list>
/// </summary>
internal static class HomecomingPatch
{
    internal static void RegisterSyncMethods()
    {
        WEReflection.RegisterSyncMethodSafe("Homecoming.IncidentWorker_Homecoming:TryExecuteFromDialogue");

        try
        {
            var letterType = AccessTools.TypeByName("Homecoming.ChoiceLetter_Homecoming");
            if (letterType == null)
            {
                Log.Warning(
                    $"{WELittleRedRidingHood.LogPrefix} Could not find type Homecoming.ChoiceLetter_Homecoming to sync letter choices, skipping.");
                return;
            }

            var lambdas = MpMethodUtil.GetLambda(letterType, "Choices", MethodType.Getter, null, 0, 1).ToArray();
            if (lambdas.Length < 2 || lambdas.Any(m => m == null))
            {
                Log.Warning(
                    $"{WELittleRedRidingHood.LogPrefix} Could not find both Homecoming letter choice lambdas, skipping letter sync.");
                return;
            }

            MP.RegisterSyncMethod(lambdas[0]);
            MP.RegisterSyncMethod(lambdas[1]);
            MP.RegisterDefaultLetterChoice(lambdas[1], letterType);
        }
        catch (Exception exception)
        {
            Log.Error($"{WELittleRedRidingHood.LogPrefix} Failed to sync Homecoming letter choices: {exception}");
        }
    }
}
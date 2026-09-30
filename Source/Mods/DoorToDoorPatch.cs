namespace MultiplayerWolfeinExtensionLittleRedRidingHoodPatch.Source.Mods;

/// <summary>
///     Door_to_Door_Recovery.dll sync.
///     <c>GameComponent_DoorToDoorRecovery.OnFollowupOptionSelected</c> runs on the
///     MomoTalk option-selected event (interface context, clicking client only) and
///     drops a USB pod plus adjusts Lone Wolf goodwill. When the Grand Library of
///     Mirrored compat patch is present the event is already replayed in synced
///     context; registering the mutators below as sync methods keeps them correct
///     even without it. The tick path (site spawn/tracking) already runs synced
///     and needs no changes. Map generation (GenStep) uses a fixed seed.
/// </summary>
internal static class DoorToDoorPatch
{
    internal static void RegisterSyncMethods()
    {
        // Instance drop-pod spawn (Find.AnyPlayerHomeMap is deterministic; Rand inside DropPodUtility must run synced).
        WEReflection.RegisterSyncMethodSafe(
            "Door_to_Door_Recovery.GameComponent_DoorToDoorRecovery:DropUsbFlashDriveToColonyCenter");
        // Static goodwill adjustment (int amount).
        WEReflection.RegisterSyncMethodSafe("Door_to_Door_Recovery.GameComponent_DoorToDoorRecovery:ApplyGoodwill");
    }
}
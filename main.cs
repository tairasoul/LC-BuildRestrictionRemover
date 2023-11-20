using HarmonyLib;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(typeof(BuildRestrictionRemover.RestrictionRemover), "LC.BuildRestrictionRemover", "1.0.0", "tairasoul")]
[assembly: MelonGame("ZeekerssRBLX", "Lethal Company")]

namespace BuildRestrictionRemover
{

    [HarmonyPatch(typeof(ShipBuildModeManager))]
    internal class BuildPatches
    {
        [HarmonyPatch("PlayerMeetsConditionsToBuild")]
        [HarmonyPostfix]
        static void PlayerConditions(ref bool __result)
        {
            __result = true;
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void Update(ShipBuildModeManager __instance)
        {
            FieldInfo placingObject = typeof(ShipBuildModeManager).GetField("placingObject", BindingFlags.NonPublic | BindingFlags.Instance);
            if (placingObject != null)
            {
                PlaceableShipObject shipObject = (PlaceableShipObject)placingObject.GetValue(__instance);
                if (shipObject != null)
                {
                    shipObject.AllowPlacementOnWalls = true;
                    shipObject.AllowPlacementOnCounters = true;
                    shipObject.doCollisionPointCheck = false;
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePostfix(ShipBuildModeManager __instance)
        {
            FieldInfo CanConfirmPosition = typeof(ShipBuildModeManager).GetField("CanConfirmPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            if (CanConfirmPosition != null)
            {
                CanConfirmPosition.SetValue(__instance, true);
            }
            __instance.ghostObjectRenderer.sharedMaterial = __instance.ghostObjectGreen;
        }
    }
    public class RestrictionRemover: MelonMod
    {
        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll(typeof(BuildPatches));
            LoggerInstance.Msg("Applied ShipBuildModeManager patches.");
        }
    }
}

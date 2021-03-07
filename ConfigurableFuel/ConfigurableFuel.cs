using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ConfigurableFuel
{

    public class RequiresFuel : MonoBehaviour
    {
        private bool requiresFuel = true;
        public bool GetRequireFuel() { return requiresFuel; }
        public void SetRequireFuel(bool reqFuel) { requiresFuel = reqFuel; }
    }

    [BepInPlugin("goonlou.ConfigurableFuel", "Configurable Fuel", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class ConfigurableFuel : BaseUnityPlugin
    {
        private static ConfigurableFuel context;

        public static ConfigEntry<bool> modEnabled;

        //public static void Debugger(string str = "") { Debug.Log($"\n{typeof(ConfigurableFuel).Namespace}:\n\t{str}"); }

        private void Awake()
        {
            context = this;

            modEnabled = Config.Bind<bool>("General", "Enabled", true, "Enable this mod");

            Config.Bind<bool>("fire_pit", "fire_pit_NoFuel", false, "Allow fire Pit to constantly burn without fuel");
            Config.Bind<string>("fire_pit", "fire_pit_FuelType", "Wood", "Fuel type for Fire Pit");
            Config.Bind<float>("fire_pit", "fire_pit_MaxFuel", 20f, "Maximum fuel level for Fire Pit");
            Config.Bind<float>("fire_pit", "fire_pit_StartFuel", 10f, "Start fuel level for Fire Pit");
            Config.Bind<float>("fire_pit", "fire_pit_FuelTimeToBurn", 10f, "Time for Fire Pit to burn 1 fuel (sec)");

            Config.Bind<bool>("bonfire", "bonfire_NoFuel", false, "Allow Bonfire to constantly burn without fuel");
            Config.Bind<string>("bonfire", "bonfire_FuelType", "Resin", "Fuel type for Bonfire pit");
            Config.Bind<float>("bonfire", "bonfire_MaxFuel", 100f, "Maximum fuel level for Bonfire pit");
            Config.Bind<float>("bonfire", "bonfire_StartFuel", 50f, "Start fuel level for Bonfire pit");
            Config.Bind<float>("bonfire", "bonfire_FuelTimeToBurn", 5f, "Time for Bonfire pit to burn 1 fuel (sec)");

            Config.Bind<bool>("hearth", "hearth_NoFuel", false, "Allow Hearth to constantly burn without fuel");
            Config.Bind<string>("hearth", "hearth_FuelType", "Wood", "Fuel type for Hearth pit");
            Config.Bind<float>("hearth", "hearth_MaxFuel", 1000f, "Maximum fuel level for Hearth pit");
            Config.Bind<float>("hearth", "hearth_StartFuel", 2332f, "Start fuel level for Hearth pit");
            Config.Bind<float>("hearth", "hearth_FuelTimeToBurn", 2f, "Time for Hearth pit to burn 1 fuel (sec)");

            if (!modEnabled.Value) return;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        [HarmonyPatch(typeof(Fireplace), "Awake")]
        static class Fireplace_Awake_Patch
        {
            public static ConfigEntry<bool> fireNoFuel;
            public static ConfigEntry<string> fireFuelType;
            public static ConfigEntry<float> fireMaxFuel;
            public static ConfigEntry<float> fireStartFuel;
            public static ConfigEntry<float> fireFuelTimeToBurn;

            static void Prefix(Fireplace __instance, ref float ___m_maxFuel, ref float ___m_startFuel, ref float ___m_secPerFuel)
            {
                string fireName = null;
                if (__instance.name.Contains("fire_pit")) {
                    fireName = "fire_pit";
                } else if (__instance.name.Contains("bonfire")) {
                    fireName = "bonfire";
                } else if (__instance.name.Contains("hearth")) {
                    fireName = "hearth";
                }

                if (fireName != null || fireName != "")
                {
                    context.Config.TryGetEntry<bool>($"{fireName}", $"{fireName}_NoFuel", out fireNoFuel);
                    context.Config.TryGetEntry<string>($"{fireName}", $"{fireName}_FuelType", out fireFuelType);
                    context.Config.TryGetEntry<float>($"{fireName}", $"{fireName}_MaxFuel", out fireMaxFuel);
                    context.Config.TryGetEntry<float>($"{fireName}", $"{fireName}_StartFuel", out fireStartFuel);
                    context.Config.TryGetEntry<float>($"{fireName}", $"{fireName}_FuelTimeToBurn", out fireFuelTimeToBurn);

                    if (!fireNoFuel.Value)
                    {
                        GameObject newFuelObject = ZNetScene.instance.GetPrefab(fireFuelType.Value);
                        if (newFuelObject != null)
                        {
                            ItemDrop newFuel = newFuelObject.GetComponent<ItemDrop>();
                            if (newFuel != null && __instance.m_fuelItem != newFuel)
                            {
                                __instance.m_fuelItem = newFuel;
                            }
                        }
                        if (fireMaxFuel != null && ___m_maxFuel != fireMaxFuel.Value)
                        {
                            ___m_maxFuel = fireMaxFuel.Value;
                        }
                        if (fireStartFuel != null && ___m_startFuel != fireStartFuel.Value)
                        {
                            ___m_startFuel = fireStartFuel.Value;
                        }
                        if (fireFuelTimeToBurn != null && ___m_secPerFuel != fireFuelTimeToBurn.Value)
                        {
                            ___m_secPerFuel = fireFuelTimeToBurn.Value;
                        }
                    } 
                    RequiresFuel requireFuelComponent = __instance.gameObject.AddComponent<RequiresFuel>();
                    if (fireNoFuel != null) {
                        requireFuelComponent.SetRequireFuel(!fireNoFuel.Value);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Fireplace), "UpdateFireplace")]
        static class Fireplace_UpdateFireplace_Patch
        {
            static void Prefix(Fireplace __instance)
            {
                bool requiresFuel = __instance.GetComponent<RequiresFuel>().GetRequireFuel();
                if (!requiresFuel)
                {
                    __instance.GetComponent<ZNetView>().GetZDO().Set("fuel", __instance.m_maxFuel);
                    return;
                }
            }
        }
    }
}
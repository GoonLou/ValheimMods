using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ConfigurableFuel
{
    [BepInPlugin("goonlou.ConfigurableFuel", "Configurable Fuel", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class ConfigurableFuel : BaseUnityPlugin
    {
        private static ConfigurableFuel context;

        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> allNoFuel;

        //public static void Debugger(string str = "") { Debug.Log($"\n{typeof(ConfigurableFuel).Namespace}:\n\t{str}"); }

        private void Awake()
        {
            modEnabled = Config.Bind<bool>("00_General", "Enabled", true, "Enable this mod");
            if (!modEnabled.Value) return;

            context = this;
            allNoFuel = Config.Bind<bool>("00_General", "all_Nofuel", false, "Allow all fires to constantly burn without fuel");

            Config.Bind<bool>("01_fire_pit", "fire_pit_NoFuel", false, "Allow fire Pit to constantly burn without fuel");
            Config.Bind<string>("01_fire_pit", "fire_pit_FuelType", "Wood", "Fuel type for Fire Pit");
            Config.Bind<float>("01_fire_pit", "fire_pit_MaxFuel", 10f, "Maximum fuel level for Fire Pit");
            Config.Bind<float>("01_fire_pit", "fire_pit_StartFuel", 1f, "Start fuel level for Fire Pit");
            Config.Bind<float>("01_fire_pit", "fire_pit_FuelTimeToBurn", 5000f, "Time for Fire Pit to burn 1 fuel (sec)");

            Config.Bind<bool>("02_bonfire", "bonfire_NoFuel", false, "Allow Bonfire to constantly burn without fuel");
            Config.Bind<string>("02_bonfire", "bonfire_FuelType", "Wood", "Fuel type for Bonfire pit");
            Config.Bind<float>("02_bonfire", "bonfire_MaxFuel", 10f, "Maximum fuel level for Bonfire pit");
            Config.Bind<float>("02_bonfire", "bonfire_StartFuel", 0f, "Start fuel level for Bonfire pit");
            Config.Bind<float>("02_bonfire", "bonfire_FuelTimeToBurn", 5000f, "Time for Bonfire pit to burn 1 fuel (sec)");

            Config.Bind<bool>("03_hearth", "hearth_NoFuel", false, "Allow Hearth to constantly burn without fuel");
            Config.Bind<string>("03_hearth", "hearth_FuelType", "Wood", "Fuel type for Hearth pit");
            Config.Bind<float>("03_hearth", "hearth_MaxFuel", 20f, "Maximum fuel level for Hearth pit");
            Config.Bind<float>("03_hearth", "hearth_StartFuel", 0f, "Start fuel level for Hearth pit");
            Config.Bind<float>("03_hearth", "hearth_FuelTimeToBurn", 5000f, "Time for Hearth pit to burn 1 fuel (sec)");

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
                if (!modEnabled.Value) return;

                string fireName = null;
                string tabName = null;
                if (__instance.name.Contains("fire_pit")) {
                    fireName = "fire_pit";
                    tabName = "01_fire_pit";
                } else if (__instance.name.Contains("bonfire")) {
                    fireName = "bonfire";
                    tabName = "02_bonfire";
                } else if (__instance.name.Contains("hearth")) {
                    fireName = "hearth";
                    tabName = "03_hearth";
                }

                if (fireName != null || fireName != "")
                {
                    RequiresFuel requireFuelComponent = __instance.gameObject.AddComponent<RequiresFuel>();
                    //Debugger($"No Fires require fuel: {allNoFuel.Value}");
                    if (!allNoFuel.Value)
                    {
                        string str = $"{fireName}:";
                        context.Config.TryGetEntry<bool>($"{tabName}", $"{fireName}_NoFuel", out fireNoFuel);
                        context.Config.TryGetEntry<string>($"{tabName}", $"{fireName}_FuelType", out fireFuelType);
                        context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_MaxFuel", out fireMaxFuel);
                        context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_StartFuel", out fireStartFuel);
                        context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_FuelTimeToBurn", out fireFuelTimeToBurn);

                        if (fireNoFuel != null)
                        {
                            requireFuelComponent.SetRequireFuel(!fireNoFuel.Value);
                            //str += $"\n\tRequires Fuel: {!fireNoFuel.Value}";

                            if (!fireNoFuel.Value)
                            {
                                GameObject newFuelObject = ZNetScene.instance.GetPrefab(fireFuelType.Value);
                                if (newFuelObject != null)
                                {
                                    ItemDrop newFuel = newFuelObject.GetComponent<ItemDrop>();
                                    if (newFuel != null && __instance.m_fuelItem != newFuel)
                                    {
                                        __instance.m_fuelItem = newFuel;
                                        //str += $"\n\tNew Type: {newFuel}";
                                    }
                                }
                                if (fireMaxFuel != null && ___m_maxFuel != fireMaxFuel.Value)
                                {
                                    ___m_maxFuel = fireMaxFuel.Value;
                                    //str += $"\n\tNew Max: {___m_maxFuel}";
                                }
                                if (fireStartFuel != null && ___m_startFuel != fireStartFuel.Value)
                                {
                                    ___m_startFuel = fireStartFuel.Value;
                                    //str += $"\n\tNew Start: {___m_startFuel}";
                                }
                                if (fireFuelTimeToBurn != null && ___m_secPerFuel != fireFuelTimeToBurn.Value)
                                {
                                    ___m_secPerFuel = fireFuelTimeToBurn.Value;
                                    //str += $"\n\tNew Burn Time: {___m_secPerFuel}";
                                }
                            }

                            //Debugger(str);
                        }
                    } 
                    else
                    {
                        requireFuelComponent.SetRequireFuel(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Fireplace), "UpdateFireplace")]
        static class Fireplace_UpdateFireplace_Patch
        {
            static void Prefix(Fireplace __instance)
            {
                if (!modEnabled.Value) return;
                bool requiresFuel = __instance.GetComponent<RequiresFuel>().GetRequireFuel();
                if (!requiresFuel)
                {
                    __instance.GetComponent<ZNetView>().GetZDO().Set("fuel", __instance.m_maxFuel);
                    return;
                }
            }
        }

        public class RequiresFuel : MonoBehaviour
        {
            private bool requiresFuel = true;
            public bool GetRequireFuel() { return requiresFuel; }
            public void SetRequireFuel(bool reqFuel) { requiresFuel = reqFuel; }
        }
    }
}
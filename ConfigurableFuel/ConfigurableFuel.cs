using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace ConfigurableFuel
{
    [BepInPlugin("goonlou.ConfigurableFuel", "Configurable Fuel", "0.1.0")]
    [BepInProcess("valheim.exe")]
    public class ConfigurableFuel : BaseUnityPlugin
    {
        private static ConfigurableFuel context;

        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> allNoFuel;
        public static ConfigEntry<bool> extinguishableFires;
        public static ConfigEntry<string> toggleFireKey;

        public static void Debugger(string str = "") { Debug.Log($"\n{typeof(ConfigurableFuel).Namespace}:\n\t{str}"); }

        private void Awake()
        {
            context = this;
            allNoFuel = Config.Bind<bool>("00_General", "all_Nofuel", false, "Allow all fires to constantly burn without fuel");
            extinguishableFires = Config.Bind<bool>("00_General", "ExtinguishableFires", true, "Allow all fires to toggle on and off");
            toggleFireKey = Config.Bind<string>("00_General", "toggleFireKey", "G", "Modifier key to toggle fires on and off. Use https://docs.unity3d.com/Manual/ConventionalGameInput.html");

            modEnabled = Config.Bind<bool>("00_General", "Enabled", true, "Enable this mod");
            if (!modEnabled.Value) return;

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

        private void FixedUpdate()
        {
            if (!modEnabled.Value || !extinguishableFires.Value) return;

            if (Input.GetKeyDown(toggleFireKey.Value.ToLower()))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10f))
                {
                    //TODO: Get hover object from Player
                    Transform transform = hit.transform;
                    ConfigureFuelComponent conFuelComp = null;
                    if (transform.gameObject.GetComponent<ConfigureFuelComponent>() != null)
                    {
                        Debugger($"Got component");
                        conFuelComp = transform.gameObject.GetComponent<ConfigureFuelComponent>();
                    } 
                    else 
                    {
                        while (conFuelComp == null)
                        {
                            if (transform.parent.gameObject.GetComponent<ConfigureFuelComponent>() != null)
                            {
                                Debugger($"Got component");
                                conFuelComp = transform.parent.gameObject.GetComponent<ConfigureFuelComponent>();
                            }
                            transform = transform.parent.transform;
                        }
                    }

                    if (conFuelComp != null)
                    {
                        bool prevState = conFuelComp.GetToggledOn();    
                        Debugger($"Before Toggle - {prevState}");
                        if (prevState)
                        {
                            conFuelComp.SetCurrentFuel(conFuelComp.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("fuel"));
                        }

                        bool newState = !prevState;
                        conFuelComp.SetToggledOn(newState);
                        Debugger($"After Toggle - {newState}");
                        if (newState)
                        {
                            Debugger($"Toggling On - {conFuelComp.name}");
                            conFuelComp.gameObject.GetComponent<ZNetView>().GetZDO().Set("fuel", conFuelComp.GetCurrentFuel()); 
                        } 
                        else
                        {
                            Debugger($"Toggling Off - {conFuelComp.name}");
                            conFuelComp.gameObject.GetComponent<ZNetView>().GetZDO().Set("fuel", 0f);
                        }

                        ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Player.m_localPlayer)).SetTrigger("interact");
                    }                    
                }
            }
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
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.AddComponent<ConfigureFuelComponent>();
                    if (!allNoFuel.Value)
                    {
                        context.Config.TryGetEntry<bool>($"{tabName}", $"{fireName}_NoFuel", out fireNoFuel);
                        context.Config.TryGetEntry<string>($"{tabName}", $"{fireName}_FuelType", out fireFuelType);
                        context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_MaxFuel", out fireMaxFuel);
                        context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_StartFuel", out fireStartFuel);
                        context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_FuelTimeToBurn", out fireFuelTimeToBurn);

                        if (fireNoFuel != null)
                        {
                            configureFuelComponent.SetDoesNotRequireFuel(fireNoFuel.Value);
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
                                    configureFuelComponent.SetCurrentFuel(___m_startFuel);
                                }
                                if (fireFuelTimeToBurn != null && ___m_secPerFuel != fireFuelTimeToBurn.Value)
                                {
                                    ___m_secPerFuel = fireFuelTimeToBurn.Value;
                                }
                            }
                        }
                    } 
                    else
                    {
                        configureFuelComponent.SetDoesNotRequireFuel(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Fireplace), "Interact")]
        static class Fireplace_Interact_Patch
        {
            static bool Prefix(Fireplace __instance, Humanoid user, bool hold, ref bool __result, ZNetView ___m_nview)
            {
                if (hold)
                {
                    return false;
                }
                if (!___m_nview.HasOwner())
                {
                    ___m_nview.ClaimOwnership();
                }
                if (modEnabled.Value && extinguishableFires.Value)
                {
                    if (!__instance.gameObject.GetComponent<ConfigureFuelComponent>().GetToggledOn())
                    {
                        user.Message(MessageHud.MessageType.Center, Localization.instance.Localize($"Turn on Fire to add {__instance.m_fuelItem.m_itemData.m_shared.m_name}"));
                        __result = false;
                    }
                }
                return __result;
            }
        }

        [HarmonyPatch(typeof(Fireplace), "UpdateFireplace")]
        static class Fireplace_UpdateFireplace_Patch
        {
            static void Prefix(Fireplace __instance)
            {
                if (modEnabled.Value)
                {
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.GetComponent<ConfigureFuelComponent>();
                    if (configureFuelComponent.GetDoesNotRequireFuel() && (!extinguishableFires.Value || (extinguishableFires.Value && configureFuelComponent.GetToggledOn())))
                    {
                        __instance.gameObject.GetComponent<ZNetView>().GetZDO().Set("fuel", __instance.m_maxFuel);
                    }                    
                }
            }
        }

        [HarmonyPatch(typeof(Fireplace), "GetHoverText")]
        static class Fireplace_GetHoverText_Patch
        {
            static void Postfix(Fireplace __instance, ref string __result, ZNetView ___m_nview)
            {
                if (modEnabled.Value && extinguishableFires.Value)
                {
                    float @float = ___m_nview.GetZDO().GetFloat("fuel");
                    string str = "Extinguish Fire";
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.GetComponent<ConfigureFuelComponent>();
                    if (!configureFuelComponent.GetToggledOn())
                    {
                        @float = configureFuelComponent.GetCurrentFuel();
                        str = "Light Fire";
                    }                    
                    __result = Localization.instance.Localize(__instance.m_name + " ( $piece_fire_fuel " + Mathf.Ceil(@float) + "/" + (int)__instance.m_maxFuel + " )\n[<color=yellow><b>$KEY_Use</b></color>] $piece_use " + __instance.m_fuelItem.m_itemData.m_shared.m_name + "\n[<color=yellow><b>1-8</b></color>] $piece_useitem\n[<color=yellow><b>"+toggleFireKey.Value+"</b></color>]"+str);
                }
            }
        }

        public class ConfigureFuelComponent : MonoBehaviour
        {
            private bool toggledOn = true;
            private bool doesNotRequireFuel = true;
            private float currentFuel = 0f;

            public bool GetToggledOn() { return toggledOn; }
            public void SetToggledOn(bool togOn) { toggledOn = togOn; }

            public bool GetDoesNotRequireFuel() { return doesNotRequireFuel; }
            public void SetDoesNotRequireFuel(bool reqFuel) { doesNotRequireFuel = reqFuel; }            
            
            public float GetCurrentFuel() { return currentFuel; }
            public void SetCurrentFuel(float curFuel) { currentFuel = curFuel; }
        }
    }
}
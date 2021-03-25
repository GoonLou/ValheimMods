using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ConfigurableFire
{
    [BepInPlugin("goonlou.ConfigurableFire", "Configurable Fire", "0.1.2")]
    [BepInProcess("valheim.exe")]
    public class ConfigurableFire : BaseUnityPlugin
    {
        private static ConfigurableFire context;

        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> allNoFuel;
        public static ConfigEntry<bool> dropFuel;
        public static ConfigEntry<bool> extinguishableFires;
        public static ConfigEntry<string> toggleFireKey;

        public static void Debugger(string str = "") { Debug.Log($"\n{typeof(ConfigurableFire).Namespace}:\n\t{str}"); }

        private void Awake()
        {
            context = this;
            allNoFuel = Config.Bind<bool>("00_General", "all_Nofuel", false, "Allow all fires to burn without fuel");
            dropFuel = Config.Bind<bool>("00_General", "dropFuel", true, "Allow for used fuel over the start fuel count to drop on break");
            extinguishableFires = Config.Bind<bool>("00_General", "ExtinguishableFires", true, "Allow all fires to be extinguishable");
            toggleFireKey = Config.Bind<string>("00_General", "toggleFireKey", "G", "Modifier key to toggle fires on and off. Use https://docs.unity3d.com/Manual/ConventionalGameInput.html");

            modEnabled = Config.Bind<bool>("00_General", "Enabled", true, "Enable this mod");
            if (!modEnabled.Value) return;

            Config.Bind<bool>("01_fire_pit", "fire_pit_NoFuel", false, "Allow Fire Pit to burn without fuel");
            Config.Bind<string>("01_fire_pit", "fire_pit_FuelType", "Wood", "Fuel type for Fire Pit. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("01_fire_pit", "fire_pit_MaxFuel", 10f, "Maximum fuel level for Fire Pit");
            Config.Bind<float>("01_fire_pit", "fire_pit_StartFuel", 1f, "Start fuel level for Fire Pit");
            Config.Bind<float>("01_fire_pit", "fire_pit_FuelTimeToBurn", 5000f, "Time for Fire Pit to burn 1 fuel (sec)");

            Config.Bind<bool>("02_bonfire", "bonfire_NoFuel", false, "Allow Bonfire to burn without fuel");
            Config.Bind<string>("02_bonfire", "bonfire_FuelType", "Wood", "Fuel type for Bonfire. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("02_bonfire", "bonfire_MaxFuel", 10f, "Maximum fuel level for Bonfire");
            Config.Bind<float>("02_bonfire", "bonfire_StartFuel", 0f, "Start fuel level for Bonfire");
            Config.Bind<float>("02_bonfire", "bonfire_FuelTimeToBurn", 5000f, "Time for Bonfire to burn 1 fuel (sec)");

            Config.Bind<bool>("03_hearth", "hearth_NoFuel", false, "Allow Hearth to burn without fuel");
            Config.Bind<string>("03_hearth", "hearth_FuelType", "Wood", "Fuel type for Hearth. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("03_hearth", "hearth_MaxFuel", 20f, "Maximum fuel level for Hearth");
            Config.Bind<float>("03_hearth", "hearth_StartFuel", 0f, "Start fuel level for Hearth");
            Config.Bind<float>("03_hearth", "hearth_FuelTimeToBurn", 5000f, "Time for Hearth to burn 1 fuel (sec)");

            Config.Bind<bool>("04_wood_torch", "wood_torch_NoFuel", false, "Allow Standing Wood Torch to burn without fuel");
            Config.Bind<string>("04_wood_torch", "wood_torch_FuelType", "Resin", "Fuel type for Standing Wood Torch. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("04_wood_torch", "wood_torch_MaxFuel", 4f, "Maximum fuel level for Standing Wood Torch");
            Config.Bind<float>("04_wood_torch", "wood_torch_StartFuel", 2f, "Start fuel level for Standing Wood Torch");
            Config.Bind<float>("04_wood_torch", "wood_torch_FuelTimeToBurn", 10000f, "Time for Standing Wood Torch to burn 1 fuel (sec)");

            Config.Bind<bool>("05_iron_torch", "iron_torch_NoFuel", false, "Allow Standing Iron Torch to burn without fuel");
            Config.Bind<string>("05_iron_torch", "iron_torch_FuelType", "Resin", "Fuel type for Standing Iron Torch pit. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("05_iron_torch", "iron_torch_MaxFuel", 6f, "Maximum fuel level for Standing Iron Torch pit");
            Config.Bind<float>("05_iron_torch", "iron_torch_StartFuel", 2f, "Start fuel level for Standing Iron Torch pit");
            Config.Bind<float>("05_iron_torch", "iron_torch_FuelTimeToBurn", 20000f, "Time for Standing Iron Torch pit to burn 1 fuel (sec)");

            Config.Bind<bool>("06_green_torch", "green_torch_NoFuel", false, "Allow Standing Green Torch to burn without fuel");
            Config.Bind<string>("06_green_torch", "green_torch_FuelType", "Guck", "Fuel type for Standing Green Torch. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("06_green_torch", "green_torch_MaxFuel", 6f, "Maximum fuel level for Standing Green Torch");
            Config.Bind<float>("06_green_torch", "green_torch_StartFuel", 2f, "Start fuel level for Standing Green Torch");
            Config.Bind<float>("06_green_torch", "green_torch_FuelTimeToBurn", 20000f, "Time for Standing Green Torch to burn 1 fuel (sec)");

            Config.Bind<bool>("07_sconce", "sconce_NoFuel", false, "Allow Sconce to burn without fuel");
            Config.Bind<string>("07_sconce", "sconce_FuelType", "Resin", "Fuel type for Sconce. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("07_sconce", "sconce_MaxFuel", 6f, "Maximum fuel level for Sconce");
            Config.Bind<float>("07_sconce", "sconce_StartFuel", 2f, "Start fuel level for Sconce");
            Config.Bind<float>("07_sconce", "sconce_FuelTimeToBurn", 20000f, "Time for Sconce to burn 1 fuel (sec)");

            Config.Bind<bool>("08_brazier", "brazier_NoFuel", false, "Allow Brazier to burn without fuel");
            Config.Bind<string>("08_brazier", "brazier_FuelType", "Coal", "Fuel type for Brazier. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            Config.Bind<float>("08_brazier", "brazier_MaxFuel", 5f, "Maximum fuel level for Brazier");
            Config.Bind<float>("08_brazier", "brazier_StartFuel", 1f, "Start fuel level for Brazier");
            Config.Bind<float>("08_brazier", "brazier_FuelTimeToBurn", 20000f, "Time for Brazier to burn 1 fuel (sec)");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        }

        private void Update()
        {
            if (!modEnabled.Value || !extinguishableFires.Value || Player.m_localPlayer == null) return;

            if (Input.GetKeyDown(toggleFireKey.Value.ToLower()))
            {
                try
                {
                    ConfigureFuelComponent configureFuelComponent = null;
                    GameObject hoverObj = Player.m_localPlayer.GetHoverObject();
                    if (hoverObj.GetComponent<ConfigureFuelComponent>() != null)
                    {
                        configureFuelComponent = hoverObj.GetComponent<ConfigureFuelComponent>();
                    } 
                    else
                    {
                        while (hoverObj.transform.parent != null)
                        {
                            hoverObj = hoverObj.transform.parent.gameObject;
                            if (hoverObj.GetComponent<ConfigureFuelComponent>() != null)
                            {
                                configureFuelComponent = hoverObj.GetComponent<ConfigureFuelComponent>();
                            }
                        }
                    }

                    if (configureFuelComponent.GetCurrentFuel() <= 0f)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, Localization.instance.Localize("Add Fuel To Light Fire"));
                    }
                    else
                    {
                        configureFuelComponent.SetToggledOn(!configureFuelComponent.GetToggledOn());                        
                        ((ZSyncAnimation)typeof(Player).GetField("m_zanim", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Player.m_localPlayer)).SetTrigger("interact");
                    }                    
                } catch { 
                    //Do Nothing
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
                } else if (__instance.name.Contains("groundtorch")) {
                    if (__instance.name.Contains("wood")) {
                        fireName = "wood_torch";
                        tabName = "04_wood_torch";
                    } else if (__instance.name.Contains("green")) {
                        fireName = "green_torch";
                        tabName = "06_green_torch";
                    } else {
                        fireName = "iron_torch";
                        tabName = "05_iron_torch";
                    }
                } else if (__instance.name.Contains("walltorch")) {
                    fireName = "sconce";
                    tabName = "07_sconce";
                } else if (__instance.name.Contains("brazier")) {
                    fireName = "brazier";
                    tabName = "08_brazier";
                }

                if (fireName != null && fireName != "")
                {
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.AddComponent<ConfigureFuelComponent>();
                    if (!allNoFuel.Value)
                    {
                        context.Config.TryGetEntry<bool>($"{tabName}", $"{fireName}_NoFuel", out fireNoFuel);

                        if (!fireNoFuel.Value)
                        {
                            context.Config.TryGetEntry<string>($"{tabName}", $"{fireName}_FuelType", out fireFuelType);
                            context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_MaxFuel", out fireMaxFuel);
                            context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_StartFuel", out fireStartFuel);
                            context.Config.TryGetEntry<float>($"{tabName}", $"{fireName}_FuelTimeToBurn", out fireFuelTimeToBurn);

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
                        else
                        {
                            ___m_startFuel = ___m_maxFuel;
                            configureFuelComponent.SetDoesNotRequireFuel(true);
                        }
                    }
                    else
                    {
                        ___m_startFuel = ___m_maxFuel;
                        configureFuelComponent.SetDoesNotRequireFuel(true);
                    }
                    
                    configureFuelComponent.SetFuelType(__instance.m_fuelItem.gameObject);

                    try
                    {
                        ZNetView m_nview = __instance.gameObject.GetComponent<ZNetView>();
                        configureFuelComponent.SetToggledOn(m_nview.GetZDO().GetBool("toggleState"));
                    }
                    catch { }
                    
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
                if (modEnabled.Value && __instance.gameObject.GetComponent<ConfigureFuelComponent>() != null)
                {
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.GetComponent<ConfigureFuelComponent>();
                    if (configureFuelComponent != null)
                    {
                        if (configureFuelComponent.GetDoesNotRequireFuel())
                        {
                            user.Message(MessageHud.MessageType.Center, Localization.instance.Localize("Fire Does Not Require Fuel"));
                            __result = false;
                            return false;
                        }
                        if (extinguishableFires.Value && !configureFuelComponent.GetToggledOn())
                        {
                            Inventory inventory = user.GetInventory();
                            if (inventory != null)
                            {
                                if (inventory.HaveItem(__instance.m_fuelItem.m_itemData.m_shared.m_name))
                                {
                                    if (configureFuelComponent.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("fuel") > __instance.m_maxFuel -1f)
                                    {
                                        user.Message(MessageHud.MessageType.Center, Localization.instance.Localize("$msg_cantaddmore", __instance.m_fuelItem.m_itemData.m_shared.m_name));
                                        __result = false;
                                        return false;
                                    }
                                    else
                                    {
                                        user.Message(MessageHud.MessageType.Center, Localization.instance.Localize("$msg_fireadding", __instance.m_fuelItem.m_itemData.m_shared.m_name));
                                        inventory.RemoveItem(__instance.m_fuelItem.m_itemData.m_shared.m_name, 1);
                                        ___m_nview.InvokeRPC("AddFuel");
                                        __result = true;
                                        return false;
                                    }
                                }
                                user.Message(MessageHud.MessageType.Center, "$msg_outof " + __instance.m_fuelItem.m_itemData.m_shared.m_name);
                                __result = false;
                                return false;
                            }
                        }
                    }
                }
                return true;               
            }
        }

        [HarmonyPatch(typeof(Fireplace), "UpdateFireplace")]
        static class Fireplace_UpdateFireplace_Patch
        {
            static void Prefix(Fireplace __instance)
            {
                if (modEnabled.Value && __instance.gameObject.GetComponent<ConfigureFuelComponent>() != null)
                {
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.GetComponent<ConfigureFuelComponent>();
                    configureFuelComponent.SetCurrentFuel(configureFuelComponent.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("fuel"));
                    if (extinguishableFires.Value && configureFuelComponent.GetToggledOn())
                    {
                        if (configureFuelComponent.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("fuel") <= 0f) configureFuelComponent.SetToggledOn(false);
                    }
                    if (configureFuelComponent.GetDoesNotRequireFuel() &&
                        (!extinguishableFires.Value || (extinguishableFires.Value && configureFuelComponent.GetToggledOn())))
                    {
                        __instance.gameObject.GetComponent<ZNetView>().GetZDO().Set("fuel", 1f);
                    }                    
                }
            }
        }

        [HarmonyPatch(typeof(Fireplace), "IsBurning")]
        static class Fireplace_IsBurning_Patch
        {
            static bool Prefix(Fireplace __instance, ref bool __result)
            {
                if (modEnabled.Value && __instance.gameObject.GetComponent<ConfigureFuelComponent>() != null)
                {
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.GetComponent<ConfigureFuelComponent>();
                    if (!configureFuelComponent.GetToggledOn())
                    {
                        __result = false;
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Fireplace), "GetHoverText")]
        static class Fireplace_GetHoverText_Patch
        {
            static void Postfix(Fireplace __instance, ref string __result, ZNetView ___m_nview)
            {
                if (modEnabled.Value && __instance.gameObject.GetComponent<ConfigureFuelComponent>() != null)
                {
                    ConfigureFuelComponent configureFuelComponent = __instance.gameObject.GetComponent<ConfigureFuelComponent>();
                    if (configureFuelComponent != null)
                    {
                        if (extinguishableFires.Value)
                        {
                            float @float = ___m_nview.GetZDO().GetFloat("fuel");
                            string str = "Extinguish Fire";
                            string colour = "yellow";

                            if (configureFuelComponent.GetDoesNotRequireFuel())
                            {
                                if (!configureFuelComponent.GetToggledOn()) str = "Light Fire";
                                __result = Localization.instance.Localize(__instance.m_name + " ( \u221E )\n[<color=" + colour + "><b>" + toggleFireKey.Value + "</b></color>] " + str + "\n[<color=yellow><b>1-8</b></color>] $piece_useitem");
                                return;
                            }

                            if (!configureFuelComponent.GetToggledOn())
                            {
                                str = "Light Fire";
                            }
                            if (configureFuelComponent.gameObject.GetComponent<ZNetView>().GetZDO().GetFloat("fuel") <= 0f)
                            {
                                colour = "grey";
                            }

                            __result = Localization.instance.Localize(__instance.m_name + " ( $piece_fire_fuel " + Mathf.Ceil(@float) + "/" + (int)__instance.m_maxFuel + " )\n[<color=yellow><b>$KEY_Use</b></color>] $piece_use " + __instance.m_fuelItem.m_itemData.m_shared.m_name + "\n[<color=" + colour + "><b>" + toggleFireKey.Value + "</b></color>] " + str + "\n[<color=yellow><b>1-8</b></color>] $piece_useitem");
                            return;

                        }

                        if (configureFuelComponent.GetDoesNotRequireFuel())
                        {
                            __result = Localization.instance.Localize(__instance.m_name + " ( \u221E )\n[<color=yellow><b>1-8</b></color>] $piece_useitem");
                            return;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ZNetScene), "Shutdown")]
        static class ZNetScene_Shutdown_Patch
        {
            static void Prefix()
            {
                if (modEnabled.Value)
                {
                    ConfigureFuelComponent[] configureFuelComponents = FindObjectsOfType<ConfigureFuelComponent>();
                    foreach (ConfigureFuelComponent configureFuelComponent in configureFuelComponents)
                    {
                        ZNetView zNetView = configureFuelComponent.gameObject.GetComponent<ZNetView>();
                        zNetView.GetZDO().Set("toggleState", configureFuelComponent.GetToggledOn());
                    }
                }
            }
        }

        public class ConfigureFuelComponent : MonoBehaviour
        {
            private bool toggledOn = true;
            private bool doesNotRequireFuel = false;
            private float currentFuel = 0f;
            private float startFuel = 0f;
            private GameObject fuelType;

            public bool GetToggledOn() { return toggledOn; }
            public void SetToggledOn(bool togOn) { toggledOn = togOn; }

            public bool GetDoesNotRequireFuel() { return doesNotRequireFuel; }
            public void SetDoesNotRequireFuel(bool reqFuel) { doesNotRequireFuel = reqFuel; }            
            
            public float GetCurrentFuel() { return currentFuel; }
            public void SetCurrentFuel(float curFuel) { currentFuel = curFuel; }

            public void SetFuelType(GameObject fueltyp) { fuelType = fueltyp; }

            private void OnDestroy()
            {
                if (!doesNotRequireFuel && dropFuel.Value)
                {
                    int count = (int)(currentFuel - startFuel);
                    Debugger($"Count: {count}");
                    while (count > 0)
                    {
                        ItemDrop component = Object.Instantiate(fuelType, this.transform.position + Vector3.up, Quaternion.identity).GetComponent<ItemDrop>();
                        component.SetStack(Mathf.Min(count, component.m_itemData.m_shared.m_maxStackSize));
                        count -= component.m_itemData.m_stack;
                    }
                }
            }
        }
    }
}
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

//WIP

namespace ValheimMod
{
    [BepInPlugin("goonlou.ValheimMod", "Valheim Mod", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class LockPortalTag : BaseUnityPlugin
    {
        private static readonly bool isDebug = true;

        public static ConfigEntry<int> nexusID;
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> unclaimedOnly;
        public static ConfigEntry<string> modKey;

        private static LockPortalTag context;

        public static void Dbgl(string str = "", bool pref = true)
        {
            if (isDebug)
                Debug.Log((pref ? typeof(LockPortalTag).Namespace + " " : "") + str);
        }
        private void Awake()
        {
            context = this;
            modEnabled = Config.Bind<bool>("General", "Enabled", true, "Enable this mod");
            //unclaimedOnly = Config.Bind<bool>("General", "UnclaimedOnly", false, "Only sleep on unclaimed beds");
            modKey = Config.Bind<string>("General", "ModKey", "l", "Modifier key to sleep without setting spawn point. Use https://docs.unity3d.com/Manual/ConventionalGameInput.html");
            //nexusID = Config.Bind<int>("General", "NexusID", 261, "Nexus mod ID for updates");

            if (!modEnabled.Value)
                return;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

        }

        [HarmonyPatch(typeof(Teleport), "GetHoverText")]
        static class Bed_GetHoverText_Patch
        {
            static void Postfix(Teleport __instance, ref string __result)
            {
                __result += Localization.instance.Localize($"\n[{modKey.Value}+<color=yellow><b>$KEY_Use</b></color>] To Lock Portal Tag");
                Dbgl(__result);
            }
        }


        [HarmonyPatch(typeof(Teleport), "Interact")]
        static class Bed_Interact_Patch
        {
            static bool Prefix()
            {
                if (!Utils.CheckKeyHeld(modKey.Value))
                {
                    Dbgl($"lock portal");
                    return true;
                }
                return false;
            }
        }



        [HarmonyPatch(typeof(Console), "InputText")]
        static class InputText_Patch
        {
            static bool Prefix(Console __instance)
            {
                if (!modEnabled.Value)
                    return true;
                string text = __instance.m_input.text;
                if (text.ToLower().Equals("lockmod reset"))
                {
                    context.Config.Reload();
                    context.Config.Save();
                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { "LockPortalTag config reloaded" }).GetValue();
                    return false;
                }
                return true;
            }
        }
    }
}
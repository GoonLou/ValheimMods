using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//WIP

namespace MultiFarmDrops
{
    [BepInPlugin("goonlou.ValheimMod", "Valheim Mod", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class MultiFarmDrops
    {
        private readonly Harmony harmony = new Harmony("goonlou.ValheimMod");

        private void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Pickable), "Drop")]
        static class Pickable_Patch
        {
            static void Prefix(GameObject __prefab, ref int ___offset, ref int __stack)
            {
                ItemDrop item = __prefab.GetComponent<ItemDrop>();
                Debug.Log($"Dropping: {item.m_itemData.m_shared.m_name} with stack: {__stack} and offset: {___offset}");
                
            }
        }
    }
}

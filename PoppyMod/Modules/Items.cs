using RoR2;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using PoppyMod.Survivors.Poppy;
using System.Runtime.CompilerServices;


namespace PoppyMod.Modules
{
	internal static class Items
	{
        public static ItemDef shieldyDef;
        public static ItemTierDef itemTierDef = new ItemTierDef
        {
            _tier = ItemTier.Boss,
            bgIconTexture = LegacyResourcesAPI.Load<Texture2D>("Textures/ItemIcons/BG/texBossBGIcon"),
            colorIndex = ColorCatalog.ColorIndex.BossItem,
            darkColorIndex = ColorCatalog.ColorIndex.BossItemDark,
            isDroppable = false,
            canScrap = false,
            canRestack = true,
            pickupRules = ItemTierDef.PickupRules.Default,
            highlightPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/HighlightTier1Item"),
            dropletDisplayPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ItemPickups/BossOrb"),
        };
    }
}
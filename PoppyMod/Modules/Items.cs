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
            highlightPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/HighlightTier3Item"),
            dropletDisplayPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ItemPickups/BossOrb"),
        };
        /*public static ItemDef shieldyDef = new ItemDef
        {
            name = "Shieldy",
            nameToken = PoppySurvivor.POPPY_PREFIX + "ITEM_SHIELDY_NAME",
            descriptionToken = PoppySurvivor.POPPY_PREFIX + "ITEM_SHIELDY_DESCRIPTION",
            loreToken = PoppySurvivor.POPPY_PREFIX + "ITEM_SHIELDY_LORE",
            pickupToken = PoppySurvivor.POPPY_PREFIX + "ITEM_SHIELDY_PICKUP",

            //_itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/BossTierDef.asset").WaitForCompletion(),
            _itemTierDef = GetItemTierDef(),

            pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion(),
            pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion(),

            canRemove = true,
            hidden = false,
            tags = new ItemTag[] { ItemTag.Utility },
        };

        private static ItemTierDef GetItemTierDef()
        {
            ItemTierDef newItemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/BossTierDef.asset").WaitForCompletion();
            newItemTierDef._tier = ItemTier.NoTier;
			newItemTierDef.canScrap = false;
            return newItemTierDef;
        }*/
    }
}
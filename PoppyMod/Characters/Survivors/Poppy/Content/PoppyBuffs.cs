using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyBuffs
    {
        // armor buff gained during roll
        public static BuffDef armorBuff;
        public static BuffDef steadfastSpeedBuff;
        public static BuffDef chargeSpeedBuff;

        public static void Init(AssetBundle assetBundle)
        {
            // For passive armor boost
            armorBuff = Modules.Content.CreateAndAddBuff("Steadfast Presence",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/ArmorBoost").iconSprite,
                Color.yellow,
                true,
                false);

            // For Steadfast Presence speed boost
            steadfastSpeedBuff = Modules.Content.CreateAndAddBuff("Steadfast Presence",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Color.yellow,
                true,
                false);

            // For Heroic Charge speed boost
            chargeSpeedBuff = Modules.Content.CreateAndAddBuff("Heroic Charge",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Color.yellow,
                true,
                false);
        }
    }
}

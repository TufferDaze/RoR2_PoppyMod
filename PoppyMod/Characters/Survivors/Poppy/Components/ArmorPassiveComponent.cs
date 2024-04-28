using System;
using RoR2;
using PoppyMod.Modules.Characters;
using UnityEngine;
using UnityEngine.Networking;
using PoppyMod.Survivors.Poppy;
using UnityEngine.UIElements;

namespace PoppyMod.Characters.Survivors.Poppy.Components
{
    public class ArmorPassiveComponent : NetworkBehaviour
    {
        private CharacterBody body;
        private float missingHPThreshhold = PoppyStaticValues.passiveMissingHPThreshhold;
        private float armorIncreaseCoef = PoppyConfig.passiveConfig.Value;

        public void Awake()
        {
            body = GetComponent<CharacterBody>();
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                FixedServerUpdate();
            }
        }

        private void FixedServerUpdate()
        {
            float healthPercentage = GetCurrentHealthPercent();
            if (healthPercentage <= missingHPThreshhold || body.HasBuff(PoppyBuffs.armorBuff))
            {
                ManagePassiveStack();
            }
        }

        private void ManagePassiveStack()
        {
            float graphOffset = 1.25f * armorIncreaseCoef;
            float graphSlope = 2.5f * armorIncreaseCoef;
            float healthPercentage = GetCurrentHealthPercent();

            int maxStacks = (int)Mathf.Clamp(graphOffset - graphSlope * healthPercentage, 0f, armorIncreaseCoef);
            int currentStacks = body.GetBuffCount(PoppyBuffs.armorBuff);

            if (currentStacks < maxStacks)
            {
                body.AddBuff(PoppyBuffs.armorBuff);
            }
            if (currentStacks > (int)armorIncreaseCoef || currentStacks > maxStacks)
            {
                body.RemoveBuff(PoppyBuffs.armorBuff);
            }
        }

        private float GetCurrentHealthPercent()
        {
            return body.healthComponent.combinedHealthFraction;
        }
    }
}
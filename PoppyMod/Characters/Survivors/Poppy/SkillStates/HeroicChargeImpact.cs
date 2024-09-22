using EntityStates;
using EntityStates.Toolbot;
using RoR2;
using UnityEngine;
//Since we are using effects from Commando's Barrage skill, we will also be using the associated namespace
//You can also use Addressables or LegacyResourcesAPI to load whichever effects you like

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class HeroicChargeImpact : BaseSkillState
    {
        public HealthComponent victimHealthComponent;
        public Vector3 idealDirection;
        public float damageBoostFromSpeed;
        public bool isCrit;

        //OnEnter() runs once at the start of the skill
        //All we do here is create a BulletAttack and fire it
        public override void OnEnter()
        {
            base.OnEnter();
            if (this.victimHealthComponent)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    attacker = base.gameObject,
                    damage = this.damageStat * HeroicChargeDash.knockbackDamageCoefficient * this.damageBoostFromSpeed,
                    crit = this.isCrit,
                    procCoefficient = 1f,
                    damageColorIndex = DamageColorIndex.Item,
                    damageType = DamageType.Stun1s,
                    position = base.characterBody.corePosition
                };
                this.victimHealthComponent.TakeDamage(damageInfo);
                GlobalEventManager.instance.OnHitEnemy(damageInfo, this.victimHealthComponent.gameObject);
                GlobalEventManager.instance.OnHitAll(damageInfo, this.victimHealthComponent.gameObject);
            }
            //base.healthComponent.TakeDamageForce(this.idealDirection * -HeroicChargeDash.knockbackForce, true, false);
            if (base.isAuthority)
            {
                base.AddRecoil(-0.5f * HeroicChargeDash.recoilAmplitude * 3f, -0.5f * HeroicChargeDash.recoilAmplitude * 3f, -0.5f * HeroicChargeDash.recoilAmplitude * 8f, 0.5f * HeroicChargeDash.recoilAmplitude * 3f);
                EffectManager.SimpleImpactEffect(ToolbotDash.knockbackEffectPrefab, base.characterBody.corePosition, base.characterDirection.forward, true);
                this.outer.SetNextStateToMain();
            }
        }

        //GetMinimumInterruptPriority() returns the InterruptPriority required to interrupt this skill
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
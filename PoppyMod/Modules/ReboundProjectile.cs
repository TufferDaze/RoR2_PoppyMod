using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.UIElements;

namespace PoppyMod.Modules
{
    public class ReboundProjectile : Orb
    {
        public static event Action<ReboundProjectile> onLightningOrbKilledOnAllBounces;
        public float speed = 80f;
        public float damageValue;
        public GameObject attacker;
        public GameObject inflictor;
        public GameObject prefab;
        public int bouncesRemaining;
        public List<HealthComponent> bouncedObjects;
        public TeamIndex teamIndex;
        public bool isCrit;
        public ProcChainMask procChainMask;
        public float procCoefficient;
        public DamageColorIndex damageColorIndex;
        public float range;
        public float damageCoefficientPerBounce;
        public int targetsToFindPerBounce = 1;
        public DamageType damageType;
        private bool canBounceOnSameTarget;
        private bool failedToKill;
        private BullseyeSearch search;

        public override void Begin()
        {
            base.duration = 0.1f;
            string path = "Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect";
            base.duration = base.distanceToTarget / this.speed;
            this.canBounceOnSameTarget = true;
            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };
            effectData.SetHurtBoxReference(this.target);
            prefab = LegacyResourcesAPI.Load<GameObject>(path);
            if (prefab)
            {
                EffectManager.SpawnEffect(prefab, effectData, true);
            }
            else
            {
                Debug.LogError("ReboundProjectile: Error getting huntress glaive prefab!");
            }
        }

        public override void OnArrival()
        {
            if (this.target)
            {
                Transform lastLocation = target.transform;
                HealthComponent healthComponent = this.target.healthComponent;
                if (healthComponent)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = this.damageValue;
                    damageInfo.attacker = this.attacker;
                    damageInfo.inflictor = this.inflictor;
                    damageInfo.force = Vector3.zero;
                    damageInfo.crit = this.isCrit;
                    damageInfo.procChainMask = this.procChainMask;
                    damageInfo.procCoefficient = this.procCoefficient;
                    damageInfo.position = this.target.transform.position;
                    damageInfo.damageColorIndex = this.damageColorIndex;
                    damageInfo.damageType = this.damageType;
                    healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
                }
                this.failedToKill |= (!healthComponent || healthComponent.alive);
                if (this.bouncesRemaining > 0)
                {
                    for (int i = 0; i < this.targetsToFindPerBounce; i++)
                    {
                        if (this.bouncedObjects != null)
                        {
                            if (this.canBounceOnSameTarget)
                            {
                                this.bouncedObjects.Clear();
                            }
                            this.bouncedObjects.Add(this.target.healthComponent);
                        }
                        HurtBox hurtBox = this.PickNextTarget(this.target.transform.position);
                        if (hurtBox)
                        {
                            ReboundProjectile projectile = new ReboundProjectile();
                            projectile.search = this.search;
                            projectile.origin = this.target.transform.position;
                            projectile.target = hurtBox;
                            projectile.attacker = this.attacker;
                            projectile.inflictor = this.inflictor;
                            projectile.teamIndex = this.teamIndex;
                            projectile.damageValue = this.damageValue * this.damageCoefficientPerBounce;
                            projectile.bouncesRemaining = this.bouncesRemaining - 1;
                            projectile.isCrit = this.isCrit;
                            projectile.bouncedObjects = this.bouncedObjects;
                            projectile.procChainMask = this.procChainMask;
                            projectile.procCoefficient = this.procCoefficient;
                            projectile.damageColorIndex = this.damageColorIndex;
                            projectile.damageCoefficientPerBounce = this.damageCoefficientPerBounce;
                            projectile.speed = this.speed;
                            projectile.range = this.range;
                            projectile.damageType = this.damageType;
                            projectile.failedToKill = this.failedToKill;
                            OrbManager.instance.AddOrb(projectile);
                        }
                        else
                        {
                            //SpawnSheildy(lastLocation);
                        }
                    }
                    return;
                }
                else
                {
                    //SpawnSheildy(lastLocation);
                }
                if (!this.failedToKill)
                {
                    Action<ReboundProjectile> action = ReboundProjectile.onLightningOrbKilledOnAllBounces;
                    if (action == null)
                    {
                        return;
                    }
                    action(this);
                }
            }
        }

        public HurtBox PickNextTarget(Vector3 position)
        {
            if (this.search == null)
            {
                this.search = new BullseyeSearch();
            }
            this.search.searchOrigin = position;
            this.search.searchDirection = Vector3.zero;
            this.search.teamMaskFilter = TeamMask.allButNeutral;
            this.search.teamMaskFilter.RemoveTeam(this.teamIndex);
            this.search.filterByLoS = false;
            this.search.sortMode = BullseyeSearch.SortMode.Distance;
            this.search.maxDistanceFilter = this.range;
            this.search.RefreshCandidates();
            HurtBox hurtBox = (from v in this.search.GetResults()
                               where !this.bouncedObjects.Contains(v.healthComponent)
                               select v).FirstOrDefault<HurtBox>();
            if (hurtBox)
            {
                this.bouncedObjects.Add(hurtBox.healthComponent);
            }
            return hurtBox;
        }

        private void SpawnSheildy(Transform enemy)
        {
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex(Items.shieldyDef.name)), enemy.position, enemy.forward * 20f);
        }
    }
}
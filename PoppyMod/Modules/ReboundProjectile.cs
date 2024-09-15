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
        public DamageType damageType;
        private bool failedToKill;
        private bool dealDamage = true;
        private BullseyeSearch search;

        public override void Begin()
        {
            base.duration = 0.1f;
            string path = "Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect";
            base.duration = base.distanceToTarget / speed;
            EffectData effectData = new EffectData
            {
                origin = origin,
                genericFloat = base.duration
            };
            effectData.SetHurtBoxReference(target);
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
            if (target)
            {
                Transform lastLocation = target.transform;
                HealthComponent healthComponent = target.healthComponent;
                if (healthComponent && dealDamage)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = damageValue;
                    damageInfo.attacker = attacker;
                    damageInfo.inflictor = inflictor;
                    damageInfo.force = Vector3.zero;
                    damageInfo.crit = isCrit;
                    damageInfo.procChainMask = procChainMask;
                    damageInfo.procCoefficient = procCoefficient;
                    damageInfo.position = target.transform.position;
                    damageInfo.damageColorIndex = damageColorIndex;
                    damageInfo.damageType = damageType;
                    healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
                }
                failedToKill = healthComponent.alive;
                if (failedToKill && bouncesRemaining > 0)
                {
                    if (bouncedObjects != null)
                    {
                        bouncedObjects.Clear();
                        bouncedObjects.Add(target.healthComponent);
                    }
                    HurtBox hurtBox = PickNextTarget(target.transform.position);
                    if (hurtBox)
                    {
                        ReboundProjectile projectile = new ReboundProjectile();
                        projectile.search = search;
                        projectile.origin = target.transform.position;
                        projectile.target = hurtBox;
                        projectile.attacker = attacker;
                        projectile.inflictor = inflictor;
                        projectile.teamIndex = teamIndex;
                        projectile.damageValue = damageValue * damageCoefficientPerBounce;
                        projectile.bouncesRemaining = bouncesRemaining - 1;
                        projectile.isCrit = isCrit;
                        projectile.bouncedObjects = bouncedObjects;
                        projectile.procChainMask = procChainMask;
                        projectile.procCoefficient = procCoefficient;
                        projectile.damageColorIndex = damageColorIndex;
                        projectile.damageCoefficientPerBounce = damageCoefficientPerBounce;
                        projectile.speed = speed;
                        projectile.range = range;
                        projectile.damageType = damageType;
                        projectile.failedToKill = failedToKill;
                        projectile.dealDamage = true;
                        OrbManager.instance.AddOrb(projectile);
                    }
                    else
                    {
                        //Debug.Log("HURTBOX NULL");
                        SpawnSheildy(lastLocation);
                    }
                    return;
                }
                else if (failedToKill)
                {
                    if (dealDamage)
                    {
                        SpawnSheildy(lastLocation);
                    }
                    else
                    {
                        SpawnSheildy(lastLocation, 0f);
                    }
                    //Debug.Log("FAILED TO KILL");
                }
                else if (bouncesRemaining > -1)
                {
                    ReboundProjectile projectile = new ReboundProjectile();
                    projectile.search = search;
                    projectile.origin = lastLocation.position;
                    projectile.target = attacker.GetComponent<CharacterBody>().mainHurtBox;
                    projectile.attacker = attacker;
                    projectile.inflictor = inflictor;
                    projectile.teamIndex = teamIndex;
                    projectile.damageValue = 0;
                    projectile.bouncesRemaining = -1;
                    projectile.isCrit = false;
                    projectile.bouncedObjects = bouncedObjects;
                    projectile.procChainMask.mask = 0u;
                    projectile.procCoefficient = 0f;
                    projectile.damageColorIndex = damageColorIndex;
                    projectile.damageCoefficientPerBounce = 0f;
                    projectile.speed = speed * 1.5f;
                    projectile.range = 9999;
                    projectile.damageType = DamageType.NonLethal;
                    projectile.failedToKill = failedToKill;
                    projectile.dealDamage = false;
                    OrbManager.instance.AddOrb(projectile);
                }
            }
        }

        public HurtBox PickNextTarget(Vector3 position)
        {
            if (search == null)
            {
                search = new BullseyeSearch();
            }
            search.searchOrigin = position;
            search.searchDirection = Vector3.zero;
            search.teamMaskFilter = TeamMask.allButNeutral;
            search.teamMaskFilter.RemoveTeam(teamIndex);
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = range;
            search.RefreshCandidates();
            HurtBox hurtBox = (from v in search.GetResults()
                               where !bouncedObjects.Contains(v.healthComponent)
                               select v).FirstOrDefault<HurtBox>();
            if (hurtBox)
            {
                bouncedObjects.Add(hurtBox.healthComponent);
            }
            return hurtBox;
        }

        private void SpawnSheildy(Transform target, float velocity = 20f)
        {
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex(Items.shieldyDef.name)), target.position, target.forward * velocity);
        }
    }
}
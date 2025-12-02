using PoppyMod.Survivors.Poppy;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PoppyMod.Modules
{
    public class ShieldyProjectile : Orb
    {
        public float speed;
        public float damageValue;
        public GameObject attacker;
        public GameObject inflictor;
        public GameObject prefab;
        public int bouncesRemaining;
        public List<HealthComponent> bouncedObjects;
        public TeamIndex teamIndex;
        public bool isCrit;
        public float procCoefficient;
        public float range;
        public float damageCoefficientPerBounce;
        public DamageType damageType;
        private bool failedToKill;
        public bool killConfirmed;
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
                Debug.LogError("ShieldyProjectile: Error getting huntress glaive prefab!");
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
                    damageInfo.procCoefficient = procCoefficient;
                    damageInfo.crit = isCrit;
                    damageInfo.damage = damageValue;
                    damageInfo.damageType = damageType;
                    damageInfo.attacker = attacker;
                    damageInfo.inflictor = inflictor;
                    //damageInfo.force = Vector3.zero;
                    damageInfo.position = target.transform.position;
                    healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
                }
                failedToKill = healthComponent.alive;
                if (!failedToKill)
                {
                    killConfirmed = true;
                }
                if (bouncesRemaining > 0)
                {
                    if (bouncedObjects != null)
                    {
                        bouncedObjects.Clear();
                        bouncedObjects.Add(target.healthComponent);
                    }
                    HurtBox hurtBox = PickNextTarget(target.transform.position);
                    if (hurtBox)
                    {
                        ShieldyProjectile projectile = new ShieldyProjectile();
                        projectile.search = search;
                        projectile.procCoefficient = procCoefficient;
                        projectile.isCrit = isCrit;
                        projectile.speed = speed;
                        projectile.range = range;
                        projectile.damageValue = damageValue * damageCoefficientPerBounce;
                        projectile.damageCoefficientPerBounce = damageCoefficientPerBounce;
                        projectile.bouncesRemaining = bouncesRemaining - 1;
                        projectile.attacker = attacker;
                        projectile.inflictor = inflictor;
                        projectile.teamIndex = teamIndex;
                        projectile.bouncedObjects = bouncedObjects;
                        projectile.damageType = damageType;
                        projectile.origin = target.transform.position;
                        projectile.target = hurtBox;
                        projectile.failedToKill = failedToKill;
                        projectile.killConfirmed = killConfirmed;
                        projectile.dealDamage = true;
                        OrbManager.instance.AddOrb(projectile);
                    }
                    else if (!killConfirmed)
                    {
                        //Debug.LogWarning($"ReboundProjectile: {bouncesRemaining} bounces left. No additional targets found. Did not kill.");
                        SpawnShieldy(lastLocation);
                    }
                    else
                    {
                        //Debug.LogWarning($"ReboundProjectile: {bouncesRemaining} bounces left. No additional targets found. Did kill. Returning to player.");
                        ReboundToPlayer(lastLocation);
                    }
                    //return;
                }
                else if (!killConfirmed)
                {
                    //Debug.Log("ReboundProjectile: No bounces left. Did not kill.");
                    SpawnShieldy(lastLocation);
                }
                else if (bouncesRemaining > -1)
                {
                    //Debug.Log("ReboundProjectile: No bounces left. Did kill. Has not returned to player.");
                    ReboundToPlayer(lastLocation);
                }
                else
                {
                    //Debug.Log("ReboundProjectile: No bounces left. Did kill. Has returned to player.");
                    //SpawnSheildy(lastLocation, 0f);
                    GiveShieldy();
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

        private void ReboundToPlayer(Transform lastLocation)
        {
            ShieldyProjectile projectile = new ShieldyProjectile();
            projectile.search = search;
            projectile.procCoefficient = 0f;
            projectile.isCrit = false;
            projectile.speed = speed * 1.5f;
            projectile.range = 9999f;
            projectile.damageValue = 0f;
            projectile.damageCoefficientPerBounce = 0f;
            projectile.bouncesRemaining = -1;
            projectile.attacker = attacker;
            projectile.inflictor = inflictor;
            projectile.damageType = DamageType.NonLethal;
            projectile.teamIndex = teamIndex;
            projectile.bouncedObjects = bouncedObjects;
            projectile.origin = lastLocation.position;
            projectile.target = attacker.GetComponent<CharacterBody>().mainHurtBox;
            projectile.failedToKill = failedToKill;
            projectile.killConfirmed = killConfirmed;
            projectile.dealDamage = false;
            OrbManager.instance.AddOrb(projectile);
        }

        private void SpawnShieldy(Transform target, float velocity = 20f)
        {
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex(PoppySurvivor.shieldyDef.name)), target.position, target.forward * velocity);
        }

        private void GiveShieldy()
        {
            attacker.GetComponent<CharacterBody>().inventory.GiveItemTemp(ItemCatalog.FindItemIndex(PoppySurvivor.shieldyDef.name), 0.001f);
            //attacker.GetComponent<CharacterBody>().inventory.GiveItem(ItemCatalog.FindItemIndex(PoppySurvivor.shieldyDef.name));
        }
    }
}
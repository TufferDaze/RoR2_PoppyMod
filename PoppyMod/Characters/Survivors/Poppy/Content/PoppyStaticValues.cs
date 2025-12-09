namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyStaticValues
    {
        // Base Values
        public const int baseHealth = 180;

        public const int baseArmor = 20;

        public const float baseDamage = 12f;

        public const float baseCrit = 1f;

        public const float baseAttackSpeed = 1f;

        public const float baseMoveSpeed = 7f;

        // Passive Values
        public const float passiveMissingHPThreshhold = 0.5f;

        public const float passiveArmorIncreaseCoefficient = 20f;

        // Primary Values
        public const float primaryProcCoefficient = 1f;

        public const float primaryDamageCoefficient = 3.4f;

        public const float primaryDamageBonusCoefficient = 0.5f; // Percent dmg per 25 bonus max HP

        public const float primaryDamageBonusHPRequirement = 25f;

        // Secondary Values
        public const float secondaryProcCoefficient = 1f;

        public const float secondaryDamageCoefficient = 2.8f;

        public const float secondaryBounceDamageCoefficient = 1.33f; // Added dmg per bounce

        public const float secondaryLockOnRange = 60f; // Huntress indicator range

        public const float secondaryBounceRange = 30f; // Range to seek next bounce target

        public const float secondaryTravelSpeed = 60f;

        public const float secondaryReturnSpeed = 90f;

        public const float secondaryHPBarrierCoefficient = 0.2f; // Percent max HP barrier on Shieldy pickup

        // Utility Values
        public const float utilityProcCoefficient = 1f;

        public const float utility1DamageCoefficient = 3.2f;

        public const float utility1MoveCoefficient = 0.5f;

        public const float utility1BuffDuration = 2f;

        public const float utility2ProcCoefficient = 0.6f;

        public const float utility2DamageCoefficient = 3f;

        public const float utility2MoveCoefficient = 0.4f;

        // Special #1 Values
        public const float special1SlamProcCoefficient = 1f;

        public const float special1WaveProcCoefficient = 0.1f;

        public const float special1MinDamageCoefficient = 8f;

        public const float special1MaxDamageCoefficient = 28f;

        public const float special1FireForce = 4000f; // Upward force of uncharged Keeper's

        public const float special1ChargeSlamForce = 10000f; // Upward force of charged Keeper's

        public const float special1ChargeWaveForce = 4000f; // Upward force of Keeper's charged waves

        // Special #2 Values
        public const float special2ProcCoefficient = 0.7f;

        public const float special2DamageCoefficient = 26f;

        public const float special2HPDamageCoefficient = 0.1f; // Percent enemy max HP to deal as dmg
    }
}
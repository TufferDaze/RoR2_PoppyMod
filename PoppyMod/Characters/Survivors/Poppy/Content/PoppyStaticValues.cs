namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyStaticValues
    {
        // Base Values
        public const int baseHealth = 160;

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

        // Secondary Values
        public const float secondaryProcCoefficient = 1f;

        public const float secondaryDamageCoefficient = 2.8f;

        public const float secondaryBounceDamageCoefficient = 1.33f;

        public const float secondaryHPCoefficient = 0.2f;

        // Utility Values
        public const float utilityProcCoefficient = 1f;

        public const float utility1DamageCoefficient = 4f;

        public const float utility2ProcCoefficient = 0.75f;

        public const float utility2DamageCoefficient = 1f; // Deals 3x this over 3 seconds

        public const float utility2MoveCoefficient = 0.4f;

        // Special #1 Values
        public const float special1SlamProcCoefficient = 1f;

        public const float special1WaveProcCoefficient = 0.1f;

        public const float special1MinDamageCoefficient = 8f;

        public const float special1MaxDamageCoefficient = 30f;

        public const float special1FireForce = 4000f;

        public const float special1ChargeSlamForce = 10000f;

        public const float special1ChargeWaveForce = 4000f;

        // Special #2 Values
        public const float special2ProcCoefficient = 0.6f;

        public const float special2DamageCoefficient = 26f;

        public const float special2HPDamageCoefficient = 0.1f;
    }
}
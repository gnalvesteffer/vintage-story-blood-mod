using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace XorberaxBlood
{
    internal class ModConfigLoader
    {
        private const string ModConfigFileName = "XorberaxBlood.json";
        private const int CurrentConfigVersion = 1;

        private readonly ICoreAPI _api;

        public ModConfigLoader(ICoreAPI api)
        {
            _api = api;
        }

        public ModConfig LoadConfig()
        {
            var modConfig = _api.LoadModConfig<ModConfig>(ModConfigFileName);
            if (modConfig?.ConfigVersion != CurrentConfigVersion)
            {
                modConfig = GenerateConfig();
            }
            return modConfig ?? GenerateConfig();
        }

        private ModConfig GenerateConfig()
        {
            var defaultModConfig = new ModConfig
            {
                ConfigVersion = CurrentConfigVersion,
                TickRate = 30,
                MinimumDamageRequiredToTriggerBlood = 2.0f,
                BloodDespawnDelay = 15.0f,
                BleedDuration = 20.0f,
                MinimumBloodParticlesOnHit = 5,
                MaximumBloodParticlesOnHit = 10,
                MinimumBloodParticlesOnBleed = 1,
                MaximumBloodParticlesOnBleed = 5,
                MinimumBleedDelay = 0.02f,
                MaximumBleedDelay = 3.0f,
                BloodColorRedAmount = 122,
                BloodColorGreenAmount = 25,
                BloodColorBlueAmount = 30,
                BloodColorAlphaAmount = 255,
                MinimumBloodSize = 1.0f,
                MaximumBloodSize = 1.0f,
                IgnoredDamageTypes = new HashSet<EnumDamageType>
                {
                    EnumDamageType.Fire,
                    EnumDamageType.Frost,
                    EnumDamageType.Heal,
                    EnumDamageType.Hunger,
                    EnumDamageType.Poison,
                    EnumDamageType.Suffocation,
                },
            };
            _api.StoreModConfig(defaultModConfig, ModConfigFileName);
            return defaultModConfig;
        }
    }
}

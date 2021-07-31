using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using HarmonyLib;
using Vintagestory.API.Common.Entities;

namespace XorberaxBlood
{
    [HarmonyPatch]
    public class XorberaxBloodModSystem : ModSystem
    {
        private static readonly Dictionary<Entity, EntityBleedBehavior> EntityBleedBehaviors = new Dictionary<Entity, EntityBleedBehavior>();

        public static readonly Random Random = new Random();
        public static ModConfig ModConfig { get; private set; }

        public override bool ShouldLoad(EnumAppSide appSide)
        {
            return appSide == EnumAppSide.Client;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            ModConfig = new ModConfigLoader(api).LoadConfig();
            new Harmony("xorberax.blood").PatchAll();
            api.Event.RegisterGameTickListener(OnGameTick, 1000 / ModConfig.TickRate);
        }

        private void OnGameTick(float deltaTime)
        {
            foreach (var entityBleedBehavior in EntityBleedBehaviors.ToList())
            {
                var entity = entityBleedBehavior.Key;
                var behavior = entityBleedBehavior.Value;
                behavior.Update(deltaTime);
                if (behavior.ShouldRemoveBehavior)
                {
                    EntityBleedBehaviors.Remove(entity);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EntityBehaviorHealth), nameof(EntityBehaviorHealth.OnEntityReceiveDamage))]
        private static void OnEntityReceiveDamage(
            EntityBehaviorHealth __instance,
            DamageSource damageSource,
            float damage
        )
        {
            if (
                ModConfig.IgnoredDamageTypes.Contains(damageSource.Type) ||
                damage < ModConfig.MinimumDamageRequiredToTriggerBlood
            )
            {
                return;
            }

            // Attach bleeding behavior to entity.
            if (EntityBleedBehaviors.ContainsKey(__instance.entity))
            {
                var entityBleedBehavior = EntityBleedBehaviors[__instance.entity];
                entityBleedBehavior.ResetBleedDuration();
            }
            else
            {
                EntityBleedBehaviors.Add(__instance.entity, new EntityBleedBehavior(__instance, ModConfig.BleedDuration));
            }

            // Make blood drop from damage.
            var particles = new SimpleParticleProperties(
                ModConfig.MinimumBloodParticlesOnHit,
                ModConfig.MaximumBloodParticlesOnHit,
                ColorUtil.ColorFromRgba(
                    ModConfig.BloodColorBlueAmount,
                    ModConfig.BloodColorGreenAmount,
                    ModConfig.BloodColorRedAmount,
                    ModConfig.BloodColorAlphaAmount
                ),
                __instance.entity.Pos.XYZ.Add(__instance.entity.LocalEyePos).Sub(0.25, 0.25, 0.25),
                __instance.entity.Pos.XYZ.Add(__instance.entity.LocalEyePos).Add(0.25, 0.25, 0.25),
                new Vec3f(
                    (float)(Random.NextDouble() - Random.NextDouble()),
                    (float)(Random.NextDouble() - Random.NextDouble()),
                    (float)(Random.NextDouble() - Random.NextDouble())
                ),
                new Vec3f(
                    (float)(Random.NextDouble() - Random.NextDouble()),
                    (float)(Random.NextDouble() - Random.NextDouble()),
                    (float)(Random.NextDouble() - Random.NextDouble())
                ) * 2.0f,
                ModConfig.BloodDespawnDelay,
                1.0f,
                ModConfig.MinimumBloodSize,
                ModConfig.MaximumBloodSize
            );
            particles.AddVelocity = new Vec3f(1, 1, 1) * (float)Random.NextDouble() * 2.0f - new Vec3f(1, 1, 1) * (float)Random.NextDouble() * 2.0f;
            __instance.entity.World.SpawnParticles(particles);
        }
    }
}

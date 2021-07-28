using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using HarmonyLib;

namespace XorberaxBlood
{
    [HarmonyPatch]
    public class XorberaxBloodModSystem : ModSystem
    {
        private const float MinimumDamageToTriggerBlood = 2.0f;
        public static readonly Random Random = new Random();
        private static readonly HashSet<EntityBleedBehavior> EntityBleedBehaviors = new HashSet<EntityBleedBehavior>();

        private static readonly HashSet<EnumDamageType> IgnoredDamageTypes = new HashSet<EnumDamageType>
        {
            EnumDamageType.Fire,
            EnumDamageType.Frost,
            EnumDamageType.Heal,
            EnumDamageType.Hunger,
            EnumDamageType.Poison,
            EnumDamageType.Suffocation,
        };

        public override bool ShouldLoad(EnumAppSide appSide)
        {
            return appSide == EnumAppSide.Client;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            new Harmony("xorberax.blood").PatchAll();
            api.Event.RegisterGameTickListener(OnGameTick, 1000 / 30);
        }

        private void OnGameTick(float deltaTime)
        {
            foreach (var entityBleedBehavior in EntityBleedBehaviors.ToList())
            {
                entityBleedBehavior.Update(deltaTime);
                if (entityBleedBehavior.ShouldRemoveBehavior)
                {
                    EntityBleedBehaviors.Remove(entityBleedBehavior);
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
            if (IgnoredDamageTypes.Contains(damageSource.Type) || damage < MinimumDamageToTriggerBlood)
            {
                return;
            }

            // Attach bleeding behavior to entity.
            EntityBleedBehaviors.Add(new EntityBleedBehavior(__instance, 20));

            // Make blood drop from damage.
            var particles = new SimpleParticleProperties(
                5,
                10,
                ColorUtil.ColorFromRgba(30, 25, 122, 255),
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
                ) * 2.0f
            );
            particles.AddVelocity = (new Vec3f(1, 1, 1) * (float)Random.NextDouble() * 2.0f) - (new Vec3f(1, 1, 1) * (float)Random.NextDouble() * 2.0f);
            particles.LifeLength = 15.0f;
            __instance.entity.World.SpawnParticles(particles);
        }
    }
}

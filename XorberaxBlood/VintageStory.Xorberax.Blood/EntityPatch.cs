using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace VintageStory.Xorberax.Blood;

[HarmonyPatch(typeof(Entity))]
public class EntityPatch
{
    [HarmonyPatch(nameof(Entity.ReceiveDamage))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    public static void ReceiveDamage(
        DamageSource damageSource,
        float damage,
        Entity __instance
    )
    {
        Console.WriteLine("Xorberax Blood: ReceiveDamage");
        
        if (XorberaxBloodModSystem.ModConfig.IgnoredDamageTypes.Contains(damageSource.Type) ||
            damage < XorberaxBloodModSystem.ModConfig.MinimumDamageRequiredToTriggerBlood)
        {
            return;
        }

        // Attach bleeding behavior to entity.
        if (XorberaxBloodModSystem.EntityBleedBehaviors.ContainsKey(__instance))
        {
            var entityBleedBehavior = XorberaxBloodModSystem.EntityBleedBehaviors[__instance];
            entityBleedBehavior.ResetBleedDuration();
        }
        else
        {
            XorberaxBloodModSystem.EntityBleedBehaviors.Add(__instance, new EntityBleedBehavior(__instance, XorberaxBloodModSystem.ModConfig.BleedDuration));
        }

        // Make blood drop from damage.
        var particles = new SimpleParticleProperties(
            XorberaxBloodModSystem.ModConfig.MinimumBloodParticlesOnHit,
            XorberaxBloodModSystem.ModConfig.MaximumBloodParticlesOnHit,
            ColorUtil.ColorFromRgba(
                XorberaxBloodModSystem.ModConfig.BloodColorBlueAmount,
                XorberaxBloodModSystem.ModConfig.BloodColorGreenAmount,
                XorberaxBloodModSystem.ModConfig.BloodColorRedAmount,
                XorberaxBloodModSystem.ModConfig.BloodColorAlphaAmount
            ),
            __instance.Pos.XYZ.Add(__instance.LocalEyePos).Sub(0.25, 0.25, 0.25),
            __instance.Pos.XYZ.Add(__instance.LocalEyePos).Add(0.25, 0.25, 0.25),
            new Vec3f(
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble())
            ),
            new Vec3f(
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble())
            ) * 2.0f,
            XorberaxBloodModSystem.ModConfig.BloodDespawnDelay,
            1.0f,
            XorberaxBloodModSystem.ModConfig.MinimumBloodSize,
            XorberaxBloodModSystem.ModConfig.MaximumBloodSize
        );
        particles.AddVelocity = new Vec3f(1, 1, 1) * (float)Random.Shared.NextDouble() * 2.0f - new Vec3f(1, 1, 1) * (float)Random.Shared.NextDouble() * 2.0f;
        __instance.World.SpawnParticles(particles);
    }
}
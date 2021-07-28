using System;
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
        private static readonly Random Random = new Random();

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            new Harmony("xorberax.blood").PatchAll();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EntityBehaviorHealth), nameof(EntityBehaviorHealth.OnEntityReceiveDamage))]
        private static void OnEntityReceiveDamage(EntityBehaviorHealth __instance)
        {
            var particles = new SimpleParticleProperties(
                5,
                10,
                ColorUtil.ColorFromRgba(30, 25, 122, 255),
                __instance.entity.Pos.XYZ.Add(__instance.entity.LocalEyePos).Sub(0.25, 0.25, 0.25),
                __instance.entity.Pos.XYZ.Add(__instance.entity.LocalEyePos).Add(0.25, 0.25, 0.25),
                new Vec3f(
                    (float)(new Random().NextDouble() - new Random().NextDouble()),
                    (float)(new Random().NextDouble() - new Random().NextDouble()),
                    (float)(new Random().NextDouble() - new Random().NextDouble())
                ) * 10.0f,
                new Vec3f(
                    (float)(new Random().NextDouble() - new Random().NextDouble()),
                    (float)(new Random().NextDouble() - new Random().NextDouble()),
                    (float)(new Random().NextDouble() - new Random().NextDouble())
                ) * 10.0f
            );
            particles.AddVelocity = (new Vec3f(1, 1, 1) * (float)Random.NextDouble() * 3) - (new Vec3f(1, 1, 1) * (float)Random.NextDouble() * 3);
            particles.LifeLength = 15.0f;
            __instance.entity.World.SpawnParticles(particles);
        }
    }
}

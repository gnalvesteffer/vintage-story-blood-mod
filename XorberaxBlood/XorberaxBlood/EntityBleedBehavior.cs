using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace XorberaxBlood
{
    public class EntityBleedBehavior
    {
        private readonly EntityBehaviorHealth _entityBehaviorHealth;
        private readonly float _initialBleedDuration;
        private float _remainingBleedDuration;
        private float _previousHealth;
        private float _bleedDelayRemaining;

        public bool ShouldRemoveBehavior { get; private set; }

        public EntityBleedBehavior(EntityBehaviorHealth entityBehaviorHealth, float remainingBleedDuration)
        {
            _entityBehaviorHealth = entityBehaviorHealth;
            _initialBleedDuration = remainingBleedDuration;
            _remainingBleedDuration = remainingBleedDuration;
            _previousHealth = entityBehaviorHealth.Health;
            _bleedDelayRemaining = CalculateRandomBleedDelay();
        }

        public void ResetBleedDuration()
        {
            _remainingBleedDuration = _initialBleedDuration;
        }

        public void Update(float deltaTime)
        {
            _remainingBleedDuration = GameMath.Max(_remainingBleedDuration - deltaTime, 0);
            _bleedDelayRemaining = GameMath.Max(_bleedDelayRemaining - deltaTime, 0);
            var didSignificantlyHeal = _entityBehaviorHealth?.Health - _previousHealth >= 1.0f;
            _previousHealth = _entityBehaviorHealth?.Health ?? _previousHealth;
            ShouldRemoveBehavior = didSignificantlyHeal ||
                                   _remainingBleedDuration == 0 ||
                                   _entityBehaviorHealth == null ||
                                   _entityBehaviorHealth.Health == 0;

            if (ShouldRemoveBehavior || _bleedDelayRemaining != 0.0f)
            {
                return;
            }

            _bleedDelayRemaining = CalculateRandomBleedDelay();
            Bleed();
        }

        public override bool Equals(object other)
        {
            return other is EntityBleedBehavior otherEntityBleedBehavior &&
                   otherEntityBleedBehavior._entityBehaviorHealth.entity == _entityBehaviorHealth.entity;
        }

        public override int GetHashCode()
        {
            return _entityBehaviorHealth.entity.GetHashCode();
        }

        private void Bleed()
        {
            var particles = new SimpleParticleProperties(
                XorberaxBloodModSystem.ModConfig.MinimumBloodParticlesOnBleed,
                XorberaxBloodModSystem.ModConfig.MaximumBloodParticlesOnBleed,
                ColorUtil.ColorFromRgba(
                    XorberaxBloodModSystem.ModConfig.BloodColorBlueAmount,
                    XorberaxBloodModSystem.ModConfig.BloodColorGreenAmount,
                    XorberaxBloodModSystem.ModConfig.BloodColorRedAmount,
                    XorberaxBloodModSystem.ModConfig.BloodColorAlphaAmount
                ),
                _entityBehaviorHealth.entity.Pos.XYZ.Add(_entityBehaviorHealth.entity.LocalEyePos.Mul(0.25, 0.25, 0.25)),
                _entityBehaviorHealth.entity.Pos.XYZ.Add(_entityBehaviorHealth.entity.LocalEyePos.Mul(0.75, 0.75, 0.75)),
                new Vec3f(
                    (float)(XorberaxBloodModSystem.Random.NextDouble() - XorberaxBloodModSystem.Random.NextDouble()),
                    (float)(XorberaxBloodModSystem.Random.NextDouble() - XorberaxBloodModSystem.Random.NextDouble()),
                    (float)(XorberaxBloodModSystem.Random.NextDouble() - XorberaxBloodModSystem.Random.NextDouble())
                ),
                new Vec3f(
                    (float)(XorberaxBloodModSystem.Random.NextDouble() - XorberaxBloodModSystem.Random.NextDouble()),
                    (float)(XorberaxBloodModSystem.Random.NextDouble() - XorberaxBloodModSystem.Random.NextDouble()),
                    (float)(XorberaxBloodModSystem.Random.NextDouble() - XorberaxBloodModSystem.Random.NextDouble())
                ),
                XorberaxBloodModSystem.ModConfig.BloodDespawnDelay,
                1.0f,
                XorberaxBloodModSystem.ModConfig.MinimumBloodSize,
                XorberaxBloodModSystem.ModConfig.MaximumBloodSize
            );
            particles.AddVelocity = new Vec3f(1, 1, 1) * (float)XorberaxBloodModSystem.Random.NextDouble() * 1.5f - new Vec3f(1, 1, 1) * (float)XorberaxBloodModSystem.Random.NextDouble() * 1.5f;
            _entityBehaviorHealth.entity.World.SpawnParticles(particles);
        }

        private float CalculateRandomBleedDelay()
        {
            var maxBleedDelay = (float)(
                XorberaxBloodModSystem.Random.NextDouble() *
                (XorberaxBloodModSystem.ModConfig.MaximumBleedDelay - XorberaxBloodModSystem.ModConfig.MinimumBleedDelay)
            );
            return XorberaxBloodModSystem.ModConfig.MinimumBleedDelay + maxBleedDelay;
        }
    }
}

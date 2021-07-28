using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace XorberaxBlood
{
    public class EntityBleedBehavior
    {
        private readonly EntityBehaviorHealth _entityBehaviorHealth;
        private float _previousHealth;
        private float _bleedTimeRemaining;
        private float _bleedDelayRemaining;

        public bool ShouldRemoveBehavior { get; private set; }

        public EntityBleedBehavior(EntityBehaviorHealth entityBehaviorHealth, float bleedDuration)
        {
            _entityBehaviorHealth = entityBehaviorHealth;
            _previousHealth = entityBehaviorHealth.Health;
            _bleedTimeRemaining = bleedDuration;
            _bleedDelayRemaining = 0.1f + (float)XorberaxBloodModSystem.Random.NextDouble() * 2.9f;
        }

        public void Update(float deltaTime)
        {
            _bleedTimeRemaining = GameMath.Max(_bleedTimeRemaining - deltaTime, 0);
            _bleedDelayRemaining = GameMath.Max(_bleedDelayRemaining - deltaTime, 0);
            var didSignificantlyHeal = _entityBehaviorHealth?.Health - _previousHealth >= 1.0f;
            _previousHealth = _entityBehaviorHealth?.Health ?? _previousHealth;
            ShouldRemoveBehavior = didSignificantlyHeal ||
                                   _bleedTimeRemaining == 0 ||
                                   _entityBehaviorHealth == null ||
                                   _entityBehaviorHealth.Health == 0;

            if (ShouldRemoveBehavior || _bleedDelayRemaining != 0.0f)
            {
                return;
            }

            _bleedDelayRemaining = 0.1f + (float)XorberaxBloodModSystem.Random.NextDouble() * 2.9f;

            var particles = new SimpleParticleProperties(
                1,
                5,
                ColorUtil.ColorFromRgba(30, 25, 122, 255),
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
                )
            );
            particles.AddVelocity = new Vec3f(1, 1, 1) * (float)XorberaxBloodModSystem.Random.NextDouble() - new Vec3f(1, 1, 1) * (float)XorberaxBloodModSystem.Random.NextDouble();
            particles.LifeLength = 15.0f;
            _entityBehaviorHealth.entity.World.SpawnParticles(particles);
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
    }
}

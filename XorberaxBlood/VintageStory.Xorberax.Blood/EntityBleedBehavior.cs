using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace VintageStory.Xorberax.Blood;

public class EntityBleedBehavior
{
    private readonly Entity _entity;
    private readonly float _initialBleedDuration;
    private float _remainingBleedDuration;
    private float _previousHealth;
    private float _bleedDelayRemaining;

    public bool ShouldRemoveBehavior { get; private set; }

    public EntityBleedBehavior(Entity entity, float remainingBleedDuration)
    {
        _entity = entity;
        _initialBleedDuration = remainingBleedDuration;
        _remainingBleedDuration = remainingBleedDuration;
        _previousHealth = entity.GetHealth();
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
        var didSignificantlyHeal = _entity?.GetHealth() - _previousHealth >= 1.0f;
        _previousHealth = _entity?.GetHealth() ?? _previousHealth;
        ShouldRemoveBehavior = didSignificantlyHeal ||
                               _remainingBleedDuration == 0 ||
                               _entity == null ||
                               _entity.GetHealth() == 0;

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
               otherEntityBleedBehavior._entity == _entity;
    }

    public override int GetHashCode()
    {
        return _entity.GetHashCode();
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
            _entity.Pos.XYZ.Add(_entity.LocalEyePos.Mul(0.25, 0.25, 0.25)),
            _entity.Pos.XYZ.Add(_entity.LocalEyePos.Mul(0.75, 0.75, 0.75)),
            new Vec3f(
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble())
            ),
            new Vec3f(
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble()),
                (float)(Random.Shared.NextDouble() - Random.Shared.NextDouble())
            ),
            XorberaxBloodModSystem.ModConfig.BloodDespawnDelay,
            1.0f,
            XorberaxBloodModSystem.ModConfig.MinimumBloodSize,
            XorberaxBloodModSystem.ModConfig.MaximumBloodSize
        );
        particles.AddVelocity = new Vec3f(1, 1, 1) * (float)Random.Shared.NextDouble() * 1.5f -
                                new Vec3f(1, 1, 1) * (float)Random.Shared.NextDouble() * 1.5f;
        _entity.World.SpawnParticles(particles);
    }

    private float CalculateRandomBleedDelay()
    {
        var maxBleedDelay = (float)(
            Random.Shared.NextDouble() *
            (XorberaxBloodModSystem.ModConfig.MaximumBleedDelay - XorberaxBloodModSystem.ModConfig.MinimumBleedDelay)
        );
        return XorberaxBloodModSystem.ModConfig.MinimumBleedDelay + maxBleedDelay;
    }
}
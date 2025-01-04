using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Vintagestory.API.Common;

namespace VintageStory.Xorberax.Blood;

public class ModConfig
{
    /// <summary>
    /// Used to check if the current config is outdated and needs to be regenerated.
    /// </summary>
    public int ConfigVersion { get; set; }

    /// <summary>
    /// Number of times per second the mod system should update behaviors such as bleeding.
    /// </summary>
    public int TickRate { get; set; }

    /// <summary>
    /// The minimum damage required to trigger blood effects.
    /// </summary>
    public float MinimumDamageRequiredToTriggerBlood { get; set; }

    /// <summary>
    /// The number of seconds before blood particles despawn.
    /// </summary>
    public float BloodDespawnDelay { get; set; }

    /// <summary>
    /// The duration in seconds that bleeding should last.
    /// </summary>
    public float BleedDuration { get; set; }

    /// <summary>
    /// The minimum number of blood particles to spawn when an entity is hit.
    /// </summary>
    public int MinimumBloodParticlesOnHit { get; set; }

    /// <summary>
    /// The maximum number of blood particles to spawn when an entity is hit.
    /// </summary>
    public int MaximumBloodParticlesOnHit { get; set; }

    /// <summary>
    /// The minimum number of blood particles to spawn during a bleed iteration.
    /// </summary>
    public int MinimumBloodParticlesOnBleed { get; set; }

    /// <summary>
    /// The maximum number of blood particles to spawn during a bleed iteration.
    /// </summary>
    public int MaximumBloodParticlesOnBleed { get; set; }

    /// <summary>
    /// The minimum number of seconds between each bleed interval.
    /// </summary>
    public float MinimumBleedDelay { get; set; }

    /// <summary>
    /// The maximum number of seconds between each bleed interval.
    /// </summary>
    public float MaximumBleedDelay { get; set; }

    /// <summary>
    /// The amount of red in the blood color.
    /// </summary>
    public byte BloodColorRedAmount { get; set; }

    /// <summary>
    /// The amount of green in the blood color.
    /// </summary>
    public byte BloodColorGreenAmount { get; set; }

    /// <summary>
    /// The amount of blue in the blood color.
    /// </summary>
    public byte BloodColorBlueAmount { get; set; }

    /// <summary>
    /// The amount of alpha in the blood color.
    /// </summary>
    public byte BloodColorAlphaAmount { get; set; }

    /// <summary>
    /// The minimum size of blood particles.
    /// </summary>
    public float MinimumBloodSize { get; set; }

    /// <summary>
    /// The maximum size of blood particles.
    /// </summary>
    public float MaximumBloodSize { get; set; }

    /// <summary>
    /// The types of damage that shouldn't trigger blood effects.
    /// </summary>
    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    public HashSet<EnumDamageType> IgnoredDamageTypes { get; set; }
}
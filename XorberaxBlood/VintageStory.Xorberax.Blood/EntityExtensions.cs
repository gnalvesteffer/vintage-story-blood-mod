using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace VintageStory.Xorberax.Blood;

public static class EntityExtensions
{
    public static float GetHealth(this Entity entity)
    {
        ITreeAttribute healthTree = entity.WatchedAttributes.GetTreeAttribute("health");
        return healthTree?.GetFloat("currenthealth") ?? 0.0f;
    }
}
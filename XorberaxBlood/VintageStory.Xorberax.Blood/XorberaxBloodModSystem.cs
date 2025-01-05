using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace VintageStory.Xorberax.Blood;

[HarmonyPatch]
public class XorberaxBloodModSystem : ModSystem
{
    public static readonly Dictionary<Entity, EntityBleedBehavior> EntityBleedBehaviors = new Dictionary<Entity, EntityBleedBehavior>();
    public static ModConfig ModConfig { get; private set; }

    public override bool ShouldLoad(EnumAppSide appSide)
    {
        return appSide == EnumAppSide.Client;
    }

    public override void Start(ICoreAPI api)
    {
        ModConfig = new ModConfigLoader(api).LoadConfig();
        new Harmony("xorberax.blood").PatchAll();
        api.Event.RegisterGameTickListener(OnGameTick, 1000 / ModConfig.TickRate);

        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Xorberax Blood Mod Loaded!");
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
}
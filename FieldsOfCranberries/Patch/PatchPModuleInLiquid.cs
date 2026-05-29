using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using HarmonyLib;

[HarmonyPatch(typeof(PModuleInLiquid), nameof(PModuleInLiquid.DoApply))]
public class PatchPModuleInLiquid
{
    public static bool Prefix(
    PModuleInLiquid __instance,
    float dt,
    Entity entity,
    EntityPos pos,
    EntityControls controls)
{
    if (!(entity?.Properties?.Attributes?["ignoreWaterFlow"].AsBool(false) ?? false))
        return true; 

    if (entity.Swimming && entity.Alive)
        __instance.HandleSwimming(dt, entity, pos, controls);
    return false; 
}
}
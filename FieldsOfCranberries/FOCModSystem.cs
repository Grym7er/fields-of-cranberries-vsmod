using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using FieldsOfCranberries.WaterHarvestableBehavior;
using FieldsOfCranberries.WaterHarvestableBEBehaviour;
using FieldsOfCranberries.FOCEntityBehaviors;

namespace FieldsOfCranberries
{
    public class FieldsOfCranberriesModSystem : ModSystem
    {
        // Called on server and client  
        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            Mod.Logger.Notification("Fields of Cranberries mod started");
            api.RegisterBlockBehaviorClass("WaterHarvestable", typeof(BlockBehaviorWaterHarvestable));
            api.RegisterBlockEntityBehaviorClass("WaterHarvestable", typeof(BEBehaviourWaterHarvestable));
            api.RegisterEntityBehaviorClass("trackberrybush", typeof(EntitySpiderTrackBerryBush));
            api.RegisterEntityBehaviorClass("waterreel", typeof(EntityBehaviorWaterreel));
            api.RegisterEntityBehaviorClass("sinkinggait", typeof(EntityBehaviorSinkingGait));
        }


    }
}

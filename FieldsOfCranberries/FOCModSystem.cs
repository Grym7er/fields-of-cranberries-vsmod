using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using FieldsOfCranberries.WaterHarvestableBehavior;
using FieldsOfCranberries.WaterHarvestableBEBehaviour;
using FieldsOfCranberries.FOCEntityBehaviors;
using FieldsOfCranberries.Config;
using System;


namespace FieldsOfCranberries
{

    public class FieldsOfCranberriesModSystem : ModSystem
    {

        public bool WildCraftFruitActive { get; private set; } = false;

        public static FieldsOfCranberriesConfigData ModConfig;
   
        // Called on server and client  
        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            Mod.Logger.Notification("Fields of Cranberries mod started");
            api.RegisterBlockBehaviorClass("WaterHarvestable", typeof(BlockBehaviorWaterHarvestable));
            api.RegisterBlockEntityBehaviorClass("WaterHarvestable", typeof(BEBehaviourWaterHarvestable));
            api.RegisterEntityBehaviorClass("trackberrybush", typeof(EntitySpiderTrackBerryBush));
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            // TryToLoadConfig(api);

            WildCraftFruitActive = api.ModLoader.IsModEnabled("wildcraftfruit");

            if (WildCraftFruitActive) {
                Mod.Logger.Notification("WildCraftFruit mod is active, enabling custom berry bush behavior");
            }
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            TryToLoadConfig(api);
            api.World.Config.SetBool("EnableSpiderSpawns", ModConfig.EnableSpiderSpawns);
            api.World.Config.SetFloat("SpiderDropRateBuff", ModConfig.SpiderDropRateBuff);
            api.World.Config.SetFloat("SpiderSpawnChance", ModConfig.SpiderSpawnChance);
        }

        private void TryToLoadConfig(ICoreAPI api) {
                //It is important to surround the LoadModConfig function in a try-catch. 
                //If loading the file goes wrong, then the 'catch' block is run.
                try
                {
                    ModConfig = api.LoadModConfig<FieldsOfCranberriesConfigData>("FieldsOfCranberriesConfig.json");
                    if (ModConfig == null) //if the 'MyConfigData.json' file isn't found...
                    {
                        ModConfig = new FieldsOfCranberriesConfigData();
                    }
                    //Save a copy of the mod config.
                    api.StoreModConfig<FieldsOfCranberriesConfigData>(ModConfig, "FieldsOfCranberriesConfig.json");
                }
                catch (Exception e)
                {
                    //Couldn't load the mod config... Create a new one with default settings, but don't save it.
                    Mod.Logger.Error("Could not load config! Loading default settings instead.");
                    Mod.Logger.Error(e);
                    ModConfig = new FieldsOfCranberriesConfigData();
                }
        }


    }
}

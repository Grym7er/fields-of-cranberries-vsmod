using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.MathTools;
using System;
using Vintagestory.GameContent;
using System.Linq;
using Vintagestory.API.Util;
using FieldsOfCranberries.WaterHarvestableBEBehaviour;

namespace FieldsOfCranberries.WaterHarvestableBehavior
{
    public class BlockBehaviorWaterHarvestable : BlockBehavior
    {
        
        public BlockBehaviorWaterHarvestable(Block block) : base(block)
        {

        }

        BEBehaviorFruitingBush behfruitingBush;
        BlockBehaviorFruitingBush bhBush;


        public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
        {
            if (this.block == null) return;

            if (block.Variant["type"] == "cranberry")
            {
                BEBehaviourWaterHarvestable beBehaviourWaterHarvestable = block.GetBEBehavior<BEBehaviourWaterHarvestable>(pos);
                bool shouldDropBerries = beBehaviourWaterHarvestable?.CheckIfWentIntoWater() ?? false;

                if (!shouldDropBerries) return;
                
                beBehaviourWaterHarvestable.TryDropBerries();
                
            }
        }
        
    }
}
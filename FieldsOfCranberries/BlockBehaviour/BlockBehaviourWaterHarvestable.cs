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


        public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
        {

            var beBehaviourWaterHarvestable = block.GetBEBehavior<BEBehaviourWaterHarvestable>(pos);
            if (beBehaviourWaterHarvestable == null || beBehaviourWaterHarvestable.MySpiderEntityId == -1) return null;


            if (block.Variant["type"] == "cranberry") //Removed redundant check for spider ID, which is already done above
            {

                return Lang.Get("fieldsofcranberries:spideronbush-tooltip") + "\n" + "SpiderID: " + beBehaviourWaterHarvestable.MySpiderEntityId;
            }
            return null;
        }
        
    }
}
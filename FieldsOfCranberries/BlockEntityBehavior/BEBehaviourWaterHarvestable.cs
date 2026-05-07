using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using FieldsOfCranberries.WaterHarvestableBehavior;
using Vintagestory.GameContent;

#nullable disable
namespace FieldsOfCranberries.WaterHarvestableBEBehaviour
{
    public class BEBehaviourWaterHarvestable : BlockEntityBehavior
    {


        private bool isInWater = false;
        protected ICoreClientAPI capi;
        private BEBehaviorFruitingBush behfruitingBush;
        protected BlockBehaviorFruitingBush bhBush;
        public bool IsInWater{
            get{
                return isInWater;
            }
            private set{
                isInWater = value;
            }
        }

        
        public BEBehaviourWaterHarvestable(BlockEntity blockentity) : base(blockentity)
        {

        }
        protected virtual float getYieldMul()
        {
            if (behfruitingBush.BState.Traits.Contains("heavybearer")) return 1.15f;
            if (behfruitingBush.BState.Traits.Contains("shybearer")) return 0.85f;
            return 1;
        }

        protected void setGrowthState(EnumFruitingBushGrowthState state)
        {
            behfruitingBush.BState.Growthstate = state;
            behfruitingBush.BState.TransitionHoursLeft = behfruitingBush.GetHoursForNextStage();
            Blockentity.MarkDirty(true);
        }



        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            base.Initialize(api, properties);
            capi = api as ICoreClientAPI;

            bhBush = Block.GetBehavior<BlockBehaviorFruitingBush>();

            behfruitingBush = Blockentity.GetBehavior<BEBehaviorFruitingBush>();

        }

        public bool CheckIfInWater()
        {
            IsInWater = Api.World.BlockAccessor.GetBlock(this.Pos, BlockLayersAccess.Fluid).LiquidCode == "water";
            return IsInWater;
        }

        public bool CheckIfWentIntoWater()
        {
            // Console.WriteLine("CheckIfWentIntoWater called. Currently in water? " + IsInWater);
            // Check if was not in water and is now in water, i.e
            // went from dry air to water
            if (!IsInWater && CheckIfInWater())
            {
                return true;
            }
            // If the other way around, i.e went from water to dry air, update IsInWater
            else if (IsInWater && !CheckIfInWater())
            {
                IsInWater = false;
                return false;
            }
            // or no state change, return false
            return false;
        }

        float[] dropRates = [0f, 0.5f, 1f, 1.5f];
        public void TryDropBerries()
        {
            if (behfruitingBush.BState.Growthstate is not EnumFruitingBushGrowthState.Ripe) return;
            
            float dropRate = getYieldMul();

            bhBush.harvestedStacks.Foreach(harvestedStack =>
            {
                ItemStack stack = harvestedStack.GetNextItemStack(dropRate);
                if (stack == null) return;

                stack.StackSize = GameMath.RoundRandom(Api.World.Rand, stack.StackSize * dropRates[(int)behfruitingBush.GetHealthState()]);

                var quantity = stack.StackSize;
                Api.World.SpawnItemEntity(stack, Pos);
                Api.World.Logger.Audit("[WaterHarvestable] Dropped {0}x{1} from {2} at {3}.",
                    quantity,
                    stack.Collectible.Code,
                    Block.Code,
                    Pos
                );
            });

            setGrowthState(EnumFruitingBushGrowthState.Mature);
            return;
        }



        public override void OnBlockPlaced(ItemStack byItemStack = null)
        {
            CheckIfInWater();
            return;
            
        }



        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            IsInWater = tree.GetBool("isInWater");
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetBool("isInWater", IsInWater);
        }
    }
}
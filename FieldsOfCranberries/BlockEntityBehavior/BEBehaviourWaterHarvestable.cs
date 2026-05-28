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
        protected ICoreServerAPI sapi;
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
        private long MySpiderEntityId = -1;

        AssetLocation entitySpawnableSpider = new AssetLocation("fieldsofcranberries:spider-wolf");

        
        public BEBehaviourWaterHarvestable(BlockEntity blockentity) : base(blockentity)
        {

        }
        protected virtual float getYieldMul()
        {
            float spiderBuff = 0.0f;

            if (MySpiderEntityId != -1) spiderBuff = 0.15f; // TODO: Make this a configurable value

            if (behfruitingBush.BState.Traits.Contains("heavybearer")) return 1.15f + spiderBuff;
            if (behfruitingBush.BState.Traits.Contains("shybearer")) return 0.85f + spiderBuff;
            return 1 + spiderBuff;
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
            sapi = api as ICoreServerAPI;
            bhBush = Block.GetBehavior<BlockBehaviorFruitingBush>();

            behfruitingBush = Blockentity.GetBehavior<BEBehaviorFruitingBush>();

        }

        public bool CheckIfInWater()
        {
            if (!Api.World.Side.IsServer()) return false;
            IsInWater = Api.World.BlockAccessor.GetBlock(this.Pos, BlockLayersAccess.Fluid).LiquidCode == "water";
            return IsInWater;
        }

        public bool CheckIfWentIntoWater()
        {
            if (!Api.World.Side.IsServer()) return false;
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

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            if (MySpiderEntityId != -1)
            {
                dsc.AppendLine(Lang.Get("fieldsofcranberries:spideronbush-tooltip"));
            }
        }

        float[] dropRates = [0f, 0.5f, 1f, 1.5f];
        public void TryDropBerries()
        {
            if (!Api.World.Side.IsServer()) return;
            if (behfruitingBush.BState.Growthstate is not EnumFruitingBushGrowthState.Ripe) return;
            if (Block.Variant["type"] != "cranberry") return;
            if (!CheckIfInWater()) return;



            

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
            Api.World.PlaySoundAt(bhBush.HarvestingSound, Pos, 0);

            setGrowthState(EnumFruitingBushGrowthState.Mature);

            TrySpawnSpider(Pos);

            return;
        }

        public void TrySpawnSpider(BlockPos spawnPos)
        {
            if (!Api.World.Side.IsServer()) return;
            #if DEBUG
            Console.WriteLine("Trying to spawn spider at {0}", spawnPos);
            #endif

            if (MySpiderEntityId != -1) return; //Don't spawn spider if there is already one on the bush
            
            #if DEBUG
            if (Api.World.Rand.NextDouble() < 1.0) // will make it smaller in deployment
            #else
            if (Api.World.Rand.NextDouble() < 0.05)
            #endif
            {
                EntityProperties type = Api.World.GetEntityType(entitySpawnableSpider);
                if (type == null)
                {
                    Api.World.Logger.Error("BEBehaviourWaterHarvestable: No such entity - {0}", entitySpawnableSpider);
                    return;
                }
                
                Entity entity = sapi.ClassRegistry.CreateEntity(type);

                
                // EntityAgent agent = entity as EntityAgent;
                // if (agent != null) agent.HerdId = herdid;

                entity.Pos.SetPosWithDimension(spawnPos);
                entity.Pos.SetYaw((float)sapi.World.Rand.NextDouble() * GameMath.TWOPI);
                entity.PositionBeforeFalling.Set(entity.Pos.X, entity.Pos.Y, entity.Pos.Z);
                                
                sapi.World.SpawnEntity(entity);
                AllocateSpiderToBush(entity, spawnPos);
                
                return;
            }
        }

        private void AllocateSpiderToBush(Entity spider, BlockPos bushPos)
        {
            #if DEBUG
            Block block = sapi.World.BlockAccessor.GetBlock(bushPos);
            Api.World.Logger.Audit("Allocating spider to {1} bush at {0}", bushPos, block.Variant["type"]);
            #endif
            
            spider.WatchedAttributes.SetBlockPos("berrybushpos", bushPos);
            MySpiderEntityId = spider.EntityId;
            Blockentity.MarkDirty(true);
            #if DEBUG
            Console.WriteLine("Allocated spider to bush at {0}", bushPos);
            #endif
        }

        public void DeallocateSpiderFromBush(Entity spider)
        {
            if (!Api.World.Side.IsServer()) return;
            
            // On spider death, spider should call this method to deallocate itself from the bush
            if (spider.EntityId == MySpiderEntityId)
            {
                MySpiderEntityId = -1;
            }
            Blockentity.MarkDirty(true);
        }



        public override void OnBlockPlaced(ItemStack byItemStack = null)
        {
            if (!Api.World.Side.IsServer()) return;

            CheckIfInWater();

            #if DEBUG
            setGrowthState(EnumFruitingBushGrowthState.Ripe); // Debug Only
            #endif

            return;
        }

        public override void OnBlockRemoved()
        {
            base.OnBlockRemoved();
            if (!Api.World.Side.IsServer()) return;
            
            if (MySpiderEntityId != -1)
            {
                Entity spider = Api.World.GetEntityById(MySpiderEntityId);
                spider.WatchedAttributes.RemoveAttribute("berrybushpos");
                DeallocateSpiderFromBush(spider);
            }
        }



        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            IsInWater = tree.GetBool("isInWater");
            MySpiderEntityId = tree.GetLong("mySpiderEntityId");
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetBool("isInWater", IsInWater);
            tree.SetLong("mySpiderEntityId", MySpiderEntityId);
        }
    }
}
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using FieldsOfCranberries.WaterHarvestableBEBehaviour;
using System;
using Vintagestory.GameContent;
using System.Collections.Generic;
#nullable disable
namespace FieldsOfCranberries.FOCEntityBehaviors
{
    public class EntityBehaviorWaterreel : EntityBehavior
    {
        public override string PropertyName() => "waterreel";

        public EntityBehaviorWaterreel(Entity entity) : base(entity)
        {

        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            base.Initialize(properties, attributes);

        }

        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            base.OnEntityDespawn(despawn);
        }

        private float sinceLastTick;

        public override void OnGameTick(float deltaTime)
        {
            base.OnGameTick(deltaTime);

            // Don't do anything if no-one is mounted
            if (!entity.GetBehavior<EntityBehaviorRideable>()?.AnyMounted() ?? false) return;

            sinceLastTick += deltaTime;

            if (sinceLastTick < 0.250f) return;

            sinceLastTick = 0;

            BlockFacing facing = BlockFacing.HorizontalFromYaw(entity.Pos.Yaw);
            BlockFacing leftFacing = facing.GetCCW();
            BlockFacing rightFacing = facing.GetCW();

            
            BlockPos pos = entity.Pos.AsBlockPos;

            BlockPos downPos = pos.DownCopy();

            // Block block = entity.World.BlockAccessor.GetBlock(downPos);

            // First bush pos to harvest: middle front
            BlockPos forwardPos = pos.AddCopy(facing.Normali);
            // Second bush pos to harvest: left front
            BlockPos leftPos = forwardPos.AddCopy(leftFacing.Normali);
            // Third bush pos to harvest: right front
            BlockPos rightPos = forwardPos.AddCopy(rightFacing.Normali);

            List<BlockPos> checkBlockPositions = new List<BlockPos> { forwardPos, leftPos, rightPos };
            foreach (BlockPos checkBlockPosition in checkBlockPositions)
            {
                BEBehaviourWaterHarvestable beBehaviourWaterHarvestable = entity.World.BlockAccessor.GetBlockEntity(checkBlockPosition)?.GetBehavior<BEBehaviourWaterHarvestable>();
                if (beBehaviourWaterHarvestable == null) continue;
                beBehaviourWaterHarvestable.TryDropBerries();

            }
            
        }


    }
}
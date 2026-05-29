using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using FieldsOfCranberries.WaterHarvestableBEBehaviour;
using System;
using Vintagestory.GameContent;

#nullable disable
namespace FieldsOfCranberries.FOCEntityBehaviors
{
    public class EntityBehaviorSinkingGait : EntityBehaviorGait
    {
        public override string PropertyName() => "sinkinggait";

        public EntityBehaviorSinkingGait(Entity entity) : base(entity)
        {

        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            base.Initialize(properties, attributes);
        }


        protected override void Move(float dt)
        {
            if (!entity.Swimming)
            {
                base.Move(dt);
                return;
            }

            else if (entity.Swimming)
            {
                
                EntityControls controls = eagent.Controls;

                double cosYaw = Math.Cos(entity.Pos.Yaw);
                double sinYaw = Math.Sin(entity.Pos.Yaw);
                controls.WalkVector.Set(sinYaw, 0, cosYaw);
                controls.WalkVector.Mul(CurrentGait.MoveSpeed * GlobalConstants.OverallSpeedMultiplier * CurrentGait.Direction * MoveSpeedModifier);
                controls.FlyVector.Set(controls.WalkVector);
                controls.FlyVector.Y = 0;  // no buoyancy

            }
        }
    }
}
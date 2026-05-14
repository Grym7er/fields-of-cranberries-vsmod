using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using FieldsOfCranberries.WaterHarvestableBEBehaviour;
using System;
#nullable disable
namespace FieldsOfCranberries.FOCEntityBehaviors
{
    public class EntitySpiderTrackBerryBush : EntityBehavior
    {
        public override string PropertyName() => "trackberrybush";

        private BlockPos berryBushPos;

        public EntitySpiderTrackBerryBush(Entity entity) : base(entity)
        {

        }
        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            base.Initialize(properties, attributes);

            
        }

        public override void AfterInitialized(bool onFirstSpawn)
        {
            base.AfterInitialized(onFirstSpawn);
        }

        private void DeAllocateSpiderFromBush(Entity spider)
        {
            #if DEBUG
            Console.WriteLine("Trying to deallocate spider from bush {0}", berryBushPos);
            #endif
            if (!entity.World.Side.IsServer()) return;

            berryBushPos = entity.WatchedAttributes.GetBlockPos("berrybushpos");
            if (berryBushPos == null) return;
            
            BlockEntity berryBush = entity.World.BlockAccessor.GetBlockEntity(berryBushPos);
            if (berryBush != null)
            {
                berryBush.GetBehavior<BEBehaviourWaterHarvestable>()?.DeallocateSpiderFromBush(spider);
                #if DEBUG
                Console.WriteLine("DeAllocated spider from bush {0}", berryBushPos);
                #endif
            }
        }

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            base.OnEntityDeath(damageSourceForDeath);
            DeAllocateSpiderFromBush(entity);

        }

        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            base.OnEntityDespawn(despawn);
            DeAllocateSpiderFromBush(entity);
        }

    }
}
﻿using Models.Map;
using Models.Map.Tiles;

namespace Models.Entities.Living
{
    public class HumanEntity : LivingEntity
    {
        public HumanEntity(Tile _tile) : base(_tile)
        {
            motor.SetTargetTile(World.Instance.GetRandomTile());
        }

        public override void Update()
        {
            base.Update();
            
            if (!motor.Working)
            {
                motor.SetTargetTile(World.Instance.GetRandomTile());
            }
        }
    }
}

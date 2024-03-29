using ColonyZ.Models.AI;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Saving;
using ColonyZ.Models.UI.Context;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ColonyZ.Models.Entities.Living
{
    public class LivingEntity : Entity, IContextProvider
    {
        public float MovementSpeed { get; protected set; }

        public AIMotor Motor { get; }

        public int HeadId { get; protected set; }

        public int BodyId { get; protected set; }

        protected int Health { get; set; }

        protected LivingEntity(Tile _tile) : base(_tile)
        {
            MovementSpeed = 1.0f;
            Health = 100;
            Motor = new AIMotor(this);
            HeadId = Random.Range(0, 2);
            BodyId = Random.Range(0, 2);
        }

        /// <summary>
        ///     Used for loading only.
        /// </summary>
        public LivingEntity() : base(null)
        {
        }

        public override void Update()
        {
            if (Health <= 0)
                World.Instance.RemoveCharacter(this);

            Motor.Update();
        }

        protected override void OnTileChanged(Tile _tile)
        {
            CurrentTile.LivingEntities.Remove(this);
            _tile.LivingEntities.Add(this);
        }

        /// <summary>
        ///     Event called when the ai motor fails to find a path to its provided target tile.
        /// </summary>
        public virtual void OnPathFailed()
        {
        }

        public override string GetSelectionDescription()
        {
            return base.GetSelectionDescription() +
                   $"Health: {Health}\n";
        }

        public override void OnSave(SaveGameWriter _writer)
        {
            base.OnSave(_writer);
            _writer.WriteProperty("health", Health);
            _writer.WriteProperty("head_id", HeadId);
            _writer.WriteProperty("body_id", BodyId);
            _writer.WriteProperty("move_speed", MovementSpeed);
        }

        public override void OnLoad(JToken _dataToken)
        {
            base.OnLoad(_dataToken);
            var name = _dataToken["entity_name"].Value<string>();
            var health = _dataToken["health"].Value<int>();
            var headID = _dataToken["head_id"].Value<int>();
            var bodyID = _dataToken["body_id"].Value<int>();
            var entity = World.Instance.SpawnCharacter(CurrentTile);
            entity.Name = name;
            entity.Health = health;
            entity.HeadId = headID;
            entity.BodyId = bodyID;
        }

        public ContextAction[] GetContextActions()
        {
            return new[]
            {
                new ContextAction("Kill", () => Health = 0)
            };
        }

        public string GetContextMenuName()
        {
            return Name;
        }
    }
}
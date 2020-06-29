using System.Collections.Generic;
using UnityEngine;
using RULESET.ENTITIES;
using RULESET.ITEMS;
using RENDERER.MAP;
using RENDERER.UTILS.Atlas;

namespace RULESET.MANAGERS
{
	public class EntityManager : MonoBehaviour
	{
		public PlayerEntity playerEntity;
		public List<ActorEntity> actors = new List<ActorEntity>();
		public List<Item> itemsOnGround = new List<Item>();

		public MapEntityRenderer entityRenderer;

		private void Awake()
		{
			playerEntity = new PlayerEntity();
			playerEntity.name = "bruh";
			playerEntity.x = 0;
			playerEntity.y = 0;

			actors.Add(new ActorEntity());
			actors[0].name = "test";
			actors[0].x = 15;
			actors[0].y = 15;
		}

		//Process the new player position recieved by the turnManager.
		public void UpdatePlayer(Vector2 mvmtDirection)
		{
			playerEntity.x += Mathf.RoundToInt(mvmtDirection.x);
			playerEntity.y += Mathf.RoundToInt(mvmtDirection.y);
			UpdateEntities();
		}

		public void UpdateEntities()
		{
			int halfWidth = ((MapViewport.viewPortRadius - 1) / 2);
			List<Entity> relevantEntities = new List<Entity>();
			foreach (var actor in actors) 
			{
				if (actor.x >= playerEntity.x - halfWidth && actor.x < playerEntity.x + halfWidth &&
					actor.y >= playerEntity.y - halfWidth && actor.y < playerEntity.y + halfWidth)
				{
					//Should we also process their turn actions here?
					relevantEntities.Add(actor);
				}
			}
			List<Item> relevantItems = new List<Item>();
			foreach (var item in itemsOnGround) 
			{
				if (item.x >= playerEntity.x - halfWidth && item.x < playerEntity.x + halfWidth &&
					item.y >= playerEntity.y - halfWidth && item.y < playerEntity.y + halfWidth)
				{
					//Should we also process their turn actions here?
					relevantItems.Add(item);
				}
			}

			entityRenderer.CreateNewBuffer(playerEntity, relevantEntities, relevantItems);
		}
	}
}
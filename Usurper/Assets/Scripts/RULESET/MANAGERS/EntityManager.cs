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
		[SerializeField]
		public MapViewport mapViewport;

		private void Awake()
		{
			playerEntity = new PlayerEntity();
			playerEntity.inventory.Add(new Item());
			playerEntity.inventory[0].name = "TestItem1";
			playerEntity.name = "bruh";
			playerEntity.x = 5;
			playerEntity.y = 5;

			actors.Add(new ActorEntity());
			actors[0].name = "test";
			actors[0].x = 15;
			actors[0].y = 15;
		}

		//Process the new player position recieved by the turnManager.
		public void UpdatePlayer(Vector2Int mvmtDirection)
		{
			if (CheckCollisionInDirection(new Vector2Int(playerEntity.x, playerEntity.y), mvmtDirection))
			{
				playerEntity.x += Mathf.RoundToInt(mvmtDirection.x);
				playerEntity.y += Mathf.RoundToInt(mvmtDirection.y);
			}
			UpdateEntities();
		}

		public void UpdateEditorCursor(Vector2Int mvmtDirection)
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

		public bool CheckCollisionInDirection(Vector2Int pos, Vector2Int direction)
		{
			//True means there is no collider in the way
			//First convert the position to the local view
			int halfWidth = ((MapViewport.viewPortRadius - 1) / 2);
			Vector2Int targetPos = (pos + direction);
			Vector2Int localTargetPos = targetPos - mapViewport.centerPosOnMap;
			if (mapViewport.lastUpdateViewData == null) return false;

			if (localTargetPos.x < -halfWidth || localTargetPos.x >= halfWidth ||
				localTargetPos.y < -halfWidth || localTargetPos.x >= halfWidth) return false;


			foreach (var actor in actors) 
			{
				if (actor.x == targetPos.x && actor.y == targetPos.y)
				{
					return false;
				}
			}

			return !mapViewport.lastUpdateViewData[localTargetPos.x + halfWidth, localTargetPos.y + halfWidth].collider;
		}
	}
}
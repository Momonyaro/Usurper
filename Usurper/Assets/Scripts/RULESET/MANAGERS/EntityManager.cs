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
		public static List<ActorEntity> actors = new List<ActorEntity>();
		public static List<CreatureSpecies> creatures = new List<CreatureSpecies>();
		public static List<Item> itemsOnGround = new List<Item>();

		public MapEntityRenderer entityRenderer;
		[SerializeField]
		public MapViewport mapViewport;

		private void Awake()
		{
			playerEntity = new PlayerEntity();
			playerEntity.name = "bruh";
			playerEntity.x = 5;
			playerEntity.y = 5;
		}

		public static void CreateEntityAtPos(int x, int y)
		{
			ActorEntity a = new ActorEntity
			{
				name = "New Actor",
				x = x,
				y = y
			};
			actors.Add(a);
		}

		public static void RemoveEntityAtPos(int x, int y)
		{
			for (int i = 0; i < actors.Count; i++)
			{
				if (actors[i].x == x && actors[i].y == y)
				{
					actors.RemoveAt(i);
					break;
				}
			}
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

			List<Gate> relevantGates;
			relevantGates = GateManager.GetRelevantGates(new Vector2Int(playerEntity.x, playerEntity.y), halfWidth);

			entityRenderer.CreateNewBuffer(playerEntity, relevantEntities, relevantItems, relevantGates);
		}

		public void QuietUpdateEntities()
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

			List<Gate> relevantGates;
			relevantGates = GateManager.GetRelevantGates(new Vector2Int(playerEntity.x, playerEntity.y), halfWidth);

			entityRenderer.CreateNewBuffer(playerEntity, relevantEntities, relevantItems, relevantGates);
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

		public static Sprite GetCreatureSprite(int speciesId, int spriteIndex = 0)
		{
			//Debug.Log("speciesId: " + speciesId + ", spriteIndex: " + spriteIndex);
			for (int i = 0; i < creatures.Count; i++)
			{
				//Debug.Log("testing an actor: actorId = " + speciesId + " against species of Id = " + creatures[i].id);
				if (creatures[i].id.Equals(speciesId))
				{
					//Later we make it fetch the correct sprite as well!
					return creatures[i].sprite;
				}
			}
			return Resources.Load<Sprite>("Sprites/spr_err");
		}
	}
}

namespace RULESET.ENTITIES
{
	public class CreatureSpecies
	{
		public string name;
		public string desc;
		public int id;
		public Sprite sprite; //Change to list to have a pool of sprites instead.
		public int[] averageStats;
		// Add creature bonuses as well!
		public List<CreatureBodyPart> bodyParts;

		public CreatureSpecies(string name, string desc, Sprite sprite)
		{
			this.name = name;
			this.desc = desc;
			this.id = 999;
			this.sprite = sprite;
			averageStats = new int[7] { 1, 1, 1, 1, 1, 1, 1 };
			bodyParts = new List<CreatureBodyPart>();
		}
	}

	public class CreatureBodyPart
	{
		public BodyPart containedBodyPart;
		public RectInt bodyPartRect;
		public int angle;

		public CreatureBodyPart(BodyPart containedBodyPart, RectInt bodyPartRect, int angle)
		{
			this.containedBodyPart = new BodyPart
			{
				name = containedBodyPart.name.ToString(),
				damageMultiplier = containedBodyPart.damageMultiplier,
				hitThreshold = containedBodyPart.hitThreshold,
				canEquipType = containedBodyPart.canEquipType,
				canHoldType = containedBodyPart.canHoldType,
				countsForDamage = containedBodyPart.countsForDamage
			};
			this.bodyPartRect = bodyPartRect;
			this.angle = angle;
		}

		public CreatureBodyPart Copy()
		{
			return new CreatureBodyPart(containedBodyPart, bodyPartRect, angle);
		}
	}
}
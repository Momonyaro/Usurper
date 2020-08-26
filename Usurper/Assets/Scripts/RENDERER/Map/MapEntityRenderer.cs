using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using RENDERER.UTILS.Atlas;
using RULESET.ENTITIES;
using RULESET.MANAGERS;
using RULESET.ITEMS;

namespace RENDERER.MAP
{
	public class MapEntityRenderer : MonoBehaviour
	{
		public List<EntityBufferObject> entityRenderBuffer = new List<EntityBufferObject>();
		public List<EntityBufferObject> storedRenderBuffer = new List<EntityBufferObject>();
		public TileObject[,] lastUpdateBackgroundData;
        public Tilemap entityViewport;
		public static float entityFlipSpeed = 0.50f;
		private float timer = 0;
		public bool inEditor = false;

		private void Awake()
		{
			timer = entityFlipSpeed;
		}

		private void FixedUpdate()
		{
			if (inEditor) return;
			timer -= Time.fixedDeltaTime;
			if (timer <= 0)
			{
				IncrementBufferedTiles();
				timer = entityFlipSpeed;
			}
		}

		private void ApplyViewportSettings()
		{
			entityViewport.size = entityViewport.WorldToCell(new Vector3Int(MapViewport.viewPortRadius, MapViewport.viewPortRadius, 2));
            entityViewport.ResizeBounds();
		}

		public void CreateNewBuffer(Entity player, List<Entity> relevantEntities, List<Item> relevantItems, List<Gate> relevantGates)
		{
			entityRenderBuffer.Clear();
			entityViewport.ClearAllTiles();
			int bufferObjCount = entityRenderBuffer.Count;

			timer = entityFlipSpeed;

			//In here we create the bufferobjects to later be rendered with RenderEntitiesWithLighting
			//Since species aren't stored yet, let's assume for now that the entity is human.

			if (!inEditor)
			{
				entityRenderBuffer.Add(new EntityBufferObject(
					new List<Sprite> { EntityManager.GetCreatureSprite(player.species) }, 0, 0, 0));
				bufferObjCount++;
			}
			
			for (int i = 0; i < relevantEntities.Count; i++) 
			{
				Vector2Int localPos = new Vector2Int(relevantEntities[i].x - player.x, relevantEntities[i].y - player.y);

				bool foundExisting = false;
				for (int q = 0; q < entityRenderBuffer.Count; q++)
				{
					if (entityRenderBuffer[q].x == localPos.x && entityRenderBuffer[q].y == localPos.y)
					{
						entityRenderBuffer[q].bufferData.Add(EntityManager.GetCreatureSprite(relevantEntities[i].species));
						foundExisting = true;
					}
				}

				if (!foundExisting)
				{
					entityRenderBuffer.Add(new EntityBufferObject(
						new List<Sprite> { EntityManager.GetCreatureSprite(relevantEntities[i].species, relevantEntities[i].spriteIndex) }, localPos.x, localPos.y, 0));
					bufferObjCount++;
				}
			}

			for (int i = 0; i < relevantGates.Count; i++)
			{
				if (!inEditor) break;
				Vector2Int localPos = new Vector2Int(relevantGates[i].x - player.x, relevantGates[i].y - player.y);

				bool foundExisting = false;
				for (int q = 0; q < entityRenderBuffer.Count; q++)
				{
					if (entityRenderBuffer[q].x == localPos.x && entityRenderBuffer[q].y == localPos.y)
					{
						entityRenderBuffer[q].bufferData.Add(SpriteAtlas.FetchSpriteByName("spr_gate_icon"));
						foundExisting = true;
					}
				}

				if (!foundExisting)
				{
					entityRenderBuffer.Add(new EntityBufferObject(
						new List<Sprite> { SpriteAtlas.FetchSpriteByName("spr_gate_icon") }, localPos.x, localPos.y, 0));
					bufferObjCount++;
				}
			}

			FindObjectOfType<MapViewport>().centerPosOnMap = new Vector2Int(player.x, player.y);
			FindObjectOfType<MapViewport>().OnMapUpdate();
		}

		public TileObject[,] RenderEntitiesWithLighting(TileObject[,] backgroundData)
		{
			int halfWidth = ((MapViewport.viewPortRadius - 1) / 2);
			//Instead of doing this, the player will be in the entityManager and be parsed with the other entities for rendering!
			for (int i = 0; i < entityRenderBuffer.Count; i++) 
			{
				Tile tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
				tile.sprite = entityRenderBuffer[i].bufferData[entityRenderBuffer[i].bufferIndex];
				tile.color = backgroundData[entityRenderBuffer[i].x + halfWidth, entityRenderBuffer[i].y + halfWidth].tile.color;
				entityViewport.SetTile(entityViewport.WorldToCell(new Vector3Int(entityRenderBuffer[i].x, entityRenderBuffer[i].y, 0)), tile);
			}

			lastUpdateBackgroundData = backgroundData;
			return backgroundData;
			// go trough the entityRenderBuffer and for each object, create a tile, set it's color and place it on the entityTilemap
		}

		public TileObject[,] RenderEntitiesWithoutLighting(TileObject[,] backgroundData)
		{
			//Instead of doing this, the player will be in the entityManager and be parsed with the other entities for rendering!
			for (int i = 0; i < entityRenderBuffer.Count; i++) 
			{
				Tile tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
				tile.sprite = entityRenderBuffer[i].bufferData[entityRenderBuffer[i].bufferIndex];
				entityViewport.SetTile(entityViewport.WorldToCell(new Vector3Int(entityRenderBuffer[i].x, entityRenderBuffer[i].y, 0)), tile);
			}

			lastUpdateBackgroundData = backgroundData;
			return backgroundData;
		}

		public void IncrementBufferedTiles()
		{
			// clear entityTilemap in order to wipe outdated tileData
			int halfWidth = ((MapViewport.viewPortRadius - 1) / 2);
			// for each entityBufferObject, increment the bufferIndex (account for overflow!) and render the new sprite onto the tile
			foreach (var entityObj in entityRenderBuffer) 
			{
				Tile tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
				entityObj.bufferIndex++;
				if (entityObj.bufferIndex >= entityObj.bufferData.Count)
					entityObj.bufferIndex = 0;
				tile.sprite = entityObj.bufferData[entityObj.bufferIndex];
				tile.color = lastUpdateBackgroundData[entityObj.x + halfWidth, entityObj.y + halfWidth].tile.color;
				entityViewport.SetTile(entityViewport.WorldToCell(new Vector3Int(entityObj.x, entityObj.y, 0)), tile);
			}
		}

		public void DrawCombatRange(Vector2Int localCenter, int distance)
		{
			int halfWidth = ((MapViewport.viewPortRadius - 1) / 2);
			distance++;
			timer = 0;

			storedRenderBuffer.CopyTo(entityRenderBuffer.ToArray());

			// Create tiles to mark area within the distance. 
			for (int y = -distance; y < distance; y++)
				for (int x = -distance; x < distance; x++)
				{
					//Check if this position is on the viewport and if there is a entity there
					Vector2Int tileToCheck = localCenter + new Vector2Int(x, y);

					if (tileToCheck.x < -halfWidth || tileToCheck.x >= halfWidth ||
						tileToCheck.y < -halfWidth || tileToCheck.x >= halfWidth) continue;

					if (Mathf.RoundToInt(Vector2.Distance(tileToCheck, localCenter)) >= distance) continue;

					bool skipThisEntity = false;
					foreach (var bufferObj in entityRenderBuffer) 
					{
						if (tileToCheck.x == bufferObj.x && tileToCheck.y == bufferObj.y)
						{
							bufferObj.bufferData.Add(SpriteAtlas.FetchSpriteByName("spr_selected"));
							skipThisEntity = true;
							break;
						}
					}
					if (skipThisEntity) continue;

					
					entityRenderBuffer.Add(new EntityBufferObject(
						new List<Sprite> { SpriteAtlas.FetchSpriteByName("spr_selected") }, tileToCheck.x, tileToCheck.y, 0));
				}
		}

		public void RedrawStoredBuffer()
		{
			entityViewport.ClearAllTiles();
			entityRenderBuffer.Clear();
			entityRenderBuffer.CopyTo(storedRenderBuffer.ToArray());
			int halfWidth = ((MapViewport.viewPortRadius - 1) / 2);
			//Instead of doing this, the player will be in the entityManager and be parsed with the other entities for rendering!
			for (int i = 0; i < entityRenderBuffer.Count; i++) 
			{
				Tile tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
				tile.sprite = entityRenderBuffer[i].bufferData[entityRenderBuffer[i].bufferIndex];
				tile.color = lastUpdateBackgroundData[entityRenderBuffer[i].x + halfWidth, entityRenderBuffer[i].y + halfWidth].tile.color;
				entityViewport.SetTile(entityViewport.WorldToCell(new Vector3Int(entityRenderBuffer[i].x, entityRenderBuffer[i].y, 0)), tile);
			}
		}

		public void InsertImmediateSprite(Sprite sprite, int x, int y)
		{
			// Place the sprite on the tilemap and reset the timer to give the new sprite a full tick before overwriting it!
			Tile tile = (Tile)ScriptableObject.CreateInstance(typeof(Tile));
			tile.sprite = sprite;
			tile.color = Color.white;
			entityViewport.SetTile(entityViewport.WorldToCell(new Vector3Int(x, y, 0)), tile);
			timer = entityFlipSpeed;
		}
	}

	public class EntityBufferObject
	{
		public List<Sprite> bufferData = new List<Sprite>();
		public int bufferIndex = 0;
		public int x = 0;
		public int y = 0;

		public EntityBufferObject(List<Sprite> bufferData, int x, int y, int startIndex)
		{
			this.bufferData = bufferData;
			this.x = x;
			this.y = y;
			bufferIndex = startIndex;
		}
	}
}
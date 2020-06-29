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
        public Tilemap entityViewport;
		public static float entityFlipSpeed = 0.3f;
		private float timer = 0;

		private void Awake()
		{
			timer = entityFlipSpeed;
		}

		private void FixedUpdate()
		{
			timer -= Time.deltaTime;
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

		public void CreateNewBuffer(Entity player, List<Entity> relevantEntities, List<Item> relevantItems)
		{
			entityRenderBuffer.Clear();
			entityViewport.ClearAllTiles();
			int bufferObjCount = entityRenderBuffer.Count;
			//In here we create the bufferobjects to later be rendered with RenderEntitiesWithLighting
			//Since species aren't stored yet, let's assume for now that the entity is human.

			entityRenderBuffer.Add(new EntityBufferObject(
			new Sprite[] {SpriteAtlas.FetchSpriteByName("spr_human_commoner_0")}, 0, 0, 0));
			bufferObjCount++;

			for (int i = 0; i < relevantEntities.Count; i++) 
			{
				Vector2Int localPos = new Vector2Int(relevantEntities[i].x - player.x, relevantEntities[i].y - player.y);
				Debug.Log(localPos);
				entityRenderBuffer.Add(new EntityBufferObject(
				new Sprite[] {SpriteAtlas.FetchSpriteByName("spr_human_commoner_0")}, localPos.x, localPos.y, 0));
				bufferObjCount++;
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


			return backgroundData;
			// go trough the entityRenderBuffer and for each object, create a tile, set it's color and place it on the entityTilemap
		}

		public void IncrementBufferedTiles()
		{
			// clear entityTilemap in order to wipe outdated tileData
			// for each entityBufferObject, increment the bufferIndex (account for overflow!) and render the new sprite onto the tile
		}

		public void InsertImmediateSprite(Sprite sprite, int x, int y)
		{
			// Place the sprite on the tilemap and reset the timer to give the new sprite a full tick before overwriting it!
		}
	}

	public class EntityBufferObject
	{
		public Sprite[] bufferData = new Sprite[0];
		public int bufferIndex = 0;
		public int x = 0;
		public int y = 0;

		public EntityBufferObject(Sprite[] bufferData, int x, int y, int startIndex)
		{
			this.bufferData = bufferData;
			this.x = x;
			this.y = y;
			bufferIndex = startIndex;
		}
	}
}
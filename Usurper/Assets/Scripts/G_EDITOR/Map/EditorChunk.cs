using UnityEngine;
using RENDERER.MAP;
using RENDERER.UTILS.Atlas;
using UnityEngine.EventSystems;


namespace EDITOR.MAP
{
    public class EditorChunk : MonoBehaviour, IPointerClickHandler
    {
        public Chunk chunkData;

        public void OnPointerClick(PointerEventData eventData)
        {
            //Find where the mouse is and if it's within this chunk!
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mouseWorldPos);

            Vector2 cStartPos = chunkData.GetChunkStartPos();
            if (mouseWorldPos.x >= cStartPos.x && mouseWorldPos.x < cStartPos.x + Chunk.chunkSize &&
                mouseWorldPos.y >= cStartPos.y && mouseWorldPos.x < cStartPos.x + Chunk.chunkSize)
            {
                //We clicked on a tile in the chunk!
                //If the PointerImageGhost selected is not null, rewrite the chunkdata to change that tile
                if (PointerImageGhost.selected.tile != null)
                {
                    Vector2 localMousePos = mouseWorldPos - cStartPos; //Get the mouse pos relative to the chunk
                    Vector2Int roundedPos = new Vector2Int(Mathf.RoundToInt(localMousePos.x), Mathf.RoundToInt(localMousePos.y));
                    ReplaceTileOnChunk(roundedPos, PointerImageGhost.selected.id);
                }
            }

            //Send the data to the EditorMap, set the data returned to chunkData and redraw the chunk!
        }

        private void ReplaceTileOnChunk(Vector2Int tilePos, int idOfNewTile)
        {
            chunkData.mapData[tilePos.x, tilePos.y] = idOfNewTile; //Please work it's like 2am
            chunkData = transform.parent.GetComponent<EditorMap>().OverwriteChunkDataOnMap(chunkData);
            DrawChunk();
        }

        private void DrawChunk()
        {

        }

        public void SetChunkData(Chunk chunk)
        {
            chunkData = chunk;
            DrawChunk();
        }
    }
}
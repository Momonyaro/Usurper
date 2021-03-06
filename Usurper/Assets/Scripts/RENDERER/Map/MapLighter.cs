using UnityEngine;
using System.Collections.Generic;
using RENDERER.UTILS.Atlas;
using RULESET.WORLD;



namespace RENDERER.MAP
{
    //Here we recive the viewport tileData and apply the correct ligthing to it based on time of day and lightSources
    public class MapLighter
    {
        private const int rayCount = 90;
        private const int maxLightsInView = 8;
        private const float logE = 2.718281828f;
        private float ambientLightLevel = 0.15f;
        //private const float ambientLightLevel = 1;

        public TileObject[,] LightPass(TileObject[,] tileData, int dist)
        {
            //int degOffset = 360 / rayCount; // should give a multiplier (ONLY IF NO DECIMAL PRODUCT!!!)
            //here we process line of sight for lightsources

            //Get the new ambientLightLevel from the clock
            // it seems we want pow(e, (-pow(currentHour - totalHours/2, 2) / totalHours))
            ambientLightLevel = Mathf.Pow(logE, (-Mathf.Pow((Clock.currentHour - Clock.totalHours / 2), 2) / Clock.totalHours));
            if (ambientLightLevel <= 0.15f) ambientLightLevel = 0.15f;

            List<Vector2Int> lightPositions = new List<Vector2Int>();
            for (int y = 0; y < MapViewport.viewPortRadius; y++)
            {
                for (int x = 0; x < MapViewport.viewPortRadius; x++)
                {
                    tileData[x, y].tile.color = new Color(ambientLightLevel, ambientLightLevel, ambientLightLevel);
                    if (tileData[x, y].lightSource && lightPositions.Count < maxLightsInView) 
                    {
                        lightPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            //Now we use these lightPositions as a basis to create the lights in the viewport
            foreach (var light in lightPositions)
            {
                tileData = DrawLight(tileData, light.x, light.y, dist);
            }

            return tileData;
        }

        private int DiagonalDistance(int x0, int y0, int x1, int y1)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
        }

        private TileObject[,] DrawLight(TileObject[,] tileData, int x, int y, int dist)
        {
            tileData[x, y].tile.color = new Color(1, 1, 1);
            int degOffset = 360 / rayCount;

            for (int i = 0; i < rayCount; i++)
            {
                int deg = i * degOffset;
                float rad = (float)deg * Mathf.Deg2Rad;
                //We need to get the hypothanuse of a right triangle where x is dist and the angle is deg
                int tileX = Mathf.RoundToInt(Mathf.Cos(rad) * (float)dist) + x;
                int tileY = Mathf.RoundToInt(Mathf.Sin(rad) * (float)dist) + y;

                int d = DiagonalDistance(x, y, tileX, tileY);

                for (int j = 0; j < d; j++)
                {
                    int tx = Mathf.RoundToInt(Mathf.Lerp(x, tileX, j / (float)d));
                    int ty = Mathf.RoundToInt(Mathf.Lerp(y, tileY, j / (float)d));
                    float cMult = 1 - ((j + 1) / (float)d);
                    if (cMult < ambientLightLevel) cMult = ambientLightLevel;

                    //CHECK BOUNDS!
                    if (tx < 0 || tx >= MapViewport.viewPortRadius) continue; 
                    if (ty < 0 || ty >= MapViewport.viewPortRadius) continue;


                    if (tileData[tx, ty].tile.color.r < cMult)
                        tileData[tx, ty].tile.color = new Color(cMult, cMult, cMult, 1);

                    if (tx == x && ty == y) continue;    
                    if (tileData[tx, ty].collider && !tileData[tx,ty].transparent) break;
                }
            }

            return tileData;
        }
    }


}
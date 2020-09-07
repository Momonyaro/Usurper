using System;
using System.Collections;
using System.Collections.Generic;
using RULESET.MANAGERS;
using RENDERER.UTILS.Atlas;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RULESET.DUNGEONS
{
    public class FPSMapBuilder : MonoBehaviour
    {
        public int width;
        public int height;
        public Gate currentGate;
        public static bool buildingDungeon = false;

        public Transform dungeonParent;

        public GameObject wallCube;     //What do we do with doors?
        //Perhaps if we change the cube from two-sided rendering we could give the illusion that you've gone through the door?
        public GameObject floorCube;
        public GameObject playerController;

        //The player could get collision data from fetching the data in here with the currentGate by checking the tile in the next position direction.


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                BuildDungeon(GateManager.Gates[0]);
            }
        }

        public void BuildDungeon(Gate toBuild)
        {
            InputManager.in3DDungeon = true;
            StartCoroutine(DungeonDestructor());
            currentGate = toBuild;
            width = toBuild.width;
            height = toBuild.height;
            StartCoroutine(DungeonConstructor());
        }

        private IEnumerator DungeonConstructor()
        {
            if (buildingDungeon) yield return new WaitForEndOfFrame();
            buildingDungeon = true;

            //Here we build the dungeon based on the specified Gate!
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Depending on the collision tag on the sprite in the atlas, spawn the wall or floor tile.
                    // Also we should spawn the dungeonPlayer on the entrance for now until a spawnPos is specified.
                    // So we spawn the 3D player camera, deactivate the 2D one and hide this with a loading screen.
                    // Unloading the camera destroys the dungeon & the 3d player and sets the 2D camera to active again.
                    TileObject tile = TileAtlas.FetchDungeonTileObjectById(currentGate.dngData[x, y]);
                    GameObject tileCube;
                    if (tile.transparent)
                    {
                        //Place floor at x,y position
                        tileCube = Instantiate(floorCube, new Vector3(x, 0, y), Quaternion.identity, dungeonParent);
                    }
                    else
                    {
                        //Place wall at x,y position
                        tileCube = Instantiate(wallCube, new Vector3(x, 0, y), Quaternion.identity, dungeonParent);
                    }

                    DungeonCube dungeonCube = tileCube.GetComponent<DungeonCube>();
                    dungeonCube.SetCubeSprite(tile.tile.sprite);
                }
            }

            buildingDungeon = false;
            yield break;
        }

        private IEnumerator DungeonDestructor()
        {
            if (buildingDungeon) yield return new WaitForEndOfFrame();
            buildingDungeon = true;

            while (dungeonParent.childCount > 0)
            {
                Destroy(dungeonParent.GetChild(0).gameObject);
            }

            buildingDungeon = false;
            yield break;
        }

        public bool FetchCollisionDataAtPosition(int x, int y)
        {
            // Might not be the best solution but it should work!
            return TileAtlas.FetchDungeonTileObjectById(currentGate.dngData[x, y]).collider;
        }

        //the buildDungeon method would spawn the correct prefab based on collision tag and hand the cube the correct sprite.
        //We would also have to create a way to place the player and choose the player facing direction.
    }
}

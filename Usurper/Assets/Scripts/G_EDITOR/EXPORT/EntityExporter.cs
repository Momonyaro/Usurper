using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RULESET.MANAGERS;
using RULESET.ENTITIES;
using RENDERER.UTILS.Atlas;
using LitJson;


namespace EDITOR.EXPORT
{
    public class EntityExporter
    {
        //This class is responsible for exporting the species and the actors.
        //Later we have to decide how we want to store actors when dialogue is a factor. (Perhaps we'll keep this as a mega file?)

        private const string fileExtention = ".dbase";

        public EntityBlock LoadEntities(string mapName)
        {
            EntityBlock toReturn = new EntityBlock(new List<ActorEntity>(), new List<CreatureSpecies>());
            JsonData fileData = JsonMapper.ToObject(File.ReadAllText(mapName + "/" + "Actors" + fileExtention));

            for (int i = 0; i < fileData["creatures"].Count; i++)
            {
                List<CompactBodyPart> bodyParts = new List<CompactBodyPart>();
                for (int y = 0; y < fileData["creatures"][i]["bodyParts"].Count; y++)
                {
                    JsonData bodyPartData = fileData["creatures"][i]["bodyParts"][y];
                    bodyParts.Add(new CompactBodyPart()
                    {
                        name = bodyPartData["name"].ToString(),
                        countsForDamage = (bool)bodyPartData["countsForDamage"],
                        hitThreshold = (int)bodyPartData["hitThreshold"],
                        damageMultiplier = (int)bodyPartData["damageMultiplier"],
                        canHoldType = (int)bodyPartData["canHoldType"],
                        canEquipType = (int)bodyPartData["canEquipType"],
                        partX = (int)bodyPartData["partX"],
                        partY = (int)bodyPartData["partY"],
                        width = (int)bodyPartData["width"],
                        height = (int)bodyPartData["height"],
                        angle = (int)bodyPartData["angle"]
                    });
                }

                string[] importedSpriteNames = new string[fileData["creatures"][i]["spriteNames"].Count];
                for (int q = 0; q < fileData["creatures"][i]["spriteNames"].Count; q++)
                {
                    importedSpriteNames[q] = fileData["creatures"][i]["spriteNames"][q].ToString();
                }

                CompactCreature c = new CompactCreature()
                {
                    name = fileData["creatures"][i]["name"].ToString(),
                    desc = fileData["creatures"][i]["desc"].ToString(),
                    id = (int)fileData["creatures"][i]["id"],
                    spriteNames = importedSpriteNames, //Change to list to have a pool of sprites instead.
                    averageStats = new int[7]
                    {
                        (int)fileData["creatures"][i]["averageStats"][0],
                        (int)fileData["creatures"][i]["averageStats"][1],
                        (int)fileData["creatures"][i]["averageStats"][2],
                        (int)fileData["creatures"][i]["averageStats"][3],
                        (int)fileData["creatures"][i]["averageStats"][4],
                        (int)fileData["creatures"][i]["averageStats"][5],
                        (int)fileData["creatures"][i]["averageStats"][6]
                    },
                    // Add creature bonuses as well!
                    bodyParts = bodyParts
                };
                Debug.Log(c.id);
                toReturn.creatures.Add(c);
            }

            for (int i = 0; i < fileData["actors"].Count; i++)
            {
                ActorEntity a = new ActorEntity()
                {
                    name = fileData["actors"][i]["name"].ToString(),
                    x = (int)fileData["actors"][i]["x"],
                    y = (int)fileData["actors"][i]["y"],
                    species = (int)fileData["actors"][i]["species"],
                    spriteIndex = (int)fileData["actors"][i]["spriteIndex"],
                    actorId = fileData["actors"][i]["actorId"].ToString(),
                    stats = new int[7]
                    {
                        (int)fileData["actors"][i]["stats"][0],
                        (int)fileData["actors"][i]["stats"][1],
                        (int)fileData["actors"][i]["stats"][2],
                        (int)fileData["actors"][i]["stats"][3],
                        (int)fileData["actors"][i]["stats"][4],
                        (int)fileData["actors"][i]["stats"][5],
                        (int)fileData["actors"][i]["stats"][6],
                    },
                };
                toReturn.actors.Add(a);
            }

            return toReturn;
        }

        public void SaveEntities(string mapPath)
        {
            string finalPath = mapPath + "/Actors" + fileExtention;
            EntityBlock export = new EntityBlock(EntityManager.actors, EntityManager.creatures);

            JsonData fileData = JsonMapper.ToJson(export);

            File.WriteAllText(finalPath, fileData.ToString());
        }
    }

    [System.Serializable]
    public struct EntityBlock
    {
        public List<ActorEntity> actors;
        public List<CompactCreature> creatures;

        public EntityBlock(List<ActorEntity> actors, List<CreatureSpecies> creatures)
        {
            this.actors = actors;
            List<CompactCreature> compacts = new List<CompactCreature>();
            for (int i = 0; i < creatures.Count; i++)
            {
                compacts.Add(new CompactCreature(creatures[i]));
            }
            this.creatures = compacts;
        }
    }

    public struct CompactCreature
    {
        public string name;
        public string desc;
        public int id;
        public string[] spriteNames; //Change to list to have a pool of sprites instead.
        public int[] averageStats;
        // Add creature bonuses as well!
        public List<CompactBodyPart> bodyParts;

        public CompactCreature(string name, string desc, string[] spriteNames)
        {
            this.name = name;
            this.desc = desc;
            this.id = 420;
            this.spriteNames = spriteNames;
            averageStats = new int[7] { 1, 1, 1, 1, 1, 1, 1 };
            bodyParts = new List<CompactBodyPart>();
        }

        public CompactCreature(CreatureSpecies creature)
        {
            this.name = creature.name;
            this.desc = creature.desc;
            this.id = creature.id;
            string[] sprNames = new string[creature.sprites.Count];
            for (int i = 0; i < creature.sprites.Count; i++)
            {
                sprNames[i] = creature.sprites[i].name;
            }
            this.spriteNames = sprNames;
            averageStats = creature.averageStats;
            bodyParts = new List<CompactBodyPart>();
            for (int i = 0; i < creature.bodyParts.Count; i++)
            {
                bodyParts.Add(new CompactBodyPart(creature.bodyParts[i]));
            }
        }
    }

    [System.Serializable]
    public class CompactBodyPart
    {
        public string name;
        public bool countsForDamage;
        public int hitThreshold;
        public int damageMultiplier;
        public int canHoldType;            // Dictionary identifier!
        public int canEquipType;           // Another Dictionary Identifier!
        public int partX;
        public int partY;
        public int width;
        public int height;
        public int angle;

        public CompactBodyPart(CreatureBodyPart compact)
        {
            name = compact.containedBodyPart.name;
            damageMultiplier = compact.containedBodyPart.damageMultiplier;
            hitThreshold = compact.containedBodyPart.hitThreshold;
            canEquipType = compact.containedBodyPart.canEquipType;
            canHoldType = compact.containedBodyPart.canHoldType;
            countsForDamage = compact.containedBodyPart.countsForDamage;
            partX = compact.bodyPartRect.x;
            partY = compact.bodyPartRect.y;
            width = compact.bodyPartRect.width;
            height = compact.bodyPartRect.height;
            angle = compact.angle;
        }

        public CompactBodyPart()
        {

        }

        public CreatureBodyPart ExpandCompacted()
        {
            CreatureBodyPart toReturn = new CreatureBodyPart(new BodyPart(), new RectInt(), 0);

            toReturn.containedBodyPart.name = name;
            toReturn.containedBodyPart.countsForDamage = countsForDamage;
            toReturn.containedBodyPart.hitThreshold = hitThreshold;
            toReturn.containedBodyPart.damageMultiplier = damageMultiplier;
            toReturn.containedBodyPart.canHoldType = canHoldType;
            toReturn.containedBodyPart.canEquipType = canEquipType;
            toReturn.bodyPartRect.x = partX;
            toReturn.bodyPartRect.y = partY;
            toReturn.bodyPartRect.width = width;
            toReturn.bodyPartRect.height = height;
            toReturn.angle = angle;

            return toReturn;
        }
    }
}

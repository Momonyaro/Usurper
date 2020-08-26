using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EDITOR.SYSTEMS;
using RENDERER.UTILS.Atlas;
using RULESET.MANAGERS;
using RULESET.ITEMS;
using LitJson;

namespace EDITOR.EXPORT
{
    public class ItemExporter
    {
        private const string databaseExtention = ".dbase";

        public void SaveItemDatabase(string path)
        {
            //Here we write the items and groups to the .dbase file
            List<ExportItem> exportItems = new List<ExportItem>();
            foreach (var item in ItemManager.itemDatabase)
            {
                exportItems.Add(new ExportItem(item));
            }

            List<ExportItemGroup> exportGroups = new List<ExportItemGroup>();
            foreach (var group in ItemManager.itemGroups)
            {
                exportGroups.Add(new ExportItemGroup(group));
            }

            ExportDatabase exportDatabase = new ExportDatabase(exportGroups, exportItems);
            JsonData databaseData = JsonMapper.ToJson(exportDatabase);
            string finalPath = path + "Items" + databaseExtention;
            File.WriteAllText(finalPath, databaseData.ToString());
        }

        public ExportDatabase LoadItemDatabase(string path)
        {
            ExportDatabase fetched = new ExportDatabase(new List<ExportItemGroup>(), new List<ExportItem>());
            JsonData fileData = JsonMapper.ToObject(File.ReadAllText(path + "/" + "Items" + databaseExtention));

            fetched.exportGroups = new ExportItemGroup[fileData["exportGroups"].Count];
            for (int i = 0; i < fileData["exportGroups"].Count; i++)
            {
                fetched.exportGroups[i] = new ExportItemGroup()
                {
                    id = (int)fileData["exportGroups"][i]["id"],
                    groupName = fileData["exportGroups"][i]["groupName"].ToString(),
                    groupSpriteName = fileData["exportGroups"][i]["groupSpriteName"].ToString()
                };
            }

            fetched.exportItems = new ExportItem[fileData["exportItems"].Count];
            for (int i = 0; i < fileData["exportItems"].Count; i++)
            {
                fetched.exportItems[i] = new ExportItem()
                {
                    name = fileData["exportItems"][i]["name"].ToString(),
                    desc = fileData["exportItems"][i]["desc"].ToString(),
                    stackable = (bool)fileData["exportItems"][i]["stackable"],
                    amount = (short)fileData["exportItems"][i]["amount"],
                    value = (short)fileData["exportItems"][i]["value"],
                    weight = (int)fileData["exportItems"][i]["weight"],     //In grams!
                    groupId = (int)fileData["exportItems"][i]["groupId"],
                    x = (int)fileData["exportItems"][i]["x"],
                    y = (int)fileData["exportItems"][i]["y"],
                    itemCategory = (ITEM_CATEGORIES)(int)fileData["exportItems"][i]["itemCategory"],
                    damageType = (DAMAGE_TYPES)(int)fileData["exportItems"][i]["damageType"],
                    damage = (short)fileData["exportItems"][i]["damage"],
                    armor = (short)fileData["exportItems"][i]["armor"],
                    range = (short)fileData["exportItems"][i]["range"],
                    usesAmmo = (bool)fileData["exportItems"][i]["usesAmmo"],
                    ammoType = (int)fileData["exportItems"][i]["ammoType"]  //This will be set to the group of the ammo
                };
            }

            return fetched;
        }
    }

    [System.Serializable]
    public struct ExportDatabase
    {
        public ExportItemGroup[] exportGroups;
        public ExportItem[] exportItems;

        public ExportDatabase(List<ExportItemGroup> exportGroups, List<ExportItem> exportItems)
        {
            this.exportGroups = exportGroups.ToArray();
            this.exportItems = exportItems.ToArray();
        }
    }

    [System.Serializable]
    public struct ExportItem
    {
        public string name;
        public string desc;
        public bool stackable;
        public short amount;
        public short value;
        public int weight;     //In grams!
        public int groupId;
        public int x;
        public int y;

        public ITEM_CATEGORIES itemCategory;
        public DAMAGE_TYPES damageType;
        public short damage;
        public short armor;
        public short range;
        public bool  usesAmmo;
        public int   ammoType;  //This will be set to the group of the ammo

        public ExportItem(Item toShrink)
        {
            name = toShrink.name;
            desc = toShrink.desc;
            stackable = toShrink.stackable;
            amount = toShrink.amount;
            value = toShrink.value;
            weight = toShrink.weight;
            groupId = toShrink.groupId;
            x = toShrink.x;
            y = toShrink.y;
            itemCategory = toShrink.itemCategory;
            damageType = toShrink.damageType;
            damage = toShrink.damage;
            armor = toShrink.armor;
            range = toShrink.range;
            usesAmmo = toShrink.usesAmmo;
            ammoType = toShrink.ammoType;
        }
    }

    [System.Serializable]
	public struct ExportItemGroup
	{
		public int id;
		public string groupName;
		public string groupSpriteName;

        public ExportItemGroup(ItemGroup toShrink)
        {
            id = toShrink.id;
            groupName = toShrink.groupName;
            groupSpriteName = toShrink.groupSprite.name;
        }
	}
}
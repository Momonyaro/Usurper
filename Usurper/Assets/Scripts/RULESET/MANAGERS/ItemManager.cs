using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RULESET.ITEMS;

namespace RULESET.MANAGERS
{
	public class ItemManager : MonoBehaviour
	{
		public static List<ItemGroup> itemGroups = new List<ItemGroup>();
		public static List<Item> itemDatabase = new List<Item>();

		private void Start()
		{
		}
	}
}



namespace RULESET.ITEMS
{
	[System.Serializable]
	public class ItemGroup
	{
		public int id = 0;
		public string groupName = "New Group";
		public Sprite groupSprite = null;
	}
}
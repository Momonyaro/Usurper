using UnityEngine;
using RULESET.ITEMS;

//Here we just want a list with items. Present that list and allow editing the properties of individual elements.
//Also create functionality for adding / removing items from the pool

namespace EDITOR.SYSTEMS
{
	public class ItemPoolEditor : MonoBehaviour
	{
		// In this editor we want users to be able to create weapon / armor types, ammunition types.
		// After exporting this, we can use the databases to have a standard for items.
		// If we want to just put a weapon in a chest, we can just call the database and get the standard version
		// We can also add a random effect to some of these items to give variety.
		// Items are stored with all their parameters in the players .WORLD save except the ones in the inventory and that are equipped. They are stored in the .PLAYER save
	}

	public class WeaponType
	{

	}

	public class ClothingType
	{

	}

	public class AmmoType
	{
		
	}
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RULESET.MANAGERS
{
	public class UiManager : MonoBehaviour
	{
		// UI manager will be responsible for handshakes between the UI and the systems.
		// The inventory for example, will be (based on the inventory) populated with list representations
		// That hold an index for the stored item as well as reacting to the mouse to create a pop up
		// Where actions can be executed based on the item. Inspect & drop will always be available though.
		// If the creature you're playing as also has hands then hold will also be available as a constant option.
	}
}
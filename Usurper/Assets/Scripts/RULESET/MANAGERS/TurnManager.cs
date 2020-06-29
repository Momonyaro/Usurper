using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RULESET.MANAGERS
{
	public class TurnManager : MonoBehaviour
	{
		// The turnmanager will be pretty simple, it simply accepts a input that allows it to do a full render-cycle
		// That means that we redraw the map, process entities and items and draw them.
		// We simply need to figure out how to process the player position and hand it to the mapViewport.
		// It could simply be a return value for the entityManager I guess but we'll see.
		

	    public void PlayerEndTurn(Vector2 mvmtDirection)
	    {
	    	//Pass the mvmtDirection to the entityManager to set the player's new position.
	    	GetComponent<EntityManager>().UpdatePlayer(mvmtDirection);
	    }
	}
}

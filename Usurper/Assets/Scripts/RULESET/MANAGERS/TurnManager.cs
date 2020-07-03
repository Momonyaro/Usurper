using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RULESET.WORLD;

namespace RULESET.MANAGERS
{
	public class TurnManager : MonoBehaviour
	{
		Clock clock = new Clock();
		// The turnmanager will be pretty simple, it simply accepts a input that allows it to do a full render-cycle
		// That means that we redraw the map, process entities and items and draw them.
		// We simply need to figure out how to process the player position and hand it to the mapViewport.
		// It could simply be a return value for the entityManager I guess but we'll see.
		

		private void Awake()
		{
			Clock.instance = clock;
		}

	    public void PlayerEndTurn(Vector2Int mvmtDirection)
	    {
	    	//Pass the mvmtDirection to the entityManager to set the player's new position.
	    	GetComponent<EntityManager>().UpdatePlayer(mvmtDirection);
	    	clock.IncrementTicksByAmount(1);
	    }

	    public void EditorEndTurn(Vector2Int mvmtDirection)
	    {
	    	GetComponent<EntityManager>().UpdateEditorCursor(mvmtDirection);
	    }
	}
}

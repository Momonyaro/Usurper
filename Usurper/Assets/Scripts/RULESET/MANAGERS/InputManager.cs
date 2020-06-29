using UnityEngine;
using RENDERER.MAP;

namespace RULESET.MANAGERS
{
	public class InputManager : MonoBehaviour
	{
		MapViewport mView;
		//Add input customizability and gamepad support later!
		//Add walk timer to skip repeated presses.

		float timeBetweenInputs = 0.25f;
		float timer = 0;

		private void FixedUpdate()
		{
			timer -= Time.deltaTime;
		}

		private void Awake()
		{
			mView = Object.FindObjectOfType<MapViewport>();
		}

		private void Update()
		{
			if (mView.inEditor)
			{
				EditorControls();
			}
			else
			{
				if (timer <= 0)
				{
					GameControls();
				}
			}
		}

		private void EditorControls()
		{
			int mvmtOffset = 1;
			if (Input.GetKey(KeyCode.LeftControl))
            {
            	mvmtOffset = 10;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    mvmtOffset = 64;
                }
            }
            if (Input.GetKey(KeyCode.W)) { mView.centerPosOnMap += Vector2Int.up * mvmtOffset; mView.OnMapUpdate(); return; }
            else if (Input.GetKey(KeyCode.A)) { mView.centerPosOnMap += Vector2Int.left * mvmtOffset; mView.OnMapUpdate(); return; }
            else if (Input.GetKey(KeyCode.S)) { mView.centerPosOnMap += Vector2Int.down * mvmtOffset; mView.OnMapUpdate(); return; }
            else if (Input.GetKey(KeyCode.D)) { mView.centerPosOnMap += Vector2Int.right * mvmtOffset; mView.OnMapUpdate(); return; }
		}

		private void GameControls()
		{
			//Later we need to pass the input to either the turnmanager or the uimanager instead of directly feeding
			//commands to the mapViewport!

			if (Input.GetKeyDown(KeyCode.I)) { StateManager.gameState = StateManager.gameState == GameStates.OVERWORLD ? GameStates.INVENTORY : GameStates.OVERWORLD; }

			if (StateManager.gameState == GameStates.OVERWORLD)
			{
				if (Input.GetKey(KeyCode.W)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2.up); timer = timeBetweenInputs; return; }
            	else if (Input.GetKey(KeyCode.A)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2.left); timer = timeBetweenInputs; return; }
	            else if (Input.GetKey(KeyCode.S)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2.down); timer = timeBetweenInputs; return; }
	            else if (Input.GetKey(KeyCode.D)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2.right); timer = timeBetweenInputs; return; }
			}
		}
	}
}
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
			if (!mView.initialized) return;
			if (mView.inEditor)
			{
				if (timer <= 0)
				{
					EditorControls();
				}
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
			if (Input.GetKeyDown(KeyCode.Escape)) { PointerImageGhost.ClearSelected(); return; }

			int mvmtOffset = 5;
			if (Input.GetKey(KeyCode.LeftControl))
            {
            	mvmtOffset = 10;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    mvmtOffset = 64;
                }
            }
            if (Input.GetKey(KeyCode.W)) { GetComponent<TurnManager>().EditorEndTurn(Vector2Int.up * mvmtOffset); timer = timeBetweenInputs; return; }
            else if (Input.GetKey(KeyCode.A)) { GetComponent<TurnManager>().EditorEndTurn(Vector2Int.left * mvmtOffset); timer = timeBetweenInputs; return; }
            else if (Input.GetKey(KeyCode.S)) { GetComponent<TurnManager>().EditorEndTurn(Vector2Int.down * mvmtOffset); timer = timeBetweenInputs; return; }
            else if (Input.GetKey(KeyCode.D)) { GetComponent<TurnManager>().EditorEndTurn(Vector2Int.right * mvmtOffset); timer = timeBetweenInputs; return; }
		}

		private void GameControls()
		{
			//Later we need to pass the input to either the turnmanager or the uimanager instead of directly feeding
			//commands to the mapViewport!

			// Inventory screen
			if (Input.GetKeyDown(KeyCode.I)) 
			{
				if (StateManager.gameState == GameStates.OVERWORLD)
					StateManager.gameState = GameStates.INVENTORY;
				else if (StateManager.gameState == GameStates.INVENTORY)
					StateManager.gameState = GameStates.OVERWORLD;

				// Change the ui.
				return;
			}

			// Targeting mode
			if (Input.GetKeyDown(KeyCode.V))
			{
				if (StateManager.gameState == GameStates.OVERWORLD)
				{
					StateManager.gameState = GameStates.COMBAT;
					FindObjectOfType<MapEntityRenderer>().DrawCombatRange(new Vector2Int(0, 0), 2);
				}
				else if (StateManager.gameState == GameStates.COMBAT)
				{
					StateManager.gameState = GameStates.OVERWORLD;
					FindObjectOfType<MapEntityRenderer>().RedrawStoredBuffer();
				}

				// Render the targeting mode
				return;
			}

			// Map screen
			if (Input.GetKeyDown(KeyCode.M))
			{
				if (StateManager.gameState == GameStates.OVERWORLD)
					StateManager.gameState = GameStates.MAP;
				else if (StateManager.gameState == GameStates.MAP)
					StateManager.gameState = GameStates.OVERWORLD;

				// Change the ui.
				return;
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				StateManager.gameState = GameStates.OVERWORLD;
				return;
			}

			if (StateManager.gameState == GameStates.OVERWORLD)
			{

				if (Input.GetKey(KeyCode.W)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2Int.up); timer = timeBetweenInputs; return; }
            	else if (Input.GetKey(KeyCode.A)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2Int.left); timer = timeBetweenInputs; return; }
	            else if (Input.GetKey(KeyCode.S)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2Int.down); timer = timeBetweenInputs; return; }
	            else if (Input.GetKey(KeyCode.D)) { GetComponent<TurnManager>().PlayerEndTurn(Vector2Int.right); timer = timeBetweenInputs; return; }
			}
		}
	}
}
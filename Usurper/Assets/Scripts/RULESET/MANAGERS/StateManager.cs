using UnityEngine;

namespace RULESET.MANAGERS
{
	public enum GameStates
	{
		OVERWORLD,
		INVENTORY,
		COMBAT,
		PAUSED,
		LOOTING,
		CONVERSATION,
		LOBOTOMIZED,
		POPUP,
		CHARACTER_CREATION,
		MAP
	}

	public delegate void OnNewState();

	public class StateManager : MonoBehaviour
	{
		public static GameStates gameState = GameStates.OVERWORLD;
		public OnNewState onNewState;
	}
}
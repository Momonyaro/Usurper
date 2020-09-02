using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RULESET.MANAGERS;

namespace RULESET.UI
{
    public class PlayerCardHUD : MonoBehaviour, IPointerClickHandler
    {
        private enum PlayerCardStates
        {
            NEUTRAL,
            GO_TO_MAX,
            GO_TO_START,
        }

        private readonly int maxScroll = 240;
        private PlayerCardStates currentState = PlayerCardStates.NEUTRAL;
        private PlayerCardStates lastAction = PlayerCardStates.GO_TO_START;
        public float scrollSpeed = 2;
        public Transform hudTransform;


        //Next thing to do here is to plug in the UI elements with the player's values! Since we don't even have separated max and current health yet though... it's a bit far off.
        public Image portrait;
        public Text healthText;
        public Image healthBar;
        public Text manaText;
        public Image manaBar;
        public Text levelText;
        public Image levelBar;



        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                currentState = (lastAction == PlayerCardStates.GO_TO_START) ? PlayerCardStates.GO_TO_MAX : PlayerCardStates.GO_TO_START;
            }
        }

        private void FixedUpdate()
        {
            if (currentState == PlayerCardStates.NEUTRAL) return;

            if (currentState == PlayerCardStates.GO_TO_MAX)
            {
                hudTransform.localPosition = Vector3.Lerp(hudTransform.localPosition, new Vector3(0, maxScroll, 0), scrollSpeed);
                if (Vector3.Distance(hudTransform.localPosition, new Vector3(0, maxScroll, 0)) < 0.3f) currentState = PlayerCardStates.NEUTRAL;
                lastAction = PlayerCardStates.GO_TO_MAX;
            }
            else
            {
                hudTransform.localPosition = Vector3.Lerp(hudTransform.localPosition, new Vector3(0, 0, 0), scrollSpeed);
                if (Vector3.Distance(hudTransform.localPosition, new Vector3(0, 0, 0)) < 0.3f) currentState = PlayerCardStates.NEUTRAL;
                lastAction = PlayerCardStates.GO_TO_START;
            }
            UpdateHUDInfo();
        }

        private void UpdateHUDInfo()
        {
            var player = EntityManager.playerEntity;
            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;
            healthText.text = $"Health:  {player.health} / {player.maxHealth}";
            manaBar.fillAmount = (float)player.mana / (float)player.maxMana;
            manaText.text = $"Mana:  {player.mana} / {player.maxMana}";
            levelBar.fillAmount = (float)player.xp / (float)player.xpNeeded;
            levelText.text = $"Level:  {player.level}";
        }
    }
}

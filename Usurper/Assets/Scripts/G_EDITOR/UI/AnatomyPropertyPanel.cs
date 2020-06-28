using UnityEngine;
using UnityEngine.UI;
using EDITOR.SYSTEMS;

public class AnatomyPropertyPanel : MonoBehaviour
{
	public InputField bodyPartNameField;
	public Toggle bodyPartCountsForDamageToggle;
	public Slider hitThreshSlider;
	public Text   hitThreshValue;
	public Slider damMultSlider;
	public Text   damMultValue;

	public void PopulateProperties()
	{
		if (CreatureAnatomyPanel.selected == null) return;


		RULESET.ENTITIES.BodyPart selectedBodyPart = CreatureAnatomyPanel.selected.containedBodyPart;
		bodyPartNameField.SetTextWithoutNotify(selectedBodyPart.name);
		bodyPartCountsForDamageToggle.SetIsOnWithoutNotify(selectedBodyPart.countsForDamage);
		hitThreshSlider.SetValueWithoutNotify(selectedBodyPart.hitThreshold);
		hitThreshValue.text = "> " + selectedBodyPart.hitThreshold;
		damMultSlider.SetValueWithoutNotify(selectedBodyPart.damageMultiplier);
		damMultValue.text = selectedBodyPart.damageMultiplier + "%";
	}
}
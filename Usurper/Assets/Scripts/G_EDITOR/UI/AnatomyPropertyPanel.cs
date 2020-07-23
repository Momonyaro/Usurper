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

	public void SetBodyPartName(string name)
	{
		CreatureAnatomyPanel.selected.containedBodyPart.name = name;
		FindObjectOfType<CreatureDesigner>().DrawCreature();
	}

	public void SetBodyPartBool(bool countsForDamage)
	{
		CreatureAnatomyPanel.selected.containedBodyPart.countsForDamage = countsForDamage;
		FindObjectOfType<CreatureDesigner>().DrawCreature();
	}

	public void SetBodyPartHitThreshold(float hitThresh)
	{
		CreatureAnatomyPanel.selected.containedBodyPart.hitThreshold = Mathf.RoundToInt(hitThresh);
		FindObjectOfType<CreatureDesigner>().DrawCreature();
	}

	public void SetBodyPartDmgMultiplier(float damageMultiplier)
	{
		CreatureAnatomyPanel.selected.containedBodyPart.damageMultiplier = Mathf.RoundToInt(damageMultiplier);
		FindObjectOfType<CreatureDesigner>().DrawCreature();
	}
}
using System.Collections;
using UnityEngine;

//Changes the glowing color of the sword depending on state
public class SwordColorChanger : MechComponent
{
	[SerializeField] Renderer rnd;
	[SerializeField] Color glowAttackColor, glowBlockColor, neutralColor;
	[SerializeField] float baseDuration = 1f;
	Color nonWindupEmission;
	[SerializeField] Light swordLight;

	public enum ColorState { Neutral, Block, Attack }
	public ColorState colorState { get; private set; }
	public ColorState prevColorState { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		//Start neutral
		TweenToNeutralColor();

		arms.stancePicker.OnStanceBegin += TweenToNeutralColor;	//White
		arms.retract.OnRetractBegin += TweenToNeutralColor;
		arms.stagger.OnStaggerBegin += TweenToNeutralColor;
		arms.blocker.OnBlockBegin += TweenToBlockColor;	//Blue

		//windup.OnWindupBegin -= TweenToAttackColor;
		//windup.OnWindupBegin += TweenToAttackColor;
	}

	IEnumerator ChangeColorRoutine(Color newGlowColor, float emission, float duration = 1f, float newLightIntensity = 1f)
	{
		if (!rnd)
			yield break;

		float timer = 0f;

		//Get the emission color from the standard shader.
		Color oldGlowColor = rnd.materials[2].GetColor("_EmissionColor");
		float oldLightIntensity = swordLight.intensity;
		newGlowColor *= emission;

		//Check if not the same, since lerping to the same color causes some kind of curve either way.
		if (colorState != prevColorState)
		{
			while (timer < duration)
			{
				timer += Time.deltaTime;

				//Tween the color from old to new.
				Color lerpGlowColor = Color.Lerp(oldGlowColor, newGlowColor, timer / duration);
				SetGlowColor(lerpGlowColor);

				//Modify the light on the sword, intensity and color.
				swordLight.color = lerpGlowColor / emission;
				swordLight.intensity = Mathf.Lerp(oldLightIntensity, newLightIntensity, timer / duration);
				yield return null;
			}
		}
	}

	void TweenToBlockColor()
	{
		colorState = ColorState.Block;
		StopAllCoroutines();
		StartCoroutine(ChangeColorRoutine(glowBlockColor, 4f, baseDuration));
	}

	void TweenToNeutralColor()
	{
		colorState = ColorState.Neutral;
		StopAllCoroutines();
		StartCoroutine(ChangeColorRoutine(neutralColor, 2f, baseDuration, 0.1f));
	}

	void TweenToAttackColor(float factor)	//Runs continously while winding up.
	{
		colorState = ColorState.Attack;
		Color targetColor = glowAttackColor;
		targetColor *= 15f;
		factor /= 2f;	//Duration of the sword powerup

		SetGlowColor(Color.Lerp(nonWindupEmission, targetColor, factor));

		//Increase light intensity and flicker while charging up the attack:
		if (factor < 1f)
		{
			swordLight.intensity = FlickerLerp(factor);
		}
		else
		{
			swordLight.intensity = 1f;
		}
	}

	float FlickerLerp(float value)
	{
		return value * Random.Range(0.8f, 1.2f);
	}

	void SetGlowColor(Color newColor)
	{
		rnd.materials[2].SetColor("_EmissionColor", newColor);
	}

	protected override void OnUpdate()
	{
		//Turn red gradually when winding up:
		if (arms.combatState == WeaponsOfficer.CombatState.Windup)
		{
			TweenToAttackColor(arms.windup.windupTimer);
		}

		//Tween down the intensity while attacking:
		if (arms.combatState == WeaponsOfficer.CombatState.Attack)
		{
			swordLight.intensity = Mathf.Lerp(swordLight.intensity, 0.2f, Time.deltaTime * 4f);
		}


		//Just set this so we can lerp from it when winding up..
		if (arms.combatState != WeaponsOfficer.CombatState.Windup
			&& arms.combatState != WeaponsOfficer.CombatState.Attack
			&& rnd != null)
		{
			nonWindupEmission = rnd.materials[2].GetColor("_EmissionColor");
		}

		prevColorState = colorState;
	}
}
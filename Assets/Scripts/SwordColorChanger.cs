using System.Collections;
using UnityEngine;

public class SwordColorChanger : MechComponent
{
	[SerializeField] Renderer rnd;
	[SerializeField] Color glowAttackColor, glowBlockColor, neutralColor;
	[SerializeField] float baseDuration = 1f;
	Color nonWindupEmission;
	[SerializeField] Light swordLight;

	protected override void OnAwake()
	{
		base.OnAwake();
	}

	void Start()
	{
		TweenToNeutralColor();
		blocker.OnBlockBegin -= TweenToBlockColor;
		blocker.OnBlockBegin += TweenToBlockColor;

		stancePicker.OnStanceBegin -= TweenToNeutralColor;
		stancePicker.OnStanceBegin += TweenToNeutralColor;
		retract.OnRetractBegin -= TweenToNeutralColor;
		retract.OnRetractBegin += TweenToNeutralColor;
		stagger.OnStaggerBegin -= TweenToNeutralColor;
		stagger.OnStaggerBegin += TweenToNeutralColor;

		//windup.OnWindupBegin -= TweenToAttackColor;
		//windup.OnWindupBegin += TweenToAttackColor;
	}

	bool CompareColor(Color col1, Color col2)
	{
		float r1 = Mathf.Floor(col1.r * 1000f) / 1000f;
		float r2 = Mathf.Floor(col2.r * 1000f) / 1000f;

		if (Mathf.Floor(col1.r * 1000f) / 1000f == Mathf.Floor(col2.r * 1000f) / 1000f
			&& Mathf.Floor(col1.g * 1000f) / 1000f == Mathf.Floor(col2.g * 1000f) / 1000f
			&& Mathf.Floor(col1.b * 1000f) / 1000f == Mathf.Floor(col2.b * 1000f) / 1000f)
		{
			return true;
		}

		return false;
	}

	IEnumerator ChangeColorRoutine(Color newGlowColor, float emission, float duration = 1f, float newLightIntensity = 1f)
	{
		float timer = 0f;
		Color oldGlowColor = rnd.materials[2].GetColor("_EmissionColor");
		float oldLightIntensity = swordLight.intensity;
		newGlowColor *= emission;

		if (!CompareColor(oldGlowColor, newGlowColor))
		{
			while (timer < duration)
			{
				timer += Time.deltaTime;

				Color lerpGlowColor = Color.Lerp(oldGlowColor, newGlowColor, timer / duration);
				SetGlowColor(lerpGlowColor);
				swordLight.color = lerpGlowColor / emission;
				swordLight.intensity = Mathf.Lerp(oldLightIntensity, newLightIntensity, timer / duration);
				yield return null;
			}
		}
	}

	void TweenToBlockColor()
	{
		StopAllCoroutines();
		StartCoroutine(ChangeColorRoutine(glowBlockColor, 4f, baseDuration));
	}

	void TweenToNeutralColor()
	{
		StopAllCoroutines();
		StartCoroutine(ChangeColorRoutine(neutralColor, 2f, baseDuration, 0.1f));
	}

	void TweenToAttackColor(float factor)
	{
		Color targetColor = glowAttackColor;
		targetColor *= 15f;
		factor /= 2f;	//Duration of the sword powerup

		SetGlowColor(Color.Lerp(nonWindupEmission, targetColor, factor));
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

	void Update()
	{
		if (arms.combatState == WeaponsOfficer.CombatState.Windup)
		{
			TweenToAttackColor(windup.windupTimer);
		}

		if (arms.combatState == WeaponsOfficer.CombatState.Attack)
		{
			swordLight.intensity = Mathf.Lerp(swordLight.intensity, 0.2f, Time.deltaTime * 4f);
		}


		if (arms.combatState != WeaponsOfficer.CombatState.Windup
			&& arms.combatState != WeaponsOfficer.CombatState.Attack)
		{
			nonWindupEmission = rnd.materials[2].GetColor("_EmissionColor");
		}

		//if (dodger.dodgeSlash)
		//{
		//	TweenToAttackColor(dodger.dodgeSlashWindupTimer);
		//}
	}
}
﻿using UnityEngine;
using System.Collections;

public class WeaponsOfficer : MechComponent
{
	public ArmMovement armMovement { get; private set; }
	public ArmRotation armRotation { get; private set; }
	public ArmControl armControl { get; private set; }
	[SerializeField] Sword weapon;

	public Sword getWeapon { get { return weapon; } }

	protected override void OnAwake()
	{
		base.OnAwake();

		armMovement = GetComponent<ArmMovement>();
		armRotation = GetComponent<ArmRotation>();
		armControl = GetComponent<ArmControl>();
	}

	void IgnoreHierarchyRecursive(Transform root, Collider otherCol)
	{
		foreach (Transform child in root)
		{
			Collider col = child.GetComponent<Collider>();

			if (col)
			{
				Physics.IgnoreCollision(col, otherCol);
			}

			IgnoreHierarchyRecursive(child, otherCol);
		}
	}

	void Start()
	{
		IgnoreHierarchyRecursive(transform.root, weapon.transform.GetChild(0).GetComponent<Collider>());
	}

	void FixedUpdate()
	{
		//armMovement.RunComponent();

	}

	void Update ()
	{
		//Move IK targets horizontally and vertically
		//armMovement.RunComponent();

		//Run attack animations, manage attacking states and gameplay
	}
}
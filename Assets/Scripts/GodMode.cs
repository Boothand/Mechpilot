using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodMode : MonoBehaviour {

    private UnityStandardAssets.Characters.FirstPerson.FirstPersonController charCTRL;
    private bool grav;

	void Start ()
    {
        charCTRL = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        grav = false;
	}
	
	
    // Update is called once per frame
	void Update ()
    {
        switch (grav)
        {
            case true:
                charCTRL.m_GravityMultiplier = 0;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    grav = false;
                }
                break;

            case false:
                charCTRL.m_GravityMultiplier = 2;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    grav = true;
                }
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravity : MonoBehaviour {

    public GameObject player;
    private UnityStandardAssets.Characters.FirstPerson.FirstPersonController charCTRL;
    private bool gravity;

    // Use this for initialization
    void Start ()
    {
        charCTRL = player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        gravity = true;
    }
	
	// Update is called once per frame
	void Update () {
        switch(gravity)
        {
            case true:
                charCTRL.m_GravityMultiplier = 2;
                if(Input.GetKeyDown(KeyCode.F))
                {
                    gravity = false;
                }
                break;
            case false:
                charCTRL.m_GravityMultiplier = 0;
                if(Input.GetKeyDown(KeyCode.F))
                {
                    gravity = true;
                }
                break;
        }
		
	}
}

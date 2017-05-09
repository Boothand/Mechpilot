using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

	private AudioSource audioSrc;

	void Start ()
	{
		audioSrc = GetComponent<AudioSource>();
		audioSrc.enabled = true;
		audioSrc.Play();
	}
	
	void Update ()
	{	
	}
}

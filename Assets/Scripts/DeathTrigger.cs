using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTrigger : MonoBehaviour {

    [SerializeField] private string LoadLevel;

    void Start ()
    {	
	}
	
	void Update ()
    {	
	}

    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(LoadLevel);
    }
}

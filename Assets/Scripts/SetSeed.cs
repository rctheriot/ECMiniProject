using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetSeed : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetGlobalSeed(string value)
    {
        Evolution.seed = System.Int32.Parse(value);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("MiniProject", LoadSceneMode.Single);
    }
}

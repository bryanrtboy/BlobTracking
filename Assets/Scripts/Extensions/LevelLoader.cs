using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			//Application.LoadLevel (0);
			SceneManager.LoadScene (0);
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			//Application.LoadLevel (1);
			SceneManager.LoadScene (1);
		}
	
	}
}

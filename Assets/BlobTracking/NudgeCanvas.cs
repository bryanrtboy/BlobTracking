using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NudgeCanvas : MonoBehaviour
{
	public float nudgeAmount = .1f;
	public Text m_textUI;

	// Use this for initialization
	void Start ()
	{

		SetGUIValues ();

	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			transform.localPosition += new Vector3 (nudgeAmount, 0, 0);
			if (m_textUI != null)
				UpdateGUI ();
		}

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			transform.localPosition -= new Vector3 (nudgeAmount, 0, 0);
			if (m_textUI != null)
				UpdateGUI ();
		}

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			transform.localPosition -= new Vector3 (0, nudgeAmount, 0);
			if (m_textUI != null)
				UpdateGUI ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			transform.localPosition += new Vector3 (0, nudgeAmount, 0);
			if (m_textUI != null)
				UpdateGUI ();
		}

		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			PlayerPrefs.SetFloat ("X", transform.localPosition.x);
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			PlayerPrefs.SetFloat ("X", transform.localPosition.x);
		}

		if (Input.GetKeyUp (KeyCode.DownArrow)) {
			PlayerPrefs.SetFloat ("Y", transform.localPosition.y);
		}

		if (Input.GetKeyUp (KeyCode.UpArrow)) {
			PlayerPrefs.SetFloat ("Y", transform.localPosition.y);
		}
	
	}

	public void SetGUIValues ()
	{
		
		float x = PlayerPrefs.GetFloat ("X");

		if (x != 0) {
			transform.localPosition = new Vector3 (x, transform.localPosition.y, transform.localPosition.z);
			//Debug.LogWarning ("Setting PlayerPrefs " + playerPrefString + " to " + s.value.ToString ());
		} else {
			Debug.LogWarning ("No Player prefs for X , setting PlayerPrefs X to " + transform.localPosition.x);
			PlayerPrefs.SetFloat ("X", transform.localPosition.x);
		}

		float y = PlayerPrefs.GetFloat ("Y");

		if (y != 0) {
			transform.localPosition = new Vector3 (transform.localPosition.x, y, transform.localPosition.z);
			//Debug.LogWarning ("Setting PlayerPrefs " + playerPrefString + " to " + s.value.ToString ());
		} else {
			Debug.LogWarning ("No Player prefs for Y, setting PlayerPrefs Y to " + transform.localPosition.y);
			PlayerPrefs.SetFloat ("Y", transform.localPosition.y);
		}

		if (m_textUI != null)
			UpdateGUI ();
	}

	void UpdateGUI ()
	{
		m_textUI.text = "Tracking Grid Centered at \nX = " + transform.localPosition.x.ToString ("F1") + "\nY = " + transform.localPosition.y.ToString ("F1");
	}
}

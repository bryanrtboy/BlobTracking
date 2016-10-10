using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Extensions;
using UnityEngine.UI;

public class CalibrationManager : MonoBehaviour
{
	public GameObject[] m_gui;
	public MeshRenderer m_grid;
	public CameraFade m_cameraFade;

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.G)) {
			if (m_grid)
				m_grid.enabled = !m_grid.enabled;

		}

		if (Input.GetKeyDown (KeyCode.I)) {
			ToggleGUI ();
		}

	}

	public void ToggleGUI ()
	{
		foreach (GameObject g in m_gui)
			g.SetActive (!g.activeSelf);
	}

	IEnumerator FadeOutScene ()
	{
		yield return StartCoroutine (m_cameraFade.BeginFadeOut (true));
	}

	IEnumerator FadeInScene ()
	{
		yield return StartCoroutine (m_cameraFade.BeginFadeIn (true));
		m_cameraFade.SetImageState (false);
	}

	void ShowGameObject (GameObject g, bool isShowing)
	{
		ShowHide showScript = g.GetComponent<ShowHide> () as ShowHide;

		if (g.activeSelf && showScript && !isShowing && showScript.m_renderers.Length > 0)
			showScript.EnableRenderers (false);
		else
			g.SetActive (isShowing);
	}


}

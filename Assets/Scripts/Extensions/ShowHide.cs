using UnityEngine;
using System.Collections;
using Extensions;

public class ShowHide : MonoBehaviour
{
	public string m_colorName = "_TintColor";
	public Vector2 m_fadeInOutDelay = new Vector2 (6f, 6f);
	public Color m_visibleColor;
	public Color[] m_randomVisibleColor;
	public Color m_invisibleColor;
	//[HideInInspector]
	public Renderer[] m_renderers;

	int m_dblCheckCount = 0;

	void Awake ()
	{
		m_renderers = GetComponentsInChildren <Renderer> (true);
		m_dblCheckCount = m_renderers.Length;

	}

	void Start ()
	{
		
	}

	void OnEnable ()
	{

		Invoke ("CheckDoubleCheck", .25f);

		foreach (Renderer r in m_renderers) {
			r.material.SetColor (m_colorName, m_invisibleColor);
		}
		EnableRenderers (true);
	}

	void OnDisable ()
	{

		StopAllCoroutines ();

		if (m_renderers.Length > 0)
			foreach (Renderer r in m_renderers)
				r.material.SetColor (m_colorName, m_invisibleColor);

	}

	//Just in case we are creating child objects that we want to fade in/out
	void CheckDoubleCheck ()
	{


		Renderer[] rrrs = this.gameObject.GetComponentsInChildren<Renderer> (true);

		if (m_dblCheckCount == rrrs.Length)
			return;

		m_renderers = rrrs;
		m_dblCheckCount = m_renderers.Length;

		foreach (Renderer r in m_renderers) {
			r.material.SetColor (m_colorName, m_invisibleColor);
		}
		EnableRenderers (true);
	}

	public void EnableRenderers (bool b)
	{
		if (m_renderers == null)
			return;

		StopAllCoroutines (); //Need to do this to allow toggling on and off quickly....

		foreach (Renderer r in m_renderers) {
			if (b) {
				if (m_randomVisibleColor.Length > 1)
					m_visibleColor = m_randomVisibleColor [Random.Range (0, m_randomVisibleColor.Length)];

				StartCoroutine (Utilities.FadeMaterialColor (r.sharedMaterial, m_colorName, m_invisibleColor, m_visibleColor, 0, m_fadeInOutDelay.x, () => {
					//Do something here
				}));
			} else {
				StartCoroutine (Utilities.FadeMaterialColor (r.sharedMaterial, m_colorName, m_visibleColor, m_invisibleColor, 0, m_fadeInOutDelay.y, () => {
//					Debug.Log ("faded out " + this.name);
					this.gameObject.SetActive (false);
				}));
			}	
		} 
	}

}
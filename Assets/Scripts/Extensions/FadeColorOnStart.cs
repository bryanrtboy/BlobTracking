using UnityEngine;
using System.Collections;
using Extensions;

public class FadeColorOnStart : MonoBehaviour
{

	public Color m_fadeFrom = Color.black;
	public Color m_fadeTo = Color.white;
	public string m_colorName = "_TintColor";
	public Vector2 m_delayMinMax = Vector2.one;

	Renderer m_renderer;
	Rigidbody m_rigidBody;


	void Awake ()
	{
		if (m_renderer == null)
			m_renderer = GetComponent<Renderer> ();

		m_rigidBody = GetComponentInParent<Rigidbody> () as Rigidbody;

	}


	void OnEnable ()
	{
		float delay = Random.Range (m_delayMinMax.x, m_delayMinMax.y);
		Invoke ("Fade", delay);
	}

	void OnDisable ()
	{
		CancelInvoke ();
	}

	public void Fade ()
	{

		if (m_renderer != null)
			StartCoroutine (Utilities.FadeMaterialColor (m_renderer.material, m_colorName, m_fadeFrom, m_fadeTo, 0, 6, () => {
				if (m_rigidBody)
					m_rigidBody.useGravity = true;

				Destroy (this);

			}));
	}

}

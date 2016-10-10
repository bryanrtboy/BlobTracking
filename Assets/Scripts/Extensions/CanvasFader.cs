using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour
{


	public bool 	m_fadeIn = true;
	public float	m_delayBeforeFadeIn = 0;
	public float	m_fadeInLength = 1;
	public bool		m_fadeOut = true;
	public float	m_delayBeforeFadeOut = 10;
	public float	m_fadeOutLength = 1;
	public bool		m_destroyAfterFadeOut = false;

	private CanvasGroup m_canvasGroup ;

	void Awake ()
	{

		m_canvasGroup = this.GetComponent<CanvasGroup> () as CanvasGroup;

		if (m_fadeIn) {
			m_canvasGroup.alpha = 0;
			Invoke ("FadeIn", m_delayBeforeFadeIn);
		}
		
		if (m_fadeOut)
			Invoke ("FadeOut", m_delayBeforeFadeOut);
	}
	
	void FadeOut ()
	{
		StartCoroutine (FadeOutGUI (m_fadeOutLength));
	}
	
	void FadeIn ()
	{
		StartCoroutine (FadeInGUI (m_fadeInLength));
	}
	
	IEnumerator FadeOutGUI (float time)
	{
		float t = m_canvasGroup.alpha;
		while (t>0.0f) {
			yield return new WaitForEndOfFrame ();
			t = Mathf.Clamp01 (t - Time.deltaTime / time);
			m_canvasGroup.alpha = t;
		}
		
		if (m_destroyAfterFadeOut)
			Destroy (this.gameObject);
	}
	
	IEnumerator FadeInGUI (float time)
	{
		float t = m_canvasGroup.alpha;
		while (t<1.0f) {
			yield return new WaitForEndOfFrame ();
			t = Mathf.Clamp01 (t + Time.deltaTime / time);
			m_canvasGroup.alpha = t;
		}	

	}
}

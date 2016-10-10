using UnityEngine;
using System.Collections;
using Extensions;

[RequireComponent (typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{

	public bool m_persist = false;
	public float m_delayBeforeStarting = 0;
	public bool m_playSequential = false;
	//If this is 0, use the clips length to determine when to start fading out (end of clip)
	public bool m_autoDestroy = false;
	public AudioFile[] m_clips;

	private AudioSource m_source;
	private int count = 0;

	// Use this for initialization
	void Awake ()
	{
		m_source = GetComponent<AudioSource> () as AudioSource;

		if (m_persist)
			DontDestroyOnLoad (this.gameObject);
	}

	void Start ()
	{
		if (!m_playSequential) {
			Invoke ("PlayRandomClip", m_delayBeforeStarting);
		} else {
			Invoke ("PlaySequentially", m_delayBeforeStarting);
		}
	}

	void PlaySequentially ()
	{
		if (count >= m_clips.Length - 1) {
			//AudioSource audioSource, AudioClip audioClip, bool fadeOut, float fadeOutDuration, bool fadeIn, float fadeInDuration, bool clipLengthIsFadeOutStartTime, float fadeOutStartTime
			StartCoroutine (Utilities.PlayClipWithFades (m_source, m_clips [count], () => {
				if (m_autoDestroy)
					Destroy (this.gameObject);
			}));

		} else {

			StartCoroutine (Utilities.PlayClipWithFades (m_source, m_clips [count], () => {
				PlaySequentially ();
			}));
		}
		count++;
	}

	void PlayRandomClip ()
	{

		int rand = Random.Range (0, m_clips.Length);

		StartCoroutine (Utilities.PlayClipWithFades (m_source, m_clips [rand], () => {

			if (m_clips [rand].m_fadeIn)
				m_clips [rand].m_fadeIn = false; //Only fade in one time if we are doing random clips...
			PlayRandomClip ();
		}));
	}
}

using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Slider))]
public class SetSliderOnAwake : MonoBehaviour
{

	public string playerPrefString = "SetTags";
	public bool m_useEditorValue = false;
	public Text m_label;
	[HideInInspector]
	public Slider m_slider;

	void OnEnable ()
	{
		m_slider = this.GetComponent<Slider> () as Slider;
		if (!m_label)
			m_label = GetComponentInChildren<Text> () as Text;
		SetGUIValues ();
	}

	void OnDisable ()
	{
		PlayerPrefs.SetFloat (playerPrefString, m_slider.value);
	}


	public void SetGUIValues ()
	{
		
		float f = PlayerPrefs.GetFloat (playerPrefString);

		if (f != 0) {
			m_slider.value = f;
			//Debug.LogWarning ("Setting PlayerPrefs " + playerPrefString + " to " + s.value.ToString ());
		} else {
			Debug.LogWarning ("No Player prefs for " + playerPrefString + ", setting PlayerPrefs " + playerPrefString + " to " + m_slider.value.ToString ());
			PlayerPrefs.SetFloat (playerPrefString, m_slider.value);
		}

		if (m_label != null)
			m_label.BroadcastMessage ("UpdateLabel", m_slider.value, SendMessageOptions.DontRequireReceiver);

	}
}

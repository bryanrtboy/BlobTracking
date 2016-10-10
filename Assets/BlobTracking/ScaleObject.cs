using UnityEngine;
using System.Collections;

public class ScaleObject : MonoBehaviour
{


	public string m_horizontalName = "HORIZONTAL_ASPECT";
	public string m_verticalName = "VERTICAL_ASPECT";

	void Start ()
	{

		float x = PlayerPrefs.GetFloat (m_horizontalName);
	
		float y = PlayerPrefs.GetFloat (m_verticalName);

		if (x > 0 && y > 0)
			transform.localScale = new Vector3 (x, transform.localScale.y, y);
	}

	void OnDisable ()
	{
		PlayerPrefs.SetFloat (m_horizontalName, transform.localScale.x);
		PlayerPrefs.SetFloat (m_verticalName, transform.localScale.z);
	}

	public void UpdateTransformVertical (float size)
	{
		transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, size);
	}

	public void UpdateTransformHorizontal (float size)
	{
		transform.localScale = new Vector3 (size, transform.localScale.y, transform.localScale.z);
	}
}

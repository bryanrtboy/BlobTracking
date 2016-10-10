using UnityEngine;
using System.Collections;

public class RandomScale : MonoBehaviour
{


	public Vector2 m_minMax = Vector2.one;

	void OnEnable ()
	{
		float rand = Random.Range (m_minMax.x, m_minMax.y);
		this.transform.localScale = new Vector3 (rand, rand, rand);
	}
}

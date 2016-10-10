using UnityEngine;
using System.Collections;

public class MoveTowards : MonoBehaviour
{

	public Vector3 m_direction;
	public Vector2 m_smoothMinMax = Vector2.one;


	float m_speed = 0;

	void OnEnable ()
	{

		m_speed = Random.Range (m_smoothMinMax.x, m_smoothMinMax.y);
		m_direction = m_direction * m_speed;

	}

	void Update ()
	{
		transform.Translate (m_direction, Space.World);
	}


}

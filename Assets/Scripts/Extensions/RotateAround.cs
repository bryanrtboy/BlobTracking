using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour
{

	public float m_speed = 10;
	public Vector3 m_axis = Vector3.up;

	Transform m_transform;

	void Awake ()
	{
		m_transform = this.transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_transform.RotateAround (m_transform.position, m_axis, m_speed * Time.deltaTime);
	}
}

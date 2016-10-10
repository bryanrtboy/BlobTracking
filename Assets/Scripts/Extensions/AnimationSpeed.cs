using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class AnimationSpeed : MonoBehaviour
{

	public float m_speed = 1;
	public bool m_refreshSpeed = false;
	private Animator m_animator;

	// Use this for initialization
	void Start ()
	{

		m_animator = this.GetComponent<Animator> () as Animator;
		m_animator.speed = m_speed;
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (m_refreshSpeed) {
			m_animator.speed = m_speed;
			m_refreshSpeed = false;
		}
	}
}

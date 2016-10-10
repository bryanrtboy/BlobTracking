using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class AnimationRandomizer : MonoBehaviour
{
	public Vector2 m_minSpeedMaxSpeed = Vector2.one;
	public Vector2 m_minIntervalMaxInterval = Vector2.one;

	float speed;
	float interval;

	Animator m_animator;

	void Awake ()
	{
		m_animator = this.GetComponent<Animator> () as Animator;
	}

	void OnEnable ()
	{
		SwitchSpeed ();
	}

	void SwitchSpeed ()
	{

		speed = Random.Range (m_minSpeedMaxSpeed.x, m_minSpeedMaxSpeed.y);

		if (Random.Range (0f, 1f) > .5f)
			speed = -speed;

		m_animator.SetFloat ("Speed", speed);
		//Debug.Log (speed);
		Invoke ("SwitchInterval", interval);
	}

	void SwitchInterval ()
	{
		interval = Random.Range (m_minIntervalMaxInterval.x, m_minIntervalMaxInterval.y);
		Invoke ("SwitchSpeed", 0);
	}
}

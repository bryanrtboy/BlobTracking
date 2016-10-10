using UnityEngine;
using System.Collections;

public class SplineNodeController : MonoBehaviour
{

	MoveBezierTowardsTarget[] m_beziers;

	void Awake ()
	{
		m_beziers = GetComponentsInChildren<MoveBezierTowardsTarget> () as MoveBezierTowardsTarget[];
	}

	public  void MoveBeziersToAnchor ()
	{
		foreach (MoveBezierTowardsTarget b in m_beziers)
			b.BezierToAnchor ();
	}

	public void MoveBeziersTowardsTargets ()
	{
		foreach (MoveBezierTowardsTarget b in m_beziers)
			b.BeziersTowardsTarget ();
	}


}

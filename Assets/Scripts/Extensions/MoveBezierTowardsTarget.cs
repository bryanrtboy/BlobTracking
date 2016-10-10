using UnityEngine;
using System.Collections;

/// <summary>
/// Positions a transform in relation to an anchor transform towards a target transform. Use slack to ease the effect over time.
/// This can effectively be used on a Playground Spline's nodes and bezier handles if set to transform where the anchor and/or target is moving.
/// Note that the effect will make a handle move as set to Bezier Mode: Free.
/// </summary>
public class MoveBezierTowardsTarget : MonoBehaviour
{

	public Transform anchor;
	// The anchor to base positioning from (usually a bezier handle's node)
	public Transform target;
	// The target to position towards (usually the next or previous transform node from the anchor)
	public float anchorToTargetDistance = .2f;
	// The distance from the anchor towards the target (normalized value)
	public float slack = 0;
	// The amount of easing over time (0 will apply the effect immediately)
	public bool rotateTowardsTarget;
	//offset from a direct path to the object (in local space)
	public Vector3 offset = Vector3.zero;

	Transform thisTransform;
	Vector3 position;
	bool isAtAnchor = true;

	void Awake ()
	{
		thisTransform = transform;
		position = thisTransform.position;
	}

	void Update ()
	{
		if (anchor == null || target == null || isAtAnchor)
			return;
		if (slack > 0)
			position = Vector3.Lerp (position, Vector3.Lerp (anchor.position, target.position, anchorToTargetDistance), Time.deltaTime / slack);
		else
			position = Vector3.Lerp (anchor.position, target.position, anchorToTargetDistance);

		

		if (rotateTowardsTarget) {
			if (slack > 0)
				thisTransform.rotation = Quaternion.RotateTowards (thisTransform.rotation, Quaternion.LookRotation (thisTransform.position - target.position, Vector3.forward) * new Quaternion (-90f, 0, 90f, 1f), Time.deltaTime / slack);
			else
				thisTransform.rotation = Quaternion.LookRotation (thisTransform.position - target.position, Vector3.forward) * new Quaternion (-90f, 0, 90f, 1f);
		}

		thisTransform.position = position + offset;
	}

	public void BezierToAnchor ()
	{
		thisTransform.position = anchor.position;
		isAtAnchor = true;
	}

	public void BeziersTowardsTarget ()
	{
		isAtAnchor = false;
	}
}

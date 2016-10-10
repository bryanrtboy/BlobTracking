using UnityEngine;
using System.Collections;

public class RendererBounds : MonoBehaviour
{
	public Renderer rend;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
	}

	void OnDrawGizmosSelected ()
	{
		Vector3 center = rend.bounds.center;
//		float radius = rend.bounds.extents.magnitude;
		Vector3 size = new Vector3 (rend.bounds.size.x, rend.bounds.size.y, rend.bounds.size.z);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (center, size);
	}
}
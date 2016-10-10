using UnityEngine;
using System.Collections;

public class RandomPlacement : MonoBehaviour
{

	public MeshRenderer m_boundingAreaForRandomPlacement;
	public bool m_randomRotation = false;
	public Vector3 m_rotationAxis = Vector3.up;

	void OnEnable ()
	{
		if (m_boundingAreaForRandomPlacement)
			RandomPlaceOnPlane ();
	}

	void RandomPlaceOnPlane ()
	{
		Vector3 offset = m_boundingAreaForRandomPlacement.bounds.center;
		float xPos = Random.Range (-m_boundingAreaForRandomPlacement.bounds.extents.x, m_boundingAreaForRandomPlacement.bounds.extents.x);
		float zPos = Random.Range (-m_boundingAreaForRandomPlacement.bounds.extents.z, m_boundingAreaForRandomPlacement.bounds.extents.z);
		float yPos = Random.Range (-m_boundingAreaForRandomPlacement.bounds.extents.y, m_boundingAreaForRandomPlacement.bounds.extents.y);

		transform.position = new Vector3 (xPos, yPos, zPos) + offset;

		if (m_randomRotation)
			//transform.RotateAround (m_rotationAxis, Random.Range (0f, 360f));
		transform.RotateAround (transform.position, m_rotationAxis, Random.Range (0f, 360f));
		//transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, Random.Range (0f, 360f), transform.localEulerAngles.z);

	}
}

using UnityEngine;
using System.Collections;
using Extensions;

public class MakeNodesFromSVG : MonoBehaviour
{

	public TextAsset textAsset;
	public GameObject nodePrefab;

	// Use this for initialization
	void Start ()
	{
		if (textAsset != null && nodePrefab != null) {
			Vector3[] array = Utilities.SVGtoVector3 (textAsset, .1f);

			for (int i = 0; i < array.Length; i++) {

				Vector3 position = array [i];
				GameObject g = Instantiate (nodePrefab, position, Quaternion.identity) as GameObject;
				g.name = nodePrefab.name + "_" + i.ToString ();
				g.transform.parent = this.gameObject.transform;
			}
		}

		Destroy (this);
	}
	

}

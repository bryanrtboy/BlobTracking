using UnityEngine;
using System.Collections;

public class DieRespawnAfterDelay : MonoBehaviour
{

	void OnCollisionEnter (Collision other)
	{
		other.gameObject.SetActive (false);
		float rand = Random.Range (1, 4);
		StartCoroutine (ReSpawn (other.gameObject, rand));

	}

	IEnumerator ReSpawn (GameObject g, float delay)
	{
		yield return new WaitForSeconds (delay);

		RandomPlacement m_random = g.GetComponent<RandomPlacement> () as RandomPlacement;
		if (m_random && !m_random.enabled)
			m_random.enabled = true;

		g.SetActive (true);


	}
}

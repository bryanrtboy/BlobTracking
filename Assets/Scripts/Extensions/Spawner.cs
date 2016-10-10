//Spawner based off Unity Angry Bots demo (for Mobile), converted to C#
//Author: Bryan Leister
//Date: Jan 2013
//
//Description: This is a helper to spawn objects as needed in the game. Also requires the static method Spawn_Object.cs script,
//which as a static method it does not have to be in the scene.
//
//Instructions: Create a GameObject named 'Caches' and attach this script to it. Set cache sizes, typical setup is
//Element 1 : Prefab - InstantBullet, cache size 6
//Element 2 : Missle, cache size 10
//
//Then, on your weapon script (AutoFireCS) you will call this when to create a bullet, works like Instantiate but
//prevents game lag in iOS games. Example code:
//
//GameObject bullet = GSK_Spawner.Spawn (bulletPrefab, spawnPoint.position, spawnPoint.rotation * coneRandomRotation) as GameObject;
//
//Destroy objects using:   DestroyMe(gameObject);  That will de-activate, rather than destroy and allow re-use in the cache.



using UnityEngine;
using System.Collections;

namespace Extensions
{
	public class Spawner : MonoBehaviour
	{

		public 	static		Spawner spawner;
		public 				ObjectCache[] caches;
		public 				Hashtable activeCachedObjects;

		[System.Serializable]
		public class ObjectCache
		{
	
			public GameObject prefab;
			public int cacheSize = 10;
			[HideInInspector]
			public GameObject[]	objects;
			private	int cacheIndex = 0;

			public void Initialize ()
			{
		
				objects = new GameObject[cacheSize];
		
				for (int i = 0; i < cacheSize; i++) {
			
					objects [i] = MonoBehaviour.Instantiate (prefab) as GameObject;
					objects [i].SetActive (false);
					objects [i].name = objects [i].name + i;
				}
		
			}

			public GameObject  GetNextObjectInCache ()
			{
				GameObject obj = null;
		
				// The cacheIndex starts out at the position of the object created
				// the longest time ago, so that one is usually free,
				// but in case not, loop through the cache until we find a free one.
				for (int i = 0; i < cacheSize; i++) {
					obj = objects [cacheIndex];
			
					// If we found an inactive object in the cache, use that.
					if (!obj.activeSelf)
						break;
			
					// If not, increment index and make it loop around
					// if it exceeds the size of the cache
					cacheIndex = (cacheIndex + 1) % cacheSize;
				}
		
				// The object should be inactive. If it's not, log a warning and use
				// the object created the longest ago even though it's still active.
				if (obj.activeSelf) {
					Debug.LogWarning (
						"Spawn of " + prefab.name +
						" exceeds cache size of " + cacheSize +
						"! Reusing already active object.", obj);
					Spawner.DestroyMe (obj);
				}
		
				// Increment index and make it loop around
				// if it exceeds the size of the cache
				cacheIndex = (cacheIndex + 1) % cacheSize;
		
				return obj;
			}
	

	
		}

		void Awake ()
		{
		
			spawner = this;
		
			int amount = 0;
		
			for (int i = 0; i < caches.Length; i++) {
				caches [i].Initialize ();												//Initialize each cache
			
				amount += caches [i].cacheSize;
			}
		
			activeCachedObjects = new Hashtable (amount);
		}

		public static GameObject Spawn (GameObject prefab, Vector3 position, Quaternion rotation)
		{
			ObjectCache cache = null;
		
			if (spawner != null) {																//Find the cache for the specified prefab
			
				for (int i = 0; i < spawner.caches.Length; i++) {
					if (spawner.caches [i].prefab == prefab)
						cache = spawner.caches [i];	
				}
			}
		
			if (cache == null)														//If there is not cache of this type, instantiate one normally
			return Instantiate (prefab, position, rotation) as GameObject;
			
		
			GameObject obj = cache.GetNextObjectInCache ();							//Find the next object in cache
		
			obj.transform.position = position;
			obj.transform.rotation = rotation;
		
			obj.SetActive (true);
			spawner.activeCachedObjects [obj] = true;
		
			return obj;
	
		}

		public static void DestroyMe (GameObject objectToDestroy)
		{
			if (spawner != null && spawner.activeCachedObjects.ContainsKey (objectToDestroy)) {
				objectToDestroy.SetActive (false);
				spawner.activeCachedObjects [objectToDestroy] = false;
			} else {
				Destroy (objectToDestroy);
				Debug.Log (objectToDestroy.name + " was not in the Spawner Dictionary, have you added it to the " + spawner.name + "'s Spawner Cache?");
			}
		}
	
	
	}
}



//Utilities
//Bryan Leister
//March, 2016

//Extensions
//Author: Bryan Leister
//Date: Mar 2016
//
//
//Description: A static method for doing various things. Does not have to be in the scene hierarchy
//
//Instructions:
//Simple play an audioclip on this object would be:
//
// _audioSource.playClip(_audioClip);
//
//
//PlayClipOnComplete will do something after the clip has played. On your object with the sound, use a script like this:
//
//		StartCoroutine(_audioSource.playClipAndOnComplete( chaseWarning, () =>
//		{
//			DoSomeCoolNewThing();
//		}
//		));
//
//
//You can also pass a null if you want by doing this:
//StartCoroutine(_audioSource.playClipAndOnComplete( chaseWarning, null));
//

using UnityEngine;
using System;
using System.Collections;

namespace Extensions
{
	public static class Utilities
	{
		#region Camera

		public static Vector3 GetWorldPositionOnPlane (Camera camera, Vector3 screenPosition, float z)
		{
			Ray ray = camera.ScreenPointToRay (screenPosition);
			Plane xy = new Plane (Vector3.forward, new Vector3 (0, 0, z));
			float distance;
			xy.Raycast (ray, out distance);
			return ray.GetPoint (distance);
		}

		#endregion

		#region Audio

		public static void PlayClip (this AudioSource audioSource, AudioClip audioClip)
		{
			audioSource.clip = audioClip;
			audioSource.Play ();
		}

		public static IEnumerator PlayClipAndOnComplete (this AudioSource audioSource, AudioClip audioClip, Action onComplete)
		{
			audioSource.PlayClip (audioClip);
		
			while (audioSource.isPlaying)
				yield return null;
		
			onComplete ();
		
		}

		public static IEnumerator PlayRandomClip (this AudioSource audioSource, AudioClip[] clips, Action onComplete)
		{
			//audioSource.Stop();
		
			int clipIndex = UnityEngine.Random.Range (0, clips.Length);
			audioSource.PlayClip (clips [clipIndex]);
		
			while (audioSource.isPlaying)
				yield return null;
		
			if (onComplete != null)
				onComplete ();
		
		}

		public static IEnumerator FadeOut (this AudioSource audioSource, AudioClip audioClip, float duration, Action onComplete)
		{


			audioSource.PlayClip (audioClip);
			float startingVolume = audioSource.volume;
		
			//fade out over time
			while (audioSource.volume > 0.0F) {
				audioSource.volume -= Time.deltaTime * startingVolume / duration;
				yield return null;
			}

			audioSource.Stop ();
			audioSource.volume = 1;
			//done fading out
			if (onComplete != null)
				onComplete ();
				
		}

		public static IEnumerator FadeOutAfterPlayingClip (this AudioSource audioSource, AudioClip audioClip, float fadeDuration, Action onComplete)
		{


			audioSource.PlayClip (audioClip);
			float startingVolume = audioSource.volume;

			float delay = audioClip.length - fadeDuration;
			if (delay < 0)
				delay = 0;
			yield return new WaitForSeconds (delay);
		
			//fade out over time
			while (audioSource.volume > 0.0F) {
				audioSource.volume -= Time.deltaTime * startingVolume / fadeDuration;
				yield return null;
			}

			audioSource.Stop ();
			audioSource.volume = 1;
			//done fading out
			if (onComplete != null)
				onComplete ();
				
		}

		public static IEnumerator FadeOutClipAfterDelay (this AudioSource audioSource, AudioClip audioClip, float delayBeforeStartingFade, float fadeDuration, Action onComplete)
		{
			audioSource.PlayClip (audioClip);
			float startingVolume = audioSource.volume;

			yield return new WaitForSeconds (delayBeforeStartingFade);
		
			//fade out over time
			while (audioSource.volume > 0.0F) {
				audioSource.volume -= Time.deltaTime * startingVolume / fadeDuration;
				yield return null;
			}

			audioSource.Stop ();
			audioSource.volume = 1;
			//done fading out
			if (onComplete != null)
				onComplete ();
				
		}

		public static IEnumerator PlayClipWithFades (this AudioSource audioSource, AudioFile audioFile, Action onComplete)
		{

			if (audioFile.m_fadeIn) {
				audioSource.volume = 0;
			} else {
				audioSource.volume = audioFile.m_maxVolume;
			}

			if (audioFile.m_loopClip)
				audioSource.loop = true;
			else
				audioSource.loop = false;

			audioSource.clip = audioFile.m_clip;
			if (audioFile.m_pickRandomPointInClip) {
				float playbackTime = UnityEngine.Random.Range (0, audioFile.m_clip.length * .75f);
				audioSource.time = playbackTime;
			}

			yield return new WaitForSeconds (audioFile.m_delayBeforePlaying);

			audioSource.Play ();

			if (audioFile.m_fadeIn) {
				if (audioFile.m_fadeInDuration <= .001f)
					audioFile.m_fadeInDuration = .5f; //Avoid divide by zero error

				while (audioSource.volume < audioFile.m_maxVolume) {
					audioSource.volume += Time.deltaTime * audioFile.m_maxVolume / audioFile.m_fadeInDuration;
					yield return null;
				}
			}

			float delay = audioFile.m_delayBeforeStartingFadeOut;

			if (delay <= 0)
				delay = audioFile.m_clip.length;

			if (audioFile.m_fadeOut)
				delay -= audioFile.m_fadeOutDuration;

			if (delay < 0)
				delay = 0;

			yield return new WaitForSeconds (delay);

			if (audioFile.m_fadeOut) {

				while (audioSource.volume > 0.0F) {
					audioSource.volume -= Time.deltaTime * audioFile.m_maxVolume / audioFile.m_fadeOutDuration;
					yield return null;
				}

			}

			audioSource.Stop ();

			//done
			if (onComplete != null)
				onComplete ();
				
		}

		public static IEnumerator FadeIn (this AudioSource audioSource, AudioClip audioClip, float duration, Action onComplete)
		{

			float endingVolume = audioSource.volume;
			audioSource.volume = 0.0F;
		
			audioSource.PlayClip (audioClip);
		
			//fade in over time
			while (audioSource.volume < endingVolume) {
				audioSource.volume += Time.deltaTime * endingVolume / duration;
				yield return null;
			}
		
			//done fading out
			if (onComplete != null)
				onComplete ();
		}

		#endregion

		#region Math

		public static float Remap (this float value, float incomingMin, float incomingMax, float resultMin, float resultMax)
		{
		
			return (value - incomingMin) / (incomingMax - incomingMin) * (resultMax - resultMin) + resultMin;
		
		}

		//Average position of the Vector3 array passed to it.
		public static Vector3 GetMeanVector (Vector3[] positions)
		{
			if (positions.Length == 0)
				return Vector3.zero;
			float x = 0f;
			float y = 0f;
			float z = 0f;
			foreach (Vector3 pos in positions) {
				x += pos.x;
				y += pos.y;
				z += pos.z;
			}
			return new Vector3 (x / positions.Length, y / positions.Length, z / positions.Length);
		}

		public static IEnumerator MoveTransformTo (this Transform obj, Vector3 to, float smoothing, Action onComplete) // if no OnComplete function, use 'null'
		{

			float time = Time.time;
			while (Vector3.Distance (obj.position, to) > 0.05f) {
				obj.position = Vector3.Lerp (obj.position, to, smoothing * Time.deltaTime);
            
				yield return null;
			}

			time = Time.time - time;
			//Debug.Log ("Reached the target in " + time.ToString ("F4") + " seconds, with smoothing of " + smoothing);
        
			yield return new WaitForEndOfFrame ();
        
			//Debug.Log ("MyCoroutine is now finished.");

			if (onComplete != null)
				onComplete ();
		}

		public static Vector3[] SVGtoVector3 (TextAsset textAsset, float scale)
		{


			string[] lines = textAsset.text.Split (',');
			Vector3[] array = new Vector3[lines.Length];

			for (int i = 0; i < array.Length; i++) {
				string[] vector = lines [i].Split (' ');
				array [i] = new Vector3 (Convert.ToSingle (vector [0]) * scale, Convert.ToSingle (vector [1]) * scale, 0);
			}

			return array;

		}

		#endregion

		#region MusicMath Utilities

		public const int MidiNoteA440 = 69;

		public static string[] NoteNames = {
			"C",
			"C#(Db)",
			"D",
			"D#(Eb)",
			"E",
			"F",
			"F#(Gb)",
			"G",
			"G#/Ab",
			"A",
			"A#(Bb)",
			"B"
		};

		private static string[] _noteNamesWithOctave;

		/// <summary>
		/// A human readable list of MIDI note names, for use by the editor, mainly
		/// </summary>
		/// <value>The midi note names.</value>
		public static string[] NoteNamesWithOctave {
			get {
				if (_noteNamesWithOctave == null) {
					_noteNamesWithOctave = new string[128];

					var octave = 0;

					for (int noteNumber = 0; noteNumber < _noteNamesWithOctave.Length; noteNumber += NoteNames.Length) {
						for (int noteName = 0; noteName < NoteNames.Length; noteName++) {
							_noteNamesWithOctave [noteNumber + noteName] = NoteNames [noteName] + octave;
						}

						octave++;
					}
				}

				return _noteNamesWithOctave;
			}
		}

		/// <summary>
		/// Converts a semitone offset to a percentage pitch
		/// </summary>
		/// <param name="semitones">number of semitones from center</param>
		/// <returns>percentage-based pitch</returns>
		public static float SemitonesToPitch (float semitones)
		{
			return Mathf.Pow (2f, semitones / 12f);
		}

		/// <summary>
		/// Converts a semitone offset to a percentage pitch
		/// </summary>
		/// <param name="semitones">number of semitones from center</param>
		/// <returns>percentage-based pitch</returns>
		public static double SemitonesToPitch (double semitones)
		{
			return Math.Pow (2.0, semitones / 12.0);
		}

		public static float PitchToSemitones (float pitch)
		{
			// pitch = 2^(semitones/12)
			// log2(pitch) = semitones/12
			// semitones = log2(pitch)*12
			return Mathf.Log (pitch, 2f) * 12;
		}

		/// <summary>
		/// Converts a MIDI note number to a frequency, based on A 440
		/// </summary>
		/// <param name="midiNote">MIDI note number to convert</param>
		/// <returns></returns>
		public static float MidiNoteToFrequency (int midiNote)
		{
			return 440f * Mathf.Pow (2f, (midiNote - MidiNoteA440) / 12f);
		}

		#endregion

		#region Texture

		public static IEnumerator FadeGUITexture (this GUITexture myGUItexture, float startLevel, float endLevel, float delay, float duration, Action onComplete)
		{
			yield return new WaitForSeconds (delay);
		
			float speed = 1.0f / duration;   
		
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed) {
			
				float f = Mathf.Lerp (startLevel, endLevel, t);
				myGUItexture.color = new Color (myGUItexture.color.r, myGUItexture.color.g, myGUItexture.color.b, f);
				yield return null;
			
			}

			onComplete ();
		
		}

		public static IEnumerator FadeMaterialAlpha (this Material materialToFade, float startLevel, float endLevel, float delay, float duration, Action onComplete)
		{
			yield return new WaitForSeconds (delay);
		
			float speed = 1.0f / duration;   

			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed) {
			
				float f = Mathf.Lerp (startLevel, endLevel, t);
				materialToFade.color = new Color (materialToFade.color.r, materialToFade.color.g, materialToFade.color.b, f);
				yield return null;
			
			}

			onComplete ();
		
		}

		public static IEnumerator FadeMaterialColor (this Material materialToFade, string colorPropertyName, Color startColor, Color endColor, float delay, float duration, Action onComplete)
		{

			yield return new WaitForSeconds (delay);

			float speed = 1.0f / duration; 
			Color c = startColor;
		
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed) {

				c = Color.Lerp (c, endColor, t);
				materialToFade.SetColor (colorPropertyName, c);
				yield return null;
			
			}
			if (onComplete != null)
				onComplete ();
		
		}

		public static IEnumerator FadeSpriteColor (this SpriteRenderer spriteToFade, Color startColor, Color endColor, float delay, float duration, Action onComplete)
		{
		
			yield return new WaitForSeconds (delay);
		
			float speed = 1.0f / duration; 
			Color c = startColor;
		
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed) {
			
				c = Color.Lerp (c, endColor, t);
				spriteToFade.color = c;
				yield return null;
			
			}
		
			onComplete ();
		
		}

		#endregion
	}

	#region Helper Classes
	public static class Vector2Extension
	{
		//Use  Vector2 direction = Vector2.right.Rotate(45f); in the code to get this...
		public static Vector2 Rotate (this Vector2 v, float degrees)
		{
			return Quaternion.Euler (0, 0, degrees) * v;
		}
	}


	[Serializable]
	public  class AudioFile
	{
		public AudioClip m_clip;
		public float m_maxVolume = 1;
		public float m_delayBeforePlaying = 0;
		public bool m_fadeIn = false;
		public float m_fadeInDuration = 0;
		public bool m_fadeOut = false;
		public float m_fadeOutDuration = 0;
		public float m_delayBeforeStartingFadeOut = 0;
		public bool m_pickRandomPointInClip = false;
		public bool m_loopClip = false;
		//Zero means that we fade out towards the end of the Clip.

	}
	#endregion
}

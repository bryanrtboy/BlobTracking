using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class CameraZoom : MonoBehaviour
{

	public Camera _camera;
	public Camera[] _cameras;

	void Start ()
	{
		if (!_camera)
			_camera = this.GetComponent<Camera> () as Camera;

		float zoom = PlayerPrefs.GetFloat ("CAMERA_ZOOM");

		if (zoom > 0)
		if (_camera.orthographic) {
			_camera.orthographicSize = zoom;

			foreach (Camera c in _cameras)
				c.orthographicSize = zoom;
		} else {
			_camera.fieldOfView = zoom;
			foreach (Camera c in _cameras)
				c.fieldOfView = zoom;
		}


	}

	void OnDisable ()
	{
		if (_camera != null)
		if (_camera.orthographic) {
			PlayerPrefs.SetFloat ("CAMERA_ZOOM", _camera.orthographicSize);
		} else {
			PlayerPrefs.SetFloat ("CAMERA_ZOOM", _camera.fieldOfView);
		}
	}

	public void UpdateZoom (float zoom)
	{
		if (zoom > 0)
		if (_camera.orthographic) {
			_camera.orthographicSize = zoom;
			foreach (Camera c in _cameras)
				c.orthographicSize = zoom;
		} else {
			_camera.fieldOfView = zoom;
			foreach (Camera c in _cameras)
				c.fieldOfView = zoom;
		}
	}
}

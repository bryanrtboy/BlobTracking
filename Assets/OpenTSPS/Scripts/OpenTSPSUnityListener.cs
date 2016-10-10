//*Edited by Bryan Leister - October 2016
//
//Fixed a bug to make sure the thread is stopped when the scene stops. Adjusted the meshBounds to
//use the renderer instead of the sharedMesh, making it easier to calibrate
//
//
//*
// * OpenTSPS + Unity3d Extension
// * Created by James George on 11/24/2010
// * 
// * This example is distributed under The MIT License
// *
// * Copyright (c) 2010 James George
// *
// * Permission is hereby granted, free of charge, to any person obtaining a copy
// * of this software and associated documentation files (the "Software"), to deal
// * in the Software without restriction, including without limitation the rights
// * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// * copies of the Software, and to permit persons to whom the Software is
// * furnished to do so, subject to the following conditions:
// *
// * The above copyright notice and this permission notice shall be included in
// * all copies or substantial portions of the Software.
// *
// * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// * THE SOFTWARE.
// */


using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TSPS;

public class OpenTSPSUnityListener : MonoBehaviour, OpenTSPSListener
{

	public int port = 12000;
	//set this from the UI to change the port
	
	//create some materials and apply a different one to each new person
	public Material[] materials;
	
	private OpenTSPSReceiver receiver;
	//a place to hold game objects that we attach to people, maps person ID => their object
	private Dictionary<int,GameObject> peopleCubes = new Dictionary<int,GameObject> ();
	
	//game engine stuff for the example
	public GameObject boundingPlane;
	//put the people on this plane
	public GameObject personMarker;
	//used to represent people moving about in our example
	public Text m_portInfoUI;
	public Toggle m_smoothingGUI;
	public bool m_useSmoothing = false;
	public float m_damping = .1f;
	public Toggle m_useMouseGUI;
	public bool m_useMouse = false;
	public LayerMask m_mask;


	Camera m_mainCamera;
	GameObject m_mousePerson;

	void Start ()
	{

		m_mainCamera = Camera.main;
		receiver = new OpenTSPSReceiver (port);
		receiver.addPersonListener (this);
//		Security.PrefetchSocketPolicy ("localhost", 8843);
		receiver.connect ();
		Debug.Log ("created receiver on port " + port);
		UpdateTSPSMessage ();
		SetGUIValues ();
	}

	void OnDisable ()
	{
		receiver.disconnect ();
	}

	void Update ()
	{
		//call this to receiver messages
		receiver.update ();
		if (m_useMouse) {
			if (m_mousePerson == null)
				m_mousePerson = Instantiate (personMarker, GetMousePosition (), Quaternion.identity) as GameObject;

			m_mousePerson.transform.position = GetMousePosition ();
		}

	}

	
	void UpdateTSPSMessage ()
	{
		if (receiver.isConnected () && m_portInfoUI) {
			m_portInfoUI.text = "Connected to TSPS on Port " + port.ToString ();
		}
	}

	public void personEntered (OpenTSPSPerson person)
	{
		//	Debug.Log (" person entered with ID " + person.id);
		GameObject personObject = (GameObject)Instantiate (personMarker, positionForPerson (person), Quaternion.identity);
		personObject.GetComponentInChildren<Renderer> ().material = materials [person.id % materials.Length];
		peopleCubes [person.id] = personObject;

	}

	public void personUpdated (OpenTSPSPerson person)
	{
		//don't need to handle the Updated method any differently for this example
		personMoved (person);
	}

	public void personMoved (OpenTSPSPerson person)
	{
//		Debug.Log ("Person updated with ID " + person.id);
		if (peopleCubes.ContainsKey (person.id)) {
			GameObject cubeToMove = peopleCubes [person.id];
			if (m_useSmoothing)
				cubeToMove.transform.position = Vector3.Lerp (cubeToMove.transform.position, positionForPerson (person), Time.deltaTime * m_damping);
			else
				cubeToMove.transform.position = positionForPerson (person);
		}
	}

	public void personWillLeave (OpenTSPSPerson person)
	{
		//	Debug.Log ("Person leaving with ID " + person.id);
		if (peopleCubes.ContainsKey (person.id)) {
			GameObject cubeToRemove = peopleCubes [person.id];
			peopleCubes.Remove (person.id);
			//delete it from the scene	
			Destroy (cubeToRemove);
		}
	}
	
	//maps the OpenTSPS coordinate system into one that matches the size of the boundingPlane
	private Vector3 positionForPerson (OpenTSPSPerson person)
	{
		//Bounds meshBounds = boundingPlane.GetComponent<MeshFilter> ().sharedMesh.bounds;
		Bounds meshBounds = boundingPlane.GetComponent<MeshRenderer> ().bounds;
		Vector3 offset = meshBounds.center;
		Vector3 pos = new Vector3 ((float)(.5f - person.centroidX) * meshBounds.size.x, 0.25f, (float)(person.centroidY - .5f) * meshBounds.size.z);

		return pos + offset;
	}

	public void SmoothMovement (bool isSmooth)
	{
		m_useSmoothing = isSmooth;
		PlayerPrefs.SetInt ("USE_SMOOTHING", Convert.ToInt32 (isSmooth));

	}

	public void UseMouseForMotion (bool useMouse)
	{
		m_useMouse = useMouse;
		PlayerPrefs.SetInt ("USE_MOUSE", Convert.ToInt32 (useMouse));

		if (!m_useMouse && m_mousePerson)
			Destroy (m_mousePerson);

	}

	public void UpdateSmoothingAmount (float amount)
	{
		m_damping = amount;
	}

	public void SetGUIValues ()
	{
		
		if (!PlayerPrefs.HasKey ("USE_SMOOTHING")) {
			PlayerPrefs.SetInt ("USE_SMOOTHING", Convert.ToInt32 (m_useSmoothing));
			Debug.Log ("No Smoothing preference set, setting preference to " + Convert.ToInt32 (m_useSmoothing).ToString ());
		} else {

			int state = PlayerPrefs.GetInt ("USE_SMOOTHING");
			if (state < 1)
				m_useSmoothing = false;
			else
				m_useSmoothing = true;
		}

		if (m_smoothingGUI) {
			if (m_useSmoothing)
				m_smoothingGUI.isOn = true;
			else
				m_smoothingGUI.isOn = false;
		} else {
			Debug.Log ("No Smoothing UI is setup in " + this.name);
		}

		if (!PlayerPrefs.HasKey ("USE_MOUSE")) {
			PlayerPrefs.SetInt ("USE_MOUSE", Convert.ToInt32 (m_useMouse));
			Debug.Log ("No Mouse preference set, setting preference to " + Convert.ToInt32 (m_useMouse).ToString ());
		} else {

			int state = PlayerPrefs.GetInt ("USE_MOUSE");
			if (state < 1)
				m_useMouse = false;
			else
				m_useMouse = true;
		}

		if (m_useMouseGUI) {
			if (m_useMouseGUI)
				m_useMouseGUI.isOn = true;
			else
				m_useMouseGUI.isOn = false;
		} else {
			Debug.Log ("No Mouse UI is setup in " + this.name);
		}

	}

	Vector3 GetMousePosition ()
	{
		
		if (m_mainCamera == null) {
			Debug.LogError ("No camera for Mouse to use on " + this.name + "!");
			return Vector3.zero;
		}


		RaycastHit hit;
		Ray ray = m_mainCamera.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 15f, m_mask)) {
			if (hit.collider != null && hit.transform.tag == "Platform")
				return hit.point;
		}

		Vector3 position = transform.position;

		return position;
	}


}

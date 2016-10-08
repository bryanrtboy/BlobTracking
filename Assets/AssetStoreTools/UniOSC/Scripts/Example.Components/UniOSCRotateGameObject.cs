/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using OSCsharp.Data;


namespace UniOSC{

	/// <summary>
	/// Rotates (localRotation) the hosting game object.
	/// For every axis you have a separate OSC address to specify
	/// </summary>
	[AddComponentMenu("UniOSC/RotateGameObject")]
	public class UniOSCRotateGameObject :  UniOSCEventTarget {

		#region public
		[HideInInspector]
		public Transform transformToRotate;

		public string X_Address;
		public string Y_Address;
		public string Z_Address;

		public float x_RotationFactor;
		public float y_RotationFactor;
		public float z_RotationFactor;
		#endregion

		#region private
		private Vector3 eulerAngles;
		private Quaternion rootRot;
		private float cx,cy,cz;
		private Quaternion rx,ry,rz;
		#endregion


		void Awake(){

		}

		public override void OnEnable(){
			_Init();
			base.OnEnable();
		}

		private void _Init(){
			
			//receiveAllAddresses = false;
			_oscAddresses.Clear();
			if(!_receiveAllAddresses){
				_oscAddresses.Add(X_Address);
				_oscAddresses.Add(Y_Address);
				_oscAddresses.Add(Z_Address);
			}
			cx=0f;cy=0f;cz=0f;
			if(transformToRotate == null){
				Transform hostTransform = GetComponent<Transform>();
				if(hostTransform != null) transformToRotate = hostTransform;
			}
			
			rootRot = transformToRotate.localRotation;
		}
	

		public override void OnOSCMessageReceived(UniOSCEventArgs args){
		
			if(transformToRotate == null) return;

			OscMessage msg = (OscMessage)args.Packet;

			if(msg.Data.Count <1)return;
			if(!( msg.Data[0] is System.Single))return;

			float value = (float)msg.Data[0] ;

			if(String.Equals(args.Address,X_Address))cx = value * x_RotationFactor;
			if(String.Equals(args.Address,Y_Address))cy = value * y_RotationFactor;
			if(String.Equals(args.Address,Z_Address))cz = value * z_RotationFactor;

			rx = Quaternion.AngleAxis (cx,  Vector3.right); 
			ry = Quaternion.AngleAxis (cy , Vector3.up);
			rz = Quaternion.AngleAxis (cz,  Vector3.forward);

			transformToRotate.localRotation = rootRot * rx*ry*rz;

		}


	}

}
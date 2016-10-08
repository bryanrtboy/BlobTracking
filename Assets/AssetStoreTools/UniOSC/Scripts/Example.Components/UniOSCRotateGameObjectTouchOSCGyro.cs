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

	[AddComponentMenu("UniOSC/TouchOSC Gyro Rotate")]
	public class UniOSCRotateGameObjectTouchOSCGyro :  UniOSCEventTarget {

		#region public
		[HideInInspector]
		public Transform transformToRotate;
		public float x_RotationFactor=90;
		public float y_RotationFactor=90;
		public float z_RotationFactor=90;
		public float damping=1;
		#endregion

		#region public
		private Quaternion rootRot;
		private float cx,cy,cz;
		private Quaternion rx,ry,rz;
		private float _damping;
		#endregion


		void Awake(){
		}


		public override void OnEnable(){

			base.OnEnable();
				if(transformToRotate == null){
				Transform hostTransform = GetComponent<Transform>();
				if(hostTransform != null) transformToRotate = hostTransform;
			}

			rootRot = transformToRotate.localRotation;
		}
	

		public override void OnOSCMessageReceived(UniOSCEventArgs args){

			if(transformToRotate == null) return;
			OscMessage msg = (OscMessage)args.Packet;

			if(msg.Data.Count <3)return;
			if(!( msg.Data[0] is System.Single))return;

			cx = (float)msg.Data[1];
			cy = 0;//(float)args.Message.Data[2];
			cz = -(float)msg.Data[0];

			cx*= x_RotationFactor;
			cy*= y_RotationFactor;
			cz*= z_RotationFactor;

			rx = Quaternion.AngleAxis (cx,  Vector3.right); 
			ry = Quaternion.AngleAxis (cy , Vector3.up);
			rz = Quaternion.AngleAxis (cz,  Vector3.forward);

			//transformToRotate.localRotation = rootRot * rx*ry*rz;

			if(Application.isEditor && !Application.isPlaying){
				_damping = 0.033f *damping;
			}else{
				_damping = Time.deltaTime *damping;
			}
			_damping = Mathf.Min(_damping,1f);
			transformToRotate.localRotation =Quaternion.Slerp(transformToRotate.localRotation, rootRot * rx*ry*rz,_damping);

		}


	}

}
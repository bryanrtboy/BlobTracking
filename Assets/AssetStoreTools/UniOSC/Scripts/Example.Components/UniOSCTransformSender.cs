/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using OSCsharp.Data;

namespace UniOSC{


    [AddComponentMenu("UniOSC/TransformSender")]
	public class UniOSCTransformSender : UniOSCEventDispatcher {

		public GameObject trackedGameObject;

		private Vector3 _currentPosition = Vector3.zero;
		private Vector3 _currentRotationEuler = Vector3.zero;

	
		public override void OnEnable ()
		{
			if (trackedGameObject == null) trackedGameObject = gameObject;
			//Here we setup our OSC message
			base.OnEnable ();
            ClearData();
			//now we could add data;
			AppendData(0f);//Translation.x
			AppendData(0f);//Translation.y
			AppendData(0f);//Translation.z

			AppendData(0f);//Rotation.x
			AppendData(0f);//Rotation.y
			AppendData(0f);//Rotation.z

			StartSendIntervalTimer();

		}

		public override void OnDisable ()
		{
			base.OnDisable ();
			StopSendIntervalTimer();
		}



		void FixedUpdate(){
			_Update();
		}
		protected override void _Update ()
		{

			base._Update ();

			if(trackedGameObject == null) return;

			_currentPosition =trackedGameObject.transform.position;
			_currentRotationEuler = trackedGameObject.transform.eulerAngles;

			OscMessage msg = null;
			if(_OSCeArg.Packet is OscMessage){
				msg = ((OscMessage)_OSCeArg.Packet);
			}else if(_OSCeArg.Packet is OscBundle){
				//bundle version
				msg = ((OscBundle)_OSCeArg.Packet).Messages[0];
			}
			if(msg != null)
			{
				msg.UpdateDataAt(0,_currentPosition.x);
				msg.UpdateDataAt(1,_currentPosition.y);
				msg.UpdateDataAt(2,_currentPosition.z);
				
				msg.UpdateDataAt(3,_currentRotationEuler.x);
				msg.UpdateDataAt(4,_currentRotationEuler.y);
				msg.UpdateDataAt(5,_currentRotationEuler.z);
			}
			

			//only send OSC messages at our specified interval
			lock(_mylock){
				if(!_isOSCDirty)return;
				_isOSCDirty = false;
			}

			_SendOSCMessage(_OSCeArg);
		
		}


	}

}

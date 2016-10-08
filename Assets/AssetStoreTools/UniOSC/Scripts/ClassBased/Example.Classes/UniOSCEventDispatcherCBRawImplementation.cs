/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using OSCsharp.Data;

namespace UniOSC{

	/// <summary>
	/// This class is a blueprint for your own implementations of the abstract class UniOSCEventDispatcherCodeBased
	/// Dispatcher forces a OSCConnection to send a OSC Message.
	/// //Don't forget the base callings !!!!
	/// </summary>
	public class UniOSCEventDispatcherCBRawImplementation : UniOSCEventDispatcherCB {

		#region constructors


		/// <summary>
		/// You have to override the constructors you want to use from the base class <see cref="UniOSC.UniOSCEventDispatcherCodeBased"/> class.
		/// 
		/// </summary>
		public UniOSCEventDispatcherCBRawImplementation(string _oscOutAddress, string _oscOutIPAddress, int _oscPort): base( _oscOutAddress, _oscOutIPAddress, _oscPort)
		{
		}

        public UniOSCEventDispatcherCBRawImplementation(string _oscOutAddress, UniOSCConnection _explicitConnection)
            : base(_oscOutAddress, _explicitConnection)
		{
		}
		#endregion


		#region events
		public override void Awake(){
			//Debug.Log("UniOSCEventDispatcherCodeBasedImplementation.Awake");			
		}


		public override void Enable() 
		{
			//Don't forget this!!!!
			base.Enable();
        }
		
		public override void Disable()
		{
			//Don't forget this!!!!
			base.Disable();
		}
		#endregion	

	}
}

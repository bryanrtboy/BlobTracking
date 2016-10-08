/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using System;
using OSCsharp.Data;

namespace UniOSC{

	/// <summary>
	/// This class is a blueprint for your own implementations of the abstract class UniOSCEventDispatcherCodeBased
	/// Dispatcher forces a OSCConnection to send a OSC Message.
	/// //Don't forget the base callings !!!!
	/// </summary>
	public class UniOSCEventDispatcherCBSimple : UniOSCEventDispatcherCB {

		#region constructors


		/// <summary>
		/// You have to override the constructors you want to use from the base class <see cref="UniOSC.UniOSCEventDispatcherCodeBased"/> class.
		/// 
		/// </summary>
		public UniOSCEventDispatcherCBSimple(string __oscOutAddress, string __oscOutIPAddress, int __oscPort): base( __oscOutAddress, __oscOutIPAddress, __oscPort)
		{
		}

        public UniOSCEventDispatcherCBSimple(string __oscOutAddress, UniOSCConnection __explicitConnection)
            : base(__oscOutAddress, __explicitConnection)
		{
		}
		#endregion


		#region events
		public override void Awake(){
			
		}


		public override void Enable() 
		{
			//Don't forget this!!!!
			base.Enable();

			//here your custom code
			


		}
		
		public override void Disable()
		{
			//Don't forget this!!!!
			base.Disable();
		}
		#endregion


		/// <summary>
		/// Just a demo method to show how you can change the data of your OSC Message
		/// </summary>
		/// <param name="val">If set to <c>true</c> value.</param>
		public void SetDataAtIndex0(object val){
            try
            {
				if(_OSCeArg.Packet is OscMessage)
				{
					((OscMessage)_OSCeArg.Packet).UpdateDataAt(0, val);
				}
				else if(_OSCeArg.Packet is OscBundle)
				{
                    foreach (OscMessage m in ((OscBundle)_OSCpkg).Messages)
                    {
                        m.UpdateDataAt(0, val);
                    }
				}
            }catch(Exception){

            }
           
			
		}

	

	}
}

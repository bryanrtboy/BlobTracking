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
	/// This class is a blueprint for your own implementations of the abstract class OSCDispatcherTargetCB
	/// //Don't forget the base callings !!!!
	///  The OnOSCMessageReceived method is where you should parse the OSC data
	/// </summary>
	[Serializable]
	public class UniOSCEventTargetCBImplementation : UniOSCEventTargetCB{

	
		#region constructors


		/// You have to override the constructors you want to use from the base class <see cref="UniOSC.UniOSCEventTargetCB"/> class.


		public UniOSCEventTargetCBImplementation(int oscPort) : base(oscPort)
		{
		}
		public UniOSCEventTargetCBImplementation(string oscAddress):base(oscAddress)
		{
		}

		public UniOSCEventTargetCBImplementation(UniOSCConnection con):base(con)
		{
		}

		public UniOSCEventTargetCBImplementation(string oscAddress, int oscPort):base(oscAddress, oscPort)
		{
		}

		public UniOSCEventTargetCBImplementation( string oscAddress,UniOSCConnection con):base(oscAddress, con)
		{
		}

		#endregion

		public override void Awake() 
		{
			//Don't forget this!!!!
			base.Awake();
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


		/// <summary>
		/// Method is called from a OSCConnection when a OSC message arrives. 
		/// The argument is a UniOSCEventArgs object where all the related data is enclosed
		/// </summary>
		/// <param name="args">OSCEventArgs</param>
		public override void OnOSCMessageReceived(UniOSCEventArgs args)
		{
			Debug.Log("UniOSCEventTargetCBImplementation.OnOSCMessageReceived:"+((OscMessage)args.Packet).Address);

			//((OscMessage)args.Packet).Data[0].
			//as we are an EventTarget we can cast the args.Packet to an OscMessage without any further check

			//args.Address
			//(OscMessage)args.Packet) => The OscMessage object
			//(OscMessage)args.Packet).Data  (get the data of the OscMessage as an object[] array)

			//It is a good practice to always check the data count before accessing the data.
			// if((OscMessage)args.Packet).Data.Count <1)return;
			
			/*to  check the data type we have several option:
			* a) 
			* if(( ((OscMessage)args.Packet).Data[0] is System.Single)
			* 
			* b) 
			* if( ((OscMessage)args.Packet).Data[0].GetType() == typeof(System.Single))
			* 
			* c) check the typeTag (see OSC documentation : http://opensoundcontrol.org/spec-1_0 
			* typeTag is a string like ',f' for a single float or ',ff' for two floats
			* if((OscMessage)args.Packet).TypeTag.Substring(1,1) == "f")
			*/
			
			//Debug.Log("typeTag: "+((OscMessage)args.Packet).TypeTag);
			
			//For addresses like  '/1/push8'  we could filter via these properties:
			//args.Group (1)
			//args.AddressRoot ("push") 
			//args.AddressIndex (8)
			//Debug.Log("Group: "+args.Group);
			//Debug.Log("AddressRoot: "+args.AddressRoot);
			//Debug.Log("AddressIndex: "+args.AddressIndex);
			//if the OSC address doesn't match this pattern the Group and AddressIndex will be default -1 and AddressRoot is empty


		}


	}
}

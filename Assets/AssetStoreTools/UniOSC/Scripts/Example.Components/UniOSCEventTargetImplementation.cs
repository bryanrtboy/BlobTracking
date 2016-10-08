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
	/// This class is a blueprint for your own implementations of the abstract class OSCDispatcherTarget
	/// //Don't forget the base callings !!!!
	///  The OnOSCMessageReceived method is where you should parse the OSC data
	/// </summary>
	public class UniOSCEventTargetImplementation :  UniOSCEventTarget {

		void Awake(){

		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start () {
			//Don't forget this!!!!
			base.Start();
			//here your custom code
		}

		/// <summary>
		/// Raises the enable event.
		/// If you want to listen to several OSC messages you have to  set the OSCAddresses property before you call base.OnEnable()
		/// OSCAddresses.Clear();
		/// OSCAddresses.Add(...);
		/// </summary>
		public override void OnEnable(){
			//optional 
			//receiveAllAddresses = true;
			_Init();
			//Don't forget this!!!!
			base.OnEnable();
			//here your custom code
		}

		/// <summary>
		/// this is a custom init function. Called from OnEnable
		/// </summary>
		private void _Init(){
			/*
			//this is an example when you listen not to all address but more than one.
			//if you set receiveAllAddresses = true it doesn't make sense to add addresses explicit as the OnOSCMessageReceived is called on all addresses
			
			receiveAllAddresses = false;
			
			_oscAddresses.Clear();
			//add the message strings to our address list
			_oscAddresses.Add(Address_A);
			_oscAddresses.Add(Address_B);
			_oscAddresses.Add(Address_C);
			*/
		}



		/// <summary>
		/// Raises the disable event.
		/// </summary>
		public override void OnDisable(){
			//Don't forget this!!!!
			base.OnDisable();
			//here your custom code
		}

		public override void Update () {
			//Don't forget this!!!!
			base.Update();
			//here your custom code
		}

		/// <summary>
		/// Method is called from a OSCConnection when a OSC message arrives. 
		/// The argument is a UniOSCEventArgs object where all the related data is enclosed
		/// </summary>
		/// <param name="args">OSCEventArgs</param>
		public override void OnOSCMessageReceived(UniOSCEventArgs args){
			//this is the method where you handle the OSC data and do your stuff with the GameObject!
			//args.Address
            //To get the OSCMessage we need to typecast the Packet 
            // OscMessage msg = (OscMessage)args.Packet;
            //msg.Data  (get the data from the OSCMessage as an object[] array)

			//It is a good practice to always check the data count before accessing the data.
			// if(args.Message.Data.Count <1)return;

            /*to  check the data type we have several option:
            * a) 
            * if(  ((OscMessage)args.Packet).Data[0] is System.Single)
            * 
            * b) 
            * if( ((OscMessage)args.Packet).Data[0].GetType() == typeof(System.Single) )
            * 
            * c) check the typeTag (see OSC documentation : http://opensoundcontrol.org/spec-1_0 
            * typeTag is a string like ',f' for a single float or ',ff' for two floats
            * if( ((OscMessage)args.Packet).TypeTag.Substring(1,1) == "f")
            */

            //Debug.Log("typeTag: "+args.Message.TypeTag);

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
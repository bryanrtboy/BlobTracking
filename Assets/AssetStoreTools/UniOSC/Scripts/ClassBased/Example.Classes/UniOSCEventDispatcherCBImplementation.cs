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
	public class UniOSCEventDispatcherCBImplementation : UniOSCEventDispatcherCB {

		#region constructors


		/// <summary>
		/// You have to override the constructors you want to use from the base class <see cref="UniOSC.UniOSCEventDispatcherCodeBased"/> class.
		/// 
		/// </summary>
		public UniOSCEventDispatcherCBImplementation(string _oscOutAddress, string _oscOutIPAddress, int _oscPort): base( _oscOutAddress, _oscOutIPAddress, _oscPort)
		{
		}

		public UniOSCEventDispatcherCBImplementation(string _oscOutAddress, UniOSCConnection _explicitConnection): base(_oscOutAddress, _explicitConnection)
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
            ClearData();
			//here your custom code

            //We append our data that we want to send with a message
            //you can also append data from outside the class : instance.AppendData(data)
            //Look below for the different way depending on your current  BundleMode. Message is default
            //To a OSCMessage we only can append data types that are supported by the OSC specification:
            //(Int32,Int64,Single,Double,String,Byte[],OscTimeTag,Char,Color,Boolean)

            //Non Bundle mode works on the OSCMessage level
            AppendData(123);//int data at index [0]
            AppendData(123f);// float data at index [1]
            AppendData("MyString");// string data at index [2]

            //if you want to delete the data just call:
            //ClearData(); 

            /*
            //This is the way to handle bundles
            SetBundleMode(true);
            OscMessage msg1 = new OscMessage("/TestA");
            msg1.Append(1f);
            msg1.Append(2);
            msg1.Append("HalloA");

            AppendData(msg1);//Append message to bundle


            OscMessage msg2 = new OscMessage("/TestB");
            msg1.Append(1f);
            msg1.Append(2);
            msg1.Append("HalloB");
			
            AppendData(msg2);//Append message to bundle
            */

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
		public void SetDataAtIndex0(bool val)
        {

			if(_OSCeArg.Packet is OscMessage){
                OscMessage msg = ((OscMessage)_OSCeArg.Packet);
                msg.UpdateDataAt(0, System.Convert.ToInt32(val));
                msg.UpdateDataAt(1, System.Convert.ToSingle(!val));
			}
            else if(_OSCeArg.Packet is OscBundle)
            {
				//bundle version
                foreach (OscMessage m in ((OscBundle)_OSCpkg).Messages)
                {
                    m.UpdateDataAt(0, System.Convert.ToInt32(val));
                    m.UpdateDataAt(1, System.Convert.ToSingle(!val));
                }
				
			}
		
		}

	

	}
}

/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using OSCsharp.Data;


namespace UniOSC{

	/// <summary>
	/// This class is a blueprint for your own implementations of the abstract class UniOSCEventDispatcher
	/// Dispatcher forces a OSCConnection to send a OSC Message.
	/// //Don't forget the base callings !!!!
	/// </summary>

	[ExecuteInEditMode]
	public class UniOSCEventDispatcherImplementation: UniOSCEventDispatcher {

		public int dynamicIntValue= 1000;
		public float dynamicFloatValue= 1000f;
		public string dynamicStringValue= "Test";

		public override void Awake ()
		{
			base.Awake ();
			//here your custom code

			
		}
		public override void OnEnable ()
		{
			//Don't forget this!!!!
			base.OnEnable ();

            //here your custom code

            //If you append data  data later you should always call this first otherwise we get more and more data appended if you toggle the enabled state of your component.
            ClearData();
			

            //We append our data that we want to send with a message
            //The best place to do this step is on Enable().This approach is more flexible through the internal way UniOSC works.
            //later in the your "MySendOSCMessageTriggerMethod" you change this data with :
            //Message mode: ((OscMessage)_OSCeArg.Packet).UpdateDataAt(index,yourValue); 
            //Bundle mode Mode: ((OscBundle)_OSCeArg.Packet).Messages[i]).UpdateDataAt(index,yourValue);
            //We only can append data types that are supported by the OSC specification:
            //(Int32,Int64,Single,Double,String,Byte[],OscTimeTag,Char,Color,Boolean)

            //Message mode
            
            AppendData(123);//int data at index [0]
            AppendData(123f);// float data at index [1]
            AppendData("MyString");// string data at index [2]

            //.......

            /*
            //This is the way to handle bundle mode
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
		public override void OnDisable ()
		{
			//Don't forget this!!!!
			base.OnDisable ();
			//here your custom code

		}


		/// <summary>
		/// Just a dummy method that shows how you trigger the OSC sending and how you could change the data of the OSC Message 
		/// </summary>
		public void MySendOSCMessageTriggerMethod(){
			//Here we update the data with a new value
			//OscMessage msg = null;
			if(_OSCeArg.Packet is OscMessage)
			{
				//message
                OscMessage msg = ((OscMessage)_OSCeArg.Packet);
                _updateOscMessageData(msg);

			}
			else if(_OSCeArg.Packet is OscBundle)
			{
				//bundle 
                foreach (OscMessage msg2 in ((OscBundle)_OSCeArg.Packet).Messages)
                {
                    _updateOscMessageData(msg2);
                }	
			}
           			
			//Here we trigger the sending
			_SendOSCMessage(_OSCeArg);
		}

        /// <summary>
        /// Just a dummy method that shows how you update the data of the OSC Message 
        /// </summary>
        private void _updateOscMessageData(OscMessage msg)
        {
            msg.UpdateDataAt(0, dynamicIntValue);
            msg.UpdateDataAt(1, dynamicFloatValue);
            msg.UpdateDataAt(2, dynamicStringValue);
        }


	}
}
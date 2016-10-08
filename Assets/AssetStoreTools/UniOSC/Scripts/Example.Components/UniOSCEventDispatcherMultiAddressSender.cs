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
    public class UniOSCEventDispatcherMultiAddressSender : UniOSCEventDispatcher
    {

        public bool bundleMode;
		public float data= 1000f;
		
        public string[] addressArray;
        private List<UniOSCEventDispatcherCB> senderList = new List<UniOSCEventDispatcherCB>();


		public override void Awake ()
		{
			base.Awake ();
			//here your custom code

			//.......
		}
		public override void OnEnable ()
		{
			//Don't forget this!!!!
			base.OnEnable ();
			//here your custom code

            useExplicitConnection = true;
            if (!useExplicitConnection)
            {
                Debug.LogWarning("Please use the Explicit connection option!");
                return;
            }

            //We append our data that we want to send with a message
            //later in the your "MySendOSCMessageTriggerMethod" you change this data  
            //We only can append data types that are supported by the OSC specification:
            //(Int32,Int64,Single,Double,String,Byte[],OscTimeTag,Char,Color,Boolean)
            if (bundleMode) {
               
                SetBundleMode(true);
                ClearData();
                AppendData(new OscMessage(oscOutAddress, data));
               // Debug.Log(((OscBundle)_OSCeArg.Packet).Messages.Count);
                if (addressArray != null)
                {
                    foreach (var d in addressArray)
                    {
                        if (String.IsNullOrEmpty(d)) continue;
                        OscMessage msg = new OscMessage(d);
                        msg.Append(data);
                        AppendData(msg);
                    }
                }
               // Debug.Log(((OscBundle)_OSCeArg.Packet).Messages.Count);
               
            }
            else
            {
                ClearData();
                AppendData(data);

                if (addressArray != null)
                {
                    foreach (var d in addressArray)
                    {
                        if (String.IsNullOrEmpty(d)) continue;
                        UniOSCEventDispatcherCB oscSender = new UniOSCEventDispatcherCBSimple(d, explicitConnection);//OSCOutAddress,OSCConnection
                       
                        oscSender.AppendData(data);                 
                        oscSender.Enable();
                      
                        senderList.Add(oscSender);
                    }
                }
            }
           

          
            

		}
		public override void OnDisable ()
		{
			//Don't forget this!!!!
			base.OnDisable ();
			//here your custom code
            for (var i = 0;i<  senderList.Count;i++)
            {
                senderList[i].Dispose();
                senderList[i] = null;
            }
            senderList.Clear();

		}


		/// <summary>
		/// Just a dummy method that shows how you trigger the OSC sending and how you could change the data of the OSC Message 
		/// </summary>
		public void MySendOSCMessageTriggerMethod(){
			//Here we update the data with a new value
           
            if (_OSCeArg.Packet is OscMessage){
                ((OscMessage)_OSCeArg.Packet).Address = oscOutAddress;
			}else if (_OSCeArg.Packet is OscBundle)
			{
                int i = 0;
				foreach(OscMessage m in ((OscBundle)_OSCeArg.Packet).Messages)
				{
                   
                   // Debug.Log(i);
                    if (i == 0) {
                        m.Address = oscOutAddress;
                    } else {
                        m.Address = addressArray[i - 1];
                    }
                  // m.Append(data)
                    i++;
				}
			}
            
			//Here we trigger the sending
			_SendOSCMessage(_OSCeArg);
            //if Message this list is empty
            foreach (var s in senderList)
            {
                s.SendOSCMessage();
            }
		}

        //Comment this Method out. Just for testing
        public void OnGUI() {

            if (GUI.Button(new Rect(0f, 0f, 120f, 100f), new GUIContent("MultiAddress\n sender", "")))
            {
                MySendOSCMessageTriggerMethod();
            }
        }


	}
}
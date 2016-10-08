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
    public class UniOSCEventDispatcherMultiConnectionSender : UniOSCEventDispatcher
    {

		
		public float data= 1000f;

        public UniOSCConnection[] connectionArray;
        //oscSender1 = new UniOSCEventDispatcherCBImplementation
       // public string[] addressArray;
        private List<UniOSCEventDispatcherCB> senderList = new List<UniOSCEventDispatcherCB>();


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
            ClearData();
            AppendData(data);//data at index [0]

            useExplicitConnection = true;
            if (!useExplicitConnection)
            {
                Debug.LogWarning("Please use the Explicit connection option!");
                return;
            }
            if (connectionArray != null)
            {
                foreach (var c in connectionArray)
                {
                    if (c== null) continue;
                    UniOSCEventDispatcherCB oscSender = new UniOSCEventDispatcherCBSimple(oscOutAddress, c);//OSCOutAddress,OSCConnection
                   
                    //oscSender.ClearData();
                    oscSender.Enable();

                    /*
                     * How to append messages to a bundle
                    oscSender.SetBundleMode(true);
                                     
                    OscMessage msg1 = new OscMessage("/TestA");
                    msg1.Append(data);
                   

                    oscSender.AppendData(msg1);
                    oscSender.AppendData(new OscMessage("/TestB",data));
                    */

                    oscSender.AppendData(data);
                   
                    senderList.Add(oscSender);
                }
            }

		}
		public override void OnDisable ()
		{
			//Don't forget this!!!!
			base.OnDisable ();
			//here your custom code

           // ClearData();
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
		public void MySendOSCMessageTrigerMethod(){
			//Here we update the data with a new value
           
            try
            {
                if (_OSCpkg is OscMessage)
                {
                    ((OscMessage)_OSCpkg).UpdateDataAt(0, data);
                }
                else if (_OSCpkg is OscBundle)
                {
                    foreach (OscMessage m in ((OscBundle)_OSCpkg).Messages)
                    {
                        m.UpdateDataAt(0, data);
                    }
                   
                }
            }
            catch (Exception)
            {

            }

			//Here we trigger the sending                  
            SendOSCMessage();

            foreach (var s in senderList)
            {
                s.SendOSCMessage();
            }
             
		}

        //Comment this Method out. Just for testing
        public void OnGUI() {

            if (GUI.Button(new Rect(Screen.width - 120f, 0f, 120f, 100f), new GUIContent("MultiConnection\n sender", "")))
            {
                MySendOSCMessageTrigerMethod();
            }
        }


	}
}
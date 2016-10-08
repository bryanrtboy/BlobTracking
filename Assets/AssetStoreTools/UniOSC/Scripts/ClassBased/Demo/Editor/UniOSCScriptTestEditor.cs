/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using OSCsharp.Data;


namespace UniOSC{
	
	/// <summary>
	/// Editor for the administration of OSCconnections, mapping files.
	/// You can also trace the OSC data flow .
	/// </summary>
	[Serializable]
	public class UniOSCScriptTestEditor : EditorWindow {
	
		#region private
		private  UniOSCEventTargetCBImplementation oscTarget1;
		private  UniOSCEventTargetCBImplementation oscTarget2;
		private  UniOSCEventTargetCBImplementation oscTarget3;
		private  UniOSCEventTargetCBImplementation oscTarget4;
		private  UniOSCEventTargetCBImplementation oscTarget5;

		private UniOSCEventDispatcherCB oscSender1;

		private int OSCPort = 8000;
		private UniOSCConnection OSCConnection;
		private int OSCConnectionID = 0;
		private string OSCAddress = "/1/push7";
		private string OSCOutAddress = "/2/push7";

		private string oscTarget1Msg;
		#endregion

		public static UniOSCScriptTestEditor Instance { get; private set; }

		static EditorWindow _windowSelf;

		public static bool IsOpen {
			get { return Instance != null; }
		}

		[MenuItem("Window/UniOSC/Test/ScriptTestEditor")]
		static void _Init(){
			_windowSelf = EditorWindow.GetWindow(typeof(UniOSCScriptTestEditor));
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
            _windowSelf.title = "UniOSC Test Editor";
#else
            _windowSelf.titleContent = new GUIContent("UniOSC Test Editor", "UniOSC Test Editor");
#endif          
			_windowSelf.minSize = new Vector2(256f,256f);
			_windowSelf.autoRepaintOnSceneChange = true;
		}

		#region Enable
		public void OnEnable() {
			Debug.Log("OnEnable");

			// We need to monitor the playmodeStateChanged event to update the references to OSCConnections (only necessary when we use the explicitConnection feature)
			//and force a new connection setup through disable/enable on our OSCEventTargets otherwise we have to re-open our Editor 
			EditorApplication.playmodeStateChanged += _HandleOnPlayModeChanged;

            //When we use a OSCConnection in the constructor of a OSCEventTarget instance we need to also store the InstanceID to be able to re-reference it on playmodeStateChanges!
            OSCConnection = FindObjectOfType<UniOSCConnection>() as UniOSCConnection;
            if (OSCConnection != null)
            {
                OSCConnectionID = OSCConnection.GetInstanceID();
                Debug.Log("Used Connection:" + OSCConnection.name);
            }
            else
            {
                Debug.LogWarning("No OSC Connection is found in scene or is enabled!");
                return;
            }


			//Here we show the different possibilities to create a OSCEventTarget from code:

			//When we only specify a port we listen to all OSCmessages on that port (We assume that there is a OSCConnection with that listening port in our scene)
			oscTarget1 = new UniOSCEventTargetCBImplementation(OSCPort);
			oscTarget1.OSCMessageReceived+=OnOSCMessageReceived1;
			oscTarget1.Enable();

			

			//This implies that we use the explicitConnection mode. (With responding to all OSCmessages)
			oscTarget2 = new UniOSCEventTargetCBImplementation(OSCConnection);
			oscTarget2.OSCMessageReceived+=OnOSCMessageReceived2;
			oscTarget2.Enable();

			//We listen to a special OSCAddress regardless of the port.
			oscTarget3 = new UniOSCEventTargetCBImplementation(OSCAddress);
			oscTarget3.OSCMessageReceived+=OnOSCMessageReceived3;
			oscTarget3.Enable();

			//The standard : respond to a given OSCAddress on a given port
			oscTarget4 = new UniOSCEventTargetCBImplementation(OSCAddress, OSCPort);
			oscTarget4.OSCMessageReceived+=OnOSCMessageReceived4;
			oscTarget4.Enable();

			//This version has the advantage that we are not bound to a special port. If the connection changes the port we still respond to the OSCMessage
			oscTarget5 = new UniOSCEventTargetCBImplementation(OSCAddress,OSCConnection);
			oscTarget5.OSCMessageReceived+=OnOSCMessageReceived5;
			oscTarget5.Enable();

			oscSender1 = new UniOSCEventDispatcherCBSimple(OSCOutAddress,OSCConnection);
            // oscSender1 = new UniOSCEventDispatcherCBSimple("Test0", "127.0.0.1", 9003);
			oscSender1.AppendData("TestData");
			oscSender1.AppendData(12345);
			oscSender1.Enable();
          
           // oscSender1.ClearData();
            //Just some test
            /*          
            oscSender1.AppendData(678);         
             */ 
           /*
            oscSender1.oscOutAddress = "/2/XXXXXX";
            oscSender1.oscOutAddress = "/2/XXXXXX2";
            oscSender1.Disable();
            oscSender1.oscOutAddress = "/2/XXXXXX3";
            oscSender1.oscOutPort = 9001;
            oscSender1.Enable();
            oscSender1.oscOutIPAddress = "127.0.0.1";
            oscSender1.useExplicitConnection = false;
            oscSender1.oscOutIPAddress = "127.0.0.1";
             oscSender1.oscOutIPAddress = "192.168.178.21";
             oscSender1.oscOutPort = 9000;
            * */

		}
		#endregion

		#region PlayModeChanged
		private void _HandleOnPlayModeChanged()
		{
            Debug.Log("_HandleOnPlayModeChanged");
			// This method is run whenever the playmode state is changed.
			if(Application.isPlaying){
				//Debug.Log("PLAY");
			}else{
				//Debug.Log("STOP");
			}
			
			//When we change the playmode we have to trigger a new Connection setup on our oscTargets, otherwise we loose our event binding!

			UniOSCConnection actualCon = null;
			if(OSCConnectionID >= 0) {
				actualCon = EditorUtility.InstanceIDToObject(OSCConnectionID) as UniOSCConnection;
			}

            oscTarget1.ForceSetupChange();           

			//If we use a explicitConnection we need to update the reference to the OSCConection:			
            oscTarget2.explicitConnection = actualCon;

            oscTarget3.ForceSetupChange();           

            oscTarget4.ForceSetupChange();            
			
            oscTarget5.explicitConnection = actualCon;
          
            oscSender1.explicitConnection = actualCon;
		
			/*
			if (! EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				
				Debug.Log("PLAY");
			}

			if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
					Debug.Log("STOP");
			}
			*/
			
			
		}
		#endregion

		// called at 10 frames per second to give the editor a chance to update.
		void OnInspectorUpdate(){
			//with unity 4 we loose Tooltips with this :-( 
			Repaint();
		}
		void OnGUI()
		{
			if(GUILayout.Button(new GUIContent("Send Data","Send Data"),GUILayout.MinHeight(30))){
				//Debug.Log("OnOSCMessageSend1:");
				oscSender1.SendOSCMessage();
			}

			GUILayout.Space(10);
			GUILayout.Label("oscTarget1 OSC IN Message:");
			GUILayout.Label(oscTarget1Msg);

		}

		#region Disable
		public void OnDisable() 
		{
			//Debug.Log("OnDisable");

			EditorApplication.playmodeStateChanged -= _HandleOnPlayModeChanged;

            if (OSCConnection == null) return;

			oscTarget1.Dispose();
			oscTarget1 = null;
			
			oscTarget2.Dispose();
			oscTarget2 = null;
			
			oscTarget3.Dispose();
			oscTarget3 = null;
			
			oscTarget4.Dispose();
			oscTarget4 = null;
			
			oscTarget5.Dispose();
			oscTarget5 = null;

			oscSender1.Dispose();
			oscSender1 = null;
		}
		#endregion

		#region callbacks
		//our custom callback methods to handle the OSC data in our editor
		private void OnOSCMessageReceived1(object sender, UniOSCEventArgs args){
			Debug.Log("OnOSCMessageReceived1:"+args.Address);
			OscMessage msg = ((OscMessage)args.Packet);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(msg.Address);
			foreach(var d in msg.Data){
				sb.AppendLine(d.ToString());
			}

			oscTarget1Msg = sb.ToString();
		}
		private void OnOSCMessageReceived2(object sender, UniOSCEventArgs args){
			Debug.Log("OnOSCMessageReceived2:"+args.Address);
		}
		private void OnOSCMessageReceived3(object sender, UniOSCEventArgs args){
			Debug.Log("OnOSCMessageReceived3:"+args.Address);
		}
		private void OnOSCMessageReceived4(object sender, UniOSCEventArgs args){
			Debug.Log("OnOSCMessageReceived4:"+args.Address);
		}
		private void OnOSCMessageReceived5(object sender, UniOSCEventArgs args){
			Debug.Log("OnOSCMessageReceived5:"+args.Address);
		}
		#endregion

	}
}

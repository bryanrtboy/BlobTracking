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
	/// Dispatcher toggle that forces a OSCConnection to send a OSC Message.
	/// Two separate states: On and Off 
	/// </summary>
	[AddComponentMenu("UniOSC/EventDispatcherToggle")]
	[ExecuteInEditMode]
	public class UniOSCEventDispatcherToggle: UniOSCEventDispatcher {

		#region public
		//[HideInInspector]
		public float onOSCDataValue=1;
		//[HideInInspector]
		public float offOSCDataValue=0;
		//[HideInInspector]
		public bool showGUI;
		//[HideInInspector]
		public float xPos;
		//[HideInInspector]
		public float yPos;
		#endregion

		#region private
		
		private GUIStyle _gs;
        private bool toggleState;
        private bool prevToggleState; 
		#endregion

		public override void Awake()
		{
			base.Awake ();
		}

		public override void OnEnable ()
		{
			base.OnEnable ();
            ClearData();
            AppendData(0f);
           

		}
		public override void OnDisable ()
		{
			base.OnDisable ();
		}
		void OnGUI(){
			if(!showGUI)return;
			RenderGUI();
		}

		void RenderGUI(){

			_gs = new GUIStyle(GUI.skin.button);
			_gs.fontSize=11;
			//gs.padding = new RectOffset(2,2,2,2);

			GUIScaler.Begin();

			//Event e = Event.current;
			GUI.BeginGroup(new Rect((Screen.width/GUIScaler.GuiScale.x)*xPos,(Screen.height/GUIScaler.GuiScale.y)*yPos,(Screen.width/GUIScaler.GuiScale.x),(Screen.height/GUIScaler.GuiScale.y)  ));

			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("");
			sb.AppendLine("Send OSC:");
			sb.AppendLine(String.Format("IP:{0}",oscOutIPAddress));
			sb.AppendLine(String.Format("Address:{0}",oscOutAddress));
			sb.AppendLine(String.Format("Port:{0}",oscOutPort));
			GUIContent buttonText = new GUIContent(sb.ToString());
			Rect toggleRect = GUILayoutUtility.GetRect(buttonText,_gs  ); 
			toggleRect.width *=1.0f;
			toggleRect.height*=1.0f;
			
            toggleState = GUI.Toggle(toggleRect, toggleState, buttonText);

            if(toggleState != prevToggleState)
            {
                Debug.Log("Toggle.Clicked:" + toggleState);
                if (toggleState) {
                    SendOSCMessageOn();
                }
                else
                {
                    SendOSCMessageOff();
                }
            }

            prevToggleState = toggleState;

			GUILayout.EndVertical();
			GUI.EndGroup();

			GUIScaler.End();
		}

		/// <summary>
		/// Sends the OSC message with the downOSCDataValue.
		/// </summary>
		public void SendOSCMessageOn(){
			if(_OSCeArg.Packet is OscMessage)
			{
				((OscMessage)_OSCeArg.Packet).UpdateDataAt(0, onOSCDataValue);
			}
			else if(_OSCeArg.Packet is OscBundle)
			{
                foreach (OscMessage m in ((OscBundle)_OSCeArg.Packet).Messages)
                {
                    m.UpdateDataAt(0, onOSCDataValue);
                }				
			}


			_SendOSCMessage(_OSCeArg);
		}

		/// <summary>
		/// Sends the OSC message with the upOSCDataValue.
		/// </summary>
		public void SendOSCMessageOff(){
			if(_OSCeArg.Packet is OscMessage)
			{
                ((OscMessage)_OSCeArg.Packet).UpdateDataAt(0, offOSCDataValue);
			}
			else if(_OSCeArg.Packet is OscBundle)
			{
                foreach (OscMessage m in ((OscBundle)_OSCeArg.Packet).Messages)
                {
                    m.UpdateDataAt(0, offOSCDataValue);
                }              
			}

			_SendOSCMessage(_OSCeArg);
		}


	}
}
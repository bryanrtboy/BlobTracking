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
	/// Dispatcher button that forces a OSCConnection to send a OSC Message.
	/// Two separate states: Down and Up 
	/// </summary>
	[AddComponentMenu("UniOSC/EventDispatcherButton")]
	[ExecuteInEditMode]
	public class UniOSCEventDispatcherButton: UniOSCEventDispatcher {

		#region public
		[HideInInspector]
		public float downOSCDataValue=1;
		[HideInInspector]
		public float upOSCDataValue=0;
		[HideInInspector]
		public bool showGUI;
		[HideInInspector]
		public float xPos;
		[HideInInspector]
		public float yPos;
		#endregion

		#region private
		private bool _btnDown;
		private GUIStyle _gs; 
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

			Event e = Event.current;
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
			Rect buttonRect = GUILayoutUtility.GetRect(buttonText,_gs  ); 
			buttonRect.width *=1.0f;
			buttonRect.height*=1.0f;
			
			if (e.isMouse && buttonRect.Contains(e.mousePosition)) { 
				if(e.type == EventType.MouseDown){
					SendOSCMessageDown();
				}
				if(e.type == EventType.MouseUp){
					SendOSCMessageUp();
				}
			} 
			
			GUI.Button(buttonRect, buttonText,_gs);
			
			GUILayout.EndVertical();
			GUI.EndGroup();

			GUIScaler.End();
		}

		/// <summary>
		/// Sends the OSC message with the downOSCDataValue.
		/// </summary>
		public void SendOSCMessageDown(){
			if(_OSCeArg.Packet is OscMessage)
			{
				((OscMessage)_OSCeArg.Packet).UpdateDataAt(0, downOSCDataValue);
			}
			else if(_OSCeArg.Packet is OscBundle)
			{
                foreach (OscMessage m in ((OscBundle)_OSCeArg.Packet).Messages)
                {
                    m.UpdateDataAt(0, downOSCDataValue);
                }				
			}


			_SendOSCMessage(_OSCeArg);
		}

		/// <summary>
		/// Sends the OSC message with the upOSCDataValue.
		/// </summary>
		public void SendOSCMessageUp(){
			if(_OSCeArg.Packet is OscMessage)
			{
                ((OscMessage)_OSCeArg.Packet).UpdateDataAt(0, upOSCDataValue);
			}
			else if(_OSCeArg.Packet is OscBundle)
			{
                foreach (OscMessage m in ((OscBundle)_OSCeArg.Packet).Messages)
                {
                    m.UpdateDataAt(0, upOSCDataValue);
                }              
			}

			_SendOSCMessage(_OSCeArg);
		}


	}
}
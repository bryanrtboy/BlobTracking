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
	[AddComponentMenu("UniOSC/EventDispatcherSlider")]
	[ExecuteInEditMode]
	public class UniOSCEventDispatcherSlider: UniOSCEventDispatcher {

		#region public

        public enum SliderMode { Horizontal,Vertical}
        public SliderMode sliderMode;
		//[HideInInspector]

        public float minOSCDataValue = 0;
        public float maxOSCDataValue = 1;
		
		//[HideInInspector]
		public bool showGUI;
		[Range(0f,1f)]
		public float xPos;
        [Range(0f, 1f)]
		public float yPos;
        public float sliderSize= 100f;
		#endregion

		#region private
		
		private GUIStyle _gs;
        private float _sliderValue;
        private float _prev_sliderValue; 
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
             
            toggleRect = new Rect(0f, 0f, sliderSize, sliderSize);
			
           // toggleState = GUI.Toggle(toggleRect, toggleState, buttonText);
            //_sliderValue = GUI.Slider(toggleRect, _sliderValue, 100, 0f, 100f, null, null, true, 0);
            switch (sliderMode)
            {
                case SliderMode.Horizontal:
                    _sliderValue = GUI.HorizontalSlider(toggleRect, _sliderValue, minOSCDataValue, maxOSCDataValue);
                    break;
                case SliderMode.Vertical:
                    _sliderValue = GUI.VerticalSlider(toggleRect, _sliderValue,maxOSCDataValue, minOSCDataValue );
                    break;

            }
            

            if (_sliderValue != _prev_sliderValue)
            {            
                SendOSCMessage();
            }

            _prev_sliderValue = _sliderValue;

			GUILayout.EndVertical();
			GUI.EndGroup();

			GUIScaler.End();
		}

		/// <summary>
		/// Sends the OSC message with the sliderValue.
		/// </summary>
		public override void SendOSCMessage(){
			if(_OSCeArg.Packet is OscMessage)
			{
                ((OscMessage)_OSCeArg.Packet).UpdateDataAt(0, _sliderValue);
			}
			else if(_OSCeArg.Packet is OscBundle)
			{
                foreach (OscMessage m in ((OscBundle)_OSCeArg.Packet).Messages)
                {
                    m.UpdateDataAt(0, _sliderValue);
                }				
			}


			_SendOSCMessage(_OSCeArg);
		}

		


	}
}
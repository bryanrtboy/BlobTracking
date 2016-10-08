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
using System.Net;
using System;
using System.Linq;

namespace UniOSC{

	[CustomEditor (typeof(UniOSCEventDispatcher),true)]
	[CanEditMultipleObjects]
	public class UniOSCEventDispatcherEditor :  Editor {

		protected UniOSCEventDispatcher _target;
	
		protected SerializedProperty _myOSCConnectionsProp ;
		protected SerializedProperty OSCConnectionsProp;
		protected SerializedProperty OSCOutPortProp ;
		protected SerializedProperty OSCOutAddressProp ;
		protected SerializedProperty OSCOutIPAddressProp ;
		protected SerializedProperty OSCOutProp;

		protected SerializedProperty UseExplicitConnectionProp;
		protected SerializedProperty ExplicitConnectionProp;

		//protected SerializedProperty sendIntervalProp;
		protected SerializedProperty drawDefaultInspectorProp;

		protected int _portIndex = 0;
		protected string[] _options;

		protected Texture2D _tex_logo;
	

		public virtual void OnEnable () {
		
			if(target  !=_target) _target = target as UniOSCEventDispatcher;

			_tex_logo = Resources.Load(UniOSCUtils.LOGO16_NAME,typeof(Texture2D)) as Texture2D;

			_myOSCConnectionsProp = serializedObject.FindProperty ("_myOSCConnections");
			OSCConnectionsProp = serializedObject.FindProperty ("OSCConnections");
			OSCOutAddressProp = serializedObject.FindProperty("_oscOutAddress");
			OSCOutPortProp = serializedObject.FindProperty("_oscOutPort");
			OSCOutIPAddressProp = serializedObject.FindProperty("_oscOutIPAddress");
			//sendIntervalProp = serializedObject.FindProperty("sendInterval");
			UseExplicitConnectionProp = serializedObject.FindProperty("_useExplicitConnection");
			ExplicitConnectionProp = serializedObject.FindProperty("_explicitConnection");
			drawDefaultInspectorProp = serializedObject.FindProperty ("_drawDefaultInspector");
		}

		override public void OnInspectorGUI(){

			GUILayout.Space(5f);
			if(_tex_logo != null){
				UniOSCUtils.DrawClickableTextureHorizontal(_tex_logo,()=>{EditorApplication.ExecuteMenuItem(UniOSCUtils.MENUITEM_EDITOR);});
			}

            EditorGUI.BeginChangeCheck();

			if (drawDefaultInspectorProp.boolValue) {
								DrawDefaultInspector ();
						}

			serializedObject.Update();
			//EditorGUI.BeginChangeCheck();

			GUILayout.BeginVertical("box");
			EditorGUILayout.PropertyField(OSCOutAddressProp,new GUIContent("OSC Out Address","") );
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			EditorGUILayout.PropertyField(UseExplicitConnectionProp,new GUIContent("Explicit Connection",UniOSCUtils.TOOLTIP_EXPLICITCONNECTION) );
			if(UseExplicitConnectionProp.boolValue)
			{
				EditorGUILayout.PropertyField(ExplicitConnectionProp,new GUIContent("Selected Connection","") );
				GUILayout.EndVertical();
			}
			else
			{
				GUILayout.EndVertical();
				GUILayout.BeginVertical("box");
				DrawIPAddress();
				DrawPort();
				GUILayout.EndVertical();
			}
			//EditorGUILayout.PropertyField(sendIntervalProp,new GUIContent("Send Interval","The interval in milliseconds to send OSC data.") );

			DrawConnectionInfo();

			serializedObject.ApplyModifiedProperties();

			if(EditorGUI.EndChangeCheck()){
				//update data (EditorUtility.SetDirty(_target) doesn't work)
                _target.ForceSetupChange(true);
				//_target.enabled = !_target.enabled;
				//_target.enabled = !_target.enabled;
			}



		}



		protected void DrawPort(){

			if(UniOSCConnection.AvailableOUTPorts.Count > 0){
				_options = UniOSCConnection.AvailableOUTPorts.Select(p=> Convert.ToString(p)).ToArray();
				_portIndex = UniOSCConnection.AvailableOUTPorts.IndexOf(OSCOutPortProp.intValue);
				_portIndex = Mathf.Max(0,_portIndex);
				_portIndex = EditorGUILayout.Popup("OSC Out Port:", _portIndex, _options);
				OSCOutPortProp.intValue = System.Convert.ToInt32(_options[_portIndex]);	
			}else{
				//No port is available
				OSCOutPortProp.intValue = 0;//??
			}
		}

		protected void DrawIPAddress(){

			if(UniOSCConnection.AvailableOUTIPAddresses.Count > 0){
				_options = UniOSCConnection.AvailableOUTIPAddresses.ToArray();
				_portIndex = UniOSCConnection.AvailableOUTIPAddresses.IndexOf(OSCOutIPAddressProp.stringValue);
				_portIndex = Mathf.Max(0,_portIndex);
				_portIndex = EditorGUILayout.Popup("OSC Out IPAddress:", _portIndex, _options);
				OSCOutIPAddressProp.stringValue = _options[_portIndex];//System.Convert.ToInt32(_options[_portIndex]);	
			}else{
				//No port is available
				OSCOutIPAddressProp.stringValue = "";//??
			}
		}

		protected void  DrawConnectionInfo(){

			if(Application.isPlaying){

				if(_myOSCConnectionsProp.arraySize == 0){
					Rect area = GUILayoutUtility.GetRect (195.0f, 40.0f);
					EditorGUI.HelpBox(area,"The Component is not connected to any OSC Connection!\nThere is no OSC Out Connection with matching Port and IPAddress!",MessageType.Warning);
				}else{
					_myOSCConnectionsProp.isExpanded=true;
					Show("Connected To:",_myOSCConnectionsProp);
				}
			}else{
				//not playing
				if(String.IsNullOrEmpty( OSCOutIPAddressProp.stringValue )){
					Rect area = GUILayoutUtility.GetRect (195.0f, 40.0f);
					EditorGUI.HelpBox(area,"Your IPAddress is empty! Please check your settings.\nThere has to be at least one OSCConnection in your project.",MessageType.Warning);
				}else{
					//EditorGUI.HelpBox(area,"",MessageType.None);
				}

			}

		}



		public static void Show (string label,SerializedProperty list) {

			EditorGUILayout.LabelField(label);
			EditorGUI.indentLevel += 1;
			if (list.isExpanded) {
				for (int i = 0; i < list.arraySize; i++) {
					UniOSCConnection con = list.GetArrayElementAtIndex(i).objectReferenceValue as UniOSCConnection;
					EditorGUILayout.LabelField(con.name);
				}
			}
			EditorGUI.indentLevel -= 1;

		}




	}
}
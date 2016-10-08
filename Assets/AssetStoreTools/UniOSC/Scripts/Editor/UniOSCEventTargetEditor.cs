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
using System.Linq;

namespace UniOSC{

	[CustomEditor(typeof(UniOSCEventTarget),true)]
	[CanEditMultipleObjects]
	public class UniOSCEventTargetEditor : Editor {

		protected UniOSCEventTarget _target;

		protected SerializedProperty ReceiveAllAddressesProp;
		protected SerializedProperty ReceiveAllPortsProp;
		protected SerializedProperty OSCAddressProp;
		protected SerializedProperty OSCPortProp;
		protected SerializedProperty FoldoutListProp;
		protected SerializedProperty AvailableINPortsProp;

		protected SerializedProperty UseExplicitConnectionProp;
		protected SerializedProperty ExplicitConnectionProp;


		protected Texture2D _tex_logo;

		protected int _portIndex = 0;
		protected string[] _options;


		public virtual void OnEnable () {
			if(target  !=_target) _target = target as UniOSCEventTarget;
			ReceiveAllAddressesProp = serializedObject.FindProperty ("_receiveAllAddresses");
			ReceiveAllPortsProp = serializedObject.FindProperty ("_receiveAllPorts");
			OSCAddressProp = serializedObject.FindProperty ("_oscAddress");
			OSCPortProp  = serializedObject.FindProperty ("_oscPort");

			UseExplicitConnectionProp = serializedObject.FindProperty("_useExplicitConnection");
			ExplicitConnectionProp = serializedObject.FindProperty("_explicitConnection");

			FoldoutListProp = serializedObject.FindProperty ("_foldoutList");

			_tex_logo = Resources.Load(UniOSCUtils.LOGO16_NAME,typeof(Texture2D)) as Texture2D;
		}


		
		override public void OnInspectorGUI(){
			GUILayout.Space(5f);
			if(_tex_logo != null){
				UniOSCUtils.DrawClickableTextureHorizontal(_tex_logo,()=>{EditorApplication.ExecuteMenuItem(UniOSCUtils.MENUITEM_EDITOR);});
			}
			//EditorGUIUtility.LookLikeControls(150f,50f);
            EditorGUIUtility.labelWidth = 150f; 
            EditorGUIUtility.fieldWidth = 50f;

			DrawDefaultInspector ();
			GUILayout.Space(5f);

			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(ReceiveAllAddressesProp,new GUIContent("Receive all Addresses","Listen to all OSC addresses. If you have performance problems it's better to specifiy the OSC address explicit and deselect this option.") );

			if(!ReceiveAllAddressesProp.boolValue){
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(OSCAddressProp,new GUIContent("OSC Address","OSC address string the component should listen to. Other addresses are skipped so in your component callback method you don't need to filter the addresses by yourself.") );
				GUILayout.EndVertical();
			}else{
				if(!Application.isPlaying){
					GUIContent gc = new GUIContent("Component listens to all addresses. Watch your performance on heavy OSC traffic.");
					Rect area =  GUILayoutUtility.GetRect(gc,GUI.skin.box);
					EditorGUI.HelpBox(area,gc.text,MessageType.Info);
				}
			}

			DrawConnectionSetup();

			DrawConnectionInfo();


			serializedObject.ApplyModifiedProperties();
			
			if(EditorGUI.EndChangeCheck()){
				//update data (EditorUtility.SetDirty(_target) doesn't work)
                _target.ForceSetupChange();
				//_target.enabled = !_target.enabled;
				//_target.enabled = !_target.enabled;
			}
		
		}

		protected void DrawConnectionSetup()
		{
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
				DrawPort();
				GUILayout.EndVertical();

			}
		}
	
		protected void DrawPort(){

			EditorGUILayout.PropertyField(ReceiveAllPortsProp,new GUIContent("Listen on all OSC Ports","If you listen on all ports you don't need to specify a port but if you feel your performance is bad you should check if it's better to deselect this option and specify the port explicit.") );
			GUILayout.Space(10);
			if(!ReceiveAllPortsProp.boolValue){

				GUILayout.Label(new GUIContent("Listening to:",""));

				if(UniOSCConnection.AvailableINPorts.Count > 0){
					_options = UniOSCConnection.AvailableINPorts.Select(p=> Convert.ToString(p)).ToArray();	
					_portIndex = UniOSCConnection.AvailableINPorts.IndexOf(OSCPortProp.intValue);
					_portIndex = Mathf.Max(0,_portIndex);
					_portIndex = EditorGUILayout.Popup("OSC Port", _portIndex, _options);
					OSCPortProp.intValue = System.Convert.ToInt32(_options[_portIndex]);
				}else{
					//??
					OSCPortProp.intValue = 0;
				}

			}else{
				//receive all ports so this value is irrelevant
				OSCPortProp.intValue = 0;
			}

		}

		protected void DrawConnectionInfo(){
				if(Application.isPlaying){

					GUILayout.BeginVertical("box");
					if(_target.ConnectToDict.Count == 0){
						GUIContent gc = new GUIContent("The component is not connected to any OSC connection! \n ");
						Rect area = GUILayoutUtility.GetRect(gc,GUI.skin.box);// GUILayoutUtility.GetRect (195.0f, 50.0f);
						EditorGUI.HelpBox(area,gc.text,MessageType.Warning);
					}else{
						ShowFoldoutConnectionStatus("Listening on:",FoldoutListProp,_target.ConnectToDict);
					}
					GUILayout.EndVertical();

				}else{
					if(ReceiveAllPortsProp.boolValue){
						GUIContent gc = new GUIContent("If you don't use a specific OSC port the component listens on all ports from all available OSC connections");
						Rect area =  GUILayoutUtility.GetRect(gc,GUI.skin.box);;//GUILayoutUtility.GetRect (195.0f, 50.0f);
						EditorGUI.HelpBox(area,gc.text,MessageType.Info);
					}
					
					
				}

		}


		protected  void ShowFoldoutConnectionStatus (string label,SerializedProperty list, IDictionary dict){

			if(String.IsNullOrEmpty(label)){
				list.isExpanded = EditorGUILayout.PropertyField(list);
			}else{
				list.isExpanded = EditorGUILayout.Foldout(list.isExpanded,label);
			}

			EditorGUI.indentLevel += 1;

			if (list.isExpanded) {
				UniOSCConnection con;
				for (int i = 0; i < dict.Count; i++) {
					con = _target.ConnectToDict.Keys.ElementAt(i);
					GUI.contentColor = con.isConnected ? Color.green: Color.red;
					EditorGUILayout.LabelField("Port:"+con.oscPort);

					for (int ii = 0; ii < _target.ConnectToDict[con].Count; ii++) {
						EditorGUI.indentLevel += 1;
						var mi = _target.ConnectToDict[con][ii];
						EditorGUILayout.LabelField("Address: "+mi.address );
						EditorGUILayout.LabelField("Mapping: Min: "+ mi.mappingMIN +" Max: "+mi.mappingMAX);
						EditorGUI.indentLevel -= 1;
					}

					GUI.contentColor = Color.white;
				}
			}

			EditorGUI.indentLevel -= 1;
		}


	}
}
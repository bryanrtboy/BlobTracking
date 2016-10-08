/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Net;
using System;

namespace UniOSC{

	[CustomEditor (typeof(UniOSCConnection))]
	[CanEditMultipleObjects]
	public class UniOSCConnectionEditor :  Editor {

		private UniOSCConnection _target;
		private Texture2D tex_logo;
		//private SerializedObject obj;

		SerializedProperty AutoConnectOSCInProp ;
		SerializedProperty OSCPortProp ;
		SerializedProperty OSCMappingFileObjListProp ;
		SerializedProperty OSCSessionFileObjListProp ;
		SerializedProperty AutoConnectOSCOutProp ;
		SerializedProperty OSCOutPortProp;
		SerializedProperty OSCOutIPAddressProp; 
		SerializedProperty FoldoutOSCOutProp; 
		SerializedProperty FoldoutOSCInProp;

        SerializedProperty TransmissionTypeInProp;
        SerializedProperty OSCMulticastIPAddressProp;
        SerializedProperty OSCOutMulticastIPAddressProp;
        SerializedProperty TransmissionTypeOutProp;

		public static Texture2D texTestMessage;
		public static Texture2D texON;
		public static Texture2D texOFF;

        protected string[] _TransmissionTypes;
        protected int _TransmissionTypeIndex = 0;
        protected string _oldOSCMulticastIPAddress;
        protected string _currOSCMulticastIPAddress;
        protected bool _isValidOSCMulticastIPAddress;

        protected string[] _TransmissionTypesOut;
        protected int _TransmissionTypeIndexOut = 0;
        protected string _oldOSCMulticastIPAddressOut;
        protected string _currOSCMulticastIPAddressOut;
        protected bool _isValidOSCMulticastIPAddressOut;

		void OnEnable () {

			if(target  !=_target) _target = target as UniOSCConnection;

			serializedObject.Update();
			tex_logo = Resources.Load(UniOSCUtils.LOGO16_NAME,typeof(Texture2D)) as Texture2D;
			LoadTextures();
		
			AutoConnectOSCInProp = serializedObject.FindProperty ("autoConnectOSCIn");
			AutoConnectOSCOutProp = serializedObject.FindProperty ("autoConnectOSCOut");
			OSCPortProp = serializedObject.FindProperty ("_oscPort");
			OSCMappingFileObjListProp = serializedObject.FindProperty("oscMappingFileObjList");
			OSCSessionFileObjListProp = serializedObject.FindProperty("oscSessionFileObjList");
			OSCOutPortProp = serializedObject.FindProperty("_oscOutPort");
            OSCOutIPAddressProp = serializedObject.FindProperty("_oscOutIPAddress");
			FoldoutOSCOutProp = serializedObject.FindProperty("foldoutOSCOut");
			FoldoutOSCInProp = serializedObject.FindProperty("foldoutOSCIn");

            TransmissionTypeInProp = serializedObject.FindProperty("_transmissionTypeIn");
            OSCMulticastIPAddressProp = serializedObject.FindProperty("_oscMulticastIPAddress");
            OSCOutMulticastIPAddressProp = serializedObject.FindProperty("_oscOutMulticastIPAddress");
            TransmissionTypeOutProp = serializedObject.FindProperty("_transmissionTypeOut");

            _TransmissionTypes = new string[TransmissionTypeInProp.enumNames.Length-2];

            Array.Copy(TransmissionTypeInProp.enumNames, _TransmissionTypes, _TransmissionTypes.Length);

            _TransmissionTypesOut = new string[TransmissionTypeInProp.enumNames.Length - 1];
            Array.Copy(TransmissionTypeInProp.enumNames, _TransmissionTypesOut, _TransmissionTypesOut.Length);
           // _TransmissionTypes = TransmissionTypeOutProp.enumNames;

			serializedObject.ApplyModifiedProperties();
		}
		public static void LoadTextures(){
			if(texTestMessage == null) texTestMessage = Resources.Load(UniOSCUtils.OSCOUTTEST_NAME,typeof(Texture2D)) as Texture2D;
			if(texON == null) texON = Resources.Load(UniOSCUtils.OSCCONNECTION_ON_NAME,typeof(Texture2D)) as Texture2D;
			if(texOFF == null) texOFF = Resources.Load(UniOSCUtils.OSCCONNECTION_OFF_NAME,typeof(Texture2D)) as Texture2D;
		}
	
		public static void Show (string label,SerializedProperty list) {

			if(String.IsNullOrEmpty(label)){
				EditorGUILayout.PropertyField(list);
			}else{
				list.isExpanded = EditorGUILayout.Foldout(list.isExpanded,label);
			}

			EditorGUI.indentLevel += 1;
			if (list.isExpanded) {
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
				for (int i = 0; i < list.arraySize; i++) {
					EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
				}
			}
			EditorGUI.indentLevel -= 1;
		}


		override public void OnInspectorGUI(){
			GUILayout.Space(5);
			if(tex_logo != null){
				UniOSCUtils.DrawClickableTextureHorizontal(tex_logo,()=>{EditorApplication.ExecuteMenuItem(UniOSCUtils.MENUITEM_EDITOR);});
			}

			serializedObject.Update();

          
			EditorGUI.BeginChangeCheck();
           

			FoldoutOSCInProp.boolValue = EditorGUILayout.Foldout(FoldoutOSCInProp.boolValue,"OSC IN");
			if(FoldoutOSCInProp.boolValue){
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(AutoConnectOSCInProp,new GUIContent("Auto connect on start","") );
				EditorGUILayout.PropertyField(OSCPortProp,new GUIContent("Port:"));
				OSCPortProp.intValue = Mathf.Min(UniOSCUtils.MAXPORT,OSCPortProp.intValue);
              
                _TransmissionTypeIndex = TransmissionTypeInProp.enumValueIndex;
                _TransmissionTypeIndex = EditorGUILayout.Popup("TransmissionType", _TransmissionTypeIndex, _TransmissionTypes);
                TransmissionTypeInProp.enumValueIndex = _TransmissionTypeIndex;                            
               
                GUI.backgroundColor = _target.hasValidOscIPAddress ? Color.green : Color.red;
           
                if (TransmissionTypeInProp.enumValueIndex == (int)OSCsharp.Net.TransmissionType.Multicast)
                {
                    EditorGUI.BeginChangeCheck();                 
                    EditorGUILayout.PropertyField(OSCMulticastIPAddressProp, new GUIContent("Multicast IP address", ""));
                    EditorGUILayout.HelpBox("A valid multicast IP address is in the range between 224.0.0.0 and 239.255.255.255 !", MessageType.Warning);
                    if (EditorGUI.EndChangeCheck())
                    {                       
                        serializedObject.ApplyModifiedProperties();                     
                        _target.ValidateOscInIPAddress();
                    }
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                    GUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField(new GUIContent("Local IP address     ", ""), GUILayout.MaxWidth(115));//
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(new GUIContent(UniOSCConnection.localIPAddress, ""), GUILayout.MaxWidth(100));//
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                 GUI.backgroundColor = Color.white;

				GUILayout.EndVertical();
			}

            if (EditorGUI.EndChangeCheck())
            {
               // Debug.Log("CHANGE");
                serializedObject.ApplyModifiedProperties();
                UniOSCConnection.Update_AvailableOSCSettings();
                _target.Force_SetupChanged_IN();          
            }

            EditorGUI.BeginChangeCheck();

			FoldoutOSCOutProp.boolValue = EditorGUILayout.Foldout(FoldoutOSCOutProp.boolValue,"OSC OUT");
			if(FoldoutOSCOutProp.boolValue){
				GUILayout.BeginVertical("box");
				EditorGUILayout.PropertyField(AutoConnectOSCOutProp,new GUIContent("Auto connect on start","") );
				EditorGUILayout.PropertyField(OSCOutPortProp,new GUIContent("Port","") );
				OSCOutPortProp.intValue = Mathf.Min(UniOSCUtils.MAXPORT,OSCOutPortProp.intValue);

               // _TransmissionTypes = TransmissionTypeOutProp.enumNames;
                _TransmissionTypeIndexOut = TransmissionTypeOutProp.enumValueIndex;
                _TransmissionTypeIndexOut = EditorGUILayout.Popup("TransmissionType", _TransmissionTypeIndexOut, _TransmissionTypesOut);
                TransmissionTypeOutProp.enumValueIndex = _TransmissionTypeIndexOut;
                           
               
                GUI.backgroundColor = _target.hasValidOscOutIPAddress ? Color.green : Color.red;
                EditorGUI.BeginChangeCheck();   
                switch (TransmissionTypeOutProp.enumValueIndex)
                {
                    case (int)OSCsharp.Net.TransmissionType.Unicast:
                        EditorGUILayout.PropertyField(OSCOutIPAddressProp, new GUIContent("Target IP address", ""));                    
                        break;
                    case (int)OSCsharp.Net.TransmissionType.Multicast:                                                            
                        EditorGUILayout.PropertyField(OSCOutMulticastIPAddressProp, new GUIContent("Multicast IP address", ""));
                        EditorGUILayout.HelpBox("A valid multicast IP address is in the range between 224.0.0.0 and 239.255.255.255 !", MessageType.Warning);
                        break;
                    case (int)OSCsharp.Net.TransmissionType.Broadcast:
                    case (int)OSCsharp.Net.TransmissionType.LocalBroadcast:
                        GUI.backgroundColor = Color.white;
                        GUILayout.BeginHorizontal("box");                     
                        EditorGUILayout.LabelField(new GUIContent("Target IP address" , ""),GUILayout.MaxWidth(115));
                        GUILayout.FlexibleSpace();                
                        EditorGUILayout.LabelField(new GUIContent( IPAddress.Broadcast.ToString(), ""),GUILayout.MaxWidth(100));
                         GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        break;

                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    _target.ValidateOscOutIPAddress(); 
                }

                GUI.backgroundColor = Color.white;
			
				GUILayout.EndVertical();
			}

			GUILayout.Space(10);

			if(EditorGUI.EndChangeCheck()) {
               // Debug.Log("CHANGE");
				serializedObject.ApplyModifiedProperties();
				UniOSCConnection.Update_AvailableOSCSettings();
                _target.Force_SetupChanged_OUT();                         
			}


			ShowOSCReciverStatus(_target);
			Show("Mapping Files",OSCMappingFileObjListProp);
			Show("Session Files",OSCSessionFileObjListProp);

			if(_target.hasOSCSessionFileAttached){
				//EditorGUILayout.PropertyField(AutoConnectOSCInProp,new GUIContent("Auto connect on start","") );
				if(GUILayout.Button(new GUIContent("Send Session Data","Send the last OSC data that are recorded with your session files."),GUILayout.Width(150f)) ){
					_target.SendSessionData();
				}
				if(!_target.isConnectedOut){
					EditorGUILayout.HelpBox("To send the session data you have to turn on OSC OUT!",MessageType.Warning);					
				}
			}

			serializedObject.ApplyModifiedProperties();

			if (GUI.changed) {
				EditorUtility.SetDirty (_target);
			}

		}

		protected void ForceUpdate(){

		}

		private static void _DropArea (Event evt, Rect area,UniOSCConnection oscConnection)
			{
				switch (evt.type) {

				case EventType.MouseDown :
					if (area.Contains(evt.mousePosition)) {
						EditorGUIUtility.PingObject(oscConnection);
						Selection.activeObject = oscConnection;
					}
					break;

				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!area.Contains (evt.mousePosition))return;
					
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					
					if (evt.type == EventType.DragPerform) {
						DragAndDrop.AcceptDrag ();
						foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences) {
							
							if(dragged_object.GetType() == typeof(UniOSCMappingFileObj) ){

								if(!oscConnection.oscMappingFileObjList.Contains((UniOSCMappingFileObj)dragged_object) ){
									oscConnection.oscMappingFileObjList.Add((UniOSCMappingFileObj)dragged_object);
								}
							}

							if(dragged_object.GetType() == typeof(UniOSCSessionFileObj) ){
								
								if(!oscConnection.oscSessionFileObjList.Contains((UniOSCSessionFileObj)dragged_object) ){
									oscConnection.oscSessionFileObjList.Add((UniOSCSessionFileObj)dragged_object);
								}
							}


						}//foreach
					}//DragPerform
					break;
				}//switch
			}


		public static void ShowOSCReciverStatus(UniOSCConnection oscConnection){

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			
			Event evt = Event.current;

			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.padding = new RectOffset(0,0,0,0);
			style.border = new RectOffset(0,0,0,0);

			#region IN

			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(200));

			Rect area = GUILayoutUtility.GetRect (195.0f, 40.0f);
			area.width*=1f;
			Rect r1 = GUILayoutUtility.GetRect (195.0f, 20.0f);
			r1.width*=1f;


			int btnsize1 = 20;
			Rect r1b = new Rect(area);
			r1b.x+= r1b.width-(btnsize1*1);
			r1b.y+= r1b.height-btnsize1;
			r1b.width = btnsize1*1;
			r1b.height= btnsize1;


			if(oscConnection.isConnected){
				GUI.contentColor = Color.white;				
				GUI.backgroundColor = oscConnection.dispatchOSC ? UniOSCUtils.CONNECTION_ON_COLOR : UniOSCUtils.CONNECTION_PAUSE_COLOR;
				//EditorGUI.HelpBox(area, "OSC IN: " + UniOSCConnection.localIPAddress + "\nPort: "+ oscConnection.oscPort + "\nListening", MessageType.Info);
                EditorGUI.HelpBox(area, "OSC IN: " + oscConnection.oscInIPAddress + "\nPort: " + oscConnection.oscPort + "\nListening", MessageType.Info);
                
				if (GUI.Button (r1,"Disconnect")){oscConnection.DisconnectOSC();}

				GUI.backgroundColor = Color.white;
				GUI.contentColor = Color.white;
				//if (GUI.Button (r1b,new GUIContent(tex2,""),style ) ){oscConnection.SendTestMessage();}
				Texture2D currTex = oscConnection.dispatchOSC ?  texON : texOFF ;
				oscConnection.dispatchOSC = GUI.Toggle(r1b,oscConnection.dispatchOSC,new GUIContent(currTex,"Turn the OSC dispatching into Unity on/off"),style );

			}else{
				GUI.contentColor = Color.white;
				GUI.backgroundColor = UniOSCUtils.CONNECTION_BG;
				//GUI.Box(area,"");//GUI.skin.box
				GUI.backgroundColor = UniOSCUtils.CONNECTION_OFF_COLOR;
				//EditorGUI.HelpBox(area, "OSC IN: "+ UniOSCConnection.localIPAddress + "\nPort: " + oscConnection.oscPort + "\nNot listening", MessageType.Warning);
                EditorGUI.HelpBox(area, "OSC IN: " +oscConnection.oscInIPAddress + "\nPort: " + oscConnection.oscPort + "\nNot listening", MessageType.Warning);
				if (GUI.Button (r1,"Connect")){
                    if (oscConnection.hasValidOscIPAddress)                   
                    {
                        oscConnection.ConnectOSC();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid IP Address", "The IPAddress you have choosen is not valid! Please use a different.", "OK");
                    }
                  
                }
			}

			_DropArea(evt, area, oscConnection);

			EditorGUILayout.EndVertical();

			#endregion IN




			if(oscConnection.oscOut){

			GUILayout.Space(5f);
			//GUILayout.FlexibleSpace();

			//OUT-------------------------

			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(200f));

			Rect area_out = GUILayoutUtility.GetRect (100.0f, 40.0f);//, GUILayout.ExpandWidth (true)
			area_out.width*=1f;//0.95f;

			Rect r2 = GUILayoutUtility.GetRect (100.0f, 20.0f);
			r2.width*=1f;//0.95f;
				int btnsize = 20;
				Rect r2b = new Rect(area_out);
				r2b.x+= r2b.width-(btnsize*1);
				r2b.y+= r2b.height-btnsize;
				r2b.width = btnsize*1;
				r2b.height= btnsize;


			if(oscConnection.isConnectedOut){
				GUI.contentColor = Color.white;
				GUI.backgroundColor = oscConnection.dispatchOSCOut ? UniOSCUtils.CONNECTION_ON_COLOR : UniOSCUtils.CONNECTION_PAUSE_COLOR;
				EditorGUI.HelpBox(area_out,"OSC OUT: "+oscConnection.oscOutIPAddress+"\nPort: "+oscConnection.oscOutPort+"\nIs sending",MessageType.Info);//oscConnection.name+
			
				if (GUI.Button (r2,"Disconnect")){oscConnection.DisconnectOSCOut();}

					GUI.backgroundColor = Color.white;
					GUI.contentColor = Color.white;

					//if ( GUI.Button (r2b,new GUIContent(texTestMessage,""),style ) ){oscConnection.SendTestMessage();}

					Texture2D currTex = oscConnection.dispatchOSCOut ?  texON : texOFF ;
					oscConnection.dispatchOSCOut = GUI.Toggle(r2b,oscConnection.dispatchOSCOut,new GUIContent(currTex,"Turn the OSC sending from Unity on/off without start/stop the network resources"),style );


				}else{
					GUI.backgroundColor = Color.red;
					GUI.contentColor = Color.white;
					EditorGUI.HelpBox(area_out,"OSC OUT: "+oscConnection.oscOutIPAddress+"\nPort: "+oscConnection.oscOutPort+"\nNot sending",MessageType.Warning);//oscConnection.name+
				
					if (GUI.Button (r2,"Connect")){
                        if(oscConnection.hasValidOscOutIPAddress)					
                        {
							oscConnection.ConnectOSCOut();
						}else{
							 EditorUtility.DisplayDialog("Invalid IP Address", "The IPAddress you have choosen is not valid! Please use a different.", "OK");

						}
						
				    }
			}
				
			_DropArea(evt,area_out,oscConnection);
			
			EditorGUILayout.EndVertical();
			//OUT--------------------------------


			}//if(oscConnection.OSCOut){

			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;


		
			EditorGUILayout.EndHorizontal();

			if(EditorGUI.EndChangeCheck()){
				EditorUtility.SetDirty(oscConnection);
			}
		}


	}
}
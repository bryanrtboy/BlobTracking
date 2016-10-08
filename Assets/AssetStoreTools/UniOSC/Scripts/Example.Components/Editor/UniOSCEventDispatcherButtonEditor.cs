/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UniOSC{

	/// <summary>
	/// Uni OSC event dispatcher button editor.
	/// </summary>
	[CustomEditor(typeof(UniOSCEventDispatcherButton))]
	[CanEditMultipleObjects]
	public class UniOSCEventDispatcherButtonEditor : UniOSCEventDispatcherEditor {

		protected SerializedProperty downOSCDataValueProp;
		protected SerializedProperty upOSCDataValueProp;
		protected SerializedProperty ShowGUIProp;
		protected SerializedProperty xProp;
		protected SerializedProperty yProp;


		override public void OnEnable () {
			base.OnEnable();
			downOSCDataValueProp = serializedObject.FindProperty("downOSCDataValue");
			upOSCDataValueProp = serializedObject.FindProperty("upOSCDataValue");
			ShowGUIProp = serializedObject.FindProperty("showGUI");
			xProp = serializedObject.FindProperty("xPos");
			yProp = serializedObject.FindProperty("yPos");
		}

		override public void OnInspectorGUI(){

			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			drawDefaultInspectorProp.boolValue = false;//drawDefaultInspectorProp is defined in the base editor
			base.OnInspectorGUI();

			EditorGUILayout.PropertyField(downOSCDataValueProp,new GUIContent("Down data value","OSC data value that is send when button is pushed. Should be normally 1") );
			EditorGUILayout.PropertyField(upOSCDataValueProp,new GUIContent("Up data value","OSC data value that is send when button is released. Should be normally 0") );

			EditorGUILayout.PropertyField(ShowGUIProp,new GUIContent("Show GUI","") );

			if(ShowGUIProp.boolValue){

				EditorGUILayout.LabelField(new GUIContent("Pos",""));

				EditorGUILayout.Slider(xProp,0f,1f,new GUIContent("x",""));
				EditorGUILayout.Slider(yProp,0f,1f,new GUIContent("y",""));
				xProp.floatValue = Mathf.Clamp( xProp.floatValue,0,1);
				yProp.floatValue = Mathf.Clamp( yProp.floatValue,0,1);
			}

		

			serializedObject.ApplyModifiedProperties();

			if(EditorGUI.EndChangeCheck()){
				//update data (EditorUtility.SetDirty(_target) doesn't work)
                _target.ForceSetupChange(true);
				//_target.enabled = !_target.enabled;
				//_target.enabled = !_target.enabled;
			}

		}

	}
}

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

	[CustomEditor(typeof(UniOSCToggle),true)]
	[CanEditMultipleObjects]
	public class UniOSCToggleEditor : UniOSCEventTargetEditor {

		protected  UniOSCToggle _targetToggle;

		protected SerializedProperty ComponentToToggleProp;
		protected SerializedProperty ToggleStateProp;

		protected int _componentIndex = 0;

		protected bool _updateFlag;


		public override void OnEnable () {
			base.OnEnable();

			if(target  !=_targetToggle) _targetToggle = target as UniOSCToggle;

			ComponentToToggleProp = serializedObject.FindProperty ("componentToToggle");
			ToggleStateProp = serializedObject.FindProperty ("toggleState");
		}


		
		override public void OnInspectorGUI(){
			GUILayout.Space(5f);
			if(_tex_logo != null){
				UniOSCUtils.DrawClickableTextureHorizontal(_tex_logo,()=>{EditorApplication.ExecuteMenuItem(UniOSCUtils.MENUITEM_EDITOR);});
			}
			//EditorGUIUtility.LookLikeControls(150f,50f);
            EditorGUIUtility.labelWidth =  150f;
            EditorGUIUtility.fieldWidth =  50f;

			DrawDefaultInspector ();
			GUILayout.Space(5f);

			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			#region component
			List<Component> comps = _targetToggle.gameObject.GetComponents<Component>().ToList();
			comps.Remove(comps.Find(c => c.GetType() == _targetToggle.GetType()));//security????
			_options = new string[comps.Count];
			for(int i = 0;i<comps.Count;i++){
				_options[i] = comps[i].GetType().ToString();
			}
		
			_componentIndex = comps.FindIndex(c => c.GetType() == _targetToggle.componentToToggle.GetType()); 

			if(_componentIndex < 0){
				//the current component was probably removed so we have to update our gameobject to prevent nasty exceptions (_compType)
				_updateFlag = true;
			}
			_componentIndex = Mathf.Max(0,_componentIndex);
			_componentIndex = EditorGUILayout.Popup("componentToToggle", _componentIndex, _options);
			_targetToggle.componentToToggle = comps[_componentIndex];
			#endregion component

			ToggleStateProp.boolValue = GUILayout.Toggle(ToggleStateProp.boolValue,new GUIContent("Toggle State",""));//,GUILayout.Width(100)

			EditorGUILayout.PropertyField(OSCAddressProp,new GUIContent("OSC Address","") );


			DrawConnectionSetup();

			DrawConnectionInfo();

			serializedObject.ApplyModifiedProperties();
			
			if(EditorGUI.EndChangeCheck() || _updateFlag){
				//update data (EditorUtility.SetDirty(_target) doesn't work)
				ForceUpdate();
			}

		
		}

		protected void ForceUpdate(){
			_targetToggle.enabled = !_targetToggle.enabled;
			_targetToggle.enabled = !_targetToggle.enabled;
			_updateFlag = false;
		}


	}
}
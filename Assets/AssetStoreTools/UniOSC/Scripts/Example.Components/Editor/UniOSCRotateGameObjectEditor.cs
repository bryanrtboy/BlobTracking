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

	[CustomEditor(typeof(UniOSCRotateGameObject))]
	[CanEditMultipleObjects]
	public class UniOSCRotateGameObjectEditor : UniOSCEventTargetEditor {

		override public void OnInspectorGUI(){
			GUILayout.Space(5f);
			if(_tex_logo != null){
				UniOSCUtils.DrawClickableTextureHorizontal(_tex_logo,()=>{EditorApplication.ExecuteMenuItem(UniOSCUtils.MENUITEM_EDITOR);});
			}

			//EditorGUIUtility.LookLikeControls(150f,100f);
            EditorGUIUtility.labelWidth =  150f;
            EditorGUIUtility.fieldWidth =  100f;

			DrawDefaultInspector ();
			GUILayout.Space(5f);
			
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			DrawConnectionSetup();

			DrawConnectionInfo();

			serializedObject.ApplyModifiedProperties();


			if(EditorGUI.EndChangeCheck()){
				_target.enabled = !_target.enabled;
				_target.enabled = !_target.enabled;
			}


		}

	}
}

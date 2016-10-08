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

	[CustomEditor(typeof(UniOSCChangeColor))]
	[CanEditMultipleObjects]
	public class UniOSCChangeColorEditor : UniOSCEventTargetEditor {

		override public void OnInspectorGUI(){
			GUILayout.Space(5);
			if(_tex_logo != null){
				UniOSCUtils.DrawClickableTextureHorizontal(_tex_logo,()=>{EditorApplication.ExecuteMenuItem(UniOSCUtils.MENUITEM_EDITOR);});
			}

			//EditorGUIUtility.LookLikeControls(150f,100f);
            EditorGUIUtility.labelWidth =  150f;
            EditorGUIUtility.fieldWidth =  100f;

			EditorGUI.BeginChangeCheck();
			DrawDefaultInspector ();
			GUILayout.Space(5f);
			
			serializedObject.Update();

			DrawConnectionSetup();
			//DrawPort();
			DrawConnectionInfo();

			serializedObject.ApplyModifiedProperties();

			if(EditorGUI.EndChangeCheck()){
				_target.enabled = !_target.enabled;
				_target.enabled = !_target.enabled;
			}


		}

	}
}

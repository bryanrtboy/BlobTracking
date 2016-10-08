/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace UniOSC{

	[CustomEditor (typeof(UniOSCSessionItem))]
	public class UniOSCSessionItemEditor : Editor {


		public void OnEnable(){

		}

		public static void OnGUI_Editor(UniOSCSessionItem obj){

			EditorGUI.BeginChangeCheck();
		
			EditorGUILayout.BeginHorizontal ();

			#region address
			obj.address = 		EditorGUILayout.TextField(new GUIContent("","Tooltip for Address"),obj.address,GUILayout.Width(UniOSCUtils.SESSIONLISTLABELRECTS[0].width) );
			#endregion address

			#region Learn
			Rect r = GUILayoutUtility.GetRect(UniOSCUtils.SESSIONLISTLABELRECTS[1].width, UniOSCUtils.SESSIONLISTLABELRECTS[1].height,GUI.skin.button, GUILayout.ExpandWidth(false));
			r.width *= 0.9f;
			if(obj.hostObj.IsLearning){
				GUIStyle gs = new GUIStyle(GUI.skin.button);
				gs.active.textColor =UniOSCUtils.LEARN_COLOR_ON  ;
				gs.normal.textColor = UniOSCUtils.LEARN_COLOR_OFF;

				if(GUI.RepeatButton(r,new GUIContent("Learn","Press to Learn OSC"),gs)){
					obj.isLearning = true;
				}else{
					obj.isLearning = false;
				}

			}
			#endregion Learn

			#region Delete
			if ( GUILayout.Button ("Delete",GUILayout.Width(UniOSCUtils.SESSIONLISTLABELRECTS[2].width),GUILayout.Height(UniOSCUtils.SESSIONLISTLABELRECTS[2].height) ) ){
				bool deleteItem = EditorUtility.DisplayDialog("Delete", "Do you want to delete this OSC Session Item with the Address:\n"+obj.address+" ?", "OK", "Cancel");
				if(deleteItem)obj.OnOSCSessionItemDelete();
			}
			#endregion Delete

			#region Values
			for(int i = 0; i<obj.data.Count;i++){
				System.Object d = obj.data[i];
				//if(d is float ) {
				EditorGUILayout.LabelField(d.ToString(),GUILayout.Width(UniOSCUtils.SESSIONLISTLABELRECTS[3].width));
				//}
			}//for

			#endregion Values

			EditorGUILayout.EndHorizontal ();

			if(EditorGUI.EndChangeCheck()){
				EditorUtility.SetDirty(obj.hostObj);
			}
		}

		public static void OnGUI_Inspector(UniOSCSessionItem obj){

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginVertical(GUI.skin.box,  GUILayout.Width(UniOSCMappingItem.MAXWIDTH),GUILayout.Height(obj.collapsed ? UniOSCMappingItem.MAXHEIGTH : 10f) );
			//EditorGUIUtility.LookLikeControls(UniOSCMappingItem.MAXWIDTH *0.35f,UniOSCMappingItem.MAXWIDTH *0.55f);
			EditorGUIUtility.labelWidth = UniOSCMappingItem.MAXWIDTH *0.35f;
			EditorGUIUtility.fieldWidth = UniOSCMappingItem.MAXWIDTH *0.55f;

			#region IN
			if(obj.hostObj.IsLearning)obj.collapsed = true;

			EditorGUILayout.BeginHorizontal ();

			obj.collapsed = EditorGUILayout.Foldout(obj.collapsed, "OSC Mapping Item "+ (obj.collapsed ? "" : obj.address) );
			EditorGUILayout.EndHorizontal ();
			#endregion IN

			if (obj.collapsed)
			{
				obj.address = EditorGUILayout.TextField(new GUIContent("OSC Address:","Tooltip for Address"),obj.address);
		
				#region Data

				EditorGUILayout.BeginVertical();
				for(int i = 0; i<4;i++){
				//for(int i = 0; i<obj.data.Count;i++){
					System.Object d = obj.data.Count > i ? obj.data[i] : "";
					EditorGUILayout.LabelField(String.Format("Data[{0}]:",i),d.ToString());//GUILayout.Width(UniOSCUtils.MAPPINGLISTLABELRECTS[3].width)
					//}
				}//for
				EditorGUILayout.EndVertical();

				#endregion Data

				GUILayout.Space(10f);

				#region Bottom
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.BeginVertical(GUILayout.Height(40f));

				#region Delete
				EditorGUILayout.BeginHorizontal();
				if ( GUILayout.Button ("Delete" ,GUILayout.Width(100)) ){
					bool deleteDispatcher = EditorUtility.DisplayDialog("Delete", "Do you want to delete this OSC Session Item with the Address:\n"+obj.address+" ?", "OK", "Cancel");
					if(deleteDispatcher)obj.OnOSCSessionItemDelete();
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				#endregion Delete

				EditorGUILayout.EndVertical();

				#region Learn
				
				if(obj.hostObj.IsLearning){
					EditorGUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace();
					
					if(obj.hostObj.IsLearning){
						if(GUILayout.RepeatButton(new GUIContent("Learn","Press to Learn OSC"),GUILayout.Width(80f),GUILayout.Height(40f) )  ){
							obj.isLearning = true;
						}else{
							obj.isLearning = false;
						}
						
					}
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
				}
				
				#endregion Learn
			

				EditorGUILayout.EndHorizontal ();
				#endregion Bottom

			}//collapsed

			EditorGUILayout.EndVertical();

			if(EditorGUI.EndChangeCheck()){
				EditorUtility.SetDirty(obj.hostObj);
			}

		}

	}
}
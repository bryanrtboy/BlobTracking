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

	[CustomEditor (typeof(UniOSCMappingItem))]
	public class UniOSCMappingItemEditor : Editor {


		public void OnEnable(){

		}

		public static void OnGUI_Editor(UniOSCMappingItem obj){

			EditorGUI.BeginChangeCheck();
		
			EditorGUILayout.BeginHorizontal ();

			#region Values
			obj.address = 		EditorGUILayout.TextField(new GUIContent("","Tooltip for Address"),obj.address,GUILayout.Width(UniOSCUtils.MAPPINGLISTLABELRECTS[0].width) );
			obj.min =  			EditorGUILayout.FloatField(new GUIContent("","Minimal value"),obj.min,GUILayout.Width(UniOSCUtils.MAPPINGLISTLABELRECTS[1].width));
			obj.max =  			EditorGUILayout.FloatField(new GUIContent("","Maximal value"),obj.max,GUILayout.Width(UniOSCUtils.MAPPINGLISTLABELRECTS[2].width));
			obj.mappingMIN =  	EditorGUILayout.FloatField(new GUIContent("","Minimal Mapping value"),obj.mappingMIN,GUILayout.Width(UniOSCUtils.MAPPINGLISTLABELRECTS[3].width));
			obj.mappingMAX =  	EditorGUILayout.FloatField(new GUIContent("","Maximal Mapping value"),obj.mappingMAX,GUILayout.Width(UniOSCUtils.MAPPINGLISTLABELRECTS[4].width));
			#endregion Values

			#region Learn
			Rect r = GUILayoutUtility.GetRect(UniOSCUtils.MAPPINGLISTLABELRECTS[5].width, UniOSCUtils.MAPPINGLISTLABELRECTS[5].height,GUI.skin.button, GUILayout.ExpandWidth(false));
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
			if ( GUILayout.Button ("Delete",GUILayout.Width(UniOSCUtils.MAPPINGLISTLABELRECTS[6].width),GUILayout.Height(UniOSCUtils.MAPPINGLISTLABELRECTS[6].height) ) ){
				bool deleteItem = EditorUtility.DisplayDialog("Delete", "Do you want to delete this OSCMappingItem with the Address:\n"+obj.address+" ?", "OK", "Cancel");
				if(deleteItem)obj.OnOSCMappingItemDelete();
			}
			#endregion Delete

			EditorGUILayout.EndHorizontal ();

			if(EditorGUI.EndChangeCheck()){
				EditorUtility.SetDirty(obj.hostObj);
			}
		}

		public static void OnGUI_Inspector(UniOSCMappingItem obj){

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
		
				#region DataMinMax
				GUILayout.Label(new GUIContent("Data Range:","The range where the incomming date is in. Most of the times this is in normalized range of 0-1."));
				
				EditorGUILayout.BeginHorizontal();
				//EditorGUIUtility.LookLikeControls(40f,50f);
				EditorGUIUtility.labelWidth =40f;
				EditorGUIUtility.fieldWidth = 50f;
				
				obj.min =  EditorGUILayout.FloatField(new GUIContent("Min:","Minimal value"),obj.min);
				GUILayout.FlexibleSpace();
				obj.max =  EditorGUILayout.FloatField(new GUIContent("Max:","Maximal value"),obj.max);
				EditorGUILayout.EndHorizontal();
				#endregion DataMinMax

				#region DataMapping
				GUILayout.Label(new GUIContent("Data Mapping:","The incomming data from the OSC Message is changed and mapped from the min-max data range to the min-max mapping range.If you don't want any mapping these values should match with the 'Data Range' above."));

				EditorGUILayout.BeginHorizontal();
				//EditorGUIUtility.LookLikeControls(40f,50f);
				EditorGUIUtility.labelWidth = 40f;
				EditorGUIUtility.fieldWidth = 50f;

				obj.mappingMIN =  EditorGUILayout.FloatField(new GUIContent("Min:","Minimal Mapping value"),obj.mappingMIN);
				GUILayout.FlexibleSpace();
				obj.mappingMAX =  EditorGUILayout.FloatField(new GUIContent("Max:","Maximal Mapping value"),obj.mappingMAX);
				EditorGUILayout.EndHorizontal();
				#endregion DataMapping

				GUILayout.Space(10f);

				#region Bottom
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.BeginVertical(GUILayout.Height(40f));

				#region Delete
				EditorGUILayout.BeginHorizontal();
				if ( GUILayout.Button ("Delete" ,GUILayout.Width(100)) ){
					bool deleteDispatcher = EditorUtility.DisplayDialog("Delete", "Do you want to delete this OSCMappingItem with the Address:\n"+obj.address+" ?", "OK", "Cancel");
					if(deleteDispatcher)obj.OnOSCMappingItemDelete();
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
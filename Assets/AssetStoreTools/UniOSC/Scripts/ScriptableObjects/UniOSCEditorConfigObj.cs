/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections.Generic;
using System;

namespace UniOSC{

	/// <summary>
	/// UniOSC editor config object. Storage of all the UniOSCEditor settings
	/// </summary>
	[Serializable]
	public class UniOSCEditorConfigObj : ScriptableObject {
		#region public

		public int selectedMappingFileObjIndex;
		public int selectedSessionFileObjIndex;
		public Vector2 configTraceScrollpos;
		public bool isOSCTracing;
		public bool isOSCLearning;
		public bool isEditorEnabled;
		public bool isLastMessageTracing;
		public GUISkin mySkin;
		public GUIStyle learnStyle;
		public List<UniOSCMappingFileObj> OSCMappingFileObjList;
		public List<UniOSCSessionFileObj> OSCSessionFileObjList;
		public int toolbarInt = 0;


		[SerializeField]
		public Texture2D tex_LearnFrame;
		[SerializeField]
		public Texture2D tex_logo;

		#endregion public

		public void OnEnable() {
		
			if(tex_LearnFrame == null) tex_LearnFrame = Resources.Load("border.5px.64",typeof(Texture2D)) as Texture2D;
		
			if(tex_logo == null) tex_logo = Resources.Load(UniOSCUtils.LOGO32_NAME,typeof(Texture2D)) as Texture2D;
		
			if(learnStyle == null) learnStyle = new GUIStyle();

			learnStyle.border = new RectOffset(7,7,7,7);
			learnStyle.normal.background = tex_LearnFrame;
			
			if(OSCMappingFileObjList == null) OSCMappingFileObjList = new List<UniOSCMappingFileObj>();
			if(OSCSessionFileObjList == null) OSCSessionFileObjList = new List<UniOSCSessionFileObj>();
		}


	}
}

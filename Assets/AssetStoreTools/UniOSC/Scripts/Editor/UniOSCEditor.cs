/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using OSCsharp.Data;
using System.Linq;

namespace UniOSC{

	/// <summary>
	/// Editor for the administration of OSCconnections, mapping files.
	/// You can also trace the OSC data flow .
	/// </summary>
	[Serializable]
	public class UniOSCEditor : EditorWindow {

		private RecompileClass _recompile;

		///<summary>
		/// Just a dummy class to check if we recompile the scripts
		/// </summary>
		private class RecompileClass
		{ 
		}

		#region public



		public static bool isOSCLearning{get{return _config.isOSCLearning;}}

		public static UniOSCEditor Instance { get; private set; }
		
		public static bool IsOpen {
			get { return Instance != null; }
		}
		#endregion public

		#region private
		//private string myLabel1, myLabel2;

		//private Rect r1;
		#endregion private

		#region static
		static List<UniOSCConnection>  _osConnectionList = new List<UniOSCConnection>();
		static List<int>  _osConnectionIDList = new List<int>();
		static EditorWindow _windowSelf;
		static UniOSCEditorConfigObj _config;

		static StringBuilder _osctraceStrb= new StringBuilder();
		static string _oscTraceStr ="";
		//static OscMessage _oscMessage;
        static OscPacket _oscPacket;
		static string _msgMode;
		static bool _isOSCLearning;
		static float _editorWidth;
		static String[] _optionsMappingFiles ;
		static String[] _optionsSessionFiles ;

		//static int toolbarInt = 0; 
		static string[] toolbarStrings = new string[] {"Mapping Files", "Session Files"};

		#endregion static


		#region constants
		public const float TRACEWIDTH = 250f;
		#endregion

		#region Events
		public static event EventHandler<UniOSCEventArgs> OSCMessageReceived;
		//public event EventHandler<ExceptionEventArgs> OSCErrorOccured;
		#endregion

		// Use this for initialization
		/// <summary>
		/// Method is called from the Unity menu.
		/// </summary>
		[MenuItem(UniOSCUtils.MENUITEM_EDITOR)]
		 static void _Init(){
			_windowSelf = EditorWindow.GetWindow(typeof(UniOSCEditor));
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
            _windowSelf.title = "UniOSC Editor";
#else
            _windowSelf.titleContent = new GUIContent("UniOSC Editor", "UniOSC Editor");
#endif
			
			_windowSelf.minSize = new Vector2(256f,256f);
			//_windowSelf.position = new Rect(10,10,1074,768);
			_windowSelf.autoRepaintOnSceneChange = true;
		}

		#region Messages 
		public void OnEnable() {
			EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
			Instance = this;
			UniOSCConnectionEditor.LoadTextures();//Textures are otherwise not loaded when Inspector for OSCCOnnection is not once visible
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// Called everytime the editor is opened or when we have to update the editor (After creating a OSCConnection,hit the 'Refresh' button) 
		/// </summary>
		public static void Init(){

			_ReadSettings();
			_ReadAllOSCMappingFiles(false,null);
			_ReadAllOSCSessionFiles(false,null);
			_GetOSCConnectionInstances();
			OSCLearning(false);//security
			if (_config.isOSCLearning) OSCLearning(true);
		
		}
		
		public void OnDisable() {
			// This avoids unwanted disposing when playing with "Maximize on Play" enabled on the Game View.
			if (Application.isPlaying) {
				//return;
			}

			EditorApplication.playmodeStateChanged -= PlaymodeStateChanged;
			
			WriteSettings();
			
			_SetupOSCConnectionsOSCMessageReceivedEvent(false);

			_config.isOSCLearning = false;
			OSCLearning(false);//security

		}

		void Start () {
		
		}

		// called at 10 frames per second to give the inspector a chance to update.
		void OnInspectorUpdate(){
		//with unity 4 we loose Tooltips with this :-( 
			Repaint();
		}

		//Called 100 times per second on all visible windows.
		void Update () {
			//Repaint();
		}

		public void  OnHierarchyChange(){
			//????
		}

		#endregion

		private void PlaymodeStateChanged()
		{
			if( EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying )
			{
				//Debug.Log( "Auto-Saving scene before entering Play mode: " + EditorApplication.currentScene );
				//EditorApplication.SaveScene();
				//EditorApplication.SaveAssets();
			}

			//On Play/Stop we loose the reference to the OSCConnections so we generate a new connection to the eventsystem via the InstanceID
			if(Application.isPlaying){
				_SetupOSCConnectionsOSCMessageReceivedEvent(true);
			}else{
				_SetupOSCConnectionsOSCMessageReceivedEvent(true);
			}
			
		}


		#region GUI
		void OnGUI(){

			if (this._recompile == null  )
			{
				// if yes assume recompile then create a reference to a recompile class
				this._recompile = new RecompileClass();
				//little hack for the editor,otherwise on start we have no OSC Connections in the Editor GUI. ([InitializeOnLoad] didn't work)
				//Init();
				_GetOSCConnectionInstances();
			}
			
			
			EditorGUI.BeginChangeCheck();

			_editorWidth = position.width;
			if(_config.isOSCTracing)  _editorWidth = _editorWidth -TRACEWIDTH;

			if(_config.mySkin != null) GUI.skin = _config.mySkin;

			//Rect for Learning Border
			Rect r_learn = new Rect(0f,0f,position.width,position.height);

			EditorGUILayout.BeginHorizontal();

			#region GUI_LEFT
			EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

			if(_config == null){
				Rect area = GUILayoutUtility.GetRect (500.0f, 80.0f);
				EditorGUI.HelpBox(area,"No configuration could be loaded! The Editor could not work in this state.",MessageType.Error);
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				return;
			}

			GUILayout.Space(4);
		
			#region header
			#region logo
			GUILayout.BeginHorizontal();
			GUILayout.Space(4);
			UniOSCUtils.DrawTexture(_config.tex_logo);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			#endregion logo


			GUILayout.BeginHorizontal();

			GUI.backgroundColor = _config.isOSCTracing ? Color.green : Color.white ;
			_config.isOSCTracing =  GUILayout.Toggle(_config.isOSCTracing,new GUIContent("Trace OSC"), GUI.skin.button,GUILayout.Height(30f),GUILayout.Width(100f));//OSCConnectionEditor.tex2
			GUI.backgroundColor = Color.white;
			

			if(GUILayout.Button(new GUIContent("Create OSC Connection",""),GUILayout.Width(160f),GUILayout.Height(30f) ) ){_CreateOSCConnection();Init();};
			if (GUILayout.Button ("Create OSC Mapping File",GUILayout.Height(30f),GUILayout.Width(160f) ) ){_CreateOSCMappingFile();}
			if (GUILayout.Button ("Create OSC Session File",GUILayout.Height(30f),GUILayout.Width(160f) ) ){_CreateOSCSessionFile();}
			

			GUI.backgroundColor = _config.isOSCLearning ? Color.green : Color.white ;
			_config.isOSCLearning =  GUILayout.Toggle(_config.isOSCLearning,new GUIContent("Learn OSC"), GUI.skin.button,GUILayout.Height(30f),GUILayout.Width(100f));//OSCConnectionEditor.tex2
			GUI.backgroundColor = Color.white;

			if(_isOSCLearning != _config.isOSCLearning){
				//Toggle was clicked
				_isOSCLearning = _config.isOSCLearning;
				
				OSCLearning(_config.isOSCLearning);
				//EditorUtility.SetDirty(_config);
			}

			//_config.isEditorEnabled = UniOSCConnection.isEditorEnabled;
			GUI.backgroundColor = _config.isEditorEnabled ? Color.green : Color.white ;
			_config.isEditorEnabled =  GUILayout.Toggle(_config.isEditorEnabled,new GUIContent("Editor Mode"), GUI.skin.button,GUILayout.Height(30f),GUILayout.Width(100f));
			GUI.backgroundColor = Color.white;
			UniOSCConnection.isEditorEnabled = _config.isEditorEnabled;
				
			GUILayout.EndHorizontal();
			#endregion header

			
			GUILayout.Space(5);
			_DrawLineH(_editorWidth-20f);
			GUILayout.Space(5);


			#region Connection
			GUILayout.BeginHorizontal();
			GUIStyle gsCon = new GUIStyle(GUI.skin.label);
			gsCon.fontSize = 18;
			GUILayout.Label("OSC Connection",gsCon);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
			OnGUI_OSCConnection();
			#endregion Connection

			#region space
	
			GUI.contentColor = Color.white;
			GUI.backgroundColor = Color.white;
				
			GUILayout.Space(5);
			_DrawLineH(_editorWidth-20f);
			GUILayout.Space(5);
			#endregion space

			#region toolbar
			_config.toolbarInt =  GUILayout.Toolbar(_config.toolbarInt, toolbarStrings,GUILayout.Width(200*toolbarStrings.Count()),GUILayout.Height(30f));
			#endregion toolbar

			GUILayout.Space(5);
//			_DrawLineH(_editorWidth-20f);
			GUILayout.Space(5);

			#region Mapping

			switch(_config.toolbarInt){
			case 0:
				OnGUI_OSCMapping();
				break;
			case 1:
				OnGUI_OSCSessionFiles();
				break;
			default:
				OnGUI_DefaultContent();
				break;
			}
				
			#endregion Mapping

			#region footer
			GUI.contentColor = Color.white;
		
			if(_osConnectionIDList.Count > 0){
				_DrawLineH(_editorWidth-20f);
				GUILayout.Space(10f);
			}

			if(GUILayout.Button(new GUIContent("Refresh Editor","Refresh the Editor. Reloads the available data"),GUILayout.Width(150),GUILayout.Height(30f)) ){Init();};
			GUILayout.Space(5f);
			#endregion footer

			EditorGUILayout.EndVertical();
			#endregion GUI_LEFT


			#region GUI_RIGHT
			if(_config.isOSCTracing){
					Rect r_tracing =  GUILayoutUtility.GetRect (1.0f, position.height-0f);
				GUI.Box(r_tracing,"",GUI.skin.box);//,GUI.skin.box  GUI.skin.customStyles[1]
				OnGUI_Trace();
			}
			#endregion GUI_RIGHT
			EditorGUILayout.EndHorizontal();

			//draw Learning Border
			if(_config.isOSCLearning){
				GUI.backgroundColor = Color.red;
				GUI.Box(r_learn,"",_config.learnStyle);
				GUI.backgroundColor = Color.white;
			}

			if(EditorGUI.EndChangeCheck()){
				EditorUtility.SetDirty(_config);
			}
		}

		void OnGUI_DefaultContent(){
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}

		void OnGUI_OSCSessionFiles(){
			GUILayout.BeginHorizontal();
			
			GUIStyle gs = new GUIStyle(GUI.skin.label);
			gs.fontSize = 18;
			GUILayout.Label("OSC Session",gs);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Space(20f);


			if( _config.OSCSessionFileObjList.Count >0 ){
				
				_config.selectedSessionFileObjIndex = EditorGUILayout.Popup("Select OSC Session File:", _config.selectedSessionFileObjIndex, _optionsSessionFiles,GUILayout.Height(20f),GUILayout.MaxWidth(500f));
				EditorGUILayout.Space();
				
				if((_config.selectedSessionFileObjIndex-1)>= _config.OSCSessionFileObjList.Count  ){ 
					_config.selectedSessionFileObjIndex = Mathf.Max(0,_config.OSCSessionFileObjList.Count -1);
				}
				
				if( _config.OSCSessionFileObjList.Count -1 >= _config.selectedSessionFileObjIndex ){
					var oscFileData = _config.OSCSessionFileObjList[_config.selectedSessionFileObjIndex];
					if(oscFileData != null) {
						//drawing the session file in editor layout
						UniOSCSessionFileObjEditor.OnGUI_OSCSessionData_Editor(oscFileData, _editorWidth-20f,position.height*0.75f);
					}
				}
				
			} else{
				EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(position.height*1.0f));
				GUILayout.FlexibleSpace();
				Rect area = GUILayoutUtility.GetRect (500.0f, 80.0f);
				EditorGUI.HelpBox(area,"There is no OSC Session file in this project.",MessageType.Info);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
		}

		void OnGUI_OSCMapping(){

			GUILayout.BeginHorizontal();
			
			GUIStyle gs = new GUIStyle(GUI.skin.label);
			gs.fontSize = 18;
			GUILayout.Label("OSC Mapping",gs);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Space(20f);
			
			if( _config.OSCMappingFileObjList.Count >0 ){
				
				_config.selectedMappingFileObjIndex = EditorGUILayout.Popup("Select OSC Mapping File:", _config.selectedMappingFileObjIndex, _optionsMappingFiles,GUILayout.Height(20f),GUILayout.MaxWidth(500f));
				EditorGUILayout.Space();
				
				if((_config.selectedMappingFileObjIndex-1)>= _config.OSCMappingFileObjList.Count  ){ 
					_config.selectedMappingFileObjIndex = Mathf.Max(0,_config.OSCMappingFileObjList.Count -1);
				}
				
				if( _config.OSCMappingFileObjList.Count -1 >= _config.selectedMappingFileObjIndex ){
					var oscMappingData = _config.OSCMappingFileObjList[_config.selectedMappingFileObjIndex];
					if(oscMappingData != null) {
						//drawing the mapping file in editor layout
						UniOSCMappingFileObjEditor.OnGUI_OSCMappingData_Editor(oscMappingData, _editorWidth-20f,position.height*0.75f);
					}
				}
				
			} else{
				EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(position.height*1.0f));
				GUILayout.FlexibleSpace();
				Rect area = GUILayoutUtility.GetRect (500.0f, 80.0f);
				EditorGUI.HelpBox(area,"There is no OSC mapping file in this project.",MessageType.Info);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
		}


		void OnGUI_OSCConnection(){
			
			EditorGUILayout.BeginHorizontal();
			//Hardcoding layout for wrapping
			int maxCols = 3;
			int minCols = 1;

			maxCols =(int) Math.Max( minCols, Math.Floor( (_editorWidth -20f)/(400f*1.0f)) );//Floor Ceiling
			if(_osConnectionIDList.Count <= 0){
				EditorGUILayout.BeginVertical();
				GUILayout.Space(10f);
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				Rect area = GUILayoutUtility.GetRect (500.0f, 80.0f);
				EditorGUI.HelpBox(area,"There is no OSCConnection discovered. Please create an OSCConnection or enable a GameObject with an OSCConnection component in your project.\nIf you can't see any OSCConnection even if you have enabled one please hit the 'Refresh Editor' button.",MessageType.Info);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10f);
				EditorGUILayout.EndVertical();
			}else{

				for(var i = 0; i<_osConnectionIDList.Count;i++){
					// Begin new row?
					if (i % maxCols == 0 && i > 0) {
						GUILayout.EndHorizontal();
						GUILayout.Space(10);
						GUILayout.BeginHorizontal();

					}
					//without storing ID we loose the connection to the instances on Play/Stop
					UniOSCConnection con = EditorUtility.InstanceIDToObject(_osConnectionIDList[i])as UniOSCConnection;
					if(con != null){ 
						EditorGUILayout.BeginVertical(GUI.skin.box,GUILayout.Width(400f));
							EditorGUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();
								GUILayout.Label (con.name);
								GUILayout.FlexibleSpace();
								EditorGUILayout.EndHorizontal();
								EditorGUILayout.BeginHorizontal();
								UniOSCConnectionEditor.ShowOSCReciverStatus(con);
							EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					}
					
				}//for

				GUILayout.FlexibleSpace();//push Layout

			}//if

			EditorGUILayout.EndHorizontal();
		}

		#region trace

		void OnGUI_Trace(){
			EditorGUILayout.BeginVertical(GUILayout.Width(TRACEWIDTH-20f));

			GUI.contentColor = Color.yellow;

			_config.configTraceScrollpos = GUILayout.BeginScrollView(_config.configTraceScrollpos,  GUILayout.Height (position.height - 100f), GUILayout.ExpandHeight (false));
			// GUILayout!!! not EditorGUILayout! So we can select.
			GUILayout.TextArea(_oscTraceStr,GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();

			GUI.contentColor = Color.white;
			
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			GUI.backgroundColor = _config.isLastMessageTracing ? Color.green : Color.white ;
		
			_config.isLastMessageTracing = GUILayout.Toggle(_config.isLastMessageTracing,"Display only last message","button",GUILayout.Height(20));
			GUI.backgroundColor = Color.white ;
			if(EditorGUI.EndChangeCheck()){
				EditorUtility.SetDirty(_config);
			}

			GUILayout.Space(5);

			if (GUILayout.Button ("Clear Trace",GUILayout.Height(30))){
				_oscTraceStr = "";
				_osctraceStrb = new StringBuilder(16384);
			}

			GUILayout.Space(8f);

			EditorGUILayout.EndVertical();
		}
		#endregion trace

		private void _DrawLineH(float width){
			Rect r1b =  GUILayoutUtility.GetRect (width, 1.0f);
			GUI.Box(r1b,"",GUI.skin.box);
		}

		#endregion
		

		#region Settings
		private  static void _ReadSettings(){
			// file ending must be '.asset' because otherwise unity can't load the right type of the scriptableObject
			//this solution is portable: get path from current script and construct the path from there
			var script = MonoScript.FromScriptableObject( Instance );
			var scriptPath = AssetDatabase.GetAssetPath( script );
			var scriptFolder = Path.GetDirectoryName( scriptPath ) ;
			scriptFolder = scriptFolder.Replace("Assets/","");

			string filepath =Path.Combine( Path.Combine(Application.dataPath,scriptFolder),UniOSCUtils.CONFIGPATH_EDITOR).Replace(@"\", "/"); 
			string relativFilepath = filepath.Replace(Application.dataPath, "Assets");

			_config = AssetDatabase.LoadAssetAtPath(relativFilepath,typeof(UniOSCEditorConfigObj) ) as UniOSCEditorConfigObj;
			if(_config == null){
				_config = ScriptableObject.CreateInstance<UniOSCEditorConfigObj>();
				string directoryName =  Path.GetDirectoryName(filepath);
				try{
					if(!Directory.Exists(directoryName)){
						Directory.CreateDirectory(directoryName);
					}
						_config.name = relativFilepath;
						AssetDatabase.CreateAsset(_config, relativFilepath);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						Debug.Log("Configuration File was created at :"+relativFilepath);
				}
				catch(Exception e){
					Debug.LogError("Generating the directory: "+directoryName+" failed!\n"+e.ToString());
				}

			}else{
				//Debug.Log("Config is there and Loaded!");
			}


			if(_config == null){
				Debug.LogError("OSCMapperEditor configuration file couldn't loaded or created!");
				return;
			}

			_isOSCLearning = _config.isOSCLearning;

		}

		private void WriteSettings(){
			
		}
		#endregion


		/// <summary>
		/// When entering the OSC learning mode the editor connects all mapping files to the event system so the OSC address for a mapping item can be recorded.
		/// </summary>
		/// <param name="flag">If set to <c>true</c> flag.</param>
		public static void OSCLearning(bool flag){
	
			UniOSCConnection.isOSCLearning = flag;
			foreach(var mfo in _config.OSCMappingFileObjList){
				if(mfo == null)continue;
				mfo.IsLearning = flag; 

				if(flag){
					OSCMessageReceived+= mfo.OnOSCMessageReceived;
				}
				else{
					OSCMessageReceived-= mfo.OnOSCMessageReceived;
				} 
				EditorUtility.SetDirty(mfo);
			}//for

			foreach(var sfo in _config.OSCSessionFileObjList){
				if(sfo == null)continue;
				sfo.IsLearning = flag; 
				
				if(flag){
					OSCMessageReceived+= sfo.OnOSCMessageReceived;
				}
				else{
					OSCMessageReceived-= sfo.OnOSCMessageReceived;
				} 
				EditorUtility.SetDirty(sfo);
			}//for



		}
		
		#region OSCMapping

		private static void _ReadAllOSCMappingFiles(bool selectLastItem, UniOSCMappingFileObj selectedOSCMappingFileObj){

			UniOSCMappingFileObj[] list = UniOSCUtils.GetAssetsOfType<UniOSCMappingFileObj>(UniOSCUtils.MAPPINGFILEEXTENSION);

			_config.OSCMappingFileObjList.Clear();
			_optionsMappingFiles = new string[list.Length];

			for(int i = 0;i<list.Length;i++){
				_optionsMappingFiles[i] = Path.GetFileNameWithoutExtension( AssetDatabase.GetAssetPath(list[i]));
				_config.OSCMappingFileObjList.Add(list[i]);
				//we have to recalculte the guid on every import ,so we can import assets from other projects
				list[i].my_guid = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath(list[i]) );
				list[i].IsLearning = _config.isOSCLearning;
			}
			_config.selectedMappingFileObjIndex = Mathf.Max(0, Mathf.Min(_config.selectedMappingFileObjIndex, _config.OSCMappingFileObjList.Count-1));

			if(_config.OSCMappingFileObjList.Count > 0 && selectLastItem){
				int index = _config.OSCMappingFileObjList.FindIndex(x=> x.my_guid == selectedOSCMappingFileObj.my_guid);
				if(index > -1) _config.selectedMappingFileObjIndex = index;
			}

		}

		[MenuItem(UniOSCUtils.MENUITEM_CREATE_MAPPING_FILE,false,2)]
		static void CreateOSCMappingFile(){
			_CreateOSCMappingFile();
		}


		  static void _CreateOSCMappingFile(){
			_config.toolbarInt =0;
			var path = EditorUtility.SaveFilePanelInProject("Create OSC Mapping File", UniOSCUtils.MAPPINGFILE_DEFAULTNAME, "asset", "Enter File Name");//CONFIGFILEEXTENSION +OSCUtils.MAPPINGFILEEXTENSION.Split('.')[0]
			if (path.Length != 0){
				UniOSCMappingFileObj mappingFileObj = ScriptableObject.CreateInstance<UniOSCMappingFileObj>() ;
				UniOSCMappingFileObj dummy = AssetDatabase.LoadAssetAtPath(path, typeof(UniOSCMappingFileObj))as UniOSCMappingFileObj;
				if (dummy == null) {
					//New Path
					AssetDatabase.CreateAsset(mappingFileObj, path);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					mappingFileObj.my_guid = AssetDatabase.AssetPathToGUID(path);
					_ReadAllOSCMappingFiles(true,mappingFileObj);
					//_Init();
				}
							
			}//if

		}

		#endregion OSCMapping
		
		#region Session

		private static void _ReadAllOSCSessionFiles(bool selectLastItem, UniOSCSessionFileObj selectedOSCSessionFileObj){
			
			UniOSCSessionFileObj[] list = UniOSCUtils.GetAssetsOfType<UniOSCSessionFileObj>(UniOSCUtils.MAPPINGFILEEXTENSION);
			
			_config.OSCSessionFileObjList.Clear();
			_optionsSessionFiles = new string[list.Length];
			
			for(int i = 0;i<list.Length;i++){
				_optionsSessionFiles[i] = Path.GetFileNameWithoutExtension( AssetDatabase.GetAssetPath(list[i]));
				_config.OSCSessionFileObjList.Add(list[i]);
				//we have to recalculte the guid on every import ,so we can import assets from other projects
				list[i].my_guid = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath(list[i]) );
				list[i].IsLearning = _config.isOSCLearning;
			}
			_config.selectedSessionFileObjIndex = Mathf.Max(0, Mathf.Min(_config.selectedSessionFileObjIndex, _config.OSCSessionFileObjList.Count-1));
			
			if(_config.OSCSessionFileObjList.Count > 0 && selectLastItem){
				int index = _config.OSCSessionFileObjList.FindIndex(x=> x.my_guid == selectedOSCSessionFileObj.my_guid);
				if(index > -1) _config.selectedSessionFileObjIndex = index;
			}
			
		}


		[MenuItem(UniOSCUtils.MENUITEM_CREATE_SESSION_FILE,false,3)]
		static void CreateOSCSessionFile(){
			_CreateOSCSessionFile();
		}
		static void _CreateOSCSessionFile(){
			_config.toolbarInt =1;
			var path = EditorUtility.SaveFilePanelInProject("Create OSC Session File", UniOSCUtils.SESSIONFILE_DEFAULTNAME, "asset", "Enter File Name");
			if (path.Length != 0){
				UniOSCSessionFileObj sessionFileObj = ScriptableObject.CreateInstance<UniOSCSessionFileObj>() ;
				UniOSCSessionFileObj dummy = AssetDatabase.LoadAssetAtPath(path, typeof(UniOSCSessionFileObj))as UniOSCSessionFileObj;
				if (dummy == null) {
					//New Path
					AssetDatabase.CreateAsset(sessionFileObj, path);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					sessionFileObj.my_guid = AssetDatabase.AssetPathToGUID(path);
					_ReadAllOSCSessionFiles(true,sessionFileObj);
					//_Init();
				}
				
			}//if
		}


		#endregion Session

		#region OSCConnection

		 static  void _GetOSCConnectionInstances(){

			_osConnectionList.Clear();
			_osConnectionIDList.Clear();

			//UniOSCConnection[] tempList = Resources.FindObjectsOfTypeAll(typeof(UniOSCConnection))as UniOSCConnection[];
			UniOSCConnection[] tempList = FindObjectsOfType(typeof(UniOSCConnection))as UniOSCConnection[];
			if( tempList != null){
				_osConnectionList = tempList.ToList();// List<UniOSCConnection>(tempList);
				foreach(var c in _osConnectionList){
					_osConnectionIDList.Add(c.GetInstanceID());
				}//for

				_SetupOSCConnectionsOSCMessageReceivedEvent(true);
			}//if
		
		}

		[MenuItem(UniOSCUtils.MENUITEM_CREATE_CONNECTION,false,1)]
		static void CreateOSCConnection(){
			_CreateOSCConnection();
		}

		static void _CreateOSCConnection(){

			GameObject go = new GameObject("OSCConnection"+_osConnectionList.Count);
			UniOSCConnection oc = go.AddComponent<UniOSCConnection>();
            
			oc.oscPort = 8000;
			oc.oscOutPort = 9000;

            oc.oscOutIPAddress = UniOSCUtils.GetLocalIPAddress();
			
			go.name = "OSCConnection."+go.GetInstanceID();
            //UniOSCConnection.Update_AvailablePorts();
			oc.Awake();
          
		}

		
		static void _SetupOSCConnectionsOSCMessageReceivedEvent(bool flag){

			foreach(var id in _osConnectionIDList){
				UniOSCConnection c = EditorUtility.InstanceIDToObject(id)as UniOSCConnection;
				if(c != null) {
					//security!!!
					c.OSCMessageReceivedRaw-= _OnOSCMessageReceived;
					c.OSCMessageSend-=  _OnOSCMessageSended;
					if(flag == true) {
						c.OSCMessageReceivedRaw+= _OnOSCMessageReceived;
						c.OSCMessageSend+=  _OnOSCMessageSended;
					}
				}
			}//for

		}
		
		#endregion
		#region callback
		static void _OnOSCMessageSended(object sender, UniOSCEventArgs args){
			if(_config.isOSCTracing == true)_TraceOSCMessage( sender,args,false);
		}

		static void _OnOSCMessageReceived(object sender, UniOSCEventArgs args){
			if(_config.isOSCTracing == true)_TraceOSCMessage( sender,args,true);
			if( OSCMessageReceived != null) OSCMessageReceived(Instance, args);
		}


	

		static void _TraceOSCMessage(object sender,UniOSCEventArgs args,bool oscIn){
		
			if(_config.isLastMessageTracing) _osctraceStrb.Length = 0;
			_msgMode = oscIn ? "IN" : "OUT";
			//_oscMessage = args.Message;
            _oscPacket = args.Packet;
			_osctraceStrb.AppendLine("----Message "+_msgMode+"-----");
			if(oscIn){
				_osctraceStrb.AppendLine("Port:" +((UniOSCConnection)sender).oscPort);
			}else{
				_osctraceStrb.AppendLine("Destination IP:" +((UniOSCConnection)sender).oscOutIPAddress);
				_osctraceStrb.AppendLine("Port:" +((UniOSCConnection)sender).oscOutPort);
			}
            

            if (_oscPacket.IsBundle) 
            {
                _osctraceStrb.AppendLine("Bundle:");
                foreach (var m in ((OscBundle)_oscPacket).Messages)
                {
                     _osctraceStrb.AppendLine(" Address: " + m.Address.ToString());

                     if (m.Data.Count > 0)
                     {
                         for (int i = 0; i < m.Data.Count; i++)
                         {
                             _osctraceStrb.AppendLine("  Data: " + m.Data[i].ToString());
                         }

                     }
                   
                }//for
               
            }
            else
            {
                _osctraceStrb.AppendLine("Address: " + _oscPacket.Address.ToString());

                if (((OscMessage)_oscPacket).Data.Count > 0)
                {
                    for (int i = 0; i < ((OscMessage)_oscPacket).Data.Count; i++)
                    {
                        _osctraceStrb.AppendLine("Data: " + ((OscMessage)_oscPacket).Data[i].ToString());
                    }

                }
            }

			//texfield don't like more then 16384 strings, so we remove on from the begin.
			//Unity4.5 has problems with more than 15000 chars?
			while(_osctraceStrb.Length > 8192){
				_osctraceStrb.Remove(0,64);
				//_osctraceStrb.Length--;
			}

			 _oscTraceStr = _osctraceStrb.ToString();
			//autoscroll to bottom
			_config.configTraceScrollpos = new Vector2(0f,Mathf.Infinity);
		}

		#endregion

	}

}


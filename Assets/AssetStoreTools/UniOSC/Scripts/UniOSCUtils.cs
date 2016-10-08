/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniOSC{

	/// <summary>
	/// Some global helper methods and the main storage for paths, colors... that uses UniOSC
	/// </summary>
	public class UniOSCUtils  {


		public  static Texture2D TEX_CONNECTION_BG;

		public const int MAXPORT= 65535;

		public const string LOGO16_NAME = "logo64x16";
		public const string LOGO32_NAME = "logo128x32";
		public const string LOGO64_NAME = "logo256x64";
		public const string OSCOUTTEST_NAME = "testMessageBtn";
		public const string OSCCONNECTION_OFF_NAME = "connection_pause";
		public const string OSCCONNECTION_ON_NAME = "connection_on";
        public const string MULTICASTREGEX = @"2(?:2[4-9]|3\d)(?:\.(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]\d?|0)){3}";


		public  static Color CONNECTION_ON_COLOR = new Color(0f,1.0f,0f);
		public  static Color CONNECTION_OFF_COLOR = new Color(1.0f,0f,0f);
		public  static Color CONNECTION_PAUSE_COLOR = new Color(1.0f,1.0f,0f);
		public  static Color CONNECTION_BG = new Color(0.59f,0.59f,0.59f,0.5f);

		public  static Color ITEM_LIST_COLOR_A = new Color(0.99f,0.99f,0.99f);
		public  static Color ITEM_LIST_COLOR_B = new Color(0.75f,0.75f,0.75f);

		public  static Color LEARN_COLOR_ON = new Color(0.05f,0.75f,0.05f);
		public  static Color LEARN_COLOR_OFF = new Color(0.75f,0.75f,0.75f);

		public const string  MENUITEM_EDITOR = "Window/UniOSC/OSCEditor";
		public const string  MENUITEM_CREATE_MAPPING_FILE = "GameObject/Create Other/UniOSC/OSC Mapping File";
		public const string  MENUITEM_CREATE_SESSION_FILE = "GameObject/Create Other/UniOSC/OSC Session File";
		public const string  MENUITEM_CREATE_CONNECTION = "GameObject/Create Other/UniOSC/OSCConnection";
		public const string  MENUITEM_CREATE_MOVE = "GameObject/Create Other/UniOSC/Move GameObject";
		public const string  MENUITEM_CREATE_ROTATE = "GameObject/Create Other/UniOSC/Rotate GameObject";
		public const string  MENUITEM_CREATE_SCALE = "GameObject/Create Other/UniOSC/Scale GameObject";
		public const string  MENUITEM_CREATE_CHANGECOLOR = "GameObject/Create Other/UniOSC/Change Material Color";
		public const string  MENUITEM_CREATE_TOGGLE = "GameObject/Create Other/UniOSC/Toggle";
		public const string  MENUITEM_CREATE_EVENTDISPATCHERBUTTON = "GameObject/Create Other/UniOSC/Send OSC Message Button";

		public const string TOOLTIP_EXPLICITCONNECTION ="If you select an explicit connection you don't have to specify your Port and IP Address as the component get the values from the connection.This is useful if you change the settings of a connection frequently and don't want to readjust the settings on every gameobject that uses the old values.";


		public  const string CONFIGPATH_EDITOR="Resources/UniOSCEditorConfig.asset";
		public  const string MAPPINGFILE_DEFAULTNAME= "OSCMappingFile";
		public  const string SESSIONFILE_DEFAULTNAME= "OSCSessionFile";
		public  const string MAPPINGFILEEXTENSION="asset";//"oscMapData.asset";

		// Address,Port,Min,Max,Dispatch,Learn,Delete
		public static Rect[] MAPPINGLISTLABELRECTS = {new Rect(),new Rect(),new Rect(),new Rect(),new Rect(),new Rect(),new Rect()};

		public static float MAPPINGLISTHEADERLABELWIDTH{
			get{
				float f = 0f;
				for(int i= 0;i < MAPPINGLISTLABELRECTS.Length ;i++){
					f+= MAPPINGLISTLABELRECTS[i].width;
				}
				return f+50f;
			}
		}
		//address,Learn,delete,Data[0],Data[1],Data[2],Data[3]
		public static Rect[] SESSIONLISTLABELRECTS = {new Rect(),new Rect(),new Rect(),new Rect(),new Rect(),new Rect(),new Rect()};
		
		public static float SESSIONLISTHEADERLABELWIDTH{
			get{
				float f = 0f;
				for(int i= 0;i < SESSIONLISTLABELRECTS.Length ;i++){
					f+= SESSIONLISTLABELRECTS[i].width;
				}
				return f+50f;
			}
		}



		/// <summary>
		/// Validates the IP address.
		/// </summary>
		/// <returns><c>true</c>, if IP adress was validated, <c>false</c> otherwise.</returns>
		/// <param name="strIP">String I.</param>
		/// <param name="address">Address.</param>
		public static bool ValidateIPAddress(string strIP, out IPAddress  address){
            address = null;
			return !String.IsNullOrEmpty(strIP) && IPAddress.TryParse(strIP, out address);
		}

        public static bool RegexMatch(string str,string exp)
        {        
            Regex regex = new Regex(@exp);
            Match match = regex.Match(str);
            return match.Success;
           
        }


		/// <summary>
		/// Maps a value to an interval.
		/// </summary>
		/// <returns>The the new mapped value</returns>
		/// <param name="val">Value.</param>
		/// <param name="srcMin">Source minimum.</param>
		/// <param name="srcMax">Source max.</param>
		/// <param name="dstMin">Destination minimum.</param>
		/// <param name="dstMax">Destination max.</param>
		public static float MapInterval(float val, float srcMin, float srcMax, float dstMin, float dstMax) {
			if (val>=srcMax) return dstMax;
			if (val<=srcMin) return dstMin;
			return dstMin + (val-srcMin) / (srcMax-srcMin) * (dstMax-dstMin);
		} 

		/// <summary>
		/// Gets the local IP address.
		/// </summary>
		/// <returns>The local IP address.</returns>
		public static string GetLocalIPAddress(){
			try{
					return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
				}catch(SocketException e){
					//Debug.LogError("Error retrieving a valid local ipAddress:"+e.Message+"/nPlease check your network settings or restart Unity.");
                    //return null;
                    //Fallback: 127.0.0.1
                    return "127.0.0.1";
				}
		}

		/// <summary>
		/// Log the specified sender, methodName and msg.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="methodName">Method name.</param>
		/// <param name="msg">Message.</param>
		public static void Log(object sender,string methodName,object msg){
			if(msg == null)msg ="";
			Debug.Log(String.Format("{0}.{1} : {2}",sender.GetType().Name,methodName,msg.ToString() ) );
		}

		/// <summary>
		/// Draws the texture.
		/// </summary>
		/// <param name="tex">Tex.</param>
		public static void DrawTexture(Texture tex) {
			if(tex == null) return;
			Rect rect = GUILayoutUtility.GetRect(tex.width, tex.height);
			GUI.DrawTexture(rect, tex);
		}

		/// <summary>
		/// Draws a clickable texture.
		/// The event is triggerd on MouseUp
		/// </summary>
		/// <param name="tex">The texture you want to display</param>
		/// <param name="evt">Event that should be called when the user clicks on the texture</param>
		public static void DrawClickableTexture(Texture tex,Action evt ) {
			if(tex == null) return;
			Rect rect = GUILayoutUtility.GetRect(tex.width, tex.height);
			GUI.DrawTexture(rect, tex);
			
			var e = Event.current;
			if (e.type == EventType.MouseUp) {
				if (rect.Contains(e.mousePosition)) {
					if(evt != null) evt();
				}
			}
		}


		/// <summary>
		/// Draws a clickable texture horizontal.
		/// </summary>
		/// <param name="tex">The texture you want to display</param>
		/// <param name="evt">Event that should be called when the user clicks on the texture</param>
		public static void DrawClickableTextureHorizontal(Texture2D tex,Action evt){
			GUILayout.BeginHorizontal();
				DrawClickableTexture (tex,evt);
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(2f);
		}


		/// <summary>
		/// Makes a texture specified by the parameters.
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="col">Color.</param>
		public static Texture2D MakeTexture( int width, int height, Color col ) {
			Color[] pix = new Color[width * height];
			
			for( int i = 0; i < pix.Length; ++i ) {
				pix[ i ] = col;	
			}
			
			Texture2D result = new Texture2D( width, height );
			result.hideFlags = HideFlags.HideAndDontSave;//??
			result.SetPixels( pix );
			result.Apply();
			return result;
		}


		public static bool IsMouseUpInArea (Rect area)
		{
			Event evt = Event.current;
			switch(evt.type){
			case EventType.MouseUp:
				if (area.Contains(evt.mousePosition)) {
					return true;
				}
				break;
			}
			return false;
		}



			#if UNITY_EDITOR

			public static void SelectObjectInHierachyFromGUID(string GUID){
				UnityEngine.Object[] gos = new UnityEngine.Object[1];
				string path = AssetDatabase.GUIDToAssetPath(GUID);
				UnityEngine.Object go = AssetDatabase.LoadAssetAtPath(path,typeof(UnityEngine.Object)) as UnityEngine.Object ;
				gos[0] = go;
				Selection.objects = gos;
			}

			#region menuItems
			[MenuItem(UniOSCUtils.MENUITEM_CREATE_MOVE,false,4)]
			static void CreateUniOSCMoveGameObject(){
				GameObject go = new GameObject("UniOSC MoveGameObject");
				 go.AddComponent<UniOSCMoveGameObject>();
				
			}
			
			[MenuItem(UniOSCUtils.MENUITEM_CREATE_ROTATE,false,4)]
			static void CreateUniOSCRotateGameObject(){
				GameObject go = new GameObject("UniOSC RotateGameObject");
				go.AddComponent<UniOSCRotateGameObject>();
				
			}
			
			[MenuItem(UniOSCUtils.MENUITEM_CREATE_SCALE,false,4)]
			static void CreateUniOSCScaleGameObject(){
				GameObject go = new GameObject("UniOSC ScaleGameObject");
				go.AddComponent<UniOSCScaleGameObject>();
				
			}
			
			[MenuItem(UniOSCUtils.MENUITEM_CREATE_CHANGECOLOR,false,4)]
			static void CreateUniOSCChangeColor(){
				GameObject go = new GameObject("UniOSC ChangeColor");
				go.AddComponent<UniOSCChangeColor>();
				
			}
			
			[MenuItem(UniOSCUtils.MENUITEM_CREATE_TOGGLE,false,4)]
			static void CreateUniOSCToggle(){
				GameObject go = new GameObject("UniOSC Toggle");
				go.AddComponent<UniOSCToggle>();
			}
			
			[MenuItem(UniOSCUtils.MENUITEM_CREATE_EVENTDISPATCHERBUTTON,false,4)]
			static void CreateUniOSCEventDispatcherButton(){
				GameObject go = new GameObject("UniOSC Send OSC Message Button");
				go.AddComponent<UniOSCEventDispatcherButton>();
				
			}

			#endregion menuItems



			/// <summary>
			/// Used to get assets of a certain type and file extension from entire project
			/// </summary>
			/// <returns>An Object array of assets.</returns>
			/// <param name="fileExtension">The file extention the type uses eg ".prefab" or ".asset".</param>
			/// <typeparam name="T">The 1st type parameter.</typeparam>
			public static T[] GetAssetsOfType<T>( string fileExtension) where T:class
			{
				List<T> tempObjects = new List<T>();
				DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
				FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);
				
				
				int goFileInfoLength = goFileInfo.Length;
				FileInfo tempGoFileInfo; 
				string tempFilePath;
				T tempGO;
				
				for (int i = 0; i < goFileInfoLength; i++)
				{
					tempGoFileInfo = goFileInfo[i];
					if (tempGoFileInfo == null)
						continue;
					
					tempFilePath = tempGoFileInfo.FullName;
					tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
					
					tempGO = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(T)) as T;
					if (tempGO == null)
					{
						continue;
					}
					else if (tempGO.GetType() != typeof(T))
					{
						//Debug.LogWarning("Skipping " + tempGO.GetType().ToString());
						continue;
					}
					
					tempObjects.Add(tempGO);
				}
				
				return tempObjects.ToArray();
			}






			#endif

	}
}

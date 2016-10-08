/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using UnityEditor;
using System;
using UniOSC;


[InitializeOnLoad]
public class UniOSCAutoRun
{
	/// <summary>
	/// Is called when open the Unity editor, or after recompilation.
	/// Triggers some initialization routines so everthing is displayed correctly on startup.
	/// </summary>
	static  UniOSCAutoRun()
	{
		UniOSCConnection.Init();
		UniOSCMappingFileObjEditor.Init();
		UniOSCSessionFileObjEditor.Init();
	}
	 
}

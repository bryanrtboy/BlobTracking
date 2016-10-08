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
	/// 
	/// </summary>
	[Serializable]
	public abstract class UniOSCFileObj : ScriptableObject
		
	{
		#region public
		public string my_guid;
		public bool IsLearning;

		[HideInInspector]
		public Vector2 scrollpos = new Vector2();
		[HideInInspector]
		public Vector2 scrollposInspector = new Vector2();

		#endregion

		#region Events
		public event EventHandler<UniOSCEventArgs> OSCMessageSend;
		#endregion

	}
}

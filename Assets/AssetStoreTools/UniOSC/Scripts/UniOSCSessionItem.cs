/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UniOSC{

	/// <summary>
	/// Uni OSC mapping item.
	/// 
	/// </summary>
	/// <author> Stefan Schlupek </author>
	[Serializable]
	public class UniOSCSessionItem {

		#region public
		[HideInInspector]
		public UniOSCSessionFileObj hostObj;
		public string address = "";
	
		//fuck, can't serialize System.object
		[SerializeField]
		//public List<object> data = new List<object>();
		public List<string> data = new List<string>();
		[SerializeField]
		public List<string> dataTypeList = new List<string>();

		public bool isLearning;

		public const int MAXWIDTH = 250;
		public const int MAXHEIGTH = 150;
	
		[HideInInspector]
		public bool collapsed= true;//GO List

		#endregion 

		#region constructor
		public UniOSCSessionItem(){
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UniOSC.UniOSCSessionItem"/> class.
		/// </summary>
		/// <param name="_hostObj">The UniOSCSessionFileObj object that host the item</param>
		public UniOSCSessionItem(UniOSCSessionFileObj _hostObj){
			hostObj = _hostObj;
		}

		#endregion 


		/// <summary>
		/// Removes this item from the UniOSCSessionFileObj host object.Afterwards it gets destroyed.
		/// </summary>
		public void OnOSCSessionItemDelete(){
			if(hostObj != null) hostObj.RemoveOSCSessionItem(this);
		}

	}
}
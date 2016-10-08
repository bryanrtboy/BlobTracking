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
using OSCsharp.Data;

namespace UniOSC{

	/// <summary>
	/// Uni OSC mapping item.
	/// 
	/// </summary>
	/// <author> Stefan Schlupek </author>
	[Serializable]
	public class UniOSCMappingItem {

		#region public
		[HideInInspector]
		public UniOSCMappingFileObj hostObj;
		public string address = "";
		public float min=0f;
		public float max=1f;
		public float mappingMIN=0f;
		public float mappingMAX=1f;

		public bool isLearning;

		public const int MAXWIDTH = 250;
		public const int MAXHEIGTH = 150;
	
		[HideInInspector]
		public bool collapsed= true;//GO List

		#endregion 

		#region constructor
		public UniOSCMappingItem(){
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UniOSC.UniOSCMappingItem"/> class.
		/// </summary>
		/// <param name="_hostObj">The UniOSCMappingFileObj object that host the item</param>
		public UniOSCMappingItem(UniOSCMappingFileObj _hostObj){
			hostObj = _hostObj;
		}
		#endregion 


		/// <summary>
		/// Removes this item from the UniOSCMappingFileObj host object.Afterwards it gets destroyed.
		/// </summary>
		public void OnOSCMappingItemDelete(){
			if(hostObj != null) hostObj.RemoveOSCMappingItem(this);
		}


		
		/// <summary>
		/// Maps the incoming OSC data.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public void MapData(UniOSCEventArgs args){
			OscMessage msg = ((OscMessage)args.Packet);
			for(int i = 0; i<msg.Data.Count;i++){
				System.Object obj = msg.Data[i];
				if(obj is float ) {
					msg.UpdateDataAt(i,UniOSCUtils.MapInterval( (float)obj,min,max,mappingMIN,mappingMAX) );
				}
			}//for
		}
		
	}
}
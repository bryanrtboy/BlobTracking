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
	/// Mapping file class . Every mapping file get stored as a .asset file. 
	/// You can copy & paste a mapping file to another Unity project but you have to aware that sometimes Unity changes the serialization format.
	 /// If you have any trouble go to 'Edit/Project Settings/Editor' and change the seriaization mode to 'Force Text' and then switch back to 'Force Binary'
	/// </summary>
	[Serializable]
	public class UniOSCMappingFileObj : UniOSCFileObj 
		
	{
		#region public
		[SerializeField]
		public List<UniOSCMappingItem> oscMappingItemList;
		#endregion public


		public void OnEnable() {
			if(oscMappingItemList == null) {
				oscMappingItemList = new List<UniOSCMappingItem>();
			}
		}


		/// <summary>
		/// Adds a new OSC Mapping item.
		/// </summary>
		public void AddOSCMappingItem(){
	
			UniOSCMappingItem newOSCDD = new UniOSCMappingItem(this);
			oscMappingItemList.Add( newOSCDD);
		
		}

		/// <summary>
		/// Removes the OSC Mapping item from the list and destroys the item instance.
		/// <see cref="UniOSC.UniOSCMappingItem.OnOSCDataDispatcherDelete"/>
		/// </summary>
		/// <param name="obj">Object to remove.</param>
		public void RemoveOSCMappingItem(UniOSCMappingItem obj){
			if(oscMappingItemList.Contains(obj)){
				oscMappingItemList.Remove(obj);
				//Destroy(obj);
				obj = null;
			}
		}


		/// <summary>
		/// Checks if we are in learning mode an writes the OSC message address into the address property of a mapping item that is in learn mode(when user hold down the 'learn' button in the editor.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">UniOSCEventArgs that contains the OSC message</param>
		public void OnOSCMessageReceived(object sender,UniOSCEventArgs args){
	
			if(IsLearning){

				foreach(var d in oscMappingItemList){

					if(d.isLearning){
					
						//d.address = args.Message.Address;
						d.address = args.Address;
					}
				}//for
			}//if
			
		}



	}
}//namespace

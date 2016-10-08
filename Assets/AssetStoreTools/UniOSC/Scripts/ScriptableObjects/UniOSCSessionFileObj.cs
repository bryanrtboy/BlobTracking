/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections.Generic;
using System;
using OSCsharp.Data;

namespace UniOSC{

	/// <summary>
	/// OSC Session file class . Every Session file get stored as a .asset file. 
	/// You can copy & paste a Session file to another Unity project but you have to aware that sometimes Unity changes the serialization format.
	 /// If you have any trouble go to 'Edit/Project Settings/Editor' and change the seriaization mode to 'Force Text' and then switch back to 'Force Binary'
	/// </summary>
	[Serializable]
	public class UniOSCSessionFileObj : UniOSCFileObj
		
	{
		#region public

		[SerializeField]
		public List<UniOSCSessionItem> oscSessionItemList;


		#endregion public


		public void OnEnable() {
			if(oscSessionItemList == null) {
				oscSessionItemList = new List<UniOSCSessionItem>();
			}
		}


		/// <summary>
		/// Adds a new OSC Session item.
		/// </summary>
		public void AddOSCSessionItem(){
			UniOSCSessionItem newOSCSI = new UniOSCSessionItem(this);
			oscSessionItemList.Add( newOSCSI);
		}

		/// <summary>
		/// Removes the OSC Session item from the list and destroys the item instance.
		/// <see cref="UniOSC.UniOSCSessionItem.OnOSCSessionItemDelete"/>
		/// </summary>
		/// <param name="obj">UniOSCSessionItem to remove.</param>
		public void RemoveOSCSessionItem(UniOSCSessionItem obj){
			if(oscSessionItemList.Contains(obj)){
				oscSessionItemList.Remove(obj);
				//Destroy(obj);
				obj = null;
			}
		}


		/// <summary>
		/// Checks if we are in learning mode an writes the OSC message address into the address property of a session item that is in learn mode(when user hold down the 'learn' button in the editor.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">UniOSCEventArgs that contains the OSC message</param>
		public void OnOSCMessageReceived(object sender,UniOSCEventArgs args){
	
			if(IsLearning){

				foreach(var d in oscSessionItemList){
					if(d.isLearning){
						//d.address = args.Message.Address;
						d.address = args.Address;
					}
				}

			}else{

				var _osi = (UniOSCSessionItem)oscSessionItemList.Find(osi => osi.address == args.Address);
				if(_osi == null) return;
				OscMessage msg = (OscMessage)args.Packet;
				if(msg.Data == null)return;
				int dc = msg.Data.Count;
				for(int i = 0;i< dc;i++){
					if(_osi.data.Count  < dc){
						_osi.data.Add(String.Empty);
						_osi.dataTypeList.Add(String.Empty);
					}
					if(_osi.dataTypeList.Count  < dc){
						_osi.dataTypeList.Add(String.Empty);
					}

					_osi.data[i] = msg.Data[i].ToString();
					//only once we get the type .( performance concerns)
					if(String.IsNullOrEmpty(_osi.dataTypeList[i])) _osi.dataTypeList[i] = msg.Data[i].GetType().FullName;
				}//for


			}//else

			
		}



	}//class

}//namespace

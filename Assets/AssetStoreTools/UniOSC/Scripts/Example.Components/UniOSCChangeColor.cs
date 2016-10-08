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
	/// Change the color of the material from the GameObjects.
	/// Option to choose between Material and SharedMaterial
	/// </summary>
	[AddComponentMenu("UniOSC/ChangeColor")]
	[RequireComponent(typeof(MeshRenderer))]
	public class UniOSCChangeColor :  UniOSCEventTarget {

		public string R_Address;
		public string G_Address;
		public string B_Address;

		public bool sharedMaterial;

		private Vector3 pos;
		private Material _mat;
	
		void Awake(){

		}

		private void _Init(){
		
			receiveAllAddresses = false;
			_oscAddresses.Clear();
			_oscAddresses.Add(R_Address);
			_oscAddresses.Add(G_Address);
			_oscAddresses.Add(B_Address);
			
			if(sharedMaterial ){
				_mat = gameObject.GetComponent<Renderer>().sharedMaterial;
			}else{
				if(Application.isPlaying){
					_mat = gameObject.GetComponent<Renderer>().material;
				}else{
					_mat = null;
				}
			}
		}


		public override void OnEnable(){
			_Init();
			base.OnEnable();
		}

	
		public override void OnOSCMessageReceived(UniOSCEventArgs args){
		
			if(((OscMessage)args.Packet).Data.Count <1)return;
			if(_mat == null)return;

			if(!( ((OscMessage)args.Packet).Data[0]  is  System.Single))return;
			float value = (float)((OscMessage)args.Packet).Data[0] ;

			if(String.Equals(args.Address,R_Address)){
				_mat.color = new Color( value,_mat.color.g,_mat.color.b);
			}
			if(String.Equals(args.Address,G_Address)){
				_mat.color = new Color( _mat.color.r,value,_mat.color.b);
			}
			if(String.Equals(args.Address,B_Address)){
				_mat.color = new Color( _mat.color.r,_mat.color.g,value);
			}


		}

		

	}
}
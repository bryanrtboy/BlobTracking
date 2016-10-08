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
	/// UniOSC event target for class based scripting.
	/// This is the abstract class you should subclass from
	/// </summary>
	[Serializable]
	public abstract class UniOSCEventTargetCB : IDisposable{

		#region private

		private bool disposed = false;

		private bool _isEnabled;

		#endregion

        #region protected

        protected List<string> _oscAddresses = new List<string>();
        protected string _oscAddress;
        protected bool _receiveAllAddresses;
        protected bool _useExplicitConnection;
        protected UniOSCConnection _explicitConnection;
        protected int _oscPort;
        protected bool _receiveAllPorts;

        #endregion

		#region public

		public event EventHandler<UniOSCEventArgs> OSCMessageReceived;

		public bool isEnabled{get{return _isEnabled;}}        
      
        /// <summary>
        /// If you call this method you will remove all addresses that you added before!
        /// </summary>
        public string oscAddressAt(int index)
        {
            if (_oscAddresses.Count >= index) { return _oscAddresses[index]; } else { return null; }
        }
		public string oscAddress
		{
            get { return _oscAddresses[0]; }
            set
            {
                _oscAddresses.Clear();
                AddAddress(value);              
                _SetupChanged();                            
            }
		}
         
        public bool AddAddress(string __oscAddress)
        {
            if (_oscAddresses.Contains(__oscAddress)) 
            {
                return false;
            }
            else
            {
                _oscAddresses.Add(__oscAddress);
                return true;
            } 
        }

		public bool receiveAllAddresses{
            get { return _receiveAllAddresses; }
            set {
                bool isChanged = _receiveAllAddresses != value;
                _receiveAllAddresses = value;
                if (isChanged)
                {
                    _SetupChanged();
                }
            }
            
		}

      
		public bool useExplicitConnection{
            get { return _useExplicitConnection; }
            set {
                bool isChanged = _useExplicitConnection != value;
                _useExplicitConnection = value;
                if (isChanged)
                {
                    if (_explicitConnection == null && _useExplicitConnection) Debug.Log("<color=orange>Current explicit connection is null!</color>");
                    _SetupChanged();
                }
            }

		}

      
        public UniOSCConnection explicitConnection
        {
            get { return _explicitConnection; }
            set
            {
                bool isChanged = _explicitConnection != value;
                _explicitConnection = value;
                if (isChanged)
                { 
                    _useExplicitConnection = true;
                    _SetupChanged(); 
                }
            }
        }

      
		public int oscPort{
			get{ return _oscPort; }
			 set{
                 if(_useExplicitConnection){Debug.Log("<color=orange>You can't set the oscPort when using explicitConnection mode!</color>");return;}
                  bool isChanged = _oscPort != value;
                _oscPort = value;
                if (isChanged)
                { 
                    _SetupChanged(); 
                }
             }
		}

      
		public bool receiveAllPorts{
            get { return _receiveAllPorts; }
            set
            {
                if (_useExplicitConnection) { Debug.Log("<color=orange>You can't set the receiveAllPorts when using explicitConnection mode!</color>"); return; }
                bool isChanged = _receiveAllPorts != value;
                _receiveAllPorts = value;
                if (isChanged)
                {
                    _SetupChanged();
                }
            }
		}

		public Dictionary<UniOSCConnection,List<UniOSCMappingItem>> ConnectToDict = new Dictionary<UniOSCConnection,List<UniOSCMappingItem>>();

		#endregion

		#region constructors

		public UniOSCEventTargetCB(int __oscPort)
		{
			//Debug.Log("UniOSCEventTargetCB.Construktor");
			_oscPort = __oscPort;
			_receiveAllPorts = false;
			_receiveAllAddresses = true;
			_useExplicitConnection = false;
			Awake();
		}
		public UniOSCEventTargetCB(string _oscAddress)
		{
			//Debug.Log("UniOSCEventTargetCB.Construktor");
			if(!_oscAddresses.Contains(_oscAddress)) _oscAddresses.Add(_oscAddress);
			_receiveAllPorts = true;
			_receiveAllAddresses = false;
			_useExplicitConnection = false;
			Awake();
		}

		public UniOSCEventTargetCB(string _oscAddress,int __oscPort)
		{
			//Debug.Log("UniOSCEventTargetCB.Construktor");
			_oscPort = __oscPort;
			_receiveAllPorts = false;
			_receiveAllAddresses = false;
			if(!_oscAddresses.Contains(_oscAddress)) _oscAddresses.Add(_oscAddress);
			_useExplicitConnection = false;
		}

		public UniOSCEventTargetCB(UniOSCConnection con)
		{
			//Debug.Log("UniOSCEventTargetCB.Construktor");
			//oscPort = con.oscPort;
			_receiveAllPorts = false;
			_receiveAllAddresses = true;
			_useExplicitConnection = true;
			_explicitConnection = con;
			Awake();
		}

		public UniOSCEventTargetCB( string _oscAddress,UniOSCConnection con )
		{
			//Debug.Log("UniOSCEventTargetCB.Construktor");
			//oscPort = con.oscPort;
			_receiveAllPorts = false;
			_receiveAllAddresses = false;
			if(!_oscAddresses.Contains(_oscAddress)) _oscAddresses.Add(_oscAddress);
			_useExplicitConnection = true;
			_explicitConnection = con;
			Awake();
		}

		#endregion

		~UniOSCEventTargetCB()
		{
			//Debug.Log("UniOSCEventTargetCB.Destruktor");
			_Dispose(false);
		}

		public virtual void Awake()
		{
			//Debug.Log("UniOSCEventTargetCB.Awake");
		}

		/// <summary>
		/// Enable this instance.
		/// </summary>
		public virtual void Enable()
		{
			//Debug.Log("UniOSCEventTargetCB.Enable");
			_Init();
			_isEnabled = true;
		}

		/// <summary>
		/// Disable this instance.
		/// </summary>
		public virtual void Disable()
		{
			//Debug.Log("UniOSCEventTargetCB.Disable");
			_DisconnectFromDispatchers();
			_isEnabled = false;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="UniOSC.UniOSCEventTargetCB"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="UniOSC.UniOSCEventTargetCB"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="UniOSC.UniOSCEventTargetCB"/> so
		/// the garbage collector can reclaim the memory that the <see cref="UniOSC.UniOSCEventTargetCB"/> was occupying.</remarks>
		public void Dispose() // Implement IDisposable
		{
			_Dispose(true);
			GC.SuppressFinalize(this);
		}

		#region privateMethods
		private void _Init(){
			//if(_oscAddresses.Count == 0 && !_oscAddresses.Contains(oscAddress)) _oscAddresses.Add(oscAddress);
			
			_DisconnectFromDispatchers();
			_ConnectToDispatchers();
		}

        private void _SetupChanged()
        {
            // Debug.Log("UniOSCEventTargetCB_SetupChanged");
            if (_isEnabled)
            {
                
                Disable();
                Enable();
            }
            else
            {
                Enable();
                Disable();
            }
            //force refresh of status
            //enabled = !enabled;
            // enabled = !enabled;
        }

        /// <summary>
        /// This method forces a reconnection. Normally you don't need to call this method explicit but when you use this class for editor scripting you have to call this method when the playmode has changed. 
        /// </summary>
        public void ForceSetupChange()
        {
            _SetupChanged();
        }


        protected void _ConnectToDispatchers()
		{
			//Debug.Log("UniOSCEventTargetCB._ConnectToDispatchers");

			if(UniOSCConnection.Instances == null)return;
           
			bool _isAvailable = false;
			
			foreach(var con in UniOSCConnection.Instances){
				
				if(con == null)continue;
				
				if(_useExplicitConnection)
				{
					if(_explicitConnection == null){
						Debug.Log("explicitConnection is Null!");
						break;//return;	
					}
					if(con != _explicitConnection ) continue;
				}
				
				if( _useExplicitConnection == true || ( _receiveAllPorts || con.oscPort == _oscPort)){

					if(!ConnectToDict.ContainsKey(con)){
						//Debug.Log("ADDED:"+con.oscPort);
						_isAvailable = true;
						ConnectToDict.Add(con,new List<UniOSCMappingItem>());
						con.OSCMessageReceived-= _OnOSCMessageReceived;
						con.OSCMessageReceived+= _OnOSCMessageReceived;
						if( _useExplicitConnection && _explicitConnection != null){
							_explicitConnection.ConnectionInStatusChange-=_OnConnectionInStatusChanged;
							_explicitConnection.ConnectionInStatusChange+=_OnConnectionInStatusChanged;
						}
					}

				}

			}//for

			if(!_isAvailable){
				Debug.LogWarning("No OSCConnection that fit the settings! No OSCMessages will be received!");
			}

		}


		private void _DisconnectFromDispatchers()
		{
			//Debug.Log("UniOSCEventTargetCB._DisconnectFromDispatchers");
			foreach(var kvp in ConnectToDict){
				kvp.Key.OSCMessageReceived-= _OnOSCMessageReceived;
			}
			ConnectToDict.Clear();
			if(explicitConnection != null) explicitConnection.ConnectionInStatusChange-=_OnConnectionInStatusChanged;//saftey

		}

		private void _OnOSCMessageReceived(object sender,UniOSCEventArgs args)
		{
			//Debug.Log("UniOSCEventTargetCB._OnOSCMessageReceived");
			if(_isDispatchAble(args.Port,args.Address))
			{
				OnOSCMessageReceived(args);
				//sent out our event so that other classes are notified
				if(OSCMessageReceived != null) OSCMessageReceived(this, args);
			}

		}

		private void _OnConnectionInStatusChanged(UniOSCConnection con)
		{
			//Debug.Log("UniOSCEventTargetCB._OnConnectionInStatusChanged");
            if (!con.isConnected) return;
           
            if (_oscPort != con.oscPort)
            {
                _oscPort = con.oscPort; _oscPort = con.oscPort;
                //force refresh of status
                _SetupChanged();
            }

		}

		private bool _isDispatchAble(int port, string address){
			return ( _useExplicitConnection || _receiveAllPorts || _oscPort == port) && ( _receiveAllAddresses || _oscAddresses.Exists(adr => adr == address && !String.IsNullOrEmpty(address) ) );	 
		}


		private void _Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					Disable();
				}
				
				disposed = true;
			}
		}

		#endregion


		/// <summary>
		/// You should override this method in a subclass to handle the OSC data.
		/// </summary>
		/// <param name="args">The current OSCEventArgs object</param>
		public abstract void OnOSCMessageReceived(UniOSCEventArgs args) ;

	}
}

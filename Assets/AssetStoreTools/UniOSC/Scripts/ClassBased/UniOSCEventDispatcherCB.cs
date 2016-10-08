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

    [Serializable]
	public abstract class UniOSCEventDispatcherCB : IDisposable {

		#region private
		private bool disposed = false;
		
		//[SerializeField]
		private bool _isEnabled;
	

		private List<UniOSCConnection> _myOSCConnections = new List<UniOSCConnection>() ;

        protected OscPacket _OSCpkg;
		protected UniOSCEventArgs _OSCeArg;
		protected System.Timers.Timer _sendIntervalTimer;
		protected bool _isOSCDirty;
		protected object _mylock = new object();

        protected string _oscOutAddress;
        protected string _oscOutIPAddress;
        protected int _oscOutPort;
        //[SerializeField]
        protected bool _useExplicitConnection;
        protected UniOSCConnection _explicitConnection;
		
		protected  void _OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
		{
			//Debug.Log(string.Format("The Elapsed event was raised at {0}", e.SignalTime));
			lock(_mylock){
				_isOSCDirty = true;
			}	
			SendOSCMessage();
		}
		
		#endregion

		#region public

		public bool isEnabled{get{return _isEnabled;}}

		public string oscOutAddress {
            get
            {
                return _oscOutAddress;
            }
			set{
                bool isChanged = _oscOutAddress != value;
               _oscOutAddress = value;
               if(isChanged) _SetupChanged(true);
            }
		}

		public string oscOutIPAddress{
			get{
                return _oscOutIPAddress;

            }
            set{
                if (_useExplicitConnection)
                {
                    Debug.Log("<color=orange>When you use the explicit connection option you can't set the OSC Out IPAddress. It is bound to the settings of the used connection.</color>");
                    return;
                }
                bool isChanged = _oscOutIPAddress != value;
                _oscOutIPAddress = value;
                if (isChanged) _SetupChanged(false);
            }
		}
		public int oscOutPort{
            get {
                return _oscOutPort;
            }
			set {
                if (_useExplicitConnection)
                {
                    Debug.Log("<color=orange>When you use the explicit connection option you can't set the OSC Out IPAddress. It is bound to the settings of the used connection.</color>");
                    return;
                }
                bool isChanged = _oscOutPort != value;
                _oscOutPort = value;
                if (isChanged) _SetupChanged(false);
            }
		}

        public bool isBundle
        {
            get { return (_OSCpkg is OscBundle); }
        }

		public float sendInterval=100;

      
        public bool useExplicitConnection
        {
            get { return _useExplicitConnection; }
            set
            {
                bool isChanged = _useExplicitConnection != value;
                _useExplicitConnection = value;
                if (isChanged)
                {
                    if (_explicitConnection == null && _useExplicitConnection) Debug.Log("<color=orange>Current explicit connection is null!</color>");
                    _SetupChanged(false);
                }
            }

        }
        public  UniOSCConnection explicitConnection
        {
            get {return _explicitConnection;}
            set
            {
                bool isChanged = _explicitConnection != value;
                _explicitConnection = value;
                if (isChanged) _SetupChanged(false);
            }
        }
        
        /// <summary>
        /// Sets the explicitConnection property to the new UniOSCConnection when we have turned on the useExplicitConnection mode
        /// </summary>
        /// <param name="newCon"></param>
       /*
		public void SetExplicitConnection(UniOSCConnection newCon){
            bool isChanged = explicitConnection != newCon;
			if(useExplicitConnection) explicitConnection = newCon;
            if (isChanged) _SetupChanged(false);
		}
        */
		#endregion

		#region constructors
		
		public UniOSCEventDispatcherCB(string __oscOutAddress, string __oscOutIPAddress,int __oscPort )
		{
			_oscOutAddress = __oscOutAddress;
			_oscOutIPAddress = __oscOutIPAddress;
			_oscOutPort = __oscPort;
			_useExplicitConnection = false;
			Awake();
		}


		public UniOSCEventDispatcherCB(string __oscOutAddress, UniOSCConnection __explicitConnection)
		{
			_oscOutAddress = __oscOutAddress;
			if(__explicitConnection != null){
			_oscOutIPAddress = __explicitConnection.oscOutIPAddress;
			_oscOutPort  = __explicitConnection.oscOutPort;
			}
			_explicitConnection = __explicitConnection;
			_useExplicitConnection = true;
			Awake();
		}

		#endregion

		public virtual void Awake()
		{}

		/// <summary>
		/// Enable this instance.
		/// </summary>
		public virtual void Enable()
		{
           // Debug.Log("UniOSCEventDispatcherCB.Enable");
			_Init();
			_isEnabled = true;
		}

		/// <summary>
		/// Disable this instance.
		/// </summary>
		public virtual void Disable()
		{
           // Debug.Log("UniOSCEventDispatcherCB.Disable");
			_DisconnectFromOSCConnections();
            //ClearData();
			_isEnabled = false;
		}
		
		public virtual void OnDestroy(){
           // Debug.Log("UniOSCEventDispatcherCB.OnDestroy");
			_DisconnectFromOSCConnections();
		}

		private void _Init()
		{
           // Debug.Log("UniOSCEventDispatcherCB._Init");
			_myOSCConnections.Clear();
			_ConnectToOSCConnections();
            if (_OSCpkg == null) {
                _SetupOSCMessage(false);
            }
            else
            {
                _SetupOSCMessage(_OSCpkg.IsBundle);
            }
			
		}
        private void _SetupChanged(bool resetMessage) {
            // Debug.Log("UniOSCEventDispatcherCB._SetupChanged");
           if(resetMessage) _OSCpkg = null;
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
        /// This method forces an reconfiguration with the current settings. Normaly you don't need to call this method. 
        /// You can specify if you want to reset all the data from the OSC Message/Bundle that is used when sending data out.
        /// </summary>
        /// <param name="resetMessage"></param>
        public void ForceSetupChange(bool resetMessage)
        {
            _SetupChanged(resetMessage);
        }

		protected void _OnConnectionOutStatusChanged(UniOSCConnection con)
		{
			if(!con.isConnectedOut)return;
			//Debug.Log("_OnConnectionOutStatusChanged");
             bool isChanged = false;

            isChanged = (_oscOutIPAddress != con.oscOutIPAddress || _oscOutPort != con.oscOutPort);

            if (isChanged)
            {
                _oscOutIPAddress = con.oscOutIPAddress;
                _oscOutPort = con.oscOutPort;
                //force refresh
                _SetupChanged(false);
            }
            /*
			if (_isEnabled){
				Disable();
				Enable();
			}
             * */
		}


		protected void _ConnectToOSCConnections()
		{
			//Debug.Log("UniOSCEventDispatcherCB._ConnectToOSCConnections");
			//Autowire the connection if no OSC connection is used via the Component Inspector  

            if (UniOSCConnection.Instances == null || UniOSCConnection.Instances.Count==0) return;

			bool _isAvailable = false;
			
			foreach(var con in UniOSCConnection.Instances){
				
				if(con == null)continue;
				
				if(_useExplicitConnection)
				{
					if(_explicitConnection == null){
                        Debug.LogWarning("<color=orange>explicitConnection is Null!</color>");
						break;//return;	
					}
					if(con != _explicitConnection ) continue;
				}

				if(_useExplicitConnection == true || (_oscOutPort == con.oscOutPort && _oscOutIPAddress == con.oscOutIPAddress)){
					//Debug.Log("con.OSCOutIPAddress:"+con.oscOutIPAddress +" con.Outport:"+con.oscOutPort);
					if(!_myOSCConnections.Contains(con) ){
						_isAvailable = true;
						_myOSCConnections.Add(con);

						if( _useExplicitConnection && _explicitConnection != null){
							_oscOutIPAddress = _explicitConnection.oscOutIPAddress;
							_oscOutPort = _explicitConnection.oscOutPort;
							_explicitConnection.ConnectionOutStatusChange-=_OnConnectionOutStatusChanged;
							_explicitConnection.ConnectionOutStatusChange+=_OnConnectionOutStatusChanged;
						}

					}
				}




			}//for

			if(!_isAvailable){
				Debug.LogWarning("No OSCConnection that fit the settings! No OSCMessages will be received!");
			}


			
		}
		
		protected void _DisconnectFromOSCConnections(){
			_myOSCConnections.Clear();
			if(_explicitConnection != null) _explicitConnection.ConnectionOutStatusChange-=_OnConnectionOutStatusChanged;//saftey
		}


		protected void _SetupOSCMessage (bool _isBundle)
		{
           // Debug.Log("UniOSCEventDispatcherCB._SetupOSCMessage:" + _isBundle);
			if(String.IsNullOrEmpty(_oscOutAddress) ||  !_oscOutAddress.StartsWith("/"))_oscOutAddress = "/"+_oscOutAddress;//
            if (_OSCpkg == null) {
                if (_isBundle) { 
                   // Debug.Log("New Bundle"); 
                    _OSCpkg = new OscBundle(); 
                } else { 
                   // Debug.Log("New OscMessage"); 
                    _OSCpkg = new OscMessage(_oscOutAddress);
                }
                
            }
            else
            {
                //_OSCpkg is already there so flip type if necessary
                if (_isBundle && _OSCpkg is OscMessage) 
                { 
                   // Debug.Log("New Bundle");
                    _OSCpkg = new OscBundle(); 
                }
                else if (!_isBundle && _OSCpkg is OscBundle) 
                {
                   // Debug.Log("New OscMessage");
                    _OSCpkg = new OscMessage(_oscOutAddress);
                }
            }
			
			_OSCeArg = new UniOSCEventArgs(_oscOutPort,_OSCpkg);
			_OSCeArg.IPAddress = _oscOutIPAddress;
		}

		/// <summary>
		/// Sets the bundle mode.
		/// </summary>
		/// <remarks>You can change the mode at any time but you have to be careful what data you trying to append with <see cref="UniOSC.UniOSCEventDispatcherCB.AppendData(object)"/></remarks>
		/// <param name="_isBundle">If set to <c>true</c> is bundle.</param>
        public void SetBundleMode(bool _isBundle)
        {
            if (_OSCpkg != null)
            {
                if ( ((_OSCpkg is OscMessage) && _isBundle) || ((_OSCpkg is OscBundle) && !_isBundle) ) _OSCpkg = null;
            }
             _SetupOSCMessage(_isBundle);
           
        }
		
		
		protected void _SendOSCMessage(UniOSCEventArgs args)
		{
			foreach(var c in _myOSCConnections){
				if(c!= null) c.SendOSCMessage(this,args);
			}
		}
		
		/// <summary>
		/// Sends the OSC message.
		/// </summary>
		public void SendOSCMessage()
		{
			_SendOSCMessage(_OSCeArg);
		}

		/// <summary>
		/// Appends the data.
		/// Depending on your bundle mode the AppendData method works in a different way.
		/// If you use bundles you can append multiple OscMessages.
		/// If you don't use bundles (default) you append data to your OscMessage
        /// We only can append data types that are supported by the OSC specification:
        ///(Int32,Int64,Single,Double,String,Byte[],OscTimeTag,Char,Color,Boolean)
		/// </summary>
		/// <param name="_data">_data.</param>
		public void AppendData(object _data)
		{
            OscBundle bundle = _data as OscBundle;
           // Debug.Log("UniOSCEventDispatcherCB.AppendData:" + (bundle != null).ToString());
            if (_OSCpkg == null) 
            {
               // if (bundle == null) { _SetupOSCMessage(false); } else { _SetupOSCMessage(true); }
                _SetupOSCMessage(bundle != null);
               // return;
            }

            if (_OSCpkg is OscBundle) {

                if (bundle == null) {
                    if (_data is OscMessage) { ((OscBundle)_OSCpkg).Append(_data); } else { Debug.LogWarning("<color=orange>You can only append a OscMessage to a OscBundle</color>"); }
                  
                }
                else {
                    Debug.LogWarning("<color=orange>You  can't append a OSCBundle to a OSCBundle</color>");        
                }           
                        
            }

            if (_OSCpkg is OscMessage)
            {

                if (bundle == null) {

                    if (_data is OscMessage) { Debug.LogWarning("<color=orange>You can't append a OSCMessage to a OSCMessage</color>"); } else { _OSCpkg.Append(_data); }
                   
                }
                
                else {
                    Debug.LogWarning("<color=orange>You can't append a Bundle to a OSCMessage</color>");
                   
                }
               
            }
			
		}


		/// <summary>
		/// Clears all data.
		/// </summary>
		public void ClearData(){
           // Debug.Log("UniOSCEventDispatcherCB.ClearData");
			if(_OSCpkg == null)return;
            if (_OSCpkg is OscMessage)
            {
                ((OscMessage) _OSCpkg).ClearData();
            }
            else if (_OSCpkg is OscBundle)
            {
                _OSCpkg = new OscBundle();
                _SetupOSCMessage(true);
            }
			
		}
        public void UpdateDataAt(int index, object value)
        {
            Debug.Log("UpdateDataAt");
            if (_OSCpkg is OscMessage)
            {
                ((OscMessage)_OSCpkg).UpdateDataAt(index,value);
            }
            else if (_OSCpkg is OscBundle)
            {
               // _OSCpkg = new OscBundle();
                if (value is OscBundle) 
                {
                    _OSCpkg = value as OscBundle;
                }
                else
                {
                   //Debug.Log("Data is not a Bundle");
                    if (value is OscMessage) 
                    {
                        if (((OscBundle)_OSCpkg).Messages.Count >= index)
                        {
                            ((OscBundle)_OSCpkg).Messages[index] = value as OscMessage;
                        }
                        
                    }
                    else
                    {
                        Debug.Log("You can only add a OSCMessage to a OSCBundle");
                    }
                }
               
            }
        }


		/// <summary>
		/// Starts the send interval timer.
		/// This is useful when you need to send OSC data frequently. With the sendInterval property you specify the interval in milliseconds
		/// </summary>
		public void StartSendIntervalTimer()
		{
			if(_sendIntervalTimer == null){
				_sendIntervalTimer = new System.Timers.Timer();
			}
			_sendIntervalTimer.Interval = sendInterval;
			_sendIntervalTimer.Elapsed-= _OnTimedEvent;
			_sendIntervalTimer.Elapsed+= _OnTimedEvent;
			_sendIntervalTimer.Enabled = true;
		}

		/// <summary>
		/// Stops the send interval timer.
		/// </summary>
		public void StopSendIntervalTimer()
		{
			if(_sendIntervalTimer == null)return;
			_sendIntervalTimer.Stop();
			_sendIntervalTimer.Elapsed-= _OnTimedEvent;
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

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting  resources.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="UniOSC.UniOSCEventDispatcherCB"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="UniOSC.UniOSCEventDispatcherCB"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="UniOSC.UniOSCEventDispatcherCB"/>
		/// so the garbage collector can reclaim the memory that the <see cref="UniOSC.UniOSCEventDispatcherCB"/> was occupying.</remarks>
		public void Dispose() // Implement IDisposable
		{
			_Dispose(true);
			GC.SuppressFinalize(this);
		}

	}
}

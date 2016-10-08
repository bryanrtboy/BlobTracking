/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using OSCsharp.Data;


namespace UniOSC{
	/// <summary>
	/// This is the abstract class you should subclass from when you want to sent OSC data
	/// </summary>
	[ExecuteInEditMode]
	public abstract class UniOSCEventDispatcher : MonoBehaviour {

		#region public
   
        public string oscOutAddress
        {
            get
            {
                return _oscOutAddress;
            }
            set
            {
                bool isChanged = _oscOutAddress != value;
                _oscOutAddress = value;
                if (isChanged) _SetupChanged(true);
            }
        }

        public string oscOutIPAddress
        {
            get
            {
                return _oscOutIPAddress;

            }
            set
            {
                if (useExplicitConnection)
                {
                    Debug.Log("<color=orange>When you use the explicit connection option you can't set the OSC Out IPAddress. It is bound to the settings of the used connection.</color>");
                    return;
                }
                bool isChanged = _oscOutIPAddress != value;
                _oscOutIPAddress = value;
                if (isChanged) _SetupChanged(false);
            }
        }
        public int oscOutPort
        {
            get
            {
                return _oscOutPort;
            }
            set
            {
                if (useExplicitConnection)
                {
                    Debug.Log("<color=orange>When you use the explicit connection option you can't set the OSC Out Port. It is bound to the settings of the used connection.</color>");
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

		[HideInInspector]
        //[Tooltip("Only for continuous senders relevant")]
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

        public UniOSCConnection explicitConnection
        {
            get { return _explicitConnection; }
            set
            {
                bool isChanged = _explicitConnection != value;
                _explicitConnection = value;
                if (isChanged) _SetupChanged(false);
            }
        }
		#endregion

       

		#region private
        [HideInInspector, SerializeField]
        protected string _oscOutAddress = "/";
        [HideInInspector, SerializeField]
        protected string _oscOutIPAddress;
        [HideInInspector, SerializeField]
        protected int _oscOutPort;
        [HideInInspector, SerializeField]
        protected bool _useExplicitConnection;
        [HideInInspector, SerializeField]
        protected UniOSCConnection _explicitConnection;

        protected OscPacket _OSCpkg;
		//protected OscMessage _OSCmsg ;
		protected UniOSCEventArgs _OSCeArg;
		protected System.Timers.Timer _sendIntervalTimer;
		protected bool _isOSCDirty;
		protected object _mylock = new object();
		[SerializeField,HideInInspector]
		protected bool _drawDefaultInspector = true;


		protected  void _OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
		{
			//Debug.Log(string.Format("The Elapsed event was raised at {0}", e.SignalTime));
			lock(_mylock){
				_isOSCDirty = true;
			}	
		}

		[SerializeField,HideInInspector]
		protected List<UniOSCConnection> _myOSCConnections = new List<UniOSCConnection>() ;
		#endregion


		public virtual void Awake () {
			//Debug.Log("UniOSCEventDispatcher.Awake");
		}
		public virtual void Start () {

		}

		public virtual void OnEnable()
		{
            //Debug.Log("UniOSCEventDispatcher.OnEnable::"+gameObject.name);
			_Init();
		}

		private void _Init()
		{
			_myOSCConnections.Clear();
			_ConnectToOSCConnections();
			
            if (_OSCpkg == null)
            {
                _SetupOSCMessage(false);
            }
            else
            {
                _SetupOSCMessage(_OSCpkg.IsBundle);
            }

			#if UNITY_EDITOR
			if(!Application.isPlaying){
				UnityEditor.EditorApplication.update -= _Update;
				UnityEditor.EditorApplication.update += _Update;
			}
			#endif
		}

        private void _SetupChanged(bool resetMessage)
        {
            //Debug.Log("UniOSCEventDispatcher._SetupChanged");
            if (resetMessage) _OSCpkg = null;
          //  if (enabled)
            {
                //force refresh of status
                enabled = !enabled;
                enabled = !enabled;
            }
        }

        /// <summary>
        /// This method is mainly for the EventDispatcherEditor to force an update of the internal message setup
        /// </summary>
        /// <param name="resetMessage"></param>
        public void ForceSetupChange(bool resetMessage)
        {
            _SetupChanged(resetMessage);
        }

		protected virtual void _Update()
		{
			//Update();
		}


		protected void _OnConnectionOutStatusChanged(UniOSCConnection con)
		{
           // Debug.Log("UniOSCEventDispatcher._OnConnectionOutStatusChanged");
			//if(!con.isConnectedOut)return;

            bool isChanged = false;

            isChanged = (_oscOutIPAddress != con.oscOutIPAddress || _oscOutPort != con.oscOutPort);

            if (isChanged)
            {
                _oscOutIPAddress = con.oscOutIPAddress;
                _oscOutPort = con.oscOutPort;
                //force refresh of status
                _SetupChanged(false);
            }

			
            /*
			enabled = !enabled;
			enabled = !enabled;
             */
		}

		protected void _ConnectToOSCConnections()
		{
			//Debug.Log("UniOSCEventDispatcher._ConnectToOSCConnections.");
			//Autowire the connection if no OSC connection is used via the Component Inspector  

			if(UniOSCConnection.Instances == null)return;
            //Debug.Log("##:" + useExplicitConnection);
            //Debug.Log("##:" + _explicitConnection.name);
            //Debug.Log("##:" + UniOSCConnection.Instances.Count);
			foreach(var con in UniOSCConnection.Instances){
				
				if(con == null)continue;
				
				if(useExplicitConnection)
				{
					if(_explicitConnection == null){
                        Debug.LogWarning("<color=orange>explicitConnection is Null!</color>");
						break;//return;	
					}
					if(con != _explicitConnection ) continue;
				}
				
				if(useExplicitConnection == true || (_oscOutPort == con.oscOutPort && _oscOutIPAddress == con.oscOutIPAddress)){
					//Debug.Log("con.OSCOutIPAddress:"+con.oscOutIPAddress +" con.Outport:"+con.oscOutPort);
					if(!_myOSCConnections.Contains(con) ){
						//_isAvailable = true;
						_myOSCConnections.Add(con);
						
						if( useExplicitConnection && _explicitConnection != null){
							_oscOutIPAddress = _explicitConnection.oscOutIPAddress;
							_oscOutPort = _explicitConnection.oscOutPort;
							_explicitConnection.ConnectionOutStatusChange-=_OnConnectionOutStatusChanged;
							_explicitConnection.ConnectionOutStatusChange+=_OnConnectionOutStatusChanged;
						}
						
					}
				}
				
				
				
				
			}//for



			}
			
		protected void _DisconnectFromOSCConnections(){
			_myOSCConnections.Clear();
			if(_explicitConnection != null) _explicitConnection.ConnectionOutStatusChange-=_OnConnectionOutStatusChanged;//saftey
		}

		public virtual void OnDestroy(){
			_DisconnectFromOSCConnections();
		}

		public virtual void OnDisable(){
		//	Debug.Log("UniOSCEventDispatcher.OnDisable");
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.update -= _Update;
			#endif
			_DisconnectFromOSCConnections();
           // ClearData();
		}
       
        protected void _SetupOSCMessage(bool _isBundle)
        {
           // Debug.Log("UniOSCEventDispatcher._SetupOSCMessage:" + isBundle);
            if (String.IsNullOrEmpty(_oscOutAddress) || !_oscOutAddress.StartsWith("/")) _oscOutAddress = "/" + _oscOutAddress;
            if (_OSCpkg == null)
            {
                if (_isBundle) { 
                   // Debug.Log("New Bundle"); 
                    _OSCpkg = new OscBundle(); 
                } 
                else 
                { 
                    //Debug.Log("New OscMessage");
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
           
            _OSCeArg = new UniOSCEventArgs(_oscOutPort, _OSCpkg);
            _OSCeArg.IPAddress = _oscOutIPAddress;

        }

		protected void _SendOSCMessage(UniOSCEventArgs args)
		{
			foreach(var c in _myOSCConnections){
				if(c!= null) c.SendOSCMessage(this,args);
			}
		}

		/// <summary>
		/// Sets the bundle mode.
		/// </summary>
		/// <remarks>You can change the mode at any time but you have to be careful what data you trying to append with <see cref="UniOSC.UniOSCEventDispatcher.AppendData(object)"/></remarks>
		/// <param name="_isBundle">If set to <c>true</c> is bundle.</param>
		public void SetBundleMode(bool _isBundle)
		{
			if (_OSCpkg != null)
			{
				if ( ((_OSCpkg is OscMessage) && _isBundle) || ((_OSCpkg is OscBundle) && !_isBundle) ) _OSCpkg = null;
			}
			_SetupOSCMessage(_isBundle);
			
		}

		/// <summary>
		/// Sends the OSC message.
		/// </summary>
        public virtual void SendOSCMessage()
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
           // Debug.Log("UniOSCEventDispatcher.AppendData:" + (bundle != null).ToString());
            if (_OSCpkg == null)
            {
               // if (bundle == null) { _SetupOSCMessage(false); } else { _SetupOSCMessage(true); }
               // return;
                _SetupOSCMessage(bundle != null);
            }

            if (_OSCpkg is OscBundle)
            {

                if (bundle == null)
                {
                    if (_data is OscMessage) { ((OscBundle)_OSCpkg).Append(_data); } else { Debug.LogWarning("<color=orange>You can only append a OscMessage to a OscBundle</color>"); }

                }
                else
                {
                    Debug.LogWarning("<color=orange>You  can't append a OSCBundle to a OSCBundle</color>");
                }

            }

            if (_OSCpkg is OscMessage)
            {

                if (bundle == null)
                {

                    if (_data is OscMessage) { Debug.LogWarning("<color=orange>You can't append a OSCMessage to a OSCMessage</color>"); } else { _OSCpkg.Append(_data); }
                }

                else
                {
                    Debug.LogWarning("<color=orange>You can't append a Bundle to a OSCMessage</color>");

                }

            }

		
		}

		public void ClearData(){
           // Debug.Log("UniOSCEventDispatcher.ClearData:" + (_OSCpkg == null).ToString());
            if (_OSCpkg == null) return;
            if (_OSCpkg is OscMessage)
            {
                ((OscMessage)_OSCpkg).ClearData();
            }
            else if(_OSCpkg is OscBundle)
            {
                _OSCpkg = new OscBundle();
                _SetupOSCMessage(true);
            }
			
		}

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

		public void StopSendIntervalTimer()
		{
			if(_sendIntervalTimer == null)return;
			_sendIntervalTimer.Stop();
			_sendIntervalTimer.Elapsed-= _OnTimedEvent;
		}
	}
}
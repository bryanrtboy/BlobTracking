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
using System.Net;
using System.Text;
using System.Threading;
using System.Linq;


namespace UniOSC{

	/// <summary>
	/// This class is responsible for all the network related tasks. It is a wrapper for OSCsharp and handles the event system for the Unity components.
	/// </summary>
	[AddComponentMenu("UniOSC/OSCConnection")]
	[ExecuteInEditMode]
	[Serializable]
	public class UniOSCConnection : MonoBehaviour {


		#region static
	
		public static string localIPAddress = null;
		public static bool isOSCLearning = false;
		public static bool isEditorEnabled = false;     

		public static List<UniOSCConnection> Instances{get{return _connectionInstances;}}
		public static List<int> AvailableINPorts{get{return _AvailableINPorts;}}
		public static List<int> AvailableOUTPorts{get{return _AvailableOUTPorts;}}
		public static List<string> AvailableOUTIPAddresses{get{return _AvailableOUTIPAddresses;}}

		private static List<UniOSCConnection> _connectionInstances;
		private static List<int> _AvailableINPorts;
		private static List<int> _AvailableOUTPorts;
		private static List<string> _AvailableOUTIPAddresses;

		#endregion static

		#region public

         [SerializeField]
        private OSCsharp.Net.TransmissionType _transmissionTypeIn;//only unicast & multicast?
         public OSCsharp.Net.TransmissionType transmissionTypeIn
         {
             get { return _transmissionTypeIn; }
             set
             {
                 bool isChanged = _transmissionTypeIn != value;
                 _transmissionTypeIn = value;
                 if (isChanged) Update_AvailableOSCSettings();
                 if (isChanged) _SetupChanged_IN();
             }
         }

         [SerializeField]
        private OSCsharp.Net.TransmissionType _transmissionTypeOut;
         public OSCsharp.Net.TransmissionType transmissionTypeOut
         {
             get { return _transmissionTypeOut; }
             set
             {
                 bool isChanged = _transmissionTypeOut != value;
                 _transmissionTypeOut = value;
                 if (isChanged) Update_AvailableOSCSettings();
                 if (isChanged) _SetupChanged_OUT();
             }
         }



		public bool autoConnectOSCIn = true;
       // [Range(0, UniOSCUtils.MAXPORT)]
        public int oscPort {
            get { return _oscPort; }
            set {
                bool isChanged = _oscPort != value;
                _oscPort = value;
               if(isChanged) Update_AvailableOSCSettings();
               if (isChanged) _SetupChanged_IN();
            }
        }

     
		//public int oscPort = 8000;
         [Range(0, UniOSCUtils.MAXPORT)]
        [SerializeField]
        private int _oscPort = 8000;


         public string oscInIPAddress
         {
             get
             {
                 string ip = null;
                 switch (transmissionTypeIn)
                 {
                     case OSCsharp.Net.TransmissionType.Unicast:
                     case OSCsharp.Net.TransmissionType.Broadcast:
                     case OSCsharp.Net.TransmissionType.LocalBroadcast:
                         ip =localIPAddress ;
                         break;
                     case OSCsharp.Net.TransmissionType.Multicast:
                         ip = _oscMulticastIPAddress;                                             
                         break;                   
                 }

                 return ip;
             }

             set
             {
                 bool isChanged = false;

                 switch (transmissionTypeIn)
                 {
                     case OSCsharp.Net.TransmissionType.Unicast:
                     case OSCsharp.Net.TransmissionType.Broadcast:
                     case OSCsharp.Net.TransmissionType.LocalBroadcast:
                         if (value != localIPAddress)
                         Debug.Log("You can only set an IP Address when you use Multicast as transmission type!");
                         break;
                     case OSCsharp.Net.TransmissionType.Multicast:
                         isChanged = _oscMulticastIPAddress != value;
                         _oscMulticastIPAddress = value;
                         _ValidateOscMulticastIPAddress();
                         break;                                         
                 }

                 if (isChanged) Update_AvailableOSCSettings();
                 if (isChanged) _SetupChanged_IN();

             }

         }

        
         public IPAddress oscInIPAddressAsIPAddress{
             get {
                 IPAddress ip = null;

                 switch (transmissionTypeIn)
                 {
                     case OSCsharp.Net.TransmissionType.Unicast:
                         ip = _oscInIPAddressAsIPAddress;
                         break;
                     case OSCsharp.Net.TransmissionType.Multicast:
                         ip = _oscMulticastIPAddressAsIPAddress;
                         break;
                 }

                 return ip;
                }
           
         }

         [SerializeField, HideInInspector]
         private IPAddress _oscInIPAddressAsIPAddress;
         [SerializeField, HideInInspector]
         private IPAddress _oscMulticastIPAddressAsIPAddress;            


        [SerializeField, HideInInspector]
        private bool _isValidOscIPAddress;
            
         [SerializeField, HideInInspector]
         private bool _isValidOscMulticastIPAddress;
         [SerializeField]
         private string _oscMulticastIPAddress = "224.0.1.0";//224.0.0.0 - 239.255.255.255 !

         private void _ValidateOscIPAddress()
         {
             //Debug.Log("OSCConnection.ValidateOscIPAddress");
             IPAddress addr;

             _isValidOscIPAddress = UniOSCUtils.ValidateIPAddress(localIPAddress, out addr);
             _oscInIPAddressAsIPAddress = addr;
         }
         private void _ValidateOscMulticastIPAddress()
         {
             //Debug.Log("OSCConnection.ValidateOscMulticastIPAddress");
             IPAddress addr;

             _isValidOscMulticastIPAddress = UniOSCUtils.ValidateIPAddress(_oscMulticastIPAddress, out addr);
             _oscMulticastIPAddressAsIPAddress = addr;

             if (_isValidOscMulticastIPAddress)
             {
                 //Second validation 
                 _isValidOscMulticastIPAddress = UniOSCUtils.RegexMatch(_oscMulticastIPAddress, UniOSCUtils.MULTICASTREGEX);
             }
         }

         public void ValidateOscInIPAddress()
         { 
               switch (transmissionTypeIn)
                 {
                     case OSCsharp.Net.TransmissionType.Unicast:
                         _ValidateOscIPAddress();
                         break;
                     case OSCsharp.Net.TransmissionType.Multicast:
                         _ValidateOscMulticastIPAddress();
                         break;
                 }
         }

         public bool hasValidOscIPAddress
         {
             get
             {
                 bool validIP = false;

                 switch (transmissionTypeIn)
                 {
                     case OSCsharp.Net.TransmissionType.Unicast:
                         validIP = _isValidOscIPAddress;
                         break;
                     case OSCsharp.Net.TransmissionType.Multicast:
                         validIP = _isValidOscMulticastIPAddress;
                         break;                  
                 }

                 return validIP;
             }
         }




		[HideInInspector]
		public bool oscOut= true;
		
		public bool autoConnectOSCOut = true;
		[HideInInspector]
		public bool foldoutOSCOut = true;
		[HideInInspector]
		public bool foldoutOSCIn = true;

        public string oscOutIPAddress
        {
            get { 

               // return _oscOutIPAddress;
                string ip = null;
                switch (transmissionTypeOut)
                {
                    case OSCsharp.Net.TransmissionType.Unicast:
                        ip = _oscOutIPAddress;
                        break;
                    case OSCsharp.Net.TransmissionType.Multicast:
                        ip = _oscOutMulticastIPAddress;
                        break;
                    case OSCsharp.Net.TransmissionType.Broadcast:
                    case OSCsharp.Net.TransmissionType.LocalBroadcast:
                        ip = IPAddress.Broadcast.ToString();
                        break;
                }

                return ip;

            }

            set
            {
                bool isChanged = false;
               
                switch (transmissionTypeOut)
                {
                    case OSCsharp.Net.TransmissionType.Unicast:
                        isChanged = _oscOutIPAddress != value;
                        _oscOutIPAddress = value;
                        if (isChanged)
                        {
                            _ValidateOscOutIPAddress();
                        }
                         
                        break;
                    case OSCsharp.Net.TransmissionType.Multicast:
                        isChanged = _oscOutMulticastIPAddress != value;
                         _oscOutMulticastIPAddress = value;
                         if (isChanged)
                         {
                             _ValidateOscOutMulticastIPAddress();                          
                         }
                        break;
                    case OSCsharp.Net.TransmissionType.Broadcast:
                    case OSCsharp.Net.TransmissionType.LocalBroadcast:
                        if (value != IPAddress.Broadcast.ToString())
                        {
                            Debug.Log("The Broadcast address can't be set. It's always " + IPAddress.Broadcast.ToString());
                        }
                       
                        break;
                }
                             
                if (isChanged) Update_AvailableOSCSettings();
                if (isChanged) _SetupChanged_OUT();
            }
        }

        public IPAddress oscOutIPAddressAsIPAddress
        {
            get
            {
                IPAddress ip = null;

                switch (transmissionTypeOut)
                {
                    case OSCsharp.Net.TransmissionType.Unicast:
                        ip = _oscOutIPAddressAsIPAddress;
                        break;
                    case OSCsharp.Net.TransmissionType.Multicast:
                        ip = _oscOutMulticastIPAddressAsIPAddress;
                        break;
                    case OSCsharp.Net.TransmissionType.Broadcast:
                    case OSCsharp.Net.TransmissionType.LocalBroadcast:
                        ip = IPAddress.Broadcast;
                        break;
                }

                return ip;
            }

        }

        [SerializeField, HideInInspector]
        private IPAddress _oscOutIPAddressAsIPAddress;
        [SerializeField, HideInInspector]
        private IPAddress _oscOutMulticastIPAddressAsIPAddress;            

        [SerializeField, HideInInspector]
        private bool _isValidOscOutIPAddress;
        [SerializeField]
        private string _oscOutIPAddress="127.0.0.1";
        [SerializeField, HideInInspector]
        private bool _isValidOscOutMulticastIPAddress;
        [SerializeField]
        private string _oscOutMulticastIPAddress = "224.0.1.0";//224.0.0.0 - 239.255.255.255 !

        private void _ValidateOscOutIPAddress()
        {
            IPAddress addr;
            _isValidOscOutIPAddress = UniOSCUtils.ValidateIPAddress(_oscOutIPAddress, out addr);
            _oscOutIPAddressAsIPAddress = addr;
        }
        private void _ValidateOscOutMulticastIPAddress()
        {
            IPAddress addr;

            _isValidOscOutMulticastIPAddress = UniOSCUtils.ValidateIPAddress(_oscOutMulticastIPAddress, out addr);
           _oscOutMulticastIPAddressAsIPAddress = addr;

            if (_isValidOscOutMulticastIPAddress)
            {
                //Second validation 
                _isValidOscOutMulticastIPAddress = UniOSCUtils.RegexMatch(_oscOutMulticastIPAddress, UniOSCUtils.MULTICASTREGEX);
            }
        }
        public void ValidateOscOutIPAddress()
        {
           // Debug.Log("UniOSCConnection.ValidateOscOutIPAddress");
            switch (transmissionTypeOut)
            {
                case OSCsharp.Net.TransmissionType.Unicast:
                    _ValidateOscOutIPAddress();
                    break;
                case OSCsharp.Net.TransmissionType.Multicast:
                    _ValidateOscOutMulticastIPAddress();
                    break;
            }
            
           
        }
         public bool hasValidOscOutIPAddress{
             get{
                 bool validIP = false;

                 switch (transmissionTypeOut)
                 {
                     case OSCsharp.Net.TransmissionType.Unicast:
                         validIP = _isValidOscOutIPAddress;
                         break;
                     case OSCsharp.Net.TransmissionType.Multicast:
                         validIP = _isValidOscOutMulticastIPAddress;
                         break;
                     case OSCsharp.Net.TransmissionType.Broadcast:
                     case OSCsharp.Net.TransmissionType.LocalBroadcast:
                        
                         validIP = true;//we always use  IPAddress.Broadcast (255.255.255.255 )so we know that is is valid
                         break;
                 }

                 return validIP;
             }
         }
                    
               

        public int oscOutPort
        {
            get { return _oscOutPort; }
            set
            {
                bool isChanged = _oscOutPort != value;
                _oscOutPort = value;
                if (isChanged) Update_AvailableOSCSettings();
                if (isChanged) _SetupChanged_OUT();
            }
        }

        [Range(0, UniOSCUtils.MAXPORT)]
        [SerializeField]
        private int _oscOutPort = 9000;

		public bool isConnected{get{return _connected;}}
		public bool isConnectedOut{get{return _connectedOut;}}
		public bool redrawFlag;//only to force the CustomInspector to redraw

		[HideInInspector]
		public bool dispatchOSC= true;
		[HideInInspector]
		public bool dispatchOSCOut= true;

		[SerializeField]
		public List<UniOSCMappingFileObj> oscMappingFileObjList = new List<UniOSCMappingFileObj>();

		public bool hasOSCMappingFileAttached{
			get{
                if (oscMappingFileObjList == null) return false;
                return oscMappingFileObjList.FindAll(mfo => mfo != null).Count > 0;
            }
		}

		[SerializeField]
		public List<UniOSCSessionFileObj> oscSessionFileObjList = new List<UniOSCSessionFileObj>();

		public bool hasOSCSessionFileAttached{
			get{
                if (oscSessionFileObjList == null) return false;
                return oscSessionFileObjList.FindAll(sfo => sfo != null).Count > 0;
            }
		}

		public bool SendSessionDataOnStart;


		#endregion public

		#region private
		private UniOSCReceiver _oscReceiver;
		private UniOSCTransmitter _oscTransmitter;
		private bool _connected;
		private bool _connectedOut;
	
		private Queue<UniOSCEventArgs> _eventQueue = new Queue<UniOSCEventArgs>();
		private UnityEngine.Object _eventLock = new UnityEngine.Object();

		private Queue<UniOSCEventArgs> _sessionEventQueue = new Queue<UniOSCEventArgs>();
		private UnityEngine.Object _sessionEventLock = new UnityEngine.Object();
		private int sessionCounter;

		private GUIStyle _gs;
		private GUIStyle _tfgs;



		#endregion private


		
		#region Events

		public event EventHandler<UniOSCEventArgs> OSCMessageReceivedRaw;
		public event EventHandler<UniOSCEventArgs> OSCMessageReceived;
		//public event EventHandler<ExceptionEventArgs> OSCErrorOccured;
		public event EventHandler<UniOSCEventArgs> OSCMessageSend;

		public event Action<UniOSCConnection> ConnectionInStatusChange;
		public event Action<UniOSCConnection> ConnectionOutStatusChange;
		
		
		#endregion

		#region Start

		/// <summary>
		/// Init this instance.
		/// Is called from Awake and OSCAutoRun
		/// </summary>
		public static void Init(){
			if(_connectionInstances == null) _connectionInstances = new List<UniOSCConnection>();
			if(_AvailableINPorts == null)_AvailableINPorts = new List<int>();
			if(_AvailableOUTPorts == null)_AvailableOUTPorts = new List<int>();
			if(_AvailableOUTIPAddresses == null) _AvailableOUTIPAddresses = new List<string>();
            if (localIPAddress == null) localIPAddress = UniOSCUtils.GetLocalIPAddress();             

		}
	
	
		public void Awake(){
            //Debug.Log("UniOSCConnection.Awake");
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            DontDestroyOnLoad(gameObject);
#endif
			Init();

            _ValidateOscIPAddress();
            _ValidateOscMulticastIPAddress();
           
            _ValidateOscOutIPAddress();
            _ValidateOscOutMulticastIPAddress();
			_Add_connectionInstance();
		}

		public IEnumerator Start () {
            //Debug.Log("UniOSCConnection.Start");
			//bug with Unity 4.3.4 and Android
			//We need some time otherwise adb.exe seems to block our ports. (cmd check with:  netstat -a -n -o)
			//http://issuetracker.unity3d.com/issues/entering-play-mode-for-the-first-time-also-launches-adb-dot-exe
			yield return new WaitForSeconds(0.1f);
		
			if(Application.isPlaying){
				if(autoConnectOSCIn) ConnectOSC();
				if(autoConnectOSCOut) ConnectOSCOut();
			}else{
				if(_connected) ConnectOSC();
				if(_connectedOut) ConnectOSCOut();
			}
			yield break;
		}

		void OnEnable(){
           // Debug.Log("OSCConnection.OnEnable");
			lock(_eventLock){
				_eventQueue.Clear();
			}
			lock(_sessionEventLock){
				_sessionEventQueue.Clear();
			}

			#if UNITY_EDITOR
			if(!Application.isPlaying){
				UnityEditor.EditorApplication.update -= _Update;
				UnityEditor.EditorApplication.update += _Update;
				_Add_connectionInstance();
			}
			#endif

		}

		private  void _Add_connectionInstance(){
			//Debug.Log("UniOSCConnection._Add_connectionInstance");
			if(_connectionInstances == null){
				_connectionInstances = new List<UniOSCConnection>();
			}

			if(!_connectionInstances.Contains(this) ) {
				_connectionInstances.Add(this);
				Update_AvailableOSCSettings();
			}

		}

		/// <summary>
		/// Updates the available ports.
		/// Should be called when a OSCConnection changes the Port.
		///
		/// </summary>
		public static void Update_AvailableOSCSettings(){
            //Debug.Log("UniOSCConnection.Update_AvailableOSCSettings");
			if(_AvailableINPorts == null){
				_AvailableINPorts = new List<int>();
			}
			_AvailableINPorts.Clear();

			if(_AvailableOUTPorts == null){
				_AvailableOUTPorts = new List<int>();
			}
			_AvailableOUTPorts.Clear();

			if(_AvailableOUTIPAddresses == null){
				_AvailableOUTIPAddresses = new List<string>();
			}
			_AvailableOUTIPAddresses.Clear();

			foreach(var con in _connectionInstances){
				if(!_AvailableINPorts.Contains(con.oscPort) ) {
					_AvailableINPorts.Add(con.oscPort);
				}

				if(!_AvailableOUTPorts.Contains(con.oscOutPort) ) {
					_AvailableOUTPorts.Add(con.oscOutPort);
				}

				if(!_AvailableOUTIPAddresses.Contains(con.oscOutIPAddress) ) {
					_AvailableOUTIPAddresses.Add(con.oscOutIPAddress);
				}

			}

		}




		#endregion Start

		#region Update
	
		void Update () {
			//_Update();
		}

		void _Update(){
			//should I only dequeue one item per frame?
			lock(_eventLock){
				while(_eventQueue.Count > 0){
	
					UniOSCEventArgs args = _eventQueue.Dequeue();
					//Event for the Editor. 
					if( OSCMessageReceivedRaw != null) OSCMessageReceivedRaw(this, args) ;

					//in learning mode we don't propagate the event to our targets.
					if(isOSCLearning || !dispatchOSC )continue;
					#if UNITY_EDITOR
					if(!Application.isPlaying && !isEditorEnabled)continue;
					#endif
					//If no MapingFileObj is attached all messages are passed through without any mapping
					if(!hasOSCMappingFileAttached){
						if( OSCMessageReceived != null) OSCMessageReceived(this, args) ;
						continue;
					}

					foreach(var mfo in oscMappingFileObjList){
						if(mfo == null)continue;

						UniOSCMappingItem m = mfo.oscMappingItemList.Find(mi => mi.address == args.Address);
						if(m == null){
							if( OSCMessageReceived != null) OSCMessageReceived(this, args) ;
						}else{
							m.MapData(args);
							if( OSCMessageReceived != null) OSCMessageReceived(this, args) ;
							//only one message with a certain value is used
							break;
						}

					}//for mfo

				}
			}//lock

			if(!hasOSCSessionFileAttached)return;
			lock(_sessionEventLock){
				sessionCounter = 16;
				while(_sessionEventQueue.Count > 0 && sessionCounter>0){
					SendOSCMessage(this,_sessionEventQueue.Dequeue());
					sessionCounter--;
				}
			}//lock

		}

		void FixedUpdate(){
			_Update();
		}

		#endregion Update

		#region IN
		private void _CreateConnection(){
           // Debug.Log("_CreateConnection");
			if(_oscReceiver != null){_DeleteConnection();}
			
            if (!hasValidOscIPAddress)
            {
                Debug.LogWarning(String.Format("<color='orange'>The connection on port: {0} with IP address {1} could not be established! (Invalid IP address)</color>", oscPort, oscInIPAddress));
                return;
            }

           
            switch (transmissionTypeIn)
            {
                case OSCsharp.Net.TransmissionType.Unicast:
                    _oscReceiver = new UniOSCReceiver(oscPort, transmissionTypeIn, null);
                    break;
                case OSCsharp.Net.TransmissionType.Multicast:
                  
                    if (oscInIPAddressAsIPAddress == null)
                    {
                        Debug.LogWarning(String.Format("<color='orange'>The connection on port: {0} with IP address {1} could not be established! (Invalid Multicast address)</color>", oscPort, oscInIPAddress));
                        return;
                    }
                    _oscReceiver = new UniOSCReceiver(oscPort, transmissionTypeIn, oscInIPAddressAsIPAddress);
                    break;
                default: return;
            }
           

			if(Application.isPlaying){
				_oscReceiver.OSCMessageReceived+= OnOSCMessageReceived;
			}else{
				_oscReceiver.OSCMessageReceived+= OnOSCMessageReceived;
			}

			if(hasOSCSessionFileAttached){

				foreach(var osf in oscSessionFileObjList){
					if(osf != null)
					OSCMessageReceivedRaw+= osf.OnOSCMessageReceived;
				}
			}

		}


		private void _DeleteConnection(){
			if(_oscReceiver == null) return ;
			//Debug.Log("UniOSCConnection._DeleteConnection:"+oscPort);
			_oscReceiver.Disconnect();
		
			if(Application.isPlaying){
				_oscReceiver.OSCMessageReceived-= OnOSCMessageReceived;
			}else{
				_oscReceiver.OSCMessageReceived-= OnOSCMessageReceived;
			}
			
			_oscReceiver = null;


			if(hasOSCSessionFileAttached){

				foreach(var osf in oscSessionFileObjList){
					if(osf != null)
						OSCMessageReceivedRaw-= osf.OnOSCMessageReceived;
				}
			}


		}



		/// <summary>
		/// creates  internally an UniOSCReciver which handles all the Network setup.
		/// Called from GUI/Inspector
		/// </summary>
		public void ConnectOSC(){
			//Debug.Log("ConnectOSC");
			_CreateConnection();
            if (_oscReceiver == null) {
                _connected = false;
              
            }
            else
            {
                _connected = _oscReceiver.Connect();
            }
			
			redrawFlag = !redrawFlag;
			if(_connected) {
				dispatchOSC= true;
				if(ConnectionInStatusChange != null)ConnectionInStatusChange(this);
			}
		}

		/// <summary>
		/// Disconnects and destroys the OSCConnection.
		/// </summary>
		public void DisconnectOSC(){
			//Debug.Log("DisconnectOSC");
			_DeleteConnection();
			_connected = false;
			dispatchOSC = false;
			redrawFlag = !redrawFlag;
			if(ConnectionInStatusChange != null)ConnectionInStatusChange(this);
		}

        private void _SetupChanged_IN()
        {
           // Debug.Log("UniOSCConnection._SetupChanged_IN");
            if (_connected)
            {
                DisconnectOSC();
                ConnectOSC();
            }
            else
            {
                if (ConnectionInStatusChange != null) ConnectionInStatusChange(this);
            }


        }

        public void Force_SetupChanged_IN()
        {
           // Debug.Log("UniOSCConnection.Force_SetupChanged_IN");
            _SetupChanged_IN();
        }

      
		#endregion

		#region OUT

		private void _CreateConnectionOut(){
			if(_oscTransmitter != null){_DeleteConnectionOut();}
			//Debug.Log("UniOSCConnection._CreateConnectionOut:"+oscOutPort);
            if (!hasValidOscOutIPAddress)
            {
                Debug.LogWarning(String.Format("<color='orange'>The connection on port: {0} with IP address {1} could not be established! (Invalid IP address)</color>", oscOutPort, oscOutIPAddress));               
                return;
            }

            _oscTransmitter = new UniOSCTransmitter(oscOutIPAddressAsIPAddress, transmissionTypeOut, oscOutPort);         
		}

		private void _DeleteConnectionOut(){
			if(_oscTransmitter == null)return;
			//Debug.Log("_DeleteConnectionOut:"+oscOutPort);
			_oscTransmitter.Close();
			_oscTransmitter = null;
		}

		/// <summary>
		/// Connects the OSC out.
		/// </summary>
		public void ConnectOSCOut(){
            //Debug.Log("UniOSCConnection.ConnectOSCOut");
			_CreateConnectionOut();
			if(_oscTransmitter == null){			
				_connectedOut = false;
				dispatchOSCOut= false;
			}else{
				bool isConnected = _oscTransmitter.Connect();
                _connectedOut = isConnected;
                dispatchOSCOut = isConnected;
                if (isConnected && ConnectionOutStatusChange != null) ConnectionOutStatusChange(this);
			}

		}

		/// <summary>
		/// Disconnects and release the OSC out connection.
		/// </summary>
		public void DisconnectOSCOut(){
            //Debug.Log("UniOSCConnection.DisconnectOSCOut");
			_DeleteConnectionOut();

			_connectedOut = false;
			dispatchOSCOut= false;
			redrawFlag = !redrawFlag;
			if(ConnectionOutStatusChange != null)ConnectionOutStatusChange(this);
		}

        private void _SetupChanged_OUT()
        {
           // Debug.Log("UniOSCConnection._SetupChanged_OUT");
            if (_connectedOut)
            {
                DisconnectOSCOut();
                ConnectOSCOut();
            }
            else
            {
                if (ConnectionOutStatusChange != null) ConnectionOutStatusChange(this);
            }


        }

        public void Force_SetupChanged_OUT()
        {
            //Debug.Log("UniOSCConnection.Force_SetupChanged_OUT");
            _SetupChanged_OUT();
        }

		#endregion



		#region End

		void OnDisable(){
            //Debug.Log("UniOSCConnection.OnDisable");
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.update -= _Update;
			#endif
			DisconnectOSC();
			DisconnectOSCOut();
		}


		void OnDestroy(){
            //Debug.Log("UniOSCConnection.OnDestroy");
			_Remove_connectionInstance();
		}

		/// <summary>
		/// Ensure that the instance is destroyed when the game is stopped in the Unity editor
		/// Close all the OSC clients and servers
		/// </summary>
		void OnApplicationQuit() 
		{
		}

		private void _Remove_connectionInstance(){
			if(_connectionInstances != null){
				if(_connectionInstances.Contains(this)){
						_connectionInstances.Remove(this);
						Update_AvailableOSCSettings();
					}
			}
		}

		#endregion End

		#region GUI
//		void OnGUI(){
//
//		}

		/// <summary>
		/// Renders the GUI of a OSCConnection in the GameView.
		/// This is different from the rendering in the editor/inspector
		/// </summary>
		public void RenderGUI(){

			if(UniOSCUtils.TEX_CONNECTION_BG == null){
				UniOSCUtils.TEX_CONNECTION_BG = UniOSCUtils.MakeTexture(2,2,UniOSCUtils.CONNECTION_BG);
			}
				
			_gs = new GUIStyle(GUI.skin.box); 
			_gs.normal.background =UniOSCUtils.TEX_CONNECTION_BG;
			_gs.fontSize =11;
		
			GUILayout.BeginVertical(_gs);
			#region Header
			GUILayout.BeginHorizontal();
			GUILayout.Label(name);
			GUILayout.EndHorizontal();
			#endregion Header

			GUILayout.BeginHorizontal();

			#region IN
			GUILayout.BeginVertical();

			GUI.backgroundColor = _connected ? UniOSCUtils.CONNECTION_ON_COLOR : UniOSCUtils.CONNECTION_OFF_COLOR;
			GUIContent gc ;

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("OSC IN");
			sb.AppendLine(String.Format("Port:{0}",oscPort));
           

			GUIStyle _gs2 = new GUIStyle(GUI.skin.label); 
			_gs2.normal.background =UniOSCUtils.TEX_CONNECTION_BG;
			_gs2.fontSize =11;
				
			GUILayout.BeginVertical(_gs);

			GUILayout.Label(new GUIContent("OSC IN"),_gs);

			GUILayout.BeginHorizontal(_gs2);
			GUILayout.Label(new GUIContent("Port:"),_gs2);
			gc= new GUIContent("65535");
			Rect areaPort = GUILayoutUtility.GetRect(gc,_gs,GUILayout.MinWidth(20f));
			oscPort =Mathf.Min(UniOSCUtils.MAXPORT, Convert.ToInt32( GUI.TextField(areaPort,oscPort.ToString())) );
			GUILayout.EndHorizontal();

			


            GUILayout.BeginHorizontal(_gs2);

                int toolbarInt = 0;
                toolbarInt = (int)transmissionTypeIn;
                 string[] toolbarStrings = new string[] { "Unicast", "Multicast" };
                 GUIStyle _gs2b = new GUIStyle(GUI.skin.toggle); _gs2b.fontSize = 12;
                 toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings,_gs2b, GUILayout.Width(75 * toolbarStrings.Count()), GUILayout.Height(25f));
                 transmissionTypeIn = (OSCsharp.Net.TransmissionType)toolbarInt;
           
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(_gs2);
            GUILayout.Label(new GUIContent("IP:"), _gs);
            gc = new GUIContent("255.255.255.255");
            Rect areaIpIn = GUILayoutUtility.GetRect(gc, _gs, GUILayout.MinWidth(100f));
            _tfgs = (transmissionTypeIn == OSCsharp.Net.TransmissionType.Unicast) ? new GUIStyle(GUI.skin.label): new GUIStyle(GUI.skin.textField);

            if (!hasValidOscIPAddress)
            {
                //When the IP-Address is invalid we get visual feedback
                _tfgs.normal.textColor = new Color(0.8f, 0.0f, 0.0f, 1f);
                _tfgs.hover.textColor = new Color(0.9f, 0.0f, 0.0f, 1f);
                _tfgs.active.textColor = Color.red;
                _tfgs.focused.textColor = Color.red;
            }

            if (transmissionTypeIn == OSCsharp.Net.TransmissionType.Unicast) 
            { 
               
                GUI.Label(areaIpIn, oscInIPAddress, _tfgs);
            }
            else
            {
                oscInIPAddress = GUI.TextField(areaIpIn, oscInIPAddress, _tfgs);
            }         
           
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

			GUI.backgroundColor = Color.white;

			if(GUILayout.Button(_connected ? "Disconnect":"Connect" ) ) 
            {
				if(_connected){
					DisconnectOSC();
				}else{
					ConnectOSC();
				}
			}

			GUILayout.EndVertical();
			#endregion IN


			GUILayout.Space(5f);

			
            #region OUT
			GUILayout.BeginVertical();


			GUI.backgroundColor = _connectedOut ? UniOSCUtils.CONNECTION_ON_COLOR : UniOSCUtils.CONNECTION_OFF_COLOR;
           
			GUIStyle _gs3 = new GUIStyle(GUI.skin.label);
			_gs3.normal.background =UniOSCUtils.TEX_CONNECTION_BG;
			_gs3.fontSize =11;

			GUILayout.BeginVertical(_gs);


			GUILayout.Label(new GUIContent("OSC OUT"),_gs);
			GUILayout.BeginHorizontal(_gs3);
			GUILayout.Label(new GUIContent("Port:"),_gs3);
			gc= new GUIContent("65536");
			Rect areaPort3 = GUILayoutUtility.GetRect(gc,_gs,GUILayout.MinWidth(20f));
			oscOutPort = Mathf.Min(UniOSCUtils.MAXPORT, Convert.ToInt32( GUI.TextField(areaPort3,oscOutPort.ToString())) );
			GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(_gs3);

            int toolbarIntOut = 0;
            toolbarIntOut = (int)transmissionTypeOut;
            string[] toolbarStringsOut = new string[] { "Unicast", "Multicast","Broadcast" };
            GUIStyle _gs3b = new GUIStyle(GUI.skin.toggle); _gs3b.fontSize = 12;
            toolbarIntOut = GUILayout.Toolbar(toolbarIntOut, toolbarStringsOut,_gs3b, GUILayout.Width(75 * toolbarStringsOut.Count()), GUILayout.Height(25f));
            transmissionTypeOut = (OSCsharp.Net.TransmissionType)toolbarIntOut;

            GUILayout.EndHorizontal();

				
			GUILayout.BeginHorizontal(_gs3);
			GUILayout.Label(new GUIContent("IP:"),_gs);
			gc= new GUIContent("255.255.255.255");
			Rect areaIpOut = GUILayoutUtility.GetRect(gc,_gs,GUILayout.MinWidth(100f));		          
            _tfgs = ((int)transmissionTypeOut >= (int)OSCsharp.Net.TransmissionType.Broadcast) ? new GUIStyle(GUI.skin.label) : new GUIStyle(GUI.skin.textField);

            if (!hasValidOscOutIPAddress)
            {
                //When the IP-Address is invalid we get visual feedback
                _tfgs.normal.textColor = new Color(0.8f, 0.0f, 0.0f, 1f);
                _tfgs.hover.textColor = new Color(0.9f, 0.0f, 0.0f, 1f);
                _tfgs.active.textColor = Color.red;
                _tfgs.focused.textColor = Color.red;
            }

            if ((int)transmissionTypeOut >= (int)OSCsharp.Net.TransmissionType.Broadcast)
            {

                GUI.Label(areaIpOut, oscOutIPAddress, _tfgs);
            }
            else
            {
                oscOutIPAddress = GUI.TextField(areaIpOut, oscOutIPAddress, _tfgs);
            }        
			

			GUILayout.EndHorizontal();
           

			GUILayout.EndVertical();
         	

			GUI.backgroundColor = Color.white;

			if(GUILayout.Button(_connectedOut ? "Disconnect":"Connect" ) ) {
				if(_connectedOut){
					DisconnectOSCOut();
				}else{
					ConnectOSCOut();
				}
			}

			
			GUILayout.EndVertical();
			#endregion OUT


			GUILayout.EndHorizontal();


			#region session
			GUILayout.BeginHorizontal();
			if(hasOSCSessionFileAttached){

				if(!_connectedOut){GUI.backgroundColor = Color.red;GUI.contentColor = Color.red;}
				if(GUILayout.Button(new GUIContent("Send Session Data","Tooltip"))){
					SendSessionData();
				}
				GUI.backgroundColor = Color.white;
				GUI.contentColor = Color.white;
			}
			GUILayout.EndHorizontal();
			#endregion


			GUILayout.EndVertical();

		}
		#endregion GUI


		#region Message
		// this method is called from a different thread so we fill the queue that is later read on FixedUpdate
		private void OnOSCMessageReceived(object sender, UniOSCEventArgs args){
			lock(_eventLock){
				_eventQueue.Enqueue(args);
			}
		}
		
		/// <summary>
		/// Sends the OSC message.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">UniOSCEventArgs</param>
		public void SendOSCMessage(object sender,UniOSCEventArgs args){

			if(!_connectedOut || !dispatchOSCOut)return;
			//check if we could send the message:
			if(args.Port == oscOutPort && args.IPAddress == oscOutIPAddress){
				if(_oscTransmitter.SendOSCMessage(this,args)){
					if(OSCMessageSend != null)OSCMessageSend(this,args);
				}
			}else{
				Debug.LogWarning("Message has different Port or IPAddress from the OSCConnection!");
			}

		}

		/// <summary>
		/// Sends the test message.
		/// Only for testing the OSC Out connection.
		/// </summary>
		public void SendTestMessage(){
			OSCsharp.Data.OscMessage msg = new OSCsharp.Data.OscMessage("/test",1f);
			UniOSCEventArgs args = new UniOSCEventArgs(oscOutPort,msg);
			args.IPAddress = oscOutIPAddress;
			SendOSCMessage(this,args);
		}

		/// <summary>
		/// Sends the session data.
		/// This is useful for updating the GUI of TouOSC for example with the last data values from incomming OSC messages.
		/// You have to add a OSC Session file to the OSCConnection to use this feature.
		/// </summary>
		public void SendSessionData(){

			if(!hasOSCSessionFileAttached)return;
			lock(_sessionEventLock){

		
			foreach(var osf in oscSessionFileObjList){
				if(osf == null) continue;
				foreach(var osi  in osf.oscSessionItemList ){
					string address = osi.address;
					if(String.IsNullOrEmpty(address))continue;
					OSCsharp.Data.OscMessage msg = new OSCsharp.Data.OscMessage(address);
					for(int i=0;i< osi.data.Count;i++){
						float number;
						bool result = Single.TryParse(osi.data[i], out number);
						if (result)
						{
							msg.Append(number);
								//typecheck? osi.dataTypeList[i]
							//msg.Append(Int32.Parse(osi.data[i]));
						}
					}
					
					UniOSCEventArgs args = new UniOSCEventArgs(oscOutPort,msg);
					args.IPAddress = oscOutIPAddress;
					//give a delay for proper update of TouchOSC
					//to prevent some lack at runtime we use a queue that is dequeued on _Update()
					//In Editor mode  we stop the running thread for a millisecond. 
					//Otherwise part of the osc messages could be lost if we send to much udp packets at the same time
					if(Application.isPlaying){
						_sessionEventQueue.Enqueue(args);
					}else{
						Thread.Sleep(1);
						SendOSCMessage(this,args);
					}
					
				}//oscSessionItemList
				
			}//oscSessionFileObjList


			}//lock
		}


	


		#endregion Message

	}
}
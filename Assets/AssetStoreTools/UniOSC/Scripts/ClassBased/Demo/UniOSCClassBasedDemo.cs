/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using UniOSC;
using OSCsharp.Data;

/// <summary>
/// Demo to show how to use the class based scripts.
/// </summary>
public class UniOSCClassBasedDemo : MonoBehaviour {

	#region public

	public string OSCAddress;
	public int OSCPort;
	public UniOSCConnection OSCConnection;

    [Space(10)]

	public string OSCAddressOUT;
	public string OSCIPAddressOUT = "192.168.178.32";
	public int OSCPortOUT;
	public UniOSCConnection OSCConnectionOUT;

     [Space(10)]

	public Light Light1;
	public Light Light2;
	public Light Light3;

     [Space(10)]
	public bool sendData;


	#endregion

	#region private
	private  UniOSCEventTargetCBImplementation oscTarget1;
	private  UniOSCEventTargetCBImplementation oscTarget2;
	private  UniOSCEventTargetCBImplementation oscTarget3;
	private  UniOSCEventTargetCBImplementation oscTarget4;
	private  UniOSCEventTargetCBImplementation oscTarget5;

	private UniOSCEventDispatcherCBImplementation oscDispatcher1;
	private UniOSCEventDispatcherCBImplementation oscDispatcher2;
	#endregion

	#region Timer
	public float sendInterval=1000;
	private System.Timers.Timer _sendIntervalTimer;
	private bool _isOSCDirty;
	private bool timerToggle;
	private object _mylock = new object();

	/// <summary>
	/// This is a demo timer to show how you can trigger the OSC sending frequently
	/// The main problem is that you have to call you SendOSC method on the main thread so with this technique 
	/// you only set a flag in the timer and in the Update() method you read this flag to decide if you should really send
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="e">E.</param>
	private  void _OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
	{
		//Debug.Log("Timer.Tick");
		lock(_mylock){
			_isOSCDirty = true;
		}	


	}

	private void StartSendIntervalTimer()
	{
		if(_sendIntervalTimer == null){
			_sendIntervalTimer = new System.Timers.Timer();
		}
		_sendIntervalTimer.Interval = sendInterval;
		_sendIntervalTimer.Elapsed-= _OnTimedEvent;
		_sendIntervalTimer.Elapsed+= _OnTimedEvent;
		_sendIntervalTimer.Enabled = true;
	}
	
	private void StopSendIntervalTimer()
	{
		if(_sendIntervalTimer == null)return;
		_sendIntervalTimer.Stop();
		_sendIntervalTimer.Elapsed-= _OnTimedEvent;
	}
	#endregion

	void Awake () {

		//Here we show the different possibilities to create a OSCEventTarget from code:

		//When we only specify a port we listen to all OSCMessages on that port (We assume that there is a OSCConnection with that listening port in our scene)
		oscTarget1 = new UniOSCEventTargetCBImplementation(OSCPort);
		oscTarget1.OSCMessageReceived+=OnOSCMessageReceived1;
        //oscTarget1.oscPort = OSCPort;

		//This implies that we use the explicitConnection mode. (With responding to all OSCmessages)
		oscTarget2 = new UniOSCEventTargetCBImplementation(OSCConnection);
		oscTarget2.OSCMessageReceived+=OnOSCMessageReceived2;

		//We listen to a special OSCAddress regardless of the port.
		oscTarget3 = new UniOSCEventTargetCBImplementation(OSCAddress);
		oscTarget3.OSCMessageReceived+=OnOSCMessageReceived3;

		//The standard : respond to a given OSCAddress on a given port
		oscTarget4 = new UniOSCEventTargetCBImplementation(OSCAddress, OSCPort);
		oscTarget4.OSCMessageReceived+=OnOSCMessageReceived4;

		//This version has the advantage that we are not bound to a special port. If the connection changes the port we still respond to the OSCMessage
		oscTarget5 = new UniOSCEventTargetCBImplementation(OSCAddress, OSCConnection);
		oscTarget5.OSCMessageReceived+=OnOSCMessageReceived5;


		oscDispatcher1 = new UniOSCEventDispatcherCBImplementation(OSCAddressOUT,OSCIPAddressOUT,OSCPortOUT);
		oscDispatcher1.AppendData("TEST1");

		oscDispatcher2 = new UniOSCEventDispatcherCBImplementation(OSCAddressOUT,OSCConnectionOUT);


        OSCConnection.transmissionTypeIn = OSCsharp.Net.TransmissionType.Multicast;
        OSCConnection.oscInIPAddress = "224.0.0.1";//Set a valid Multicast address.
	}
	



	void OnEnable()
	{
		//Debug.Log("UniOSCCodeBasedDemo.OnEnable");

		//Just to create a OSCEventTarget isn't enough. We nedd to enable it:

		oscTarget1.Enable();

		oscTarget2.Enable();

		oscTarget3.Enable();

		oscTarget4.Enable();

		oscTarget5.Enable();


		oscDispatcher1.Enable();

		oscDispatcher2.Enable();
		//just a test to show that you can clear and add data (is overwritten with 0 and 1 at Update())
		oscDispatcher2.ClearData();
		oscDispatcher2.AppendData("TEST2");
		oscDispatcher2.AppendData("TEST2.1");

		// We want sent continously data so start our timer to set a flag at a given interval
		// The interval is in milliseconds.
		StartSendIntervalTimer();
	}

	void OnDisable()
	{
		//Here we only disable our OSCEventTargets so we can temporary disconnect from the OSCMessage stream. To re-connect we only have to call Enable()
		oscTarget1.Disable();
				
		oscTarget2.Disable();
				
		oscTarget3.Disable();
				
		oscTarget4.Disable();
				
		oscTarget5.Disable();

		StopSendIntervalTimer();
	}

	void OnDestroy()
	{
		//Clean up things and release recources!!!!
		//Otherwise our callbacks can still respond even if our GameObject with this script is destroyed/removed from the scene

		oscTarget1.Dispose();
		oscTarget1 = null;
		
		oscTarget2.Dispose();
		oscTarget2 = null;
		
		oscTarget3.Dispose();
		oscTarget3 = null;
		
		oscTarget4.Dispose();
		oscTarget4 = null;
		
		oscTarget5.Dispose();
		oscTarget5 = null;
	}

	void Update () {
		
		//only send OSC messages at our specified interval. The _isOSCDirty flag is set at  _OnTimedEvent()
		lock(_mylock){
			if(!_isOSCDirty)return;
			_isOSCDirty = false;
		}
		
		if(sendData)
		{
			//Here we change the data of the OSCMessage.
			oscDispatcher1.SetDataAtIndex0(timerToggle);
			oscDispatcher2.SetDataAtIndex0(timerToggle);
			
			oscDispatcher1.SendOSCMessage();
			oscDispatcher2.SendOSCMessage();
			
			timerToggle = ! timerToggle;
		}
		
	}

	#region callbacks

	void OnOSCMessageReceived1(object sender, UniOSCEventArgs args)
	{
		Debug.Log("UniOSCCodeBasedDemo.OnOSCMessageReceived1:"+ _GetAddressFromOscPacket(args));
		if(Light1 != null) Light1.enabled = !  Light1.enabled ;
	}

	void OnOSCMessageReceived2(object sender, UniOSCEventArgs args)
	{
		Debug.Log("UniOSCCodeBasedDemo.OnOSCMessageReceived2:"+ _GetAddressFromOscPacket(args));
		if(Light2 != null) Light2.enabled = !  Light2.enabled ;
	}

	void OnOSCMessageReceived3(object sender, UniOSCEventArgs args)
	{
		Debug.Log("UniOSCCodeBasedDemo.OnOSCMessageReceived3:"+ _GetAddressFromOscPacket(args));
		if(Light3 != null) Light3.enabled = !  Light3.enabled ;
	}

	void OnOSCMessageReceived4(object sender, UniOSCEventArgs args){
		Debug.Log("UniOSCCodeBasedDemo.OnOSCMessageReceived4:"+ _GetAddressFromOscPacket(args));
	}

	void OnOSCMessageReceived5(object sender, UniOSCEventArgs args)
	{
		Debug.Log("UniOSCCodeBasedDemo.OnOSCMessageReceived5:"+ _GetAddressFromOscPacket(args));
	}

	private string _GetAddressFromOscPacket(UniOSCEventArgs args){
		return (args.Packet is OscMessage) ? ((OscMessage)args.Packet).Address : ((OscBundle)args.Packet).Address ;
	}

	#endregion

}

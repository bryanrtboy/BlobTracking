/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

using OSCsharp.Data;
using OSCsharp.Net;
using OSCsharp.Utils;

namespace UniOSC{

	//Based on: https://github.com/valyard/TUIOsharp/blob/master/TUIOsharp/TuioServer.cs
	//https://github.com/valyard/OSCsharp

	/// <summary>
	/// Uni OSC receiver.
	/// </summary>
	public class UniOSCReceiver  {

		#region Public 

		public int Port { get; private set; }
		public int FrameNumber { get; private set; }

		#endregion Public


		#region Private 
		
		private UDPReceiver udpReceiver;
		
		#endregion Private


		#region Events

		public event EventHandler<UniOSCEventArgs> OSCMessageReceived;
		public event EventHandler<ExceptionEventArgs> OSCErrorOccured;

		#endregion Events


		#region Constructors

        public UniOSCReceiver() : this(3333, TransmissionType.Unicast, null)
		{}

        public UniOSCReceiver(int port, string MulticastAddress): this(port, TransmissionType.Multicast,IPAddress.Parse( MulticastAddress))
		{}
       
        public UniOSCReceiver(int port, TransmissionType ttype, IPAddress MulticastAddress)
        {
            Port = port;
            udpReceiver = null;
         
            switch (ttype)
            {
                case TransmissionType.Unicast:
                    udpReceiver = new UDPReceiver(port, false);
                    break;
                case TransmissionType.Multicast:

                    if (MulticastAddress == null)
                    {
                        udpReceiver = new UDPReceiver(port, false);
                        Debug.Log("<color='orange'>No applicable MulticastAddress! Fallback to Unicast</color>");
                    }
                    else
                    {

                        udpReceiver = new UDPReceiver(port, MulticastAddress, false);
                    }

                    break;
                case TransmissionType.Broadcast:
                case TransmissionType.LocalBroadcast:
                     udpReceiver = new UDPReceiver(null , port, TransmissionType.Broadcast, null, false);//IPAddress.Any
                    break;
            }
           

         
            if(udpReceiver == null){
                Debug.Log("No UDP Receiver was created!");
                return;
            }
            udpReceiver.MessageReceived += handlerOscMessageReceived;
            udpReceiver.ErrorOccured += handlerOscErrorOccured;
            udpReceiver.BundleReceived += handlerOSCBundleReceived;
        }
		
		#endregion

		
		#region Public methods
		/// <summary>
		/// Connect this instance.
		/// </summary>
		public bool Connect()
		{
			//Debug.Log("OSCReceiver.Connect:"+Port);
			try{
				if (!udpReceiver.IsRunning)udpReceiver.Start();
				return true;
			}
           
            catch(Exception e)
            {
                Debug.LogWarning(String.Format("The connection on port: {0} could not be established!\n{1}", Port, e.Message));
                return false;
            }
		}

		/// <summary>
		/// Disconnect this instance.
		/// </summary>
		public void Disconnect()
		{		
            if (udpReceiver == null) return;
            //Debug.Log("UniOSCReceiver.Disconnect:" + udpReceiver.IsRunning);
			if (udpReceiver.IsRunning) {udpReceiver.Stop();}          
		}
		
		#endregion


		#region Private functions
		
		private void parseOscMessage(OscMessage message)
		{

			if( OSCMessageReceived != null) OSCMessageReceived(this, new UniOSCEventArgs(Port,message )) ;

		}


		private void parseOscBundle(OscBundle bundle){

			foreach(var message in bundle.Messages){
				parseOscMessage(message);
			}

			if(bundle.Bundles.Count >0){
				foreach(var _bundle in bundle.Bundles){
					parseOscBundle(_bundle);
				}
			}
		}

		#endregion


		#region Event handlers
		
		private void handlerOscErrorOccured(object sender, ExceptionEventArgs exceptionEventArgs)
		{
			if( OSCErrorOccured != null) OSCErrorOccured(this, exceptionEventArgs);
		}
		
		private void handlerOscMessageReceived(object sender, OscMessageReceivedEventArgs oscMessageReceivedEventArgs)
		{
			parseOscMessage(oscMessageReceivedEventArgs.Message);
		}


		private void handlerOSCBundleReceived(object sender, OscBundleReceivedEventArgs  oscBundleReceivedEventArgs)
		{
			parseOscBundle(oscBundleReceivedEventArgs.Bundle);
		}

		#endregion
		

	}
}






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

using System.Net;
using System.Net.Sockets;

using OSCsharp.Data;
using OSCsharp.Net;

using OSCsharp.Utils;

namespace UniOSC{

	//based on: https://github.com/valyard/TUIOsharp/blob/master/TUIOsharp/TuioServer.cs
	//https://github.com/valyard/OSCsharp

	public class UniOSCTransmitter  {

		#region Private vars

		private UDPTransmitter udpTransmitter ;
       
		
		#endregion
		
		
		#region Public properties
		
		public IPAddress IPAddress { get; private set; }
		public int Port { get; private set; }

        public TransmissionType transmissionType { get; private set; }
		
		#endregion

		#region Events
		
		//public event EventHandler<OSCEventArgs> OSCMessageSend;
		public event EventHandler<ExceptionEventArgs> OSCErrorOccured;
		
		
		#endregion

		#region Constructors
		
		public UniOSCTransmitter() : this("127.0.0.1",3333)
		{
           
		}

		public UniOSCTransmitter(string ipAddress, int port ) : this(IPAddress.Parse(ipAddress), port)
        {}

        public UniOSCTransmitter(IPAddress ipAddress, int port) : this(ipAddress, TransmissionType.Unicast, port)
		{}

        //new
        public UniOSCTransmitter(IPAddress ipAddress, TransmissionType ttype, int port)
            
        {         
            IPAddress = ipAddress;
            Port = port;
            transmissionType = ttype;          
        }

		#endregion


		public bool Connect()
		{
			if(udpTransmitter != null) Close ();
			//udpTransmitter = new UDPTransmitter(IPAddress,Port);
            udpTransmitter = new UDPTransmitter(IPAddress,  Port, transmissionType);
            try
            {
                udpTransmitter.Connect();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Could not create a valid UDP Transmitter: "+e.Message);
                udpTransmitter = null;
                return false;
            }
            return true;
		}


		public void Close(){
			if(udpTransmitter != null){            
			udpTransmitter.Close();
			udpTransmitter = null;
			}
		}


		public bool SendOSCMessage(object sender,UniOSCEventArgs args){

			if(udpTransmitter != null){
				try{
                    udpTransmitter.Send(args.Packet);
					return true;
				}catch(Exception e){
					Debug.LogWarning(e.ToString());
					return false;
				}
			}
			return false;
		}

	}
}
/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using System;
using OSCsharp.Data;
using System.Text.RegularExpressions;
using System.Linq;


namespace UniOSC{

	/// <summary>
	/// A  wrapper to a OscMessage class to also store the port and have a quick way to access the message address. 
	/// UniOSC use this class for the internal communication
	/// <para> this is a paragraph</para>
	/// <para>
	/// <seealso cref="UniOSC.OSCEventTarget"/>
	/// <see cref="UniOSC.OSCEventTarget"/>
	/// </para>
	/// </summary>

	public class UniOSCEventArgs : EventArgs
	{
       // public OscMessage Message { get { return _Message; } }
        public OscPacket Packet { get { return _Packet; } }
       
		public string Address{get{return _Address;}}
		public int Port{get{return _Port;}}
		public string IPAddress;//for outgoing messages

		public int Group{get{return _Group;}}
		public string AddressRoot{get{return _AddressRoot;}}
		public int AddressIndex{get{return _AddressIndex;}}

        private OscPacket _Packet;
       // private OscMessage _Message;
		private string _Address;
		private int _Port;

		private int _Group = -1;
		private string _AddressRoot;
		private int _AddressIndex = -1;

        //OscPacket
        //OscMessage
        //OscBundle
        public UniOSCEventArgs(int port, OscPacket packet)
		{
            /*
            if (packet.IsBundle) {
               // _Bundle = message as OscBundle;
            } else {
               // _Message = message as OscMessage;
            }
             * */

            _Packet = packet;
			_Address = packet.Address;
			_Port = port;


			string[] s = _Address.Split('/');
			if(s.Length < 2)return;
			try{
				_Group = Int32.Parse(s[1]);
			}catch(System.Exception){

			}

			if(s.Length < 3)return;
			try{
				_AddressRoot = new String(s[2].Where(Char.IsLetter).ToArray());
				var data = Regex.Match(s[2], @"\d+").Value;
				_AddressIndex = Int32.Parse(data);
			}catch(System.Exception){

			}


		}
	}

}
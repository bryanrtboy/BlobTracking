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
	/// Uni OSC abstract item is the base class for Mapping/Session Items.
	/// </summary>
	[Serializable]
	public abstract class UniOSCAbstractItem  {
		public bool isLearning;
		public string address = "";
	}

}

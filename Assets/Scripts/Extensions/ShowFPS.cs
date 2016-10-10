//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Extensions
{
	public class ShowFPS : MonoBehaviour
	{

		public float fpsMeasuringDelta = 2.0f;
		public Text m_text;

		private float timePassed;
		private int m_FrameCount = 0;
		private float	m_FPS = 0.0f;

		void Start ()
		{
			timePassed = 0.0f;
			if (m_text == null)
				Destroy (this);
		}

		void Update ()
		{
			m_FrameCount = m_FrameCount + 1;
			timePassed = timePassed + Time.deltaTime;
		
			if (timePassed > fpsMeasuringDelta) {
				m_FPS = m_FrameCount / timePassed;
			
				timePassed = 0.0f;
				m_FrameCount = 0;
				m_text.text = "FPS: " + m_FPS.ToString ("F2");
			}
		}
	}
}

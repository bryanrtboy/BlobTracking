using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowSliderNumber : MonoBehaviour
{

		public string prefix = "Value: ";
		public string suffix = "";
		public bool  showAsFloat = true;
	
		public void UpdateLabel (float value)
		{
				Text lbl = GetComponent<Text> ();
				if (lbl != null) {
						if (showAsFloat) {
								lbl.text = prefix + value.ToString ("F2") + suffix;
						} else {
								lbl.text = prefix + value.ToString ("F0") + suffix;
						}
				}
		}
}

//https://gist.github.com/darktable/2018687
using UnityEngine;
using System.Collections;

namespace UnityEngine {
	  /// <summary>
	  /// Usage:
	  ///
	  /// (optional) Call GUIScaler.Initialize() in Start(), Awake() or OnEnable() (only needed once)
	  /// Call GUIScaler.Begin() at the top of your OnGUI() methods
	  /// Call GUIScaler.End() at the bottom of your OnGUI() methods
	  ///
	  /// WARNING: If you don't match Begin() and End() strange things will happen.
	  /// </summary>
	  public static class GUIScaler {
	    // 160 is the dpi of the 1st generation iPhone, a good base value.
	    const float BASE_SCALE = 160.0f;
	    static bool initialized = false;
	    static bool scaling = false;
	    static Vector3 guiScale = Vector3.one;
	    static Matrix4x4 restoreMatrix = Matrix4x4.identity;

	    /// <summary>
	    /// Initialize the gui scaler with a specific scale.
	    /// </summary>
	    public static void Initialize(float scale) {
	      if (initialized) return;
	      initialized = true;

	      // scale will be 0 on platforms that have unknown dpi (usually non-mobile)
	      // if the scale is less than 10% don't bother, it just makes gui look bad.
	      if (scale == 0 || scale < 1.1f) return;

	      guiScale.Set(scale, scale, scale);
	      scaling = true;
	    }

	    /// <summary>
	    /// Initialize the gui scaler using the detected screen dpi.
	    /// </summary>
	    public static void Initialize() {
	      Initialize(Screen.dpi / BASE_SCALE);
	    }

	    /// <summary>
	    /// All gui elements drawn after this
	    /// will be scaled.
	    /// </summary>
	    public static void Begin() {
	      if (!initialized) Initialize();

	      if (!scaling) return;

	      restoreMatrix = GUI.matrix;

	      GUI.matrix = GUI.matrix * Matrix4x4.Scale(guiScale);
	    }

	    /// <summary>
	    /// Restores the default gui scale.
	    /// All gui elements drawn after this
	    /// will not be scaled.
	    /// </summary>
	    public static void End() {
	      if (!scaling) return;

	      GUI.matrix = restoreMatrix;
	    }

			public static Vector3 GuiScale{
				get{return guiScale;}
			}
	  }
}

using UnityEngine;
using UnityEditor;

namespace MirrorGear.Mercury
{
	public static class MercuryGUI
	{
		private static GUIStyle _defaultRectStyle;

		public static GUIStyle DefaultRectSytle
		{
			get
			{
				if (_defaultRectStyle == null)
				{
					_defaultRectStyle = new GUIStyle();
					_defaultRectStyle.margin = new RectOffset(0, 0, 0, 0);
					_defaultRectStyle.border = new RectOffset(0, 0, 0, 0);
					_defaultRectStyle.padding = new RectOffset(0, 0, 0, 0);
					_defaultRectStyle.normal.background = MakeTex(5, 5, new Color32(70, 70, 70, 255));
				}

				return _defaultRectStyle;
			}
		}

		public static Rect GetGUIRect(float height)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(false, height);

			int controlID = GUIUtility.GetControlID(FocusType.Passive, controlRect);

			if(Event.current.type == EventType.Repaint)
			{
				DefaultRectSytle.Draw(controlRect, GUIContent.none, controlID);
			}

			return controlRect;
		}

		public static Texture2D MakeTex(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];
			for(int i = 0; i < pix.Length; ++i)
			{
				pix[i] = col;
			}
			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}
	}
}


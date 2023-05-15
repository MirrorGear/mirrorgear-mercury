using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MirrorGear.Mercury
{
	[CustomEditor(typeof(MercuryServerSettings))]
	public class MercuryServerSettingsEditor : Editor
	{
		private MercuryServerSettings _mercuryServerSettings;

		private void OnEnable()
		{
			_mercuryServerSettings = target as MercuryServerSettings;

			UpdateLogDefine();
		}

		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();
			DrawInspector();

			this.serializedObject.ApplyModifiedProperties();
		}

		private void DrawInspector()
		{
			if(_mercuryServerSettings == null)
				return;

			//var mercurySetting = target as MercuryServerSettings;

			EditorGUILayout.BeginVertical();

			//EditorGUILayout.BeginHorizontal();
			//EditorGUILayout.LabelField("MercuryEngine Version: ");
			//EditorGUILayout.LabelField(mercurySetting.version, GUILayout.Width(100));
			//EditorGUILayout.EndHorizontal();

			//var logType = (EnabledNetworkLogType)EditorGUILayout.EnumFlagsField("Network Log", _mercuryServerSettings.networkLogType);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("authKey"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("webServiceKey"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("clientVersion"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("sendPerSecond"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("broadCastRange"));
			//EditorGUILayout.PropertyField(serializedObject.FindProperty("serverType"));

			EditorGUILayout.EndVertical();

			//if(logType != _mercuryServerSettings.networkLogType)
			//{
			//	_mercuryServerSettings.networkLogType = logType;
			//	UpdateLogDefine();
			//}
		}

		private void UpdateLogDefine()
		{
			if(_mercuryServerSettings == null)
				return;

			var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

			var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
			List<string> symbols = defineSymbols.Split(';').ToList();

			int symbolCount = symbols.Count;

			#region LogType

			if((_mercuryServerSettings.networkLogType & EnabledNetworkLogType.Normal) == EnabledNetworkLogType.Normal)
			{
				if(symbols.Any(s => s == "MECURY_NORMAL_LOG") == false)
					symbols.Add("MECURY_NORMAL_LOG");
			}
			else if(symbols.Any(s => s == "MECURY_NORMAL_LOG") == true)
			{
				symbols.Remove("MECURY_NORMAL_LOG");
			}
			
			if((_mercuryServerSettings.networkLogType & EnabledNetworkLogType.Warning) == EnabledNetworkLogType.Warning)
			{
				if(symbols.Any(s => s == "MECURY_WARNING_LOG") == false)
					symbols.Add("MECURY_WARNING_LOG");
			}
			else if(symbols.Any(s => s == "MECURY_WARNING_LOG") == true)
			{
				symbols.Remove("MECURY_WARNING_LOG");
			}

			if((_mercuryServerSettings.networkLogType & EnabledNetworkLogType.Error) == EnabledNetworkLogType.Error)
			{
				if(symbols.Any(s => s == "MECURY_ERROR_LOG") == false)
					symbols.Add("MECURY_ERROR_LOG");
			}
			else if(symbols.Any(s => s == "MECURY_ERROR_LOG") == true)
			{
				symbols.Remove("MECURY_ERROR_LOG");
			}

			#endregion

			#region ServerType

			if(symbols.Any(m => m == "MERCURY_SERVER_PUB") == true)
				symbols.Remove("MERCURY_SERVER_PUB");
			if(symbols.Any(m => m == "MERCURY_SERVER_DEV") == true)
				symbols.Remove("MERCURY_SERVER_DEV");
			if(symbols.Any(m => m == "MERCURY_SERVER_PRI_W") == true)
				symbols.Remove("MERCURY_SERVER_PRI_W");
			if(symbols.Any(m => m == "MERCURY_SERVER_PRI") == true)
				symbols.Remove("MERCURY_SERVER_PRI");

			switch(_mercuryServerSettings.serverType)
			{
				case ServerType.pub:
					if(symbols.Any(m => m == "MERCURY_SERVER_PUB") == false)
						symbols.Add("MERCURY_SERVER_PUB");
					break;
				case ServerType.dev:
					if(symbols.Any(m => m == "MERCURY_SERVER_DEV") == false)
						symbols.Add("MERCURY_SERVER_DEV");
					break;
				case ServerType.pri_w:
					if(symbols.Any(m => m == "MERCURY_SERVER_PRI_W") == false)
						symbols.Add("MERCURY_SERVER_PRI_W");
					break;
				case ServerType.pri:
					if(symbols.Any(m => m == "MERCURY_SERVER_PRI") == false)
						symbols.Add("MERCURY_SERVER_PRI");
					break;
			}

			#endregion

			if(symbolCount != symbols.Count)
				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", symbols.ToArray()));
		}
	}
}


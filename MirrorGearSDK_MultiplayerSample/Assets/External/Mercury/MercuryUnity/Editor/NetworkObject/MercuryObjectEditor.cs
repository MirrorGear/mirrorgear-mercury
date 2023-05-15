using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MirrorGear.Mercury
{

	[CustomEditor(typeof(MercuryObject))]
	public class MercuryObjectEditor : Editor
	{
		private MercuryObject _mercuryObject;
		private bool _isShowComponents = true;
		private GUIStyle _foldoutStyle;

		private void Reset()
		{
			//var objects = FindObjectsOfType<MercuryObject>();

			//uint objId = (uint)objects.Length;
			//foreach(var mo in objects)
			//{
			//	mo.SetObjectId(--objId);
			//}
		}

		private void OnEnable()
		{
			_mercuryObject = target as MercuryObject;
		}

		[InitializeOnLoadMethod]
		private static void SetPhotonViewExecutionOrder()
		{
			int mercuryObjectExecutionOrder = -16000;
			GameObject go = new GameObject();
			MercuryObject pv = go.AddComponent<MercuryObject>();
			MonoScript monoScript = MonoScript.FromMonoBehaviour(pv);

			if(mercuryObjectExecutionOrder != MonoImporter.GetExecutionOrder(monoScript))
			{
				MonoImporter.SetExecutionOrder(monoScript, mercuryObjectExecutionOrder); // very early but allows other scripts to run even earlier...
			}

			DestroyImmediate(go);
		}

		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();

			DrawCommonGUI();

			DrawMercuryComponet();

			this.serializedObject.ApplyModifiedProperties();
		}

		private void DrawCommonGUI()
		{
			EditorGUILayout.Space(5);

			EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");

			SerializedProperty sp = serializedObject.FindProperty("_objectId");

			sp.intValue = EditorGUILayout.IntField("MercuryObject ID", sp.intValue);

			EditorGUILayout.EndVertical();

			EditorGUILayout.Space(5);
		}

		private void DrawMercuryComponet()
		{
			if(!_mercuryObject)
				return;

			_mercuryObject.FindMercuryComponent();

			var mercuryComponents = _mercuryObject.GetFieldValue<List<NetworkComponent>>("_networkComponents");

			if(mercuryComponents != null && mercuryComponents.Count > 0)
			{
				if(_foldoutStyle == null)
				{
					_foldoutStyle = new GUIStyle(EditorStyles.foldout);
					//_foldoutStyle.margin = new RectOffset(2, 2, 2, 2);
					//_foldoutStyle.normal.background = MercuryGUI.MakeTex(5, 5, new Color32(70, 70, 70, 255));
				}

				_isShowComponents = EditorGUILayout.BeginFoldoutHeaderGroup(_isShowComponents, "NetworkComponents", _foldoutStyle);

				if(_isShowComponents == false)
					return;

				EditorGUI.BeginDisabledGroup(true);
				foreach(var component in mercuryComponents)
				{
					EditorGUILayout.ObjectField(component, typeof(object), true);
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace MirrorGear.Mercury
{
	[CustomEditor(typeof(MercuryAnimator))]
	public class MercuryAnimatorEditor : Editor
	{
		private MercuryAnimator _mercuryAnimator;
		private Animator _animtor;
		private AnimatorController _animatorController;

		private bool _isShowAnimatorLayerWeight = true;
		private bool _isShowAnimatorParameter = true;

		private void OnEnable()
		{
			this._mercuryAnimator = target as MercuryAnimator;

			if(this._mercuryAnimator)
			{
				var animator = _mercuryAnimator.GetFieldValue<Animator>("_animator");
				if(animator != null)
				{
					_animtor = animator;

					if(_animtor)
						_animatorController = _animtor.GetRuntimeAnimatorController() as AnimatorController;

					InitLayer();
					InitParameters();
					this.serializedObject.ApplyModifiedProperties();
				}
			}
		}
		
		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();

			DrawAnimatorLayerWeight();

			DrawAnimatorParameter();

			this.serializedObject.ApplyModifiedProperties();
		}

		private void InitLayer()
		{
			List<SynchronizedLayer> hasLayers = _mercuryAnimator.GetFieldValue<List<SynchronizedLayer>>("_synchronizedLayers");

			if(hasLayers != null && hasLayers.Count > 0)
			{
				var result = hasLayers.Where(layer => layer.layerIndex > _animatorController.layers.Length).ToList();

				if(result != null && result.Count > 0)
				{
					foreach(var layer in result)
						_mercuryAnimator.RemoveLayer(layer);
				}
			}
		}

		private void InitParameters()
		{
			List<SynchronizedParameter> hasParameters = _mercuryAnimator.GetFieldValue<List<SynchronizedParameter>>("_synchronizedParameters");
			List<SynchronizedParameter> removeParamters = new List<SynchronizedParameter>();

			if (hasParameters != null && hasParameters.Count > 0)
			{
				var parameters = _animatorController.parameters;

				foreach (var parameter in hasParameters)
				{
					if (parameters.Where(p => p.name == parameter.name).Any() == false)
						removeParamters.Add(parameter);
				}

				foreach (var parameter in removeParamters)
					_mercuryAnimator.RemoveParameter(parameter);
			}
		}

		private void DrawAnimatorLayerWeight()
		{
			if(_mercuryAnimator == null)
				return;

			_isShowAnimatorLayerWeight = EditorGUILayout.Foldout(_isShowAnimatorLayerWeight, "AnimatorLayerWeight");

			if(_isShowAnimatorLayerWeight == false)
				return;

			float lineHeight = 21;

			int layerCount = (_animatorController) ? _animatorController.layers.Length : 0;
			
			Rect layerRect = MercuryGUI.GetGUIRect(layerCount * lineHeight);
			
			for(int i = 0; i < layerCount; ++i)
			{
				if(!_mercuryAnimator.HasLayer(i))
					_mercuryAnimator.SetLayer(i, SynchronizeType.Disable);

				var synchronizedLayer = _mercuryAnimator.GetLayer(i);

				Rect itemRect = new Rect(layerRect.xMin, layerRect.yMin + (i * lineHeight), layerRect.width - 5, lineHeight);

				Rect labelRect = new Rect(itemRect.xMin + 5, itemRect.yMin, EditorGUIUtility.labelWidth - 5, itemRect.height);
				GUI.Label(labelRect, $"Layer {i}");

				Rect popupRect = new Rect(itemRect.xMin + EditorGUIUtility.labelWidth, itemRect.yMin + 1.48f, itemRect.width - EditorGUIUtility.labelWidth, itemRect.height);
				var synchronizeType = synchronizedLayer.synchronizeType;

				synchronizeType = (SynchronizeType)EditorGUI.EnumPopup(popupRect, synchronizeType);

				if(synchronizeType != synchronizedLayer.synchronizeType)
				{
					Undo.RecordObject(_mercuryAnimator, "Modify Animator Layer");
					_mercuryAnimator.SetLayer(i, synchronizeType);
				}
			}
		}

		private void DrawAnimatorParameter()
		{
			if(_mercuryAnimator == null || _animatorController == null)
				return;

			_isShowAnimatorParameter = EditorGUILayout.Foldout(_isShowAnimatorParameter, "AnimatorParameter");

			if(_isShowAnimatorParameter == false)
				return;

			float lineHeight = 21;
			int parameterCount = _animatorController.parameters.Length;
			
			Rect layerRect = MercuryGUI.GetGUIRect(parameterCount * lineHeight);

			string paramterValue = "";

			for(int i = 0; i < parameterCount; ++i)
			{
				var parameter = _animatorController.parameters[i];

				bool isPlaying = Application.isPlaying && this._animtor.gameObject.activeInHierarchy;

				var playing = isPlaying;
				ParameterType parameterType = ParameterType.None;
				switch (parameter.type)
				{
					case AnimatorControllerParameterType.Trigger:
					case AnimatorControllerParameterType.Bool:
						{
							parameterType = (parameter.type == AnimatorControllerParameterType.Bool)
								? ParameterType.Bool
								: ParameterType.Trigger;

							paramterValue = playing ? _animtor.GetBool(parameter.name).ToString() : parameter.defaultBool.ToString();

							if (parameterType == ParameterType.Trigger)
								paramterValue = $"{paramterValue} T";
						}
						break;
					case AnimatorControllerParameterType.Float:
						{
							parameterType = ParameterType.Float;

							paramterValue = playing ? _animtor.GetFloat(parameter.name).ToString("0.00") : parameter.defaultFloat.ToString("0.00");
						}
						break;
					case AnimatorControllerParameterType.Int:
						{
							parameterType = ParameterType.Int;

							paramterValue = playing ? _animtor.GetInteger(parameter.name).ToString() : parameter.defaultInt.ToString();
						}
						break;
				}

				if (!_mercuryAnimator.HasParameter(parameter.name))
					_mercuryAnimator.SetParameter(parameter.name, parameterType, SynchronizeType.Disable);

				var synchronizedParameter = _mercuryAnimator.GetParameter(parameter.name);

				Rect itemRect = new Rect(layerRect.xMin, layerRect.yMin + (i * lineHeight), layerRect.width - 5, lineHeight);

				Rect labelRect = new Rect(itemRect.xMin + 5, itemRect.yMin - 1, EditorGUIUtility.labelWidth - 5, itemRect.height);
				GUI.Label(labelRect, $"{parameter.name} ({paramterValue})");

				Rect poupRect = new Rect(itemRect.xMin + EditorGUIUtility.labelWidth, itemRect.yMin + 1.48f, itemRect.width - EditorGUIUtility.labelWidth, itemRect.height);
				var synchronizeType = synchronizedParameter.synchronizeType;

				synchronizeType = (SynchronizeType)EditorGUI.EnumPopup(poupRect, synchronizeType);

				if(synchronizeType != synchronizedParameter.synchronizeType || parameterType != synchronizedParameter.parameterType)
				{
					Undo.RecordObject(_mercuryAnimator, "Modify Animator Layer");
					_mercuryAnimator.SetParameter(parameter.name, parameterType, synchronizeType);
				}
			}
		}
	}
}



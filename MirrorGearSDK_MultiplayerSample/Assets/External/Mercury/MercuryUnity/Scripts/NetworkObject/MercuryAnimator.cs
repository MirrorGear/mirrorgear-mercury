using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public enum ParameterType
	{
		None,
		Float,
		Int,
		Bool,
		Trigger,
	}

	public enum SynchronizeType
	{
		Disable,
		Asynchronization,
		Synchronization,
	}

	[Serializable]
	public class SynchronizedParameter
	{
		public ParameterType parameterType;
		public SynchronizeType synchronizeType;
		public string name;

		public void Update(ParameterType parameterType, SynchronizeType synchronizeType)
		{
			this.parameterType = parameterType;
			this.synchronizeType = synchronizeType;
		}
	}

	[Serializable]
	public class SynchronizedLayer
	{
		public SynchronizeType synchronizeType;
		public int layerIndex;
	}
	 
	[RequireComponent(typeof(Animator))]
	public class MercuryAnimator : NetworkComponent
	{
		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private List<SynchronizedLayer> _synchronizedLayers = new List<SynchronizedLayer>();
		[SerializeField]
		private List<SynchronizedParameter> _synchronizedParameters = new List<SynchronizedParameter>();

		private MercuryStream _mercuryStream;
		private MercuryStream _receiveStream;

		private void OnDestroy()
		{
			_mercuryStream = null;
			_receiveStream = null;
		}
		
		#region  Editor

		#if UNITY_EDITOR
		private void OnValidate()
		{
			if(_animator == null)
				_animator = GetComponent<Animator>();
		}
		#endif

		#endregion

		#region  Animator Layer

		public bool HasLayer(int layerIndex)
		{
			return _synchronizedLayers.Exists(layer => layer.layerIndex == layerIndex);
		}

		public SynchronizedLayer GetLayer(int layerIndex)
		{
			if(HasLayer(layerIndex))
				return _synchronizedLayers[layerIndex];

			return null;
		}

		public void SetLayer(int layerIndex, SynchronizeType synchronizeType)
		{
			if(HasLayer(layerIndex))
			{
				_synchronizedLayers[layerIndex].synchronizeType = synchronizeType;
			}
			else
			{
				_synchronizedLayers.Add(new SynchronizedLayer()
				{
					layerIndex = layerIndex,
					synchronizeType = synchronizeType
				});
			}
		}

		public void RemoveLayer(SynchronizedLayer layer)
		{
			if (_synchronizedLayers.Contains(layer))
				_synchronizedLayers.Remove(layer);
		}

		#endregion

		#region  Animator Parameter
		public bool HasParameter(string name)
		{
			return _synchronizedParameters.Exists(parameter => parameter.name == name);
		}

		public int GetParameterIndex(string name)
		{
			return _synchronizedParameters.FindIndex(parameter => parameter.name == name);
		}

		public void SetParameter(string name, ParameterType parameterType, SynchronizeType synchronizeType)
		{
			int index = GetParameterIndex(name);

			if (index == -1)
			{
				_synchronizedParameters.Add(new SynchronizedParameter()
				{
					name = name,
					parameterType = parameterType,
					synchronizeType = synchronizeType
				});
			}
			else
				_synchronizedParameters[index].Update(parameterType, synchronizeType);
		}

		public SynchronizedParameter GetParameter(string name)
		{
			int index = GetParameterIndex(name);

			if (index != -1)
				return _synchronizedParameters[index];

			return null;
		}

		public void RemoveParameter(SynchronizedParameter paramter)
		{
			if (_synchronizedParameters.Contains(paramter))
			{
				_synchronizedParameters.Remove(paramter);
			}
		}

		#endregion

		#region Sync

		public override NetworkData OnMercurySerialize()
		{
			if(mercuryObject.IsMine == false)
				return null;

			if(_mercuryStream == null)
				_mercuryStream = new MercuryStream();

			if(_synchronizedParameters.Any(m => m.synchronizeType == SynchronizeType.Synchronization) == false)
				return null;

			foreach (var layer in _synchronizedLayers)
			{
				if(layer.synchronizeType != SynchronizeType.Synchronization)
					continue;

				_mercuryStream.SendNext(_animator.GetLayerWeight(layer.layerIndex));
			}

			foreach(var parameter in _synchronizedParameters)
			{
				if(parameter.synchronizeType != SynchronizeType.Synchronization)
					continue;

				switch(parameter.parameterType)
				{
					case ParameterType.Bool:
						{
							_mercuryStream.SendNext(_animator.GetBool(parameter.name));
						}
						break;
					case ParameterType.Trigger:
						{
							_mercuryStream.SendNext(_animator.GetBool(parameter.name));
						}
						break;
					case ParameterType.Float:
						{
							_mercuryStream.SendNext(_animator.GetFloat(parameter.name));
						}
						break;
					case ParameterType.Int:
						{
							_mercuryStream.SendNext(_animator.GetInteger(parameter.name));
						}
						break;
				}
			}

			return _mercuryStream.GetSerializeData(DynamicCode.Animation, BroadCastType.Range, mercuryObject.AccountId, mercuryObject.objectId);
		}

		public override void OnMercuryDeserialize(NetworkData data)
		{
			if(data.type != NetworkDataType.Dynamic)
				return;

			if (_receiveStream == null)
				_receiveStream = new MercuryStream();

			_receiveStream.SetNetworkData(data);

			foreach(var layer in _synchronizedLayers)
			{
				if(layer.synchronizeType != SynchronizeType.Synchronization)
					continue;

				var weight = _receiveStream.ReceiveNext<int>();

				MercuryDebug.Log($"layer : {layer.layerIndex} , {weight}");
			}
			
			foreach(var parameter in _synchronizedParameters)
			{
				if(parameter.synchronizeType != SynchronizeType.Synchronization)
					continue;

				switch(parameter.parameterType)
				{
					case ParameterType.Bool:
						{
							var parameterValue = _receiveStream.ReceiveNext<bool>();
							_animator.SetBool(parameter.name, parameterValue);
						}
						break;
					case ParameterType.Trigger:
						{
							var parameterValue = _receiveStream.ReceiveNext<bool>();
							if(parameterValue == true)
								_animator.SetTrigger(parameter.name);
						}
						break;
					case ParameterType.Float:
						{
							var parameterValue = _receiveStream.ReceiveNext<float>();
							_animator.SetFloat(parameter.name, parameterValue);
						}
						break;
					case ParameterType.Int:
						{
							var parameterValue = _receiveStream.ReceiveNext<int>();
							_animator.SetInteger(parameter.name, parameterValue);
						}
						break;
				}
			}
		}

#endregion

		public void UpdateAnimation()
		{
			var data = OnMercurySerialize();
			mercuryObject.syncDatas.Enqueue(data);
		}
	}
}
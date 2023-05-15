using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercuryStream
	{
		private List<object> _curDatas = new List<object>();
		private List<object> _preDatas;

		private DynamicData _dynamicData = new DynamicData();
		private int _curValIdx = -1;
		public DynamicData dynamicData => _dynamicData;

		public void SetNetworkData(NetworkData data)
		{
			if (data != null)
				data = dynamicData;

			_curValIdx = -1;
		}

		public NetworkData GetSerializeData(DynamicCode code, BroadCastType broadCastType = BroadCastType.Range, ulong accountId = UInt64.MaxValue, uint objectId = UInt32.MaxValue)
		{
			_dynamicData = new DynamicData();

			if(accountId != UInt64.MaxValue)
				_dynamicData.accountId = accountId;

			if(objectId != UInt32.MaxValue)
				_dynamicData.objectId = objectId;

			_dynamicData.broadCastType = broadCastType;
			_dynamicData.code = code;
			//_curDatas.Insert(0, (byte)code);

			if(_preDatas == null)
			{
				_preDatas = new List<object>(_curDatas);
			}
			else
			{
				bool isSameData = false;
				
				int n = 0;
				foreach(var data in _preDatas)
				{
					isSameData = data.ObjectValueCompare(_curDatas[n]);
					++n;

					if(isSameData == false)
						break;
				}

				if(isSameData == true)
				{
					_curDatas.Clear();
					return null;
				}

				_preDatas = new List<object>(_curDatas);
			}

			_dynamicData.datas = new List<object>(_curDatas);
			_curDatas.Clear();

			return _dynamicData;
		}

		public void SendNext(object obj)
		{
			_curDatas.Add(obj);
		}

		public T ReceiveNext<T>() where T : unmanaged
		{
			if(_dynamicData == null || _dynamicData.datas == null)
				return default(T);

			if (++_curValIdx >= _dynamicData.datas.Count) 
				return default(T);

			if (_dynamicData.datas[_curValIdx] is not T)
			{
				Debug.LogWarning($"ReceiveNextType: {_dynamicData.datas[_curValIdx].GetType()}, Into: {typeof(T)}");
				return default(T);
			}

			return (T)_dynamicData.datas[_curValIdx];
		}
	}

#if MERCURY_VER_0_0_1

		[NonSerialized]
		private List<object> _streamObject = new List<object>();

		public NetworkData GetSerializeData(DynamicCode code, BroadCastType broadCastType = BroadCastType.None, ulong accountId = UInt64.MaxValue, uint objectId = UInt32.MaxValue)
		{
			if(accountId != UInt64.MaxValue)
				_dynamicData.accountId = accountId;

			if(objectId != UInt32.MaxValue)
				_dynamicData.objectId = objectId;

			this._dynamicData.boradCastType = broadCastType;

			byte[] dataBytes = ObjectToByte(_streamObject);

			//MercuryDebug.Log($"Animator Byte Length : {((dataBytes != null) ? dataBytes.Length : 0)}");

			_dynamicData.streamBuffer.Clear();
			_dynamicData.SetCode(code);

			if(dataBytes != null)
				_dynamicData.streamBuffer.PutByteArray(dataBytes);

			_streamObject.Clear();
			
			return _dynamicData;
		}

		public void SendNext(object obj)
		{
			_streamObject.Add(obj);
		}

		public T ReceiveNext<T>() where T : unmanaged
		{
			if(_dynamicData == null || _dynamicData.streamBuffer == null)
				return default(T);

			return _dynamicData.streamBuffer.GetData<T>();
		}

		private byte[] ObjectToByte(List<object> objects)
		{
			try
			{
				if (objects != null && objects.Count > 0)
				{
					using(MemoryStream stream = new MemoryStream())
					{
						foreach(var obj in _dynamicData.datas)
						{
							var bytes = obj.ToBytes();

							if(bytes != null)
								stream.Write(bytes, 0, bytes.Length);
						}

						return stream.ToArray();
					}
				}
			}
			catch(Exception e)
			{
				MercuryDebug.LogError(e.Message);
			}
			
			return null;
		}

#endif
}



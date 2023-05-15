using System;
using System.Threading;
using UnityEngine;

namespace MirrorGear.Mercury
{
	public class MercuryTransform : NetworkComponent
	{
		private Vector3 _networkPosition;
		private DateTime _preMoveTime;

		private float _speed;
		private bool _isMove = false;
		private float _autoSendTime = 5f;
		private float _stopTime = 0f;
		
		private Vector3 _lastPosition = Vector3.zero;

		public bool isLocalPosition = false;

		public Vector3 Position
		{ 
			get
			{
				return isLocalPosition ? transform.localPosition : transform.position;
			}
			set 
			{
				if(isLocalPosition == true)
					transform.localPosition = value;
				else
					transform.position = value;
			}
		}

		private void Start()
		{
			_lastPosition = Position;
			_networkPosition = Position;

			_preMoveTime = DateTime.Now;
		}

		protected void Update()
		{
			if(mercuryObject.IsMine == false)
			{
				Position = Vector3.MoveTowards(Position, _networkPosition, _speed * Time.deltaTime);
			}
			else
			{
				if (_isMove) _stopTime = 0;
				else		_stopTime += Time.deltaTime;
				
				if(_stopTime >= _autoSendTime)
				{
					UpdateTransform();
					_stopTime = 0f;
				}
			}
		}

		public void UpdateTransform()
		{
			mercuryObject.syncDatas.Enqueue(GetCurrentTransformData());
		}

		public override NetworkData OnMercurySerialize()
		{
			var beforeMove = _isMove;
			_isMove = _lastPosition != Position;

			//UpdateTransform();
			if (beforeMove != _isMove) return GetCurrentTransformData();
			else if (!_isMove) return null;
			else return GetCurrentTransformData();
		}

		private NetworkData GetCurrentTransformData()
		{
			_lastPosition = Position;

			return new TransformData
			{
				accountId = MercuryNetwork.LocalPlayer.accountId,
				objectId = mercuryObject.objectId,
				posX = Position.x,
				posY = Position.y,
				posZ = Position.z,
				rotX = transform.localEulerAngles.x,
				rotY = transform.localEulerAngles.y,
				rotZ = transform.localEulerAngles.z,
				status = _isMove ? (ushort)1: (ushort)0,
			};
		}

		public override void OnMercuryDeserialize(NetworkData data)
		{
			if(data is TransformData transformData)
			{
				_networkPosition = new Vector3()
				{
					x = transformData.posX,
					y = transformData.posY,
					z = transformData.posZ,
				};

				var distance = Vector3.Distance(Position, _networkPosition);
				var moveTime = (DateTime.Now - _preMoveTime).TotalSeconds;

				if (moveTime == 0)
					moveTime = 1f / MercuryNetwork.ServerSettings.sendPerSecond;

				_speed = distance / (float)moveTime;
				_preMoveTime = DateTime.Now;
				transform.rotation = Quaternion.Euler(0, transformData.rotY, 0);
			}
		}
	}
}
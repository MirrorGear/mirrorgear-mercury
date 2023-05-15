using MirrorGear.Mercury;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareObjectSample : MercurySyncObject
{
    private bool IsMine => _syncData.ownerId == MercuryNetwork.LocalPlayer.accountId;
    private bool IsEmpty => _syncData.ownerId == 0;

    public float delaySecond = 0.3f;
    private bool _updating = false;
    private MercuryObjectSyncData _syncData;

    private void OnEnable()
    {
        _syncData ??= new();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsLocalPlayer(other.gameObject)) return;

		if(IsMine || IsEmpty)
        {
			if (_updating) return;

			var rot = transform.rotation.eulerAngles;
			rot.y += 10;

			_syncData.accountId = MercuryNetwork.LocalPlayer.accountId;
			_syncData.ownerId = MercuryNetwork.LocalPlayer.accountId;
			_syncData.roomId = MercuryClient.CurrentRoom.roomId;
			_syncData.property = rot.ToString();

			Spread();
		}
    }

    private void OnTriggerExit(Collider other)
    {
		if (!IsLocalPlayer(other.gameObject)) return;

		if (IsMine)
        {
            _syncData.accountId = 0;
            _syncData.ownerId = 0;

            Spread(true);
        }
    }

    private void Spread(bool immediate = false)
    {
        if(immediate || ! _updating)
        {
			// Delay
			_updating = true;
			mercuryObject.syncDatas.Enqueue(_syncData);
			StartCoroutine(Init());
		}
	}

    private void UpdateObject()
    {
        // (0.00, 10.00, 0.00)
        string[] rots = _syncData.property[1..^1].Split(',');
        if (rots.Length < 3)
        {
            Debug.LogWarning("can't parse share object property: " + _syncData.property);
            return;
        }

        float.TryParse(rots[0], out var x);
        float.TryParse(rots[1], out var y);
        float.TryParse(rots[2], out var z);

		transform.rotation = Quaternion.Euler(x,y,z);
	}

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(delaySecond);
        _updating = false;
    }

    private bool IsLocalPlayer(GameObject go)
    {
        var mo = go.GetComponent<MercuryObject>();
        return mo != null && mo.IsMine;
    }

	#region IMercuryObservable
	public override void OnMercuryDeserialize(NetworkData data)
    {
        if(data is MercuryObjectSyncData syncData)
        {
            _syncData = syncData;
            UpdateObject();
        }
    }
	#endregion
}

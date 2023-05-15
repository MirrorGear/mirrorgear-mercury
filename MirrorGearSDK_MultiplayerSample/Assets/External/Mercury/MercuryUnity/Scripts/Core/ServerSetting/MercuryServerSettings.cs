using System;
using System.Collections.Generic;
using MirrorGear.Mercury;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MercuryServerSettings), menuName = "Mercury/ServerSettings")]
public class MercuryServerSettings : ScriptableObject
{
    [Header("[Engine Version]")]
    [Tooltip("[ServerVersion]")]
    public string version = "abscd";
    public EnabledNetworkLogType networkLogType;

    [Header("[Server Setting]")]
    [Space(10)]

	[Tooltip("Auth KEY")]
    public string authKey = "";
	[Tooltip("Web Service Key")]
    public string webServiceKey = "";

    public uint clientVersion = 0;
	[Space(10)]

	[Tooltip("Number of packets sent per second")]
    public int sendPerSecond = 10; //ms                                      //TODO : 배포시 dll쪽으로 빼야함
	[Tooltip("Server to client broadcast range")]
    public BroadCastRange broadCastRange = BroadCastRange.NORMAL_RANGE;
    
    public ServerType serverType;
}

[Serializable]
public struct ServerInfo
{
	[SerializeField, Tooltip("[ServerIP] => ex)127.0.0.1 or Url")]
    public string IP;
	[SerializeField, Tooltip("[ServerPort] => ex) 10000 ~ ")]
    public int Port;
}
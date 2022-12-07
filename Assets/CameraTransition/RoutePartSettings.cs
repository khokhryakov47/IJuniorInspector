using System;
using UnityEngine;

[Serializable]
public class RoutePartSettings
{
    public Vector3 Position;
    public Vector3 Rotation;
    public float MoveDuration;

    public RoutePartSettings(Vector3 position)
    {
        Position = position;
    }
}

using System;
using UnityEngine;

[Serializable]
public class Route
{
    [HideInInspector] public string ID;
    [HideInInspector] public RouteName Name;
    [HideInInspector] public Color DrawColor;
    [HideInInspector] public RoutePartSettings[] PartSettings;
}

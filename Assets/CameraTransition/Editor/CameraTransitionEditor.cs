
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;
using System.Linq;

[CustomEditor(typeof(CameraTransition))]
public class CameraTransitionEditor : Editor
{
    private const string _enumFile = "RouteName";

    private CameraTransition _cameraTransition;
    private string _pathToEnumFile;
    private string _routeName = "New Route Name";

    private void OnEnable()
    {
        _cameraTransition = (CameraTransition)target;
        RouteSection.DeletePressed += RemoveRoute;
        _pathToEnumFile = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(_enumFile)[0]);
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        _cameraTransition.Routes = RefreshRoutes(_cameraTransition.Routes);

        RouteSection.Draw(_cameraTransition.Routes);

        DrawNewClipSection();
    }

    private void OnSceneGUI()
    {
        if (_cameraTransition.Routes == null)
            return;

        List<RouteName> hideRouteNames = RouteSection.HideRouteNames;
        List<Route> routes = RemoveHideRoutes(_cameraTransition.Routes, hideRouteNames);

        for (int routeNumber = 0; routeNumber < routes.Count; routeNumber++)
        {
            RoutePartSettings[] routePartSettings = routes[routeNumber].PartSettings;

            for (int pointNumber = 0; pointNumber < routePartSettings.Length; pointNumber++)
            {
                CreateFreeMoveHandle(routePartSettings[pointNumber], routes[routeNumber].DrawColor);

                if(pointNumber > 0)
                {
                    CreateLine(routePartSettings[pointNumber - 1], routePartSettings[pointNumber], routes[routeNumber].DrawColor);
                }
            }
        }
    }

    private List<Route> RemoveHideRoutes(List<Route> routes, List<RouteName> hideRouteNames)
    {
        List<Route> hideRoutes = new List<Route>();

        for (int i = 0; i < routes.Count; i++)
        {
            for (int j = 0; j < hideRouteNames.Count; j++)
            {
                if(hideRouteNames[j] == routes[i].Name)
                {
                    hideRoutes.Add(routes[i]);
                }
            }
        }

        List<Route> newRoutes = new List<Route>(routes);

        for (int i = 0; i < hideRoutes.Count; i++)
        {
            newRoutes.Remove(hideRoutes[i]);
        }

        return newRoutes;
    }

    private void CreateFreeMoveHandle(RoutePartSettings routePart, Color color)
    {
        Handles.color = color;
        Vector3 newPosition = Handles.FreeMoveHandle(routePart.Position, Quaternion.identity, 0.5f, Vector3.zero, Handles.CylinderHandleCap);

        if(routePart.Position != newPosition)
        {
            newPosition.y = routePart.Position.y;
            routePart.Position = newPosition;
        }
    }

    private void CreateLine(RoutePartSettings sourceRoute, RoutePartSettings targetRoute, Color color)
    {
        Handles.color = color;
        Handles.DrawLine(sourceRoute.Position, targetRoute.Position);
    }

    private void DrawNewClipSection()
    {
        _routeName = EditorGUILayout.TextField("Name", _routeName);
        DrawAddButton();
    }

    private void DrawAddButton()
    {
        if (GUILayout.Button("Add"))
        {
            AddRoute();
        }
    }

    private void AddRoute()
    {
        if (_routeName == string.Empty)
            return;

        if (!Regex.IsMatch(_routeName, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
            return;

        Array array = Enum.GetValues(typeof(RouteName));
        if(array.Length != 0)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if(_routeName == array.GetValue(i).ToString())
                {
                    Debug.LogError("A path with the same name has already been");
                    return;
                }
            }
        }

        EnumEditor.WriteToFile(_routeName, _pathToEnumFile);
        Refresh();

        _routeName = string.Empty;
    }

    private void RemoveRoute(Route route)
    {
        if (!EnumEditor.TryRemoveFromFile(route.Name.ToString(), _pathToEnumFile))
            return;

        Refresh();
    }

    private void Refresh()
    {
        Debug.Log("WAIT");
        var realivePath = _pathToEnumFile.Substring(_pathToEnumFile.IndexOf("Assets"));
        AssetDatabase.ImportAsset(realivePath);
    }

    private List<Route> RefreshRoutes(List<Route> oldRoutes)
    {
        int rountRoute = Enum.GetNames(typeof(RouteName)).Length;
        List<Route> routes = new List<Route>(rountRoute);

        for (int i = 0; i < rountRoute; i++)
        {
            RouteName routeName = (RouteName)i;
            Route route = TryRestoreRoute(oldRoutes, routeName.ToString());

            if(route == null)
            {
                route = CreateNewRoute(routeName);
            }

            routes.Add(route);
        }

        return routes;
    }

    private Route TryRestoreRoute(List<Route> oldRoutes, string name)
    {
        return oldRoutes.FirstOrDefault(o => o.Name.ToString() == name);
    }

    private Route CreateNewRoute(RouteName routeName)
    {
        Route route = new Route
        {
            Name = routeName,
            PartSettings = new RoutePartSettings[1]
            {
                new RoutePartSettings(Vector3.zero)
            }
        };

        return route;
    }

    private void OnDisable()
    {
        RouteSection.DeletePressed -= RemoveRoute;
    }
}

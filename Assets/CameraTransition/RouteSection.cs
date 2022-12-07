using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class RouteSection
{
    public static event UnityAction<Route> DeletePressed;

    public static List<RouteName> HideRouteNames = new List<RouteName>();

    public static void Draw(List<Route> routes)
    {
        for (int routeNumber = 0; routeNumber < routes.Count; routeNumber++)
        {
            string routeName = routes[routeNumber].Name.ToString();
            bool isHide = HideRouteNames.Contains(routes[routeNumber].Name);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(routeName);

            DrawDeleteRouteButton(routes[routeNumber]);
            if (isHide)
                DrawShowButton(routes[routeNumber].Name);
            else
                DrawHideButton(routes[routeNumber].Name);

            EditorGUILayout.EndHorizontal();

            if (isHide == false)
            {
                routes[routeNumber].DrawColor = EditorGUILayout.ColorField("Draw color", routes[routeNumber].DrawColor);

                for (int i = 0; i < routes[routeNumber].PartSettings.Length; i++)
                {
                    RoutePartSettings routePartSettings = routes[routeNumber].PartSettings[i];

                    EditorGUILayout.BeginVertical(GUI.skin.window);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Point {i + 1}");
                    DrawDeletePathButton(routes[routeNumber], i);
                    EditorGUILayout.EndHorizontal();

                    routePartSettings.Position = EditorGUILayout.Vector3Field("Position", routePartSettings.Position);
                    routePartSettings.Rotation = EditorGUILayout.Vector3Field("Rotation", routePartSettings.Rotation);
                    routePartSettings.MoveDuration = EditorGUILayout.FloatField("Move Duration", routePartSettings.MoveDuration);

                    EditorGUILayout.EndVertical();
                }

                DrawAddRoutePathButton(routes[routeNumber]);
            }

            EditorGUILayout.EndVertical();
        }
    }

    private static void DrawHideButton(RouteName routeName)
    {
        if (GUILayout.Button("⇓", GUILayout.Width(20), GUILayout.Height(20)))
        {
            HideRouteNames.Add(routeName);
        }
    }

    private static void DrawShowButton(RouteName routeName)
    {
        if (GUILayout.Button("⇒", GUILayout.Width(20), GUILayout.Height(20)))
        {
            HideRouteNames.Remove(routeName);
        }
    }

    private static void DrawDeleteRouteButton(Route route)
    {
        if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
        {
            DeletePressed?.Invoke(route);
        }
    }

    private static void DrawAddRoutePathButton(Route route)
    {
        if (GUILayout.Button("Add", GUILayout.Width(45), GUILayout.Height(30)))
        {
            var routePartSettings = new RoutePartSettings[route.PartSettings.Length + 1];
            for (int i = 0; i < routePartSettings.Length - 1; i++)
            {
                routePartSettings[i] = route.PartSettings[i];
            }

            routePartSettings[routePartSettings.Length - 1] = new RoutePartSettings(routePartSettings[routePartSettings.Length - 2].Position + new Vector3(3, 0, 0));
            route.PartSettings = routePartSettings;
        }
    }

    private static void DrawDeletePathButton(Route route, int index)
    {
        if (GUILayout.Button("-", GUILayout.Width(17), GUILayout.Height(17)))
        {
            RoutePartSettings[] parts = new RoutePartSettings[route.PartSettings.Length - 1];
            for (int i = 0; i < index; i++)
            {
                parts[i] = route.PartSettings[i];
            }

            for (int i = index + 1; i < parts.Length + 1; i++)
            {
                parts[i - 1] = route.PartSettings[i];
            }

            route.PartSettings = parts;
        }
    }
}

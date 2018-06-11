using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(PointManager))]
public class PointEditor : Editor
{
    Texture nodeTexture;
    static GUIStyle handleStyle = new GUIStyle();
    List<int> alignedPoints = new List<int>();

    void OnEnable()
    {
        nodeTexture = Resources.Load<Texture>("Handle");
        if (nodeTexture == null)
        {
            nodeTexture = EditorGUIUtility.whiteTexture;
        }

        handleStyle.alignment = TextAnchor.MiddleCenter;
        handleStyle.fixedWidth = 15;
        handleStyle.fixedHeight = 15;
    }

    void OnSceneGUI()
    {
        PointManager.Instance = (target as PointManager);
        Vector3[] localPoints = PointManager.Instance.nodes.ToArray();
        Vector3[] worldPoints = new Vector3[PointManager.Instance.nodes.Count];

        for (int i = 0; i < worldPoints.Length; i++)
        {
            worldPoints[i] = PointManager.Instance.transform.TransformPoint(localPoints[i]);
        }

        DrawPoints(worldPoints);
        DrawNodes(worldPoints);

        #region AddPoint
        if (Event.current.shift)
        {
            Vector3 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Vector3 pointLocalMousePos = PointManager.Instance.transform.InverseTransformPoint(mousePos);
            Vector3 nodeOnPoly = HandleUtility.ClosestPointToPolyLine(worldPoints);

            float handleSize = HandleUtility.GetHandleSize(nodeOnPoly);
            int nodeIndex = FindNodeIndex(worldPoints, nodeOnPoly);

            Handles.DrawLine(worldPoints[nodeIndex - 1], mousePos);
            Handles.DrawLine(worldPoints[nodeIndex], mousePos);

            if (Handles.Button(mousePos, Quaternion.identity, handleSize * .1f, handleSize, HandleFunc))
            {
                pointLocalMousePos.y = 1f;
                Undo.RecordObject(PointManager.Instance, "InsertNode");
                PointManager.Instance.nodes.Insert(nodeIndex, pointLocalMousePos);
                Event.current.Use();
            }
        }
        #endregion

        #region DeletePoint
        if (Event.current.control)
        {
            int indexToDelete = FindeNearestNodeToMouse(worldPoints);
            Handles.color = Color.red;
            float handleSize = HandleUtility.GetHandleSize(worldPoints[0]);

            if (Handles.Button(worldPoints[indexToDelete], Quaternion.identity, handleSize * 0.09f, handleSize, DeleteHandle))
            {
                Undo.RecordObject(PointManager.Instance, "Remove Node");
                PointManager.Instance.nodes.RemoveAt(indexToDelete);
                indexToDelete = -1;
                Event.current.Use();
            }
            Handles.color = Color.white;
        }
        #endregion
    }


    private int FindeNearestNodeToMouse(Vector3[] worldPoints)
    {
        Vector3 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        mousePos.y = 1;
        int index = -1;
        float minDistnce = float.MaxValue;
        for (int i = 0; i < worldPoints.Length; i++)
        {
            float distance = Vector3.Distance(worldPoints[i], mousePos);
            if (distance < minDistnce)
            {
                index = i;
                minDistnce = distance;
            }
        }
        return index;
    }

    private int FindNodeIndex(Vector3[] worldPoints, Vector3 nodeOnPoly)
    {
        float smallestdis = float.MaxValue;
        int prevIndex = 0;
        for (int i = 1; i < worldPoints.Length; i++)
        {
            float distance = HandleUtility.DistanceToPolyLine(worldPoints[i - 1], worldPoints[i]);
            if (distance < smallestdis)
            {
                prevIndex = i - 1;
                smallestdis = distance;
            }
        }
        return prevIndex + 1;
    }

    private void HandleFunc(int controlID, Vector3 position, Quaternion rotation, float size)
    {
        if (controlID == GUIUtility.hotControl)
            GUI.color = Color.red;
        else
            GUI.color = Color.green;
        Handles.Label(position, new GUIContent(nodeTexture), handleStyle);
        GUI.color = Color.white;
    }

    private void DeleteHandle(int controlID, Vector3 position, Quaternion rotation, float size)
    {
        GUI.color = Color.red;
        Handles.Label(position, new GUIContent(nodeTexture), handleStyle);
        GUI.color = Color.white;
    }

    private void DrawNodes(Vector3[] worldPoints)
    {
        for (int i = 0; i < PointManager.Instance.nodes.Count; i++)
        {
            Vector3 pos = PointManager.Instance.transform.TransformPoint(PointManager.Instance.nodes[i]);
            float handleSize = HandleUtility.GetHandleSize(pos);
            Vector3 newPos = Handles.FreeMoveHandle(pos, Quaternion.identity, handleSize * 0.09f, Vector3.one, HandleFunc);

            if (newPos != pos)
            {
                CheckAlignment(worldPoints, handleSize * 0.1f, i, ref newPos);
                Undo.RecordObject(PointManager.Instance, "Move Node");
                PointManager.Instance.nodes[i] = PointManager.Instance.transform.InverseTransformPoint(newPos);
            }
        }
    }

    private void DrawPoints(Vector3[] worldPoints)
    {
        if (Event.current.shift) Handles.color = Color.green;
        else if (Event.current.control) Handles.color = Color.red;
        else Handles.color = Color.white;
        for (int i = 0; i < worldPoints.Length - 1; i++)
        {
            if (alignedPoints.Contains(i) && alignedPoints.Contains(i + 1))
            {
                Color currentColor = Handles.color;
                Handles.color = Color.green;
                Handles.DrawLine(worldPoints[i], worldPoints[i + 1]);
                Handles.color = currentColor;
            }
            else
                Handles.DrawLine(worldPoints[i], worldPoints[i + 1]);

        }
        Handles.color = Color.white;
    }

    bool CheckAlignment(Vector3[] worldNodes, float offset, int index, ref Vector3 position)
    {
        alignedPoints.Clear();
        bool aligned = false;
        //check straight lines
        //check previous line
        if (index >= 2)
        {
            //represent the line with the equation y=mx+b
            float dy = worldNodes[index - 1].y - worldNodes[index - 2].y;
            float dx = worldNodes[index - 1].x - worldNodes[index - 2].x;
            float m = dy / dx;
            float b = worldNodes[index - 1].y - m * worldNodes[index - 1].x;

            float newX = (position.x + m * (position.y - b)) / (m * m + 1);
            float newY = (m * (position.x + m * position.y) + b) / (m * m + 1);
            Vector3 newPos = new Vector3(newX, newY);
            float distance = Vector3.Distance(newPos, position);
            if (distance * distance < offset * offset)
            {
                position.x = newX;
                position.y = newY;
                aligned = true;
                alignedPoints.Add(index - 1);
                alignedPoints.Add(index - 2);
            }
        }
        //check next line
        if (index < worldNodes.Length - 2)
        {
            //represent the line with the equation y=mx+b
            float dy = worldNodes[index + 1].y - worldNodes[index + 2].y;
            float dx = worldNodes[index + 1].x - worldNodes[index + 2].x;
            float m = dy / dx;
            float b = worldNodes[index + 1].y - m * worldNodes[index + 1].x;

            float newX = (position.x + m * (position.y - b)) / (m * m + 1);
            float newY = (m * (position.x + m * position.y) + b) / (m * m + 1);
            Vector3 newPos = new Vector3(newX, newY);
            float distance = Vector3.Distance(newPos, position);
            if (distance * distance < offset * offset)
            {
                position.x = newX;
                position.y = newY;
                aligned = true;
                alignedPoints.Add(index + 1);
                alignedPoints.Add(index + 2);
            }
        }
        //check vertical
        //check with the prev node
        //the node can be aligned to the prev and next node at once, we need to return more than one alginedTo Node
        if (index > 0)
        {
            float dx = Mathf.Abs(worldNodes[index - 1].x - position.x);
            if (dx < offset)
            {
                position.x = worldNodes[index - 1].x;
                alignedPoints.Add(index - 1);
                aligned = true;
            }
        }
        //check with the next node
        if (index < worldNodes.Length - 1)
        {
            float dx = Mathf.Abs(worldNodes[index + 1].x - position.x);
            if (dx < offset)
            {
                position.x = worldNodes[index + 1].x;
                alignedPoints.Add(index + 1);
                aligned = true;
            }
        }
        //check horizontal
        if (index > 0)
        {
            float dy = Mathf.Abs(worldNodes[index - 1].y - position.y);
            if (dy < offset)
            {
                position.y = worldNodes[index - 1].y;
                alignedPoints.Add(index - 1);
                aligned = true;
            }
        }
        //check with the next node
        if (index < worldNodes.Length - 1)
        {
            float dy = Mathf.Abs(worldNodes[index + 1].y - position.y);
            if (dy < offset)
            {
                position.y = worldNodes[index + 1].y;
                alignedPoints.Add(index + 1);
                aligned = true;
            }
        }


        if (aligned)
            alignedPoints.Add(index);

        return aligned;
    }
}

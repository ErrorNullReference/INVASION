using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class OutsideViewPointCalculator : MonoBehaviour
{
    public float AngleTreshold;

    public ReferenceVector3 DefaultSpawnPoint;
    public SOListVector3Container SpawnPoints;
    public SOListPlayerContainer Players;
    public SOVariableVector3 NearestPointOutsideView;
    public SOListVector3Container SpawnPointsOutsideView;
    public SOVariableVector3 CameraOffset;
    public bool DrawGizmos;

    Vector3[] cameraBounds;
    Vector3[][] playerBounds;

    void Start()
    {
        cameraBounds = new Vector3[4];
        playerBounds = new Vector3[4][];
        for (int i = 0; i < playerBounds.Length; i++)
            playerBounds[i] = new Vector3[4];

        CalculateBounds(new Vector3(0, CameraOffset.Value.y, 0));

        GetPosition();
    }

    private void OnEnable()
    {
        if (cameraBounds != null)
            GetPosition();
    }

    void LateUpdate()
    {
        GetPosition();
    }

    void CalculateBounds(Vector3 position)
    {
        float FovVertical = Camera.main.fieldOfView + AngleTreshold;
        float FovHorizontal = FovVertical * Camera.main.aspect * (Camera.main.aspect / 10f + 1);

        float angle = FovVertical / 2f * Mathf.Deg2Rad;
        Vector3 zDir = new Vector3(0, Mathf.Sin(angle), Mathf.Cos(angle));
        angle = FovHorizontal / 2f * Mathf.Deg2Rad;
        Vector3 xDir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

        Vector3 A = new Vector3(-xDir.x, zDir.y, zDir.z).normalized;
        Vector3 B = new Vector3(xDir.x, zDir.y, zDir.z).normalized;
        Vector3 C = new Vector3(-xDir.x, -zDir.y, zDir.z).normalized;
        Vector3 D = new Vector3(xDir.x, -zDir.y, zDir.z).normalized;

        Quaternion rot = Camera.main.transform.rotation;
        A = rot * A;
        B = rot * B;
        C = rot * C;
        D = rot * D;

        float angleA = Vector3.Angle(A, Vector3.down) * Mathf.Deg2Rad;
        float angleB = Vector3.Angle(B, Vector3.down) * Mathf.Deg2Rad;
        float angleC = Vector3.Angle(C, Vector3.down) * Mathf.Deg2Rad;
        float angleD = Vector3.Angle(D, Vector3.down) * Mathf.Deg2Rad;

        float magA = Mathf.Sqrt(Mathf.Tan(angleA) * Mathf.Tan(angleA) + 1) * position.y;
        float magB = Mathf.Sqrt(Mathf.Tan(angleB) * Mathf.Tan(angleB) + 1) * position.y;
        float magC = Mathf.Sqrt(Mathf.Tan(angleC) * Mathf.Tan(angleC) + 1) * position.y;
        float magD = Mathf.Sqrt(Mathf.Tan(angleD) * Mathf.Tan(angleD) + 1) * position.y;

        cameraBounds[0] = position + A * magA;
        cameraBounds[1] = position + B * magB;
        cameraBounds[2] = position + C * magC;
        cameraBounds[3] = position + D * magD;
    }

    void GetPosition()
    {
        //if there are no spawn points set default value
        if (!SpawnPoints || SpawnPoints.Elements.Count == 0)
        {
            NearestPointOutsideView.Value = DefaultSpawnPoint;
            return;
        }

        //if there are no players set random position
        if (!Players || Players.Elements.Count == 0)
        {
            NearestPointOutsideView.Value = SpawnPoints[Random.Range(0, SpawnPoints.Elements.Count)];
            return;
        }

        PopulateSpawnPointList();

        //return GetNearestSpawnPoint();
    }

    //Vector3 GetNearestSpawnPoint()
    //{
    //    Vector3 pos = Vector3.zero;

    //    int length = Players.Elements.Count;
    //    for (int i = 0; i < length; i++)
    //    {
    //        pos += Players[i].transform.position;
    //    }
    //    pos /= length;

    //    float min = float.MaxValue;
    //    Vector3 best = DefaultSpawnPoint;

    //    if (SpawnPointsOutsideView.Elements.Count <= 0)
    //        return SpawnPoints[Random.Range(0, SpawnPoints.Elements.Count)];

    //    for (int i = 0; i < SpawnPointsOutsideView.Elements.Count; i++)
    //    {
    //        Vector3 spawnP = SpawnPointsOutsideView[i];
    //        float magnitude = (pos - spawnP).sqrMagnitude;
    //        if (magnitude <= min)
    //        {
    //            min = magnitude;
    //            best = spawnP;
    //        }
    //    }

    //    return best;
    //}

    void PopulateSpawnPointList()
    {
        //prepares output list
        SpawnPointsOutsideView.Elements.Clear();

        Vector3 allPlayersCenter = Vector3.zero;

        int pLength = Players.Elements.Count;

        for (int i = 0; i < pLength; i++)
        {
            Vector3 currentPlayerPos = Players[i].transform.position;

            //prepares center of all players
            allPlayersCenter += currentPlayerPos;

            //prepares player bounds
            Vector3 pPos = currentPlayerPos + CameraOffset;
            pPos.y = 0;

            for (int j = 0; j < playerBounds.Length; j++)
                playerBounds[i][j] = cameraBounds[j] + pPos;
        }
        //center of all players
        allPlayersCenter /= pLength;

        bool contained = false;
        //prepares to calculate nearest point outside view
        float min = float.MaxValue;
        NearestPointOutsideView.Value = DefaultSpawnPoint;

        int spawnLength = SpawnPoints.Elements.Count;

        for (int i = 0; i < spawnLength; i++)
        {
            //Calculate whetever current point is inside/outside all players view
            Vector3 point = SpawnPoints[i];
            contained = false;
            for (int j = 0; j < pLength; j++)
            {
                if (BoundsContains(playerBounds[j], point))
                {
                    contained = true;
                    break;
                }
            }

            //if point is outside view add it to the list of outside points and calculate if it is the nearest to all players center
            if (!contained)
            {
                SpawnPointsOutsideView.Elements.Add(point);

                float magnitude = (allPlayersCenter - point).sqrMagnitude;
                if (magnitude <= min)
                {
                    min = magnitude;
                    NearestPointOutsideView.Value = point;
                }
            }
        }

        //if no point is outside view range get random pos
        if (Mathf.Approximately(min, float.MaxValue))
            NearestPointOutsideView.Value = SpawnPoints[Random.Range(0, spawnLength)];
    }

    bool BoundsContains(Vector3[] bounds, Vector3 point)
    {
        bool b1 = PointInTrianlge(point, bounds[0], bounds[1], bounds[2]);
        bool b2 = PointInTrianlge(point, bounds[1], bounds[2], bounds[3]);

        return b1 != b2;
    }

    float Sign(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (p1.x - p3.x) * (p2.z - p3.z) - (p2.x - p3.x) * (p1.z - p3.z);
    }

    bool PointInTrianlge(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        bool b1 = Sign(point, v1, v2) < 0;
        bool b2 = Sign(point, v2, v3) < 0;
        bool b3 = Sign(point, v3, v1) < 0;

        return b1 == b2 && b2 == b3;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;

        if (cameraBounds == null || cameraBounds.Length != 4 || playerBounds == null || playerBounds.Length != 4)
        {
            cameraBounds = new Vector3[4];
            playerBounds = new Vector3[4][];
            for (int i = 0; i < playerBounds.Length; i++)
                playerBounds[i] = new Vector3[4];
        }

        LateUpdate();

        Gizmos.color = Color.red;

        for (int i = 0; i < playerBounds.Length; i++)
        {
            Gizmos.DrawLine(playerBounds[i][0], playerBounds[i][1]);
            Gizmos.DrawLine(playerBounds[i][1], playerBounds[i][3]);
            Gizmos.DrawLine(playerBounds[i][3], playerBounds[i][2]);
            Gizmos.DrawLine(playerBounds[i][2], playerBounds[i][0]);
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < SpawnPoints.Elements.Count; i++)
            Gizmos.DrawSphere(SpawnPoints[i], 0.5f);

        Gizmos.color = Color.red;
        for (int i = 0; i < SpawnPointsOutsideView.Elements.Count; i++)
            Gizmos.DrawSphere(SpawnPointsOutsideView[i], 1);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(NearestPointOutsideView, 1.5f);
    }
    #endif
}

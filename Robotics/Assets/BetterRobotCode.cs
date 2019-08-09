using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class BetterRobotCode : MonoBehaviour {
    GameObject[] markers;
    int index = 0;
    public bool running = false;
    public float totalDistance;
    float distance;
    Vector3 lastPosition;
    Quaternion angle;
    Vector2 middle, start, end;
    bool ended = false;
    StreamWriter writer;
    public string debugStuff;
	// Use this for initialization
	void Start () {
        markers = GameObject.FindGameObjectsWithTag("Line");
        Array.Sort(markers, CompareObNames);
        lastPosition = transform.position;
        start = new Vector2(transform.position.z, transform.position.x);
        //writer = new StreamWriter("Assets/Resources/Output.txt", true);
    }

    // Update is called once per frame
    void Update () {
        if (running)
        {
            transform.Translate(0f, 0f, 0.1f);
            transform.LookAt(markers[index].transform.position);
            if (Vector3.Distance(transform.position, markers[index].transform.position) < 1f)
            {
                index++;
            }
            if (index >= markers.Length) {
                running = false;
            }
            debugStuff = index.ToString();
        }
	}

    int CompareObNames(GameObject x, GameObject y)
    {
        return int.Parse(x.name) < int.Parse(y.name) ? 1 : 0;
    }

    public void initMarkers() {
        markers = GameObject.FindGameObjectsWithTag("MiniMarker");
        //Array.Sort(markers, CompareObNames);
        distance = 0f;
    }

    Vector2 getCircleCenter(Vector2 A, Vector2 B, Vector2 C) {
        float yDelta_a = B.y - A.y;
        float xDelta_a = B.x - A.x;
        float yDelta_b = C.y - B.y;
        float xDelta_b = C.x - B.x;
        Vector2 center = new Vector2(0, 0);

        float aSlope = yDelta_a / xDelta_a;
        float bSlope = yDelta_b / xDelta_b;
        center.x = (aSlope * bSlope * (A.y - C.y) + bSlope * (A.x + B.x)
            - aSlope * (B.x + C.x)) / (2 * (bSlope - aSlope));
        center.y = -1 * (center.x - (A.x + B.x) / 2) / aSlope + (A.y + B.y) / 2;

        return center;
    }

    private void OnApplicationQuit()
    {
        //writer.Flush();
        //writer.Close();
    }
}

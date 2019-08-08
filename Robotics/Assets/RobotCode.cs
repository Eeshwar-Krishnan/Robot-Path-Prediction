using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
public class RobotCode : MonoBehaviour {
    GameObject[] lineObjects;
    Line line;
    float speed = 0.1f;
    int index = 0;
    volatile bool moving = false;
    bool configuring = true;
    TextAsset text;
    StreamWriter writer;
	// Use this for initialization
	void Start () {
        lineObjects = GameObject.FindGameObjectsWithTag("Line");
        Array.Sort(lineObjects, CompareObNames);
        line = lineObjects[0].GetComponent<Line>();
        writer = new StreamWriter("Assets/Resources/Output.txt", true);
    }
	
	// Update is called once per frame
	void Update () {
        if (moving)
        {
            transform.Translate(0, 0, speed);
            transform.eulerAngles = new Vector3(0f, line.getRotation(new Vector2(transform.position.x, transform.position.z)), 0f);
        }
        if (configuring) {
            moving = false;
            transform.eulerAngles = new Vector3(0f, line.getRotation(new Vector2(transform.position.x, transform.position.z)), 0f);
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("Wall") && hit.distance < Vector3.Distance(transform.position, line.transform.position))
                {
                    Debug.Log(hit.distance + " " + Vector3.Distance(transform.position, line.transform.position));
                    line.changeRadius(0.1f);
                }
                else {
                    configuring = false;
                    moving = true;
                    writer.WriteLine("Path " + index + " Curve Values: " + line.radiusModF + "F, " + line.radiusModS + "S");
                }
            }
            else {
                configuring = false;
                moving = true;
                writer.WriteLine("Path " + index + " Curve Values: " + line.radiusModF + "F, " + line.radiusModS + "S");
            }
        }
        if (line.isFinishedLine(new Vector2(transform.position.x, transform.position.z))) {
            line.radiusModS = 1f;
            index++;
            if (index < lineObjects.Length)
            {
                line = lineObjects[index].GetComponent<Line>();
                moving = false;
                StartCoroutine(Rotate(gameObject, line.getRotation(new Vector2(transform.position.x, transform.position.z))));
            }
            else {
                //index = 0;
                //line = lineObjects[0].GetComponent<Line>();
                moving = false;
                //StartCoroutine(Rotate(gameObject, line.getRotation(new Vector2(transform.position.x, transform.position.z))));
            }
        }
        
	}

    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

    IEnumerator Rotate(GameObject myObject, float angle)
    {
        float moveSpeed = 0.1f;
        float time = Time.time;
        angle = angle < 0 ? (360 - Math.Abs(angle)) : angle;
        while (Math.Abs(angle - transform.eulerAngles.y) > 0.5f)
        {
            myObject.transform.rotation = Quaternion.Slerp(myObject.transform.rotation, Quaternion.Euler(0, angle, 0), moveSpeed * (Time.time - time));
            yield return null;
        }
        myObject.transform.rotation = Quaternion.Euler(0, angle, 0);
        configuring = true;
        yield return null;
    }

    private void OnApplicationQuit()
    {
        writer.Flush();
        writer.Close();
    }
}

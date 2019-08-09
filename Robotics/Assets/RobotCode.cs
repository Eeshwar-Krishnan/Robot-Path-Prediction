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
    float timer;
    public GameObject mini;
    public GameObject robot;
    float number = 1f, totalNumObjects = 0f;
    GameObject temp;
    bool ended = false;
    float totalDistanceTraveled = 0;
    Vector3 lastPosition;
	// Use this for initialization
	void Start () {
        lineObjects = GameObject.FindGameObjectsWithTag("Line");
        Array.Sort(lineObjects, CompareObNames);
        line = lineObjects[0].GetComponent<Line>();
        writer = new StreamWriter("Assets/Resources/Output.txt", true);
        lastPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (moving)
        {
            transform.Translate(0, 0, speed * line.getSpeedMod());
            transform.eulerAngles = new Vector3(0f, line.getRotation(new Vector2(transform.position.x, transform.position.z)), 0f);
        }
        if (configuring) {
            moving = false;
            transform.eulerAngles = new Vector3(0f, line.getRotation(new Vector2(transform.position.x, transform.position.z)), 0f);
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit, Mathf.Infinity))
            {
               // Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("Wall") && hit.distance < Vector3.Distance(transform.position, line.transform.position) && line.isConfig)
                {
                    //Debug.Log(hit.distance + " " + Vector3.Distance(transform.position, line.transform.position));
                    line.changeRadius(0.1f);
                }
                else {
                    configuring = false;
                    moving = true;
                    //writer.WriteLine("Path " + index + " Curve Values: " + line.radiusModF + "F, " + line.radiusModS + "S");
                    //writer.WriteLine("Turn " + line.getRotation(new Vector2(transform.position.x, transform.position.z)));

                }
            }
            else {
                configuring = false;
                moving = true;
                //writer.WriteLine("Path " + index + " Curve Values: " + line.radiusModF + "F, " + line.radiusModS + "S");
                //writer.WriteLine("Turn " + line.getRotation(new Vector2(transform.position.x, transform.position.z)));
            }
        }
        if (line.isFinishedLine(new Vector2(transform.position.x, transform.position.z))) {
            index++;
            totalNumObjects += number;
            number = 0;
            if (index < lineObjects.Length)
            {
                //writer.WriteLine("Coordinate " + (number + 1) + ": " + new Vector2(line.transform.position.z, line.transform.position.x));
                //writer.WriteLine("Next Movement");
                line = lineObjects[index].GetComponent<Line>();
                moving = false;
                StartCoroutine(Rotate(gameObject, line.getRotation(new Vector2(transform.position.x, transform.position.z))));
            }
            else
            {
                if (!ended) {
                    temp = Instantiate(mini, transform.position, Quaternion.identity);
                    temp.name = (totalNumObjects + 1).ToString();
                    robot.GetComponent<BetterRobotCode>().initMarkers();
                    robot.GetComponent<BetterRobotCode>().running = true;
                    robot.GetComponent<BetterRobotCode>().totalDistance = totalDistanceTraveled;
                    //index = 0;
                    //line = lineObjects[0].GetComponent<Line>();
                    moving = false;
                    //StartCoroutine(Rotate(gameObject, line.getRotation(new Vector2(transform.position.x, transform.position.z))));
                    ended = true;
                }
            }
        }
        timer += Time.deltaTime;
        if (timer > 0.1f && moving) {
            temp = Instantiate(mini, transform.position, Quaternion.identity);
            //temp.name = (number+totalNumObjects).ToString();
            number++;
            //writer.WriteLine("Coordinate " + number + ": " + new Vector2(temp.transform.position.z, temp.transform.position.x));
            timer = 0f;
        }
        totalDistanceTraveled += Vector3.Distance(lastPosition, transform.position);
        lastPosition = transform.position;
        if (Math.Abs(transform.position.x - line.transform.position.x) < 0.5f) {
            Debug.Log(transform.position.z);
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

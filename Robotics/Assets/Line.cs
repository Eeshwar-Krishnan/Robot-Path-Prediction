﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {
    public float radiusModS, radiusModF;
    public bool strafeAdjust;
    public bool backUp;
	// Use this for initialization
	void Start () {
        if (backUp) {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public bool isFinishedLine(Vector2 position) {
        return Vector2.Distance(position, new Vector2(transform.position.x, transform.position.z)) < 0.1f;
    }

    public float getRotation(Vector2 position) {
        return (getAngle(new Vector2((transform.position.z - position.y) * radiusModS, (transform.position.x - position.x) * radiusModF))) + (backUp ? 180 : 0);
    }

    private float getAngle(Vector2 to) {
        return Mathf.Rad2Deg * Mathf.Atan2(to.y, to.x);
    }

    public void changeRadius(float amount) {
        if (strafeAdjust)
        {
            radiusModS += amount;
        }
        else {
            radiusModF += amount;
        }
    }

    public float getSpeedMod() {
        return backUp ? -1 : 1;
    }
}

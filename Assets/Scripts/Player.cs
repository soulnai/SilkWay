﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

using System.Collections.Generic;

public class Player : MonoBehaviour {

    public GameObject CurrentPOI;

    public int Resource = 10;

    public List<GameObject> visitedNodes = new List<GameObject>();

    public List<Companion> companions = new List<Companion>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

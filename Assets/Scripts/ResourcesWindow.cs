using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class ResourcesWindow : MonoBehaviour {

    public GameStateManager GM;
    public GameObject ResourcesAmount;

    public static event Action<POI> OnResourcesChanged;

    // Use this for initialization
    void Start () {
        
        GM = GameObject.Find("GM").GetComponent<GameStateManager>();

        GameObject ResourcesAmount = GameObject.Find("Amount");

    }

    private void UpdateResources()
    {

    }

    // Update is called once per frame
    void Update () {
	
	}
}

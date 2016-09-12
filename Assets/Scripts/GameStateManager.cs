using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using EnumSpace;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour {

    public GameObject player;
    public MapGenerator MG;
    public GameObject EventWindow;
    public List<RandomEvent> AllEvents = new List<RandomEvent>();

    public GameStates gamestate;

    public List<GameObject> tmpButtons;

    // Use this for initialization
    void Start () {
        MG = GameObject.Find("MapController").GetComponent<MapGenerator>();
        EventWindow = GameObject.Find("EventWindow");
        EventWindow.SetActive(false);
        gamestate = GameStates.GlobalMap;
        EventsGenerate();
    }

    private void EventsGenerate()
    {
        RandomEvent Nothing = new RandomEvent();

        Nothing.type = EnumSpace.EventType.Nothing;
        Nothing.StartMessageWindowHeader = "Unepected findings Start";
        Nothing.EndMessageWindowHeader = "Unepected findings End";
        Nothing.StartMessageWindowText = "One of your companions find something. You should take a look.";
        Nothing.EndMessageWindowTextSuccess = "You folow your {0} and he takes you to remains of old caravain. But still some valuables can be retrived from it.";
        Nothing.EndMessageWindowTextFailure = "You folow your {0}. But it was only desert rat.";

        Nothing.companions["dog"] = 0.1f;
        Nothing.companions["ranger"] = 0.3f;

        AllEvents.Add(Nothing);

        RandomEvent Luck = new RandomEvent();

        Luck.type = EnumSpace.EventType.Luck;

        AllEvents.Add(Luck);

        RandomEvent Trade = new RandomEvent();

        Trade.type = EnumSpace.EventType.Trade;

        AllEvents.Add(Trade);
        
        RandomEvent Fight = new RandomEvent();

        Fight.type = EnumSpace.EventType.Fight;
        Fight.StartMessageWindowHeader = "Random event - Fight Start";
        Fight.EndMessageWindowHeader = "Random event - Fight End";
        Fight.StartMessageWindowText = "Some local bandits came out of the corner. Prepare to fight.";
        Fight.EndMessageWindowTextSuccess = "Your {0} esealy beat bandits. Now they run away.";
        Fight.EndMessageWindowTextFailure = "Your {0} lost the fight. And you get some punches too.";

        Fight.companions["dog"] = 0.4f;
        Fight.companions["ranger"] = 0.2f;

        AllEvents.Add(Fight);
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void SetPlayer(GameObject playertoset)
    {
        player = playertoset;
    }

    public void LaunchEvent(POI poi)
    {
        if (player.GetComponent<Player>().CurrentPOI != MG.endPOI)
        {

            if (poi.RandomEvent != false && poi.Events.Count > 0)
            {
                gamestate = GameStates.EventWindow;
                ProcessEvent(poi);
                EventWindow.SetActive(true);
            }
            else
            {
                gamestate = GameStates.GlobalMap;
            }
        }
        else
        {
            gamestate = GameStates.GlobalMap;
        }
        Debug.Log(gamestate);
        //TODO: create message window
    }

    private void CheckVictory()
    {
        if (player.GetComponent<Player>().Resource <= 0 && player.GetComponent<Player>().CurrentPOI.GetComponent<POI>().Victory != true)
        {
            Debug.Log("You fail to reach Silk Way. No resources to continue");
            gamestate = GameStates.GameMenu;
            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Failure";
            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "You fail to reach Silk Way. No resources to continue";
            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(false);
            EventWindow.GetComponent<MessageWindowController>().Button.GetComponentInChildren<Text>().text = "Quit";
            EventWindow.SetActive(true);
        }
        else if (player.GetComponent<Player>().CurrentPOI.GetComponent<POI>().Victory == true)
        {
            Debug.Log("Victory! You discovered SilkWay!");
            gamestate = GameStates.GameMenu;
            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Success!";
            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "Victory! You discovered SilkWay!";
            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(false);
            EventWindow.GetComponent<MessageWindowController>().Button.GetComponentInChildren<Text>().text = "Victory!";
            EventWindow.SetActive(true);
        }

    }

    public void MoveUnitToPOI(POI targetPOI)
    {
        if (player.GetComponent<Player>().CurrentPOI.GetComponent<POI>().ConnectedNodes.Contains(targetPOI) /*&& !player.GetComponent<Player>().visitedNodes.Contains(targetPOI.gameObject)*/)
        {
            if (player.GetComponent<Player>().Resource >= targetPOI.GetComponent<POI>().PathCost)
            {
                gamestate = GameStates.PlayerMovement;
                player.GetComponent<Player>().visitedNodes.Add(player.GetComponent<Player>().CurrentPOI);
                player.gameObject.transform.DOMove(targetPOI.transform.position, 2).OnComplete(() => MovementCompleted(targetPOI));
            }
            else
            {
                Debug.Log("Not Enough Resources!");
                CheckVictory();
            }
        }
        else
        {
            Debug.Log("Nodes have no connection!");
        }
    }

    private void MovementCompleted(POI targetPOI)
    {
        player.GetComponent<Player>().CurrentPOI = targetPOI.gameObject;
        player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource - targetPOI.GetComponent<POI>().PathCost;
        CheckVictory();
        RevealNeighbours(player.GetComponent<Player>().CurrentPOI);
        LaunchEvent(targetPOI);
    }

    public void RevealNeighbours(GameObject poi)
    {
        foreach (POI point in poi.GetComponent<POI>().ConnectedNodes)
        {
            MG.DrawConnections(poi.GetComponent<POI>(), point);
            point.gameObject.SetActive(true);
            point.gameObject.transform.DOShakeScale(0.5f, 0.5f, 3, 50);
        }
    }

    public void ProcessEvent(POI poi)
    {
        EnumSpace.EventType ev = poi.Events[0].type;
        switch (ev)
        {
            case EnumSpace.EventType.Nothing:
                {
                    Debug.Log("Random event - Nothing");
                    poi.RandomEvent = false;

                        foreach (var item in poi.Events[0].companions.Keys)
                        {
                            if (player.GetComponent<Player>().companions.Contains(item))
                            {
                            GameObject Button = Instantiate(Resources.Load("prefab/Button"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                            string tmpitem = item;
                            GameObject tmpButton = Button;
                            tmpButton.transform.SetParent(EventWindow.transform, false);
                            tmpButton.GetComponent<Button>().onClick.AddListener(() => ProcessChoiseResult(poi.Events[0], tmpitem));
                            tmpButton.GetComponentInChildren<Text>().text = tmpitem + " (Chance = " + (0.3f+ poi.Events[0].companions[tmpitem]) * 100 + "%)";
                            tmpButtons.Add(tmpButton);
                            tmpButton.SetActive(true);
                            }
                        }
                    

                    if (player.GetComponent<Player>().companions.Contains("dog") || player.GetComponent<Player>().companions.Contains("ranger"))
                    {
                        EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = poi.Events[0].StartMessageWindowHeader;
                        EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = poi.Events[0].StartMessageWindowText;
                    }
                    else
                    {
                        EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
                        EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Nothing here";
                        EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "Nothing can be found here. Just rocky desert. You still have " + player.GetComponent<Player>().Resource + " resources.";
                    }
                    break;
                }
                    
            case EnumSpace.EventType.Trade:
                {
                    int success = UnityEngine.Random.Range(0, 2);
                    Debug.Log("Random event - Trade");
                    Debug.Log("Trade result = " + success);
                    if (success == 0)
                    {
                        Debug.Log("Unfortunate trade");
                        int loss = UnityEngine.Random.Range(1, 3);
                        Debug.Log("You lost " + loss + " resources");
                        player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource - loss;
                        Debug.Log("Total resources =  " + player.GetComponent<Player>().Resource);
                        poi.RandomEvent = false;
                        EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
                        EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Unfortunate trade";
                        EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "You lost " + loss + " resources. And now you have " + player.GetComponent<Player>().Resource + " resources.";
                    }
                    if (success == 1)
                    {
                        Debug.Log("Great trade");
                        int gain = UnityEngine.Random.Range(1, 3);
                        Debug.Log("You gain " + gain + " resources");
                        player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource + gain;
                        Debug.Log("Total resources =  " + player.GetComponent<Player>().Resource);
                        poi.RandomEvent = false;
                        EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
                        EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Great trade";
                        EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "You gain " + gain + " resources. And now you have " + player.GetComponent<Player>().Resource + " resources.";
                    }
                    break;
                }
            case EnumSpace.EventType.Fight:
                {
                    poi.RandomEvent = false;

                    foreach (var item in poi.Events[0].companions.Keys)
                    {
                        if (player.GetComponent<Player>().companions.Contains(item))
                        {
                            GameObject Button = Instantiate(Resources.Load("prefab/Button"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                            string tmpitem = item;
                            GameObject tmpButton = Button;
                            tmpButton.transform.SetParent(EventWindow.transform, false);
                            tmpButton.GetComponent<Button>().onClick.AddListener(() => ProcessChoiseResult(poi.Events[0], tmpitem));
                            tmpButton.GetComponentInChildren<Text>().text = tmpitem + " (Chance = " + (0.3f + poi.Events[0].companions[tmpitem]) * 100 + "%)";
                            tmpButtons.Add(tmpButton);
                            tmpButton.SetActive(true);
                        }
                    }


                    if (player.GetComponent<Player>().companions.Contains("dog") || player.GetComponent<Player>().companions.Contains("ranger"))
                    {
                        EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = poi.Events[0].StartMessageWindowHeader;
                        EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = poi.Events[0].StartMessageWindowText;
                    }
                    else
                    {
                        int success = UnityEngine.Random.Range(0, 2);
                        Debug.Log("Random event - Fight");
                        Debug.Log("Fight result = " + success);
                        if (success == 0)
                        {
                            Debug.Log("Unfortunate Fight");
                            int loss = UnityEngine.Random.Range(1, 3);
                            Debug.Log("You lost " + loss + " resources");
                            player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource - loss;
                            Debug.Log("Total resources =  " + player.GetComponent<Player>().Resource);
                            poi.RandomEvent = false;
                            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
                            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Unfortunate Fight";
                            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "You was beaten and robbered. Lost " + loss + " resources. And now you have " + player.GetComponent<Player>().Resource + " resources.";
                        }
                        if (success == 1)
                        {
                            Debug.Log("Great Fight");
                            int gain = UnityEngine.Random.Range(1, 3);
                            Debug.Log("You gain " + gain + " resources");
                            player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource + gain;
                            Debug.Log("Total resources =  " + player.GetComponent<Player>().Resource);
                            poi.RandomEvent = false;
                            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
                            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Great Fight";
                            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "You win! Bandits flee and left some resources. Gained " + gain + " resources. And now you have " + player.GetComponent<Player>().Resource + " resources.";
                        }
                    }
                    break;
                }
            case EnumSpace.EventType.Luck:
                {
                    Debug.Log("Random event - Luck");
                    int gain = UnityEngine.Random.Range(1, 3);
                    Debug.Log("You gain " + gain + " resources");
                    player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource + gain;
                    Debug.Log("Total resources =  " + player.GetComponent<Player>().Resource);
                    poi.RandomEvent = false;
                    EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
                    EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Luck!";
                    EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "Even this deserts can provide something useful. Gained " + gain + " resources. And now you have " + player.GetComponent<Player>().Resource + " resources.";
                    break;
                }
        }
    }

    private void ProcessChoiseResult(RandomEvent Event, string companion)
    {
        float baseChance = 0.3f;
        float result_chance = baseChance + Event.companions[companion];
        float rnd = UnityEngine.Random.value;
        if (rnd < result_chance)
        {
            Debug.Log(rnd);
            Debug.Log(companion);
            Debug.Log("Unexpected findings");
            int gain = UnityEngine.Random.Range(1, 3);
            Debug.Log("You gain " + gain + " resources");
            player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource + gain;
            Debug.Log("Total resources =  " + player.GetComponent<Player>().Resource);
            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
            foreach (GameObject b in tmpButtons)
            {
                b.SetActive(false);
                Destroy(b);
            }
            tmpButtons.Clear();
            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = Event.EndMessageWindowHeader;
            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = String.Format(Event.EndMessageWindowTextSuccess, companion) + " And now you have " + player.GetComponent<Player>().Resource + " resources.";
        }
        else
        {
            Debug.Log("Unexpected findings");
            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
            foreach (GameObject b in tmpButtons)
            {
                b.SetActive(false);
                Destroy(b);
            }
            tmpButtons.Clear();
            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = Event.EndMessageWindowHeader;
            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = String.Format(Event.EndMessageWindowTextFailure, companion) + " And now you have " + player.GetComponent<Player>().Resource + " resources.";
        }
        EventWindow.SetActive(true);
        foreach (GameObject b in tmpButtons)
        {
            b.SetActive(false);
            Destroy(b);
        }
        tmpButtons.Clear();
    }

    private void PrepareWindow(EnumSpace.EventType typeEvent)
    {
        throw new NotImplementedException();
    }
}

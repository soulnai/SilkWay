using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using EnumSpace;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameStateManager : MonoBehaviour {

    public GameObject player;
    public MapGenerator MG;
    public GameObject EventWindow;
    public List<RandomEvent> AllEvents = new List<RandomEvent>();

    public GameStates gamestate;

    public List<GameObject> tmpButtons;
    public AudioSource play;
    AudioClip EventSound;
    // Use this for initialization
    void Start () {
        MG = GameObject.Find("MapController").GetComponent<MapGenerator>();
        EventWindow = GameObject.Find("EventWindow");
        EventWindow.SetActive(false);
        gamestate = GameStates.GlobalMap;
        EventsGenerate();
        EventSound = Resources.Load("sounds/OGG/Accept", typeof(AudioClip)) as AudioClip;


    }

    private void SetCompanions()
    {
        Companion trader = new Companion();
        trader.CompanionName = "trader";
        trader.eventReactions.Add(EnumSpace.EventType.Trade, 0.7f);
        trader.eventReactions.Add(EnumSpace.EventType.Fight, 0.1f);
        trader.eventReactions.Add(EnumSpace.EventType.Nothing, 0.1f);
        trader.eventStringsSuccess.Add(EnumSpace.EventType.Nothing, "Trader accidentaly finds something usable.");
        trader.eventStringsLoss.Add(EnumSpace.EventType.Nothing, "It's just a camel skull.");
        trader.eventStringsSuccess.Add(EnumSpace.EventType.Trade, "Traders make a few steps from you and whispers something to each other. It's strange, but as result you get profit.");
        trader.eventStringsLoss.Add(EnumSpace.EventType.Trade, "No trade. At all, only losts.");
        trader.eventStringsSuccess.Add(EnumSpace.EventType.Fight, "First time of your life you see a Trader-berserker!!!");
        trader.eventStringsLoss.Add(EnumSpace.EventType.Fight, "What are you excpect from a trader in a fight? ");

        Companion dog = new Companion();
        dog.CompanionName = "dog";
        dog.eventReactions.Add(EnumSpace.EventType.Trade, 0.05f);
        dog.eventReactions.Add(EnumSpace.EventType.Fight, 0.7f);
        dog.eventReactions.Add(EnumSpace.EventType.Nothing, 0.1f);
        dog.eventStringsSuccess.Add(EnumSpace.EventType.Nothing, "Dog accidentaly finds something usable.");
        dog.eventStringsLoss.Add(EnumSpace.EventType.Nothing, "It's just a desert rat.");
        dog.eventStringsSuccess.Add(EnumSpace.EventType.Trade, "Dog just charm a trader and you get a profit from a deal.");
        dog.eventStringsLoss.Add(EnumSpace.EventType.Trade, "No trade. At all, only losts.");
        dog.eventStringsSuccess.Add(EnumSpace.EventType.Fight, "Dog wins!");
        dog.eventStringsLoss.Add(EnumSpace.EventType.Fight, "Dog lost a fight.");

        Companion ranger = new Companion();
        ranger.CompanionName = "ranger";
        ranger.eventReactions.Add(EnumSpace.EventType.Trade, 0.1f);
        ranger.eventReactions.Add(EnumSpace.EventType.Fight, 0.4f);
        ranger.eventReactions.Add(EnumSpace.EventType.Nothing, 0.7f);
        ranger.eventStringsSuccess.Add(EnumSpace.EventType.Nothing, "Ranger accidentaly finds something usable.");
        ranger.eventStringsLoss.Add(EnumSpace.EventType.Nothing, "It's just a caravain remains.");
        ranger.eventStringsSuccess.Add(EnumSpace.EventType.Trade, "Ranger told some coll stories to a trader. It makes profit!");
        ranger.eventStringsLoss.Add(EnumSpace.EventType.Trade, "No trade. At all, only losts.");
        ranger.eventStringsSuccess.Add(EnumSpace.EventType.Fight, "Ranger wins!");
        ranger.eventStringsLoss.Add(EnumSpace.EventType.Fight, "Ranger lost a fight.");
        
        player.GetComponent<Player>().companions.Add(trader);
        player.GetComponent<Player>().companions.Add(dog);
        player.GetComponent<Player>().companions.Add(ranger);
        
    }

    private void EventsGenerate()
    {
        RandomEvent Nothing = new RandomEvent();

        Nothing.type = EnumSpace.EventType.Nothing;
        Nothing.StartMessageWindowHeader = "Unepected findings Start";
        Nothing.EndMessageWindowHeader = "Unepected findings End";
        Nothing.StartMessageWindowText = "One of your companions find something. You should take a look.";

        AllEvents.Add(Nothing);

        RandomEvent Luck = new RandomEvent();

        Luck.type = EnumSpace.EventType.Luck;

        AllEvents.Add(Luck);

        RandomEvent Misfortune = new RandomEvent();

        Misfortune.type = EnumSpace.EventType.Misfortune;

        AllEvents.Add(Misfortune);

        RandomEvent Trade = new RandomEvent();

        Trade.type = EnumSpace.EventType.Trade;

        Trade.StartMessageWindowHeader = "Random event - Trade Start";
        Trade.EndMessageWindowHeader = "Random event - Trade End";
        Trade.StartMessageWindowText = "You meet another caravain. And have an opportunity to trade.";

        AllEvents.Add(Trade);

        RandomEvent Fight = new RandomEvent();

        Fight.type = EnumSpace.EventType.Fight;
        Fight.StartMessageWindowHeader = "Random event - Fight Start";
        Fight.EndMessageWindowHeader = "Random event - Fight End";
        Fight.StartMessageWindowText = "Some local bandits came out of the corner. Prepare to fight.";

        AllEvents.Add(Fight);
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void SetPlayer(GameObject playertoset)
    {
        player = playertoset;
        SetCompanions();
    }

    public void LaunchEvent(POI poi)
    {
        if (player.GetComponent<Player>().CurrentPOI != MG.endPOI)
        {

            if (poi.RandomEvent != false && poi.Events.Count > 0)
            {
                gamestate = GameStates.EventWindow;
                ProcessEvent(poi);
                PlaySound(EventSound);
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
    }

    public void PlaySound(AudioClip sound) 
    {
        play.PlayOneShot(sound);
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
        if (player.GetComponent<Player>().CurrentPOI.GetComponent<POI>().ConnectedNodes.Contains(targetPOI))
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
        poi.RandomEvent = false;
        switch (ev)
        {
            case EnumSpace.EventType.Nothing:
                {
                    Debug.Log("Random event - Nothing");
                    poi.RandomEvent = false;

                    GenerateButtons(poi);

                    if (CompanionCheck(ev))
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


                    GenerateButtons(poi);

                    if (CompanionCheck(ev))
                    {
                        EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = poi.Events[0].StartMessageWindowHeader;
                        EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = poi.Events[0].StartMessageWindowText;
                    }
                    else
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
                    }
                    break;
                }
            case EnumSpace.EventType.Fight:
                {
                    poi.RandomEvent = false;

                    GenerateButtons(poi);


                    if (CompanionCheck(ev))
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
            case EnumSpace.EventType.Misfortune:
                {
                    Debug.Log("Random event - Misfortune");
                    int lost = UnityEngine.Random.Range(1, 3);
                    Debug.Log("You lost " + lost + " resources");
                    player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource - lost;
                    Debug.Log("Total resources =  " + player.GetComponent<Player>().Resource);
                    poi.RandomEvent = false;
                    EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
                    EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = "Misfortune!";
                    EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = "This unwelcome land hides a lot of dangers. You lost " + lost + " resources. And now you have " + player.GetComponent<Player>().Resource + " resources.";
                    break;
                }
        }
    }

    private void GenerateButtons(POI poi)
    {
        
        foreach (var item in player.GetComponent<Player>().companions)
            {
                GameObject Button = Instantiate(Resources.Load("prefab/Button"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                Companion tmpitem = item;
                GameObject tmpButton = Button;
                tmpButton.transform.SetParent(EventWindow.transform, false);
                tmpButton.GetComponent<Button>().onClick.AddListener(() => ProcessChoiseResult(poi.Events[0], tmpitem));
                tmpButton.GetComponentInChildren<Text>().text = tmpitem.CompanionName + " (Chance = " + (0.2f + tmpitem.eventReactions[poi.Events[0].type]) * 100 + "%)";
                tmpButtons.Add(tmpButton);
                tmpButton.SetActive(true);
            }
//        }
    }

    private bool CompanionCheck(EnumSpace.EventType ev)
    {
        if (player.GetComponent<Player>().companions.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        /*
        switch (ev)
        {
            case EnumSpace.EventType.Nothing:
                {
                    if (player.GetComponent<Player>().companions.Contains("dog") || player.GetComponent<Player>().companions.Contains("ranger"))
                        return true;
                    break;
                }
            case EnumSpace.EventType.Fight:
                {
                    if (player.GetComponent<Player>().companions.Contains("dog") || player.GetComponent<Player>().companions.Contains("ranger"))
                        return true;
                    break;
                }
        }*/

    }

    private void ProcessChoiseResult(RandomEvent Event, Companion companion)
    {
        float baseChance = 0.2f;
        float result_chance = baseChance + companion.eventReactions[Event.type];
        float rnd = UnityEngine.Random.value;
        if (rnd < result_chance)
        {
            Debug.Log(rnd);
            Debug.Log(companion.CompanionName);
            int gain = UnityEngine.Random.Range(1, 3);
            player.GetComponent<Player>().Resource = player.GetComponent<Player>().Resource + gain;
            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
            foreach (GameObject b in tmpButtons)
            {
                b.SetActive(false);
                Destroy(b);
            }
            tmpButtons.Clear();
            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = Event.EndMessageWindowHeader;
            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = companion.eventStringsSuccess[Event.type] + " And now you have " + player.GetComponent<Player>().Resource + " resources.";
        }
        else
        {
            Debug.Log(rnd);
            Debug.Log(companion.CompanionName);
            EventWindow.GetComponent<MessageWindowController>().Button.SetActive(true);
            foreach (GameObject b in tmpButtons)
            {
                b.SetActive(false);
                Destroy(b);
            }
            tmpButtons.Clear();
            EventWindow.GetComponent<MessageWindowController>().EventName.GetComponent<TextMeshProUGUI>().text = Event.EndMessageWindowHeader;
            EventWindow.GetComponent<MessageWindowController>().EventDescription.GetComponent<TextMeshProUGUI>().text = companion.eventStringsLoss[Event.type] + " And now you have " + player.GetComponent<Player>().Resource + " resources.";
        }
        EventWindow.SetActive(true);
        foreach (GameObject b in tmpButtons)
        {
            b.SetActive(false);
            Destroy(b);
        }
        tmpButtons.Clear();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PressStartToJoinPlayerSelector : MonoBehaviour {

    public int maxPlayerCount = 4;

    private List<PlayerMap> playerMap; //Maps Rewired Player ids to game player ids
    private int gamePlayerIdCounter = 0;

    private class PlayerMap
    {
        public int rewiredPlayerId;
        public int gamePlayerId;
        public int controllerId;

        public PlayerMap (int rewiredPlayerId, int gamePlayerId, int controllerId)
        {
            this.rewiredPlayerId = rewiredPlayerId;
            this.gamePlayerId = gamePlayerId;
            this.controllerId = controllerId;
        }
    }

    private void Awake()
    {
        playerMap = new List<PlayerMap>();
        ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnected;
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 4; i++)
        {
            if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame"))
            {
                AssignNextPlayer(i);
            }
        }
    }

    private void OnDestroy()
    {
        ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnected;
    }

    private void AssignNextPlayer(int rewiredPlayerId)
    {
        if (playerMap.Count >= maxPlayerCount)
        {
            return;
        }
        
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
        Controller r_player_controller = null;
        //Note: gamePlayerId incrementing unnecessarily
        if (rewiredPlayer.controllers.joystickCount <= 1)
        {
            foreach (Controller cm in rewiredPlayer.controllers.Controllers)
            {
                if (cm.type == ControllerType.Joystick)
                {
                    bool cancel = false;
                    for (int i = 0; i < playerMap.Count; i++)
                    {
                        if (cm.id == playerMap[i].controllerId)
                        {
                            cancel = true;
                            break;
                        }
                    }
                    if (!cancel)
                    {
                        int gamePlayerId = GetNextGamePlayerId();
                        r_player_controller = cm;
                        playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId, r_player_controller.id));
                    }

                    break;
                }
            }
        }

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, "Assignment");

        // Enable UI control for this Player now that he has joined
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "FreeSelect");
    }

    private int GetNextGamePlayerId()
    {
        return gamePlayerIdCounter++;
    }

    public void NewControllerFound(int rewiredPlayerId, Controller player_controller, bool activeOn)
    {
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
        if (activeOn)
        {
            if (player_controller.type == ControllerType.Joystick)
            {
                int gamePlayerId = GetNextGamePlayerId();
                playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId, player_controller.id));

                // Disable the Assignment map category in Player so no more JoinGame Actions return
                rewiredPlayer.controllers.maps.SetMapsEnabled(false, "Assignment");

                // Enable UI control for this Player now that he has joined
                rewiredPlayer.controllers.maps.SetMapsEnabled(true, "FreeSelect");
            }
        } else
        {
            // Disable the Assignment map category in Player so no more JoinGame Actions return
            rewiredPlayer.controllers.maps.SetAllMapsEnabled(false);

            // Enable UI control for this Player now that he has joined
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Assignment");
        }
    }

    public void forceActivePlayer(int pID)
    {
        if (ReInput.players.GetPlayer(pID).controllers.joystickCount > 0 || ReInput.players.GetPlayer(pID).controllers.hasKeyboard)
        {
            AssignNextPlayer(pID);
        }
    }

    private void OnControllerPreDisconnected(ControllerStatusChangedEventArgs args)
    {
        bool check = false;
        for (int i = 0; i < playerMap.Count; i++)
        {
            if (playerMap[i].controllerId == args.controllerId)
            {
                check = true;
                break;
            }
        }
        if (check)
        {
            gamePlayerIdCounter--;
            //Player rewiredPlayer = ReInput.players.GetPlayer(args.controllerId);
            Debug.Log("Removed: " + args.controllerId);
            playerMap.RemoveAll(a => a.controllerId == args.controllerId);
        }
    }
}

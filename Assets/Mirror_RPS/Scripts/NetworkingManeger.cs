using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

namespace RPS
{
    public class NetworkingManeger : NetworkManager
    {
        static NetworkingManeger _instance;
        public static NetworkingManeger Instance => _instance;
        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }
        public bool IsHost => IsServer && IsClient;
        public string localPlayerName { get; private set; }
        List<NetworkingPlayer> players = new List<NetworkingPlayer>();
        public NetworkingPlayer LocalPlayer => players.First(x => x.isLocalPlayer);
        public NetworkingPlayer OtherPlayer => players.First(x => !x.isLocalPlayer);
        public override void Awake()
        {
            base.Awake();
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            players = new List<NetworkingPlayer>();
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;
            Debug.Log("Server started");
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            IsClient = true;
            Debug.Log("Client started");
        }

        public void updatePlayerName(string playerName)
        {
            localPlayerName = playerName;
            Debug.Log($"Player name updated to {localPlayerName}");
        }

        public void AddPlayer(NetworkingPlayer player)
        {
            if (!players.Contains(player)) players.Add(player);
        }
        public void RemovePlayer(NetworkingPlayer player)
        {
            if (players.Contains(player)) players.Remove(player);
        }
        [Server]
        bool AllPlayersPlayed()
        {
            return players.Count == 2 && players.All(x => x.PlayerMove != PlayerMove.None);
        }
        [Server]
        public void CheckToCalculateResult()
        {
            if (!AllPlayersPlayed()) return;
            PlayerMove p1Move = players[0].PlayerMove;
            PlayerMove p2Move = players[1].PlayerMove;
            //  x x -> 0 -> x-x=0
            //  1 0 , 2 1 , 0 2 -> 1 // x (x-1)%3 -> 1 -> (x-(x-1))%3=1
            //  0 1 , 1 2 , 2 0 -> 2 // x (x+1)%3 -> 2 -> (x-(x+1))%3=2
            //  a b => a-b= {-1,0,1} => 0 = draw , 1 = p1 win , 2 = p1 lose => x+1 -> 1 draw 2 win 3 lose
            //  (p1Move - p2Move)%3+1 = {1,2,3} => 1 = draw , 2 = p1 win , 3 = p1 lose
            EndResult p1Result = (EndResult)(((int)p1Move - (int)p2Move + 3) % 3 + 1);
            EndResult p2Result = (EndResult)(((int)p2Move - (int)p1Move + 3) % 3 + 1);
            Debug.Log($"Player 1 Move: {p1Move}, Player 2 Move: {p2Move}");
            Debug.Log($"Player 1 Result: {p1Result}, Player 2 Result: {p2Result}");
            players[0].UpdateScore(p1Result == EndResult.Win);
            players[1].UpdateScore(p2Result == EndResult.Win);
            players[0].TargetSetEndResult(p1Result);
            players[1].TargetSetEndResult(p2Result);
        }
    }
}

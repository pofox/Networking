using UnityEngine;
using Mirror;

namespace RPS
{
    public class NetworkingPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(onNameUpdated))] string pName;
        [SyncVar(hook = nameof(OnMoveUpdated))] PlayerMove playerMove;
        [SyncVar(hook = nameof(OnScoreUpdated))] int score;
        gameplayUI gameUI;

        public PlayerMove PlayerMove => playerMove;
        private void Awake()
        {
            gameUI = FindFirstObjectByType<gameplayUI>();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            CmdUpdatePlayerName(NetworkingManeger.Instance.localPlayerName);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            NetworkingManeger.Instance.AddPlayer(this);
        }

        [Server]
        public void UpdateScore(bool increment)
        {
            score += increment ? 1 : 0;
        }

        #region RPCs

        [Command]
        void CmdUpdatePlayerName(string playerName)
        {
            pName = playerName;
        }
        [Command]
        public void CmdUpdatePlayerMove(PlayerMove newMove)
        {
            playerMove = newMove;
            NetworkingManeger.Instance.CheckToCalculateResult();
        }
        [TargetRpc]
        public void TargetSetEndResult(EndResult result)
        {
            gameUI.DisplayEndResult(result);
            CmdUpdatePlayerMove(PlayerMove.None);
        }

        #endregion

        #region Hooks

        void onNameUpdated(string oldVal,string newVal)
        {
            pName = newVal;
            gameUI.UpdatePlayerName(isLocalPlayer, pName);
        }
        void OnMoveUpdated(PlayerMove oldMove, PlayerMove newMove)
        {
            playerMove = newMove;
            Debug.Log($"Player netId of {netId} changed to {newMove}");
        }

        void OnScoreUpdated(int oldScore, int newScore)
        {
            score = newScore;
            gameUI.UpdateScore(isLocalPlayer, score);
        }

        #endregion

        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkingManeger.Instance.RemovePlayer(this);
        }
    }
}

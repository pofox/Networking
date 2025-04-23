using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPS
{
    public enum PlayerMove
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scissors = 3,
    }
    public enum EndResult
    {
        None = 0,
        Draw = 1,
        Win = 2,
        Lose = 3,
    }
    public class gameplayUI : MonoBehaviour
    {
        [Header("Player UI")]
        [SerializeField] TextMeshProUGUI txt_localplayerName;
        [SerializeField] TextMeshProUGUI txt_localplayerScore;
        [Header("Opponent UI")]
        [SerializeField] TextMeshProUGUI txt_opponentName;
        [SerializeField] TextMeshProUGUI txt_opponentScore;
        [Header("Game UI")]
        [SerializeField] Button btn_Rock;
        [SerializeField] Button btn_Paper;
        [SerializeField] Button btn_Scissors;
        [SerializeField] Animator popup;
        [SerializeField] TextMeshProUGUI txt_Result;

        public void UpdatePlayerName(bool islocal, string playerName)
        {
            if (islocal)
            {
                txt_localplayerName.text = playerName;
            }
            else
            {
                txt_opponentName.text = playerName;
            }
        }
        public void OnMoveSelected(int Move)
        {
            NetworkingManeger.Instance.LocalPlayer.CmdUpdatePlayerMove((PlayerMove)Move);
        }

        public void UpdateScore(bool islocal, int score)
        {
            if (islocal)
            {
                txt_localplayerScore.text = "Score : " + score.ToString();
            }
            else
            {
                txt_opponentScore.text = "Score : " + score.ToString();
            }
        }
        public void DisplayEndResult(EndResult result)
        {
            txt_Result.text = result.ToString();
            popup.SetTrigger("Show");
            Invoke("ResetButtons", 2f);
        }
        void ResetButtons()
        {
            btn_Rock.interactable = true;
            btn_Paper.interactable = true;
            btn_Scissors.interactable = true;
        }
    }
}

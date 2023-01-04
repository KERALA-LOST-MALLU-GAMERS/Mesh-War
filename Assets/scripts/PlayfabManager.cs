using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayfabManager : MonoBehaviour
{

    public GameObject panelMainMenu;
    public GameObject panelStartGame;
    public GameObject panelScoreBoard;
    public GameObject buttonStartGame;
    public GameObject inputFieldName;

    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    // Update is called once per frame
    void Login()
    {
        var request = new LoginWithCustomIDRequest {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result) {
        Debug.Log("Success login/account create!");
        string name = null;
        if(result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        
        if(name != null)
            inputFieldName.GetComponent<TMP_InputField>().text = name;
    }

    public void SubmitButtonStartGame() {
        var inputName = inputFieldName.GetComponent<TMP_InputField>().text; 

        if (inputName == "")
            inputName = "Guest";

        var request = new UpdateUserTitleDisplayNameRequest {
            DisplayName = inputName,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result) {
        Debug.Log("Updated display name");
        panelMainMenu.SetActive(false);
    }

    void OnError(PlayFabError error) { 
        Debug.Log("Error while logging in/creating account!");
    }

    public void SendScoreBoard(int score) {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreBoardUpdate, OnError);
    }

    void OnScoreBoardUpdate(UpdatePlayerStatisticsResult result) {
        Debug.Log("Success on score board update"); 
    }

    public void GetScoreBoard() {
        var request = new GetLeaderboardRequest
            {
                StatisticName = "Score",
            };
            PlayFabClientAPI.GetLeaderboard(request, OnScoreBoardGet, OnError);
    }
    
    void OnScoreBoardGet(GetLeaderboardResult result) {
        foreach (var item in result.Leaderboard) {
                Debug.Log(item.Position + " " + item.DisplayName + " " + item.StatValue);
        }
    }
}

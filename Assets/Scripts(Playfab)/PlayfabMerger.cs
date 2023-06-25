using PlayFab;
using PlayFab.ClientModels;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Runtime.Serialization;

[System.Serializable]
public class PlayfabMerger : MonoBehaviour
{
    [Header("GameObject")]
    [SerializeField] GameObject login;
    [SerializeField] GameObject character;

    [Header("UI")]
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;

    [Header("Character")]
    [SerializeField] CharacterStats characterStats;

    private const string TITLE_ID = "E3BB5";
    private const string CHARACTER_DATA_KEY = "PlayerStats";


    #region Init
    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result) => Debug.Log("Loged In");

    void OnError(PlayFabError error) => Debug.LogError(error.ErrorMessage + "; " + error.GenerateErrorReport());
    #endregion

    #region Account Login
    public void RegisterButton()
    {
        if (passwordInput.text.Length < 6)
        {
            Debug.Log("Passwarod is too short"); return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void ResestPassButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = TITLE_ID
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }


    void OnLoginSuccess(LoginResult obj)
    {
        Debug.Log("You have successfuly logged in");
        OnLoginManager();
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registered and logged in");
        OnLoginManager();
    }

    private void OnPasswordReset(SendAccountRecoveryEmailResult obj)
    {
        Debug.Log("Password reset email sent");
    }
    #endregion

    #region Player Data
    public void LoadPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecevied, OnError);
    }

    private void OnDataRecevied(GetUserDataResult obj)
    {
        Debug.Log("RecivedData");
        if (obj.Data != null & obj.Data.ContainsKey(CHARACTER_DATA_KEY)) {
            var character = JsonUtility.FromJson<Character>(obj.Data[CHARACTER_DATA_KEY].Value);
            characterStats.LoadData(character);
        }
    }

    public void SavePlayerData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> {
                {CHARACTER_DATA_KEY, characterStats.GetCharacterJson()}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnUserDataSuccess, OnError);
    }

    private void OnUserDataSuccess(UpdateUserDataResult obj)
    {
        Debug.Log("request for data save sent");
    }
    #endregion

    #region UI Manipulation
    void OnLoginManager()
    {
        login.SetActive(false);
        character.SetActive(true);
    }
    #endregion

}

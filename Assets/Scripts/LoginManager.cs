using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using FullSerializer;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class LoginManager : MonoBehaviour
{

    public DataInitializer dataInitializer;

    public GameObject LoginMessage;
    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;

    public TMP_Text messageText;

    public TMP_Text userText;
    public TMP_Text passphraseText;

    public TMP_Dropdown locationDropdown;

    public string curUsername;
    public string curPassphrase;

    public Button LoginButton;
    public GameObject LoginButtonObject;

    void Start() {
        CloseLoginMessage();
    }

    public void TryLogin() {

        LoginButton.interactable = false;
        LoginButtonObject.SetActive(false);

        if (locationDropdown.value == 0) {
            DisplayLoginMessage(6);
            return;
        }

        curUsername = userText.text;
        curPassphrase = passphraseText.text;

        if (curUsername.Length >= 3 && curUsername.Substring(0, 2) == "~&") {
            string id = curUsername.Substring(2);
            curUsername = id;
            DisplayLoginMessage(0);
        } else if (curUsername.Length >= 3 && curUsername.Substring(0, 2) == "~~") {
            string id = curUsername.Substring(2);
            curUsername = id;
            DeleteUser();
        } else {
            string path = "dashboards/" + curUsername + "/passphrase";
            FirebaseDatabase.GetJSON(path, gameObject.name, "UserFound", "NoUserFound");
        }
    }

    public void DeleteUser() {
        string path = "dashboards/" + curUsername + "/";
        FirebaseDatabase.DeleteJSON(path, gameObject.name, "UserDeleted", "UserNotDeleted");
    }

    public void UserDeleted(string response) {
        DisplayLoginMessage(4);
    }

    public void UserNotDeleted(string error) {
        Debug.Log(error);
        DisplayLoginMessage(5);
    }

    public void CreateUser() {
        string path = "dashboards/" + curUsername + "/passphrase";
        string JSONString = Serialize(typeof(string), (object) curPassphrase);
        FirebaseDatabase.PostJSON(path, JSONString, gameObject.name, "UserCreated", "UserNotCreated");
    }

    void UserCreated(string response) {
        dataInitializer.userID = curUsername;
        dataInitializer.curLocation = (locationDropdown.value);
        dataInitializer.TryPullUserData();
    }

    void UserNotCreated(string error) {
        Debug.Log(error);
        DisplayLoginMessage(3);
    }
    
    void UserFound(string passphraseJSON) {

        if (passphraseJSON == null || passphraseJSON == "null") {    
            DisplayLoginMessage(2);
            return;
        }

        string correctPassphrase = (string) Deserialize(typeof(string), passphraseJSON);

        if (curPassphrase == correctPassphrase) {
            dataInitializer.userID = curUsername;
            dataInitializer.curLocation = (locationDropdown.value);
            dataInitializer.TryPullUserData();
            DisplayLoginMessage(7);
        } else {
            DisplayLoginMessage(1);
        }
    }

    void NoUserFound(string error) {
        DisplayLoginMessage(2);
    }

    public void DisplayLoginMessage(int message) {
        Button1.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);
        LoginMessage.SetActive(true);

        if (message == 0) {
            Button2.SetActive(true);
            Button3.SetActive(true);
            string temp = "Would you like to initialize a user with ID: \"" + curUsername + "\" and passphrase: \"" + curPassphrase + "\"?";
            messageText.text = temp;
        } else if (message == 7) {
            messageText.text = "Success, logging in...";
        }
        else {
            Button1.SetActive(true);
            if (message == 1) {
                messageText.text = "Login failed: Incorrect passphrase.";
            } else if (message == 2) {
                messageText.text = "Login failed: No user with that ID.";
            } else if (message == 3) {
                messageText.text = "Failed to create user.";
            } else if (message == 4) {
                messageText.text = "User " + curUsername + " deleted.";
            } else if (message == 5) {
                messageText.text = "User failed to be deleted.";
            } else if (message == 6) {
                messageText.text = "Please choose a location before logging in.";
            }
        }
    }

    public void CloseLoginMessage() {
        LoginButton.interactable = true;
        LoginButtonObject.SetActive(true);
        LoginMessage.SetActive(false);
        Button1.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);
    }

    private static readonly fsSerializer _serializer = new fsSerializer();

    public static object Deserialize(Type type, string serializedState) {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }

    public static string Serialize(Type type, object value) {
        // serialize the data
        fsData data;
        _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }
}

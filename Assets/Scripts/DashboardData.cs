using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FullSerializer;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class Dashboard {

    [fsIgnore]
    public Dictionary<int, Texture2D> userPictures;
    public int userPicturesAmount;

    public string username;
    public string bio;
    public List<string> affinityNames;
    public Color usernameColor;
    public Color bioColor;
    public List<int> headerImages;
    public List<Vector3> headerScales;
    public List<Vector3> headerPositions;
    public List<Color> headerColors;
    public int backgroundImage;
    public Color backgroundColor;
    public int profileImage;
    public Vector3 profileScale;
    public Vector3 profilePosition;
    public Color profileColor;
    public int layoutType;

    public string userID;

    public int currentLocation;

    public Dashboard(string newUserID) {
        init();
        userID = newUserID;
    }

    public Dashboard() {
        init();
    }

    void init() {
        userPictures = new Dictionary<int, Texture2D>();
        userPicturesAmount = 0;
        username = "Your Name Here";
        bio = "Enter a Bio Here";
        affinityNames = new List<string>();
        usernameColor = Color.black;
        bioColor = Color.black;
        headerImages = new List<int>();
        headerScales = new List<Vector3>();
        headerPositions = new List<Vector3>();
        headerColors = new List<Color>();
        headerImages.Add(-1);
        headerScales.Add(new Vector3(1.14f, 1.14f, 1.0f));
        headerPositions.Add(new Vector3(-14.2f, 186.8f, 0.0f));
        headerColors.Add(Color.white);
        backgroundImage = -1;
        backgroundColor = new Color(0.3411765f, 0.7372549f, 0.7686275f);
        profileImage = -1;
        profileScale = new Vector3(0.19f, 0.19f, 1.0f); 
        profilePosition = new Vector3(-0.75f, 1.78f, 0.0f);
        profileColor = Color.white;
        layoutType = 0;
        userID = "nobody";
        currentLocation = 5;
    }

}

public static class DashboardData
{
    public static Dashboard currentUserDashboard;
    public static List<Dashboard> allUserDashboards = new List<Dashboard>();

    public static Dictionary<string, Color> categoryColors = new Dictionary<string, Color>(){
        {"PROGRAMMING", Color.cyan},
        {"INPUTS", Color.yellow},
        {"OUTPUTS", Color.magenta},
        {"WIRING", Color.green},
        {"DESIGN", Color.blue},
        {"PURPOSE", Color.red},
        {"NOTHING", Color.white},
    };

    public static Dictionary<string, string> categoryNames = new Dictionary<string, string>(){
        {"Makecode", "PROGRAMMING"},
        {"Debugging", "PROGRAMMING"},
        {"Variables", "PROGRAMMING"},
        {"Functions", "PROGRAMMING"},
        {"Conditionals", "PROGRAMMING"},
        {"Temperature Sensor", "INPUTS"},
        {"Distance Sensor", "INPUTS"},
        {"Touch Sensor", "INPUTS"},
        {"Sound Sensor", "INPUTS"},
        {"Gyroscope", "INPUTS"},
        {"LEDs", "OUTPUTS"},
        {"Servo Motors", "OUTPUTS"},
        {"DC Motors", "OUTPUTS"},
        {"Music and Sound", "OUTPUTS"},
        {"Conductive Tape", "WIRING"},
        {"Wiring", "WIRING"},
        {"Breadboards", "WIRING"},
        {"Batteries", "WIRING"},
        {"Microcontrollers", "WIRING"},
        {"Soldering", "WIRING"},
        {"Creativity", "DESIGN"},
        {"Problem Solving", "DESIGN"},
        {"Cardboard Design", "DESIGN"},
        {"Ideation", "DESIGN"},
        {"Prototyping", "DESIGN"},
        {"Laser Cutting", "DESIGN"},
        {"Sewing", "DESIGN"},
        {"3D Printing", "DESIGN"},
        {"Social Justice", "PURPOSE"},
        {"Community Building", "PURPOSE"},
        {"Empowerment", "PURPOSE"},
        {"Racial Justice", "PURPOSE"},
        {"Women's Rights", "PURPOSE"},
        {"LGBTQ+", "PURPOSE"},
        {"Family", "PURPOSE"},
        {"Well-being", "PURPOSE"},
        {"Mental Health", "PURPOSE"},
    };

    public static Dictionary<int, string> locationNumbers = new Dictionary<int, string>(){
        {1, "WELL"},
        {2, "SDLL"},
        {3, "DREAAM"},
        {4, "Milan"},
        {5, "Remote"},
        {6, "Nowhere Important"},
    };

    public static Dictionary<int, string> affinityTypeNumbers = new Dictionary<int, string>(){
        {1, "PROGRAMMING"},
        {2, "INPUTS"},
        {3, "OUTPUTS"},
        {4, "WIRING"},
        {5, "DESIGN"},
        {6, "PURPOSE"},
    };
}

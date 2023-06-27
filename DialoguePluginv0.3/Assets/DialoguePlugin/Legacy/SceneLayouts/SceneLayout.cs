using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[CreateAssetMenu(fileName = "SceneLayout", menuName = "Dialogue Plugin/Scene Layout")]
public class SceneLayout : MonoBehaviour //ScriptableObject
{
    public Image background;
    public Image dialogueCharacter;
    public Image textFieldBackground;
    public Text nameField;
    public Text textField;
    public GameObject viewPortContent;
    public float buttonSpacing;
    public GameObject buttonPrefab;
    public GameObject skipButton;
    public GameObject nextButton;
    public GameObject backButton;
}

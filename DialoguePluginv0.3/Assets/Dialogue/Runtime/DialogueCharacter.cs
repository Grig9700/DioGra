using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class DialogueCharacter : ScriptableObject
{
    public List<CharacterExpressions> expressions = new List<CharacterExpressions>();
}

[Serializable]
public struct CharacterExpressions
{
    public string emotion;
    public Image image;
}
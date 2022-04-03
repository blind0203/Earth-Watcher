using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute]
public class Card : ScriptableObject
{
    public int TeamIndex;
    public Sprite TeamImage;

    public int[] TeamValues;
}

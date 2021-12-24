using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu, System.Serializable]
public class PictureProgression : ScriptableObject {

    public Image bg;
    public Image[] lockedParts;
    public Image[] unlockedParts;

}

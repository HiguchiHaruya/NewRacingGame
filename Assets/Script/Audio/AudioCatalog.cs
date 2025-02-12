using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/AudioCatalog")]
public class AudioCatalog : ScriptableObject
{
    public string clipID;
    public AudioClip clip;
}

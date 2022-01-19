using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    [SerializeField] protected string npcName;
    public string NPCName { get { return npcName; } }
}

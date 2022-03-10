using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skills Data", menuName = "Scriptable Object/Skills Data", order = int.MaxValue)]
public class SkillsInfoSO : ScriptableObject
{
    public Pair<string, SkillInfo[]> playerOriginBodySkills;
    public List<Pair<Enemy.EnemyType, SkillInfo[]>> monsterSkillsList;
}

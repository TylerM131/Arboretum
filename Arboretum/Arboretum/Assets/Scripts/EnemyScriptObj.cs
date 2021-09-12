using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptObj", menuName = "EnemyScriptObj")]

public class EnemyScriptObj : ScriptableObject
{
    public int damage;
    public int initialHealth;
}

using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class BaseEnemyStatsInfo : ScriptableObject
{
    [Header("Enemy Stats")]
    public float maxHealthState = 100f;
    public float armorState = 5f;
    public float moveSpeedState = 3f;
    public float attackDamageState = 5f;
    public float attackRangeState = 2f;
    public float attackSpeedState = 1f;
    public float expRewardState = 50f;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseEnemy :IBaseHuman
{
    public  Vector2 PlayerDirection();

    public void FollowPlayer(Vector2 targetPos);

    public void Create(BaseEnemyStatsInfo baseEnemyInfo);

    public void Spawn(Vector2 spawnPosition);

    public void DeSpawn();
    public void ChasePlayer(Vector2 targetPos);
    public void AwardExp();

    public void SlowAttack(float power, float duration);
    public void KnockBack(Vector2 direction,float power, float knockbackTime);
    public float PlayerDistance(Vector2 playerPos);


    IEnumerator SlowEffect(float power, float duration);
    IEnumerator DieEffect(float duration);
    IEnumerator KnockbackEffect(float duration);
    IEnumerator StunEffect(float duration);

}

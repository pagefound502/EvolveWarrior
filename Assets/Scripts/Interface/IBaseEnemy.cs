using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseEnemy :IBaseHuman
{
    public Vector2 PlayerDirection();

    public void FollowPlayer(Vector2 targetPos);

    public void Create(BaseEnemyStatsInfo baseEnemyInfo);

    public void Spawn(Vector2 spawnPosition);

    public void DeSpawn();

    public void AwardExp();
    
    public void KnowBack();
    public float PlayerDistance(Vector2 playerPos);

}

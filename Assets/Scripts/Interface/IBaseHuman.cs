using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseHuman 
{
    public void Walk();
    public void TakeDamage(float damage);
    public void AttackDamage();
    public void TryAttack();
    public void Stun();
    public void Die();
    public void CalculateHealth(float value, bool isHeal=false);


}


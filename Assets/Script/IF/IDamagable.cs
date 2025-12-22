using UnityEngine;

public interface IDamagable
{
    bool isAlive { get; }
    bool Damage(int damage);


}

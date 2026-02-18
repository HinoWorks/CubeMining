using UnityEngine;

public class BulletCont_Bow : BulletBase
{

    public override void Init(int _damage, float _lifetime, Vector3 _direction)
    {
        base.SetBulletType(BulletType.Piercing);
        base.Init(_damage, _lifetime, _direction);
    }

}

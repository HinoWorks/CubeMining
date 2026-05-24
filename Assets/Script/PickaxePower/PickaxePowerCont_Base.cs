using UnityEngine;

public class PickaxePowerCont_Base : MonoBehaviour
{
    protected PickaxePowerLevel EquippedLevelData;

    public void Init(PickaxePowerLevel _EquippedLevelData)
    {
        EquippedLevelData = _EquippedLevelData;
    }

    public virtual void Activate() { }


    public virtual void OnDestroyCall()
    {
        Destroy(this.gameObject);
    }


}

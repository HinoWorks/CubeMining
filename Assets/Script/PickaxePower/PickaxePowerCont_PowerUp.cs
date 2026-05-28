using UnityEngine;

public class PickaxePowerCont_PowerUp : PickaxePowerCont_Base
{
    [SerializeField] GameObject pf_PowerUp;

    private float damageUpRate => EquippedLevelData.value_1;
    private float speedUpRate => EquippedLevelData.value_2;
    private float sizeUpRate => EquippedLevelData.value_3;
    private float powerUpDuration => EquippedLevelData.value_4;



    private bool isPowerUp = false;
    private float powerUpDurationRemaining = 0f;

    private GameObject obj_powerUp;


    public override void Activate()
    {
        if (isPowerUp) return;

        Debug.Log("Power == PowerUp");
        powerUpDurationRemaining = powerUpDuration;
        PickaxePowerManager.Inst?.ApplyPickaxePowerBuff(damageUpRate, speedUpRate, sizeUpRate);
        CameraManager.Inst.ShakeCamera_Large();
        StaticManager.SlowGameTime_PickaxePower();
        CreatePowerUp();
        isPowerUp = true;
    }

    private void CreatePowerUp()
    {
        obj_powerUp = Instantiate(pf_PowerUp, transform) as GameObject;
        obj_powerUp.transform.position = AttackManager.Inst.currentPickaxePosition;
    }
    private void EndPowerUp()
    {
        if (!isPowerUp) return;

        isPowerUp = false;
        powerUpDurationRemaining = 0f;
        PickaxePowerManager.Inst?.EndPickaxePowerBuff();
        DeletePowerUp();
        Debug.Log("EndPowerUp");
    }

    private void DeletePowerUp()
    {
        if (obj_powerUp == null) return;
        Destroy(obj_powerUp);
        obj_powerUp = null;
    }

    void Update()
    {
        if (!isPowerUp) return;
        obj_powerUp.transform.position = AttackManager.Inst.currentPickaxePosition;
        powerUpDurationRemaining -= Time.deltaTime;
        if (powerUpDurationRemaining <= 0f)
            EndPowerUp();
    }

    public override void OnDestroyCall()
    {
        EndPowerUp();
        base.OnDestroyCall();
    }



}

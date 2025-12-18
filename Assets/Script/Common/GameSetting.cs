using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameSetting : MonoBehaviour
{
    void Start()
    {
        // ** Game内FlameRate固定 **
        QualitySettings.vSyncCount = 0; // VSyncをOFFにする
        Application.targetFrameRate = 45; //60FPSに設定
        //"Edit"→"Project Settings"→"Quality"__"VSync Count"を変更
        //"Every Second VBlank"（垂直同期の半分, "Don't Sync"（垂直同期無し）
        //垂直同期無し設定で上記設定が反映される

        // 物理演算使用時はFixedUpDate周期も変更
        // ProjectSetting - Time - FixedTimeStep
        // 0.02 -> 0.0166
        //**********************




        // ***************
        // ** DOTween-Sequence生成時の自動再生をOFFとする **
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;
        // ***************

    }

}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Text;

/// <summary>
/// GameParamManagerの各値を表示するデバッグパネル
/// </summary>
public class UI_DebugParamPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_paramDisplay_gameBase;
    [SerializeField] TextMeshProUGUI tmp_paramDisplay_blockGenerate;
    [SerializeField] TextMeshProUGUI tmp_paramDisplay_attack;
    [SerializeField] GameObject targetObj;
    //[SerializeField] ScrollRect scrollRect;

    private StringBuilder stringBuilder_gameBase = new StringBuilder();
    private StringBuilder stringBuilder_blockGenerate = new StringBuilder();
    private StringBuilder stringBuilder_attack = new StringBuilder();
    private float updateInterval = 0.1f; // 更新間隔（秒）
    private float lastUpdateTime = 0f;

    private void Update()
    {
        // キー入力で表示/非表示を切り替え（F1キー）
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            TogglePanel();
            tmp_paramDisplay_gameBase.ForceMeshUpdate();
            tmp_paramDisplay_blockGenerate.ForceMeshUpdate();
            tmp_paramDisplay_attack.ForceMeshUpdate();
        }

        // パネルが表示されている場合のみ更新
        if (targetObj != null && targetObj.activeSelf && Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateParamDisplay();
            lastUpdateTime = Time.time;
        }
    }

    private void TogglePanel()
    {
        if (targetObj != null)
        {
            bool newState = !targetObj.activeSelf;
            targetObj.SetActive(newState);

            // 表示された時に即座に更新
            if (newState)
            {
                UpdateParamDisplay();
            }
        }
    }

    private void UpdateParamDisplay()
    {
        stringBuilder_gameBase.Clear();
        stringBuilder_blockGenerate.Clear();
        stringBuilder_attack.Clear();

        // GameBaseParamの表示
        stringBuilder_gameBase.AppendLine("=== Game Base Param ===");
        stringBuilder_gameBase.AppendLine($"Ingame Time: {GameParamManager.gameBaseParam.ingameTime:F2}");
        stringBuilder_gameBase.AppendLine($"Bonus Rate: {GameParamManager.gameBaseParam.bonusRate:F2}");
        stringBuilder_gameBase.AppendLine();

        // BlockGenerateParamの表示
        stringBuilder_blockGenerate.AppendLine("=== Block Generate Param ===");
        for (int i = 0; i < GameParamManager.list_blockGenerateParam.Count; i++)
        {
            var blockParam = GameParamManager.list_blockGenerateParam[i];
            stringBuilder_blockGenerate.AppendLine($"Block [{blockParam.blockIndex}]");
            stringBuilder_blockGenerate.AppendLine($"  Active: {blockParam.isActive}");
            stringBuilder_blockGenerate.AppendLine($"  HP: {blockParam.hp}");
            stringBuilder_blockGenerate.AppendLine($"  Base Value: {blockParam.baseValue}");
            stringBuilder_blockGenerate.AppendLine($"  Generate Interval: {blockParam.generateInterval:F2}");
            stringBuilder_blockGenerate.AppendLine($"  Count: {blockParam.count}");
            stringBuilder_blockGenerate.AppendLine($"  Size: {blockParam.size:F2}");
            stringBuilder_blockGenerate.AppendLine();
        }

        // AttackParamの表示
        stringBuilder_attack.AppendLine("=== Attack Param ===");
        for (int i = 0; i < GameParamManager.list_attackParam.Count; i++)
        {
            var attackParam = GameParamManager.list_attackParam[i];
            stringBuilder_attack.AppendLine($"Attack [{attackParam.attackUnitIndex}]");
            stringBuilder_attack.AppendLine($"  Active: {attackParam.isActive}");
            stringBuilder_attack.AppendLine($"  Damage: {attackParam.damage:F2}");
            stringBuilder_attack.AppendLine($"  Alive Time: {attackParam.aliveTime:F2}");
            stringBuilder_attack.AppendLine($"  CT: {attackParam.ct:F2}");
            stringBuilder_attack.AppendLine($"  Count: {attackParam.count}");
            stringBuilder_attack.AppendLine($"  Attack Interval: {attackParam.attackInterval:F2}");
            stringBuilder_attack.AppendLine($"  Size: {attackParam.size:F2}");
            stringBuilder_attack.AppendLine();
        }

        tmp_paramDisplay_gameBase.text = stringBuilder_gameBase.ToString();
        tmp_paramDisplay_blockGenerate.text = stringBuilder_blockGenerate.ToString();
        tmp_paramDisplay_attack.text = stringBuilder_attack.ToString();
    }
}


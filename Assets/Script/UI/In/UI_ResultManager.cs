using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class UI_ResultManager : UI_PopUpBase
{
    [SerializeField] UI_ResultUnitCont[] ui_resultUnitConts;

    [Space(10)]
    [Header("アーティファクト")]
    [SerializeField] GameObject obj_line;
    [SerializeField] GameObject parent_artifact;
    [SerializeField] UI_ResultArtifactCont[] ui_resultArtifactConts;

    [Space(10)]
    [Header("ボタン")]
    [SerializeField] GameObject obj_buttons;

    public override void Open()
    {
        base.Open();
        Init();
    }
    public override void Close()
    {
        base.Close();
    }


    private async void Init()
    {
        // == 初期化 ==
        obj_buttons.SetActive(false);
        obj_line.SetActive(false);
        parent_artifact.SetActive(false);
        foreach (var ui_resultUnitCont in ui_resultUnitConts)
        {
            ui_resultUnitCont.gameObject.SetActive(false);
        }
        foreach (var ui_resultArtifactCont in ui_resultArtifactConts)
        {
            ui_resultArtifactCont.gameObject.SetActive(false);
        }
        // -----


        var resourceDataList = InGameManager.Inst.Get_ResourceDataList();
        resourceDataList.Sort((a, b) => a.resourceType.CompareTo(b.resourceType));

        await UniTask.Delay(500);
        int index = 0;
        foreach (var resourceData in resourceDataList)
        {
            var ui_resultUnitCont = ui_resultUnitConts[index];
            var currentCount = SaveLoader.Inst.Get_ResourceCount(resourceData.resourceType);
            //Debug.Log($"resourceType: {resourceData.resourceType}, Get: {resourceData.resourceCount}, current: {currentCount} => result: {resourceData.resourceCount + currentCount}");
            await ui_resultUnitCont.SetData(resourceData.resourceType, resourceData.resourceCount, currentCount);
            index++;
        }

        // アーティファクトゲットしていた場合、表示
        if (InGameManager.Inst.Get_ArtifactCount() > 0)
        {
            await UniTask.Delay(200);
            obj_line.SetActive(true);
            parent_artifact.SetActive(true);
            var artifactIndexList = InGameManager.Inst.Get_ArtifactIndexList();
            index = 0;
            foreach (var artifactIndex in artifactIndexList)
            {
                await UniTask.Delay(200);
                var ui_resultArtifactCont = ui_resultArtifactConts[index];
                var artifactData = SOLoader.ArtifactData.Get_ArtifactData(artifactIndex);
                ui_resultArtifactCont.SetData(artifactData);
                index++;
            }
        }

        await UniTask.Delay(250);
        obj_buttons.SetActive(true);
    }




    #region -- on Click --
    public void OnClick_IngameReady()
    {
        Close();
        GameWatcher.Inst.SetGameState(GameStateType.ResultEnd_ToIngameReady);
    }
    public void OnClick_OutGame()
    {
        Close();
        GameWatcher.Inst.SetGameState(GameStateType.ResultEnd_ToOutGame);
    }
    #endregion
}

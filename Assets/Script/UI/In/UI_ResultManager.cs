using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class UI_ResultManager : UI_PopUpBase
{
    [SerializeField] UI_ResultUnitCont[] ui_resultUnitConts;

    [SerializeField] Transform parent_artifact;
    [SerializeField] GameObject pf_getArtifactCont;
    private List<UI_GetArtifactCont> ui_getArtifactConts = new List<UI_GetArtifactCont>();
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
        obj_buttons.SetActive(false);
        foreach (var ui_resultUnitCont in ui_resultUnitConts)
        {
            ui_resultUnitCont.gameObject.SetActive(false);
        }

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



        await UniTask.Delay(200);
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

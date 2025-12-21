using UnityEngine;
using UniRx;

public class UI_ResultManager : UI_PopUpBase
{



    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }




    #region -- on Click --
    public void OnClick_Next()
    {
        Close();
        GameWatcher.Inst.SetGameState(GameStateType.OutGame);
    }
    #endregion
}

using UnityEngine;
using Cysharp.Threading.Tasks;


public class UI_OutGameTabBase : MonoBehaviour
{
    protected bool isReloadFin = false;
    protected OutGame_MenuType thisMenuType;



    public virtual void Start_OnceInit()//主にコールバックを設定
    {
    }

    public virtual async void Init(OutGame_MenuType _outGameMenuType)
    {
        var isActive = _outGameMenuType == thisMenuType;

        if (isActive)
        {
            await UniTask.WaitUntil(() => isReloadFin);
        }
        this.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// アウトゲームに移行した時、一度だけデータを更新する
    /// </summary>
    public virtual async void ToOutGame_InitData() { }

    public void ToInGame_ResetLoadFlag()
    {
        isReloadFin = false;
    }

}

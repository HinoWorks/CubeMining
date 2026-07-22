using UnityEngine;

public class UI_ClearManager : MonoBehaviour
{
    public void Open()
    {

        this.gameObject.SetActive(true);
    }

    public void OnClick_Close()
    {
        this.gameObject.SetActive(false);
    }
}

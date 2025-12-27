using UnityEngine;
using UnityEngine.Events;

public class HButtonConnect : MonoBehaviour
{
    [SerializeField] GameObject obj_mouseOver;
    [SerializeField] GameObject obj_clickSelect;

    public UnityAction rightClick;

    void Awake()
    {
        var button = this.GetComponent<HButton>();
        button.onMouseOver += Set_MouseOverActive;
        button.onSelect += Set_SelectActive;
        button.rightClick += RightClickAction;
    }

    public void Set_MouseOverActive(bool _active)
    {
        if (obj_mouseOver == null) return;
        obj_mouseOver.SetActive(_active);
    }

    public void Set_SelectActive(bool _active)
    {
        if (obj_clickSelect == null) return;
        obj_clickSelect.SetActive(_active);
    }

    public void RightClickAction()
    {
        if (rightClick != null)
        {
            rightClick.Invoke();
        }
    }


}

using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{


    public static InputManager Inst;
    [SerializeField] private InputActionAsset inputActions;

    /*
    [Space(5)]
    [SerializeField] InputActionReference move_Left;
    [SerializeField] InputActionReference move_Right;
    [SerializeField] InputActionReference move_Up;
    [SerializeField] InputActionReference move_Down;

    [Space(5)]
    [SerializeField] InputActionReference rotate_R;
    [SerializeField] InputActionReference rotate_L;
    */

    [Space(5)]
    [SerializeField] InputActionReference focus;
    [SerializeField] InputActionReference zoom_Action;

    [Space(5)]
    [SerializeField] InputActionReference menu_CreateBlock;
    [SerializeField] InputActionReference menu_Upgrade;
    [SerializeField] InputActionReference esc_action;



/*
    public InputActionReference Move_Left => move_Left;
    public InputActionReference Move_Right => move_Right;
    public InputActionReference Move_Up => move_Up;
    public InputActionReference Move_Down => move_Down;

    public InputActionReference Rotate_R => rotate_R;
    public InputActionReference Rotate_L => rotate_L;
    */

    public InputActionReference Focus => focus;
    public InputActionReference Zoom_Action => zoom_Action;

    public InputActionReference Menu_CreateBlock => menu_CreateBlock;
    public InputActionReference Menu_Upgrade => menu_Upgrade;
    public InputActionReference ESC => esc_action;




    private void Awake()
    {
        if (Inst == null) { Inst = this; }
        else { Destroy(this); }
        SwitchToPlayer();
    }

    private void Set_Map(string _mapName)
    {
        // すべてのマップを一度無効化
        foreach (var map in inputActions.actionMaps)
            map.Disable();

        // 指定したマップだけ有効化
        var targetMap = inputActions.FindActionMap(_mapName, throwIfNotFound: false);
        if (targetMap != null)
            targetMap.Enable();
    }

    public void SwitchToUI()
    {
        Set_Map("UI");
    }

    public void SwitchToPlayer()
    {
        Set_Map("Default");
    }



}

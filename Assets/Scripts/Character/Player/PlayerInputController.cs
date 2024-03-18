using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour, CrossPlayerInput.IMainPlayMapActions
{
    public static PlayerInputController instance;
    public CrossPlayerInput InputActions { get; private set; }
    
    public CrossPlayerInput.MainPlayMapActions MainActions { get; private set; }

    private PlayerController controller;
    public bool isControlable { get; private set; }

    private void Awake()
    {
        instance = this;
        InputActions = new CrossPlayerInput();

        MainActions = InputActions.MainPlayMap;
        MainActions.AddCallbacks(this);
    }

    public void InitPlayerInputController()
    {
        controller = PlayerManager.instance.player.controller;
    }


    public void OnSkill1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            if (PlayerManager.instance.CanUseSkill(0))
                controller.CallSkill(0);
        }
    }

    public void OnSkill2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            if (PlayerManager.instance.CanUseSkill(1))
                controller.CallSkill(1);
        }
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        
    }

    private void OnEnable()
    {
        InputActions.Enable();
        isControlable = true;
    }

    private void OnDisable()
    {
        InputActions.Disable();
        isControlable = false;
    }
}

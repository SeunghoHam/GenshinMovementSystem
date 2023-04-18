using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))] // 컴포넌트에 이 스크립트를 추가하면 PlayerInput 스크립트도 추가됨
public class Player : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }

    public Transform MainCameraTransform { get; private set; }
    
    // PlyaerInput
    public PlayerInput Input { get; private set; }
    // PlayerState
    private PlayerMovementStateMachine movementStateMachine; 

    private void Awake()
    {
        // StateMachine 이 활성화 되기 전에 해야함
        Input = GetComponent<PlayerInput>();
        Rigidbody = GetComponent<Rigidbody>();
        MainCameraTransform = Camera.main.transform;
        
        movementStateMachine = new PlayerMovementStateMachine(this);

    }

    private void Start()
    {
        movementStateMachine.ChangeState(movementStateMachine.IdlingState);
    }

    private void Update()
    {
        movementStateMachine.HandleInput();
        movementStateMachine.Update();
    }

    private void FixedUpdate()
    {
        movementStateMachine.PhysicsUpdate();
    }
}

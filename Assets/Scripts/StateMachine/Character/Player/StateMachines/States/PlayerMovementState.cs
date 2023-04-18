using System.Reflection;
using Cinemachine;
using UnityEngine;

public class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine stateMachine;
    protected Vector2 movementInput;
    
    protected float baseSpeed = 2f;
    protected float speedModifier = 2f;

    protected Vector3 currentTargetRotation;
    protected Vector3 timeToReachTargetRotation;
    protected Vector3 dampedTargetRotationCurrentVelocity;
    protected Vector3 dampedTargetRotationPassedTime;
    public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
    {
        stateMachine = playerMovementStateMachine;
        
        InitializeData();
    }

    private void InitializeData()
    {
        timeToReachTargetRotation.y = 0.14f;
        
    }

    #region ::: IState Methods :::

    

    public virtual void Enter()
    {
        Debug.Log("State " + GetType().Name);
    }

    public virtual void Exit()
    {
        
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }


    public virtual void Update()
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        Move();
    }


    #endregion

    #region ::: Main Methods :::

    private void ReadMovementInput()
    {
        movementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }
    private void Move()
    {
        if (movementInput == Vector2.zero || 
            speedModifier == 0f)
        {
            Debug.Log("이동없음");
            return;
        }

        Vector3 movementDirection = GetMovementInputDirection();

        float targetRotationYAngle = Rotate(movementDirection);
        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);
        
        float movementSpeed = GetMovementSpeed();
        
        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
        
        stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed
                                               -currentPlayerHorizontalVelocity
            , ForceMode.VelocityChange);
    }
 
    private float Rotate(Vector3 direction)
    {
        // Atan2 = 탄젠트
        // 음의 각도와 양의 각도 사이에서 결정할 때 ( 180, -180 이 동일한 위치일 때)
        float directionAngle = UpdateTargetRotation(direction);
        RotateTowardsTargetRotation();
        return directionAngle;
    }



    private float GetDirectionAngle(Vector3 direction)
    {
        // x - direction.x / y = direction.z 할당시키기
        float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (directionAngle < 0f)
        {
            directionAngle += 360f;
        }
        return directionAngle;
    }
    
    private float AddCameraRotationToAngle(float angle)
    {
        // "Horizontal Axis"값 필드를 업데이트하여 확인할 수 있음
        angle += stateMachine.Player.MainCameraTransform.eulerAngles.y;

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return angle;
    }
    private void UpdateTargetRotation(float targetAngle)
    {
        currentTargetRotation.y = targetAngle;

        dampedTargetRotationPassedTime.y = 0f;
    }
    
    #endregion


    #region ::: Reuseable Methods :::

    protected Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;

        playerHorizontalVelocity.y = 0f;
        return playerHorizontalVelocity;
    }
    protected float GetMovementSpeed()
    {
        return baseSpeed * speedModifier;
    }
    protected Vector3 GetMovementInputDirection()
    {
        return new Vector3(movementInput.x, 0f, movementInput.y);
    }
    protected void RotateTowardsTargetRotation()
    {
        float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;
        if (currentYAngle == currentTargetRotation.y) // 목표에 도달
        {
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, currentTargetRotation.y, 
            ref dampedTargetRotationCurrentVelocity.y, 
            timeToReachTargetRotation.y - dampedTargetRotationPassedTime.y);
        dampedTargetRotationPassedTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);
        
        stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
    }
    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
        float directionAngle = GetDirectionAngle(direction);
        if (shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);    
        }
        
        if (directionAngle != currentTargetRotation.y)
        {
            UpdateTargetRotation(directionAngle);
        }

        return directionAngle;
    }
    protected  Vector3 GetTargetRotationDirection(float targetAngle)
    {
        return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    #endregion
}

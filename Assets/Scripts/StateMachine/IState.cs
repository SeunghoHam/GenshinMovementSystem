public interface IState
{
    // Enter : 상태가 현재 상태가 될 때마다 실행된다
    // Exit : 상태가 이전 상태가 될 때마다 실행된다
    
    public void Enter();
    public void Exit();
    public void HandleInput();
    public void Update(); // Monobehavior
    public void PhysicsUpdate(); // Monobehaviour
}

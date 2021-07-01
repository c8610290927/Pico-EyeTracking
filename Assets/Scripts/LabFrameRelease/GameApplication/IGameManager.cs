using System.Collections;

public interface IGameManager
{
    int Weight { get; }
    void ManagerInit();
    IEnumerator ManagerDispose();
}
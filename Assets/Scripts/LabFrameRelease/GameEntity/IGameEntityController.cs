using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEntityController 
{
    List<GameEntityBase> GameEntities { get; set; }
    void EntityBuild(GameEntityBase @base);
    void EntityDestroy(GameEntityBase @base);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityController : MonoBehaviour,IGameEntityController
{   
    public List<GameEntityBase> GameEntities { get; set; }

    public void EntityBuild(GameEntityBase @base)
    {
        GameEntities.Add(@base);
    }

    public void EntityDestroy(GameEntityBase @base)
    {
        GameEntities.Remove(@base);
        Destroy(@base.gameObject);
    }

 
}

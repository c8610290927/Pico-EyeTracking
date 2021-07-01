using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEntityBase:MonoBehaviour
{
    public string Describe;

    protected IGameEntityController EntityController;

    public void Awake()
    {
        EntityController = GameEntityManager.Instance.EntityController;
        EntityController.EntityBuild(this);
    }
  
    public virtual void EntityInit()
    {
      
    }

    public abstract void EntityDispose();

    public virtual void EntityDestroy()
    {
        EntityController.EntityDestroy(this);        
    }
   
}

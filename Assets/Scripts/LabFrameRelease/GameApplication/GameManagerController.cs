using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManagerController 
{
   private static readonly List<IGameManager> s_GameManagers=new List<IGameManager>();

   public static void ManagersInit()
   {
       s_GameManagers.ForEach(p=>p.ManagerInit());
   }

   public static void ManagersDispose()
   {
       s_GameManagers.ForEach(p=>p.ManagerDispose());
       s_GameManagers.Clear();
   }

   public static T GetManager<T>() where  T:class
   {
       Type interfaceType = typeof(T);
       if (!interfaceType.IsInterface)
       {
           throw new Exception( string.Format("You must get module by interface, but '{0}' is not.", interfaceType.FullName));
       }
       Type moduleType = Type.GetType(interfaceType.Name.Substring(1));
       foreach (var sGameManager in s_GameManagers)
       {
           if (sGameManager.GetType()==moduleType)
           {
               return sGameManager as T;
           }
       }
       var manager= Activator.CreateInstance(moduleType ?? throw new InvalidOperationException()) as T;
        s_GameManagers.Add(manager as IGameManager);
        return manager;
   }
}

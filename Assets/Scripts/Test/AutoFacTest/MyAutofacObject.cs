using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autofac;


public sealed class MyAutofacObject : IDoThing
{
    public MyAutofacObject()
    {
        Debug.Log("Constructor for our object!");
    }

    public string DoThing()
    {
        return "Test";
    }
}


public interface IDoThing
{
    string DoThing();
}

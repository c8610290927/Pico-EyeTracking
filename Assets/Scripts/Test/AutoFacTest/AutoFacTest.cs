using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autofac;
using UnityEngine.UI;


public class AutoFacTest : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        var containerBuilder = new Autofac.ContainerBuilder();
        containerBuilder.RegisterType<MyAutofacObject>().SingleInstance();

        var container = containerBuilder.Build();
        var instance = container.Resolve<MyAutofacObject>();

        text.text= instance.DoThing();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}

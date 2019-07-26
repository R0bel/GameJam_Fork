using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingOrder : Attribute
{
    public int Priority { get; set; }
}

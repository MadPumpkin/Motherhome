using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ConsoleCommandAttribute: Attribute
{
    public string Name;
    public string Description = "";
}
using System.Collections.Generic;
using UnityEngine;

public class Anwser : ScriptableObject
{
    public int anwser { get; set; }
    public List<int> diceroll { get; set; }
}

public class Question : ScriptableObject
{
    public string questionText { get; set; }
    public List<Anwser> anwser { get; set; }
}

public class Root : ScriptableObject
{
    public List<Question> questions { get; set; }
}



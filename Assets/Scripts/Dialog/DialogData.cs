using UnityEngine;
[CreateAssetMenu(fileName = "New Dialog Data", menuName = "Dialog/Dialog Data")]
public class DialogData : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] sentences;
}

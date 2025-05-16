
[System.Serializable]
public class Achievement
{
    public string title;
    public string description;
    public int currentProgress;
    public int requiredProgress;
    public bool isCompleted => currentProgress >= requiredProgress;
}
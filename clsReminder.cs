namespace AlarmClock;

public class Reminder(bool check, string name, string dateString, int repeat, int action, string progPath, bool askBefore, bool hibernate)
{
    private bool check = check;
    private string name = name;
    private string dateString = dateString;
    private DateTime datetime = string.IsNullOrEmpty(dateString) ? DateTime.MinValue : Utilities.ConvertStringToDateTime(dateString);
    private int repeat = repeat;
    private int action = action;
    private string progPath = progPath;
    private bool askBefore = askBefore;
    private bool hibernate = hibernate;

    public bool Check
    {
        get => check; set => check = value;
    }

    public string Name
    {
        get => name; set => name = value;
    }

    public string DateString
    {
        get => dateString;
        set
        {
            dateString = value;
            datetime = Utilities.ConvertStringToDateTime(value);
        }
    }

    public DateTime Datetime
    {
        get => datetime; set
        {
            datetime = value; 
            dateString = datetime.ToString("ddMMyyyyHHmmss");
        }
    }

    public int Repeat
    {
        get => repeat; set => repeat = value;
    }

    public int Action
    {
        get => action; set => action = value;
    }

    public string ProgPath
    {
        get => progPath; set => progPath = value;
    }

    public bool AskBefore
    {
        get => askBefore; set => askBefore = value;
    }

    public bool Hibernate
    {
        get => hibernate; set => hibernate = value;
    }

}
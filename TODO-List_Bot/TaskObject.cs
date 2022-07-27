using System.Security.Cryptography;

namespace TODO_List_Bot;

public class TaskObject
{
    
    //(string name, string decription, int year, int month, int day, int hour, int minute)
    public TaskObject(string name)
    {
        Name = name;
        // Decription = decription;
        // Date = new DateOnly(year, month, day);
        // Time = new TimeOnly(hour, minute);
    }

    public string Name { get; set; }
    // public string Decription { get; set; }
    // public DateOnly Date { get; set; }  
    // public TimeOnly Time { get; set; }
}
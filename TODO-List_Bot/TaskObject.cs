using System.Security.Cryptography;
using TODO_List_Bot.Commands;
using TODO_List_Bot.Interfaces;

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
public static class Extensions
    { 

public static ICommand? Do(this TaskObject task) {
        if (task is null) {
            return null;
        }
        return task.Name switch {
            "finish" => new FinishTask( ),
            "edit" => new EditTask( ),
            "delete" => new DeleteTask( ),
            _ => null,
        };
    }
}
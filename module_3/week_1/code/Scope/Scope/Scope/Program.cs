// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

static void DisplayMessage()
{
    string day = "Friday";

    if(day == "Friday")
    {
        int hour1 = 5;
        int hour2 = 10;
        Console.WriteLine(hour1);
        Console.WriteLine("Value of hour1 and hour2 = " + "{0}" +" {1}", hour1, hour2);
        //Console.WriteLine(hour2);
    }
    else if(day == "Saturday")
    {
        int hour2 = 4;
    }
    else
    {
        int hour3 = 7;

    }
}

DisplayMessage();




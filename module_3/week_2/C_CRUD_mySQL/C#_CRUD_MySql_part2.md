# CRUD App using C# and SQL Part 2

## Overview
In this lesson we'll build another CRUD app, this time using an Object Relational Mapper to persist the relationship between models.


## Setup

As before, create another new console project called "crudSQL_ORM"
Next, edit the .csproj file by right clicking the project in the left hand column and click Edit Project File.
Paste in the following;

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp7.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Data.SqlClient" Version="4.4.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="2.0.0" />
  </ItemGroup>

</Project>
```

This is the same as last time with the exception of the Microsoft.EntityFrameworkCore.SqlServer which has been added and will provide us ORM functionality.

For this example, let’s create two tables. The first will hold data about “users”. Create a User.cs file in the crudSQL_ORM folder (not the main).

```

using System;
using System.Collections.Generic;

namespace crudSQL_ORM
{
    public class User
    {
        public int UserId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public virtual IList<Task> Tasks { get; set; }

        public String GetFullName()
        {
            return this.FirstName + " " + this.LastName;
        }
        public override string ToString()
        {
            return "User [id=" + this.UserId + ", name=" + this.GetFullName() + "]";
        }
    }
}
```

Here we have declared the property names and types out User wil require as well as any methods.
Notably there is no need for a constructor or getter and setters.

Lets create a second table to assign tasks to users. Create a new class named Task in the same folder.

```
using System;
namespace crudSQL_ORM
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsComplete { get; set; }
        public virtual User AssignedTo { get; set; }

        public override string ToString()
        {
            return "Task [id=" + this.TaskId + ", title=" + this.Title + ", dueDate=" + this.DueDate.ToString() + ", IsComplete=" + this.IsComplete + "]";
        }
    }
}
```

Let’s also create a class for the Entity Framework Database context.  Create a new class named CRUDORMContext.cs. 
```
using System;
using Microsoft.EntityFrameworkCore;

namespace crudSQL_ORM
{
    public class CRUDORMContext : DbContext
    {
        string _connectionString;
        public CRUDORMContext(string connectionString)
        {
            this._connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this._connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}
```
Lets take this opportunity to check our dependencies are installed correctly (you may be noticing some red lines).
Right click the project and click Manage NuGet Packages.
Search for and select;
```
System.Data.SqlClient
Microsoft.EntityFrameworkCore.SqlServer
```

Install the packages and accept the user agreements.
Click build -> rebuild project.

(Instructor note: Had to change to target framework 4 here to make EFSampleContext recognised in the program.cs)

Next, lets establish our db connection and perform some CRUD operations.

Program.cs
```
namespace crudSQL_ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("** C# CRUD sample with Entity Framework Core and SQL Server **\n");
            try
            {
                // Build connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "localhost,1433"; 
                builder.UserID = "SA";           
                builder.Password = "Pearljam0004!!";
                builder.InitialCatalog = "sampleDB";
                builder.Encrypt = false;


                using (CRUDORMContext context = new CRUDORMContext(builder.ConnectionString))
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    Console.WriteLine("Created database schema from C# classes.");
                    <!-- Insert further content here -->
                }
            }   catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("All done. Press any key to finish...");
            Console.ReadKey(true);
        }
    }
}
```
Here we are using a string builder to create our connection string as we did in the previous example. One difference is that the ORM automatically implements encryption, something we will dispense with for the moment by adding Encrypt = false to our connection string.

We will be using an instance of our CRUDORMContext class to execute all our db communications. The following code therefore will be placed under the Console.WriteLine.
Lets create a User and a Task instance and save them to the db then assign a task to a user;
```
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    Console.WriteLine("Created database schema from C# classes.");

                    // Create demo: Create a User instance and save it to the database
                    User newUser = new User { FirstName = "Anna", LastName = "Shrestinian" };
                    context.Users.Add(newUser);
                    context.SaveChanges();
                    Console.WriteLine("\nCreated User: " + newUser.ToString());

                    // Create demo: Create a Task instance and save it to the database
                    Task newTask = new Task() { Title = "Ship Helsinki", IsComplete = false, DueDate = DateTime.Parse("04-01-2017") };
                    context.Tasks.Add(newTask);
                    context.SaveChanges();
                    Console.WriteLine("\nCreated Task: " + newTask.ToString());

                    // Association demo: Assign task to user
                    newTask.AssignedTo = newUser;
                    context.SaveChanges();
                    Console.WriteLine("\nAssigned Task: '" + newTask.Title + "' to user '" + newUser.GetFullName()
```

Running the file at this point, you should see;

```
Created database schema from C# classes.

Created User: User [id=1, name=Anna Shrestinian]

Created Task: Task [id=1, title=Ship Helsinki, dueDate=4/1/2017 12:00:00 AM, IsComplete=False]

Assigned Task: 'Ship Helsinki' to user 'Anna Shrestinian'
All done. Press any key to finish...
```

Lets demonstrate finding the incomplete tasks assigned to Anna.

```
                    // Read demo: find incomplete tasks assigned to user 'Anna'
                    Console.WriteLine("\nIncomplete tasks assigned to 'Anna':");
                    var query = from t in context.Tasks
                                where t.IsComplete == false &&
                                t.AssignedTo.FirstName.Equals("Anna")
                                select t;
                    foreach (var t in query)
                    {
                        Console.WriteLine(t.ToString());
                    }
```

We can change the due date of the first task;
```
                    // Update demo: change the 'dueDate' of a task
                    Task taskToUpdate = context.Tasks.First(); // get the first task
                    Console.WriteLine("\nUpdating task: " + taskToUpdate.ToString());
                    taskToUpdate.DueDate = DateTime.Parse("06-30-2016");
                    context.SaveChanges();
                    Console.WriteLine("dueDate changed: " + taskToUpdate.ToString());
```
Delete all tasks with a due date in 2016.
```
                    // Delete demo: delete all tasks with a dueDate in 2016
                    Console.WriteLine("\nDeleting all tasks with a dueDate in 2016");
                    DateTime dueDate2016 = DateTime.Parse("12-31-2016");
                    query = from t in context.Tasks
                            where t.DueDate < dueDate2016
                            select t;
                    foreach (Task t in query)
                    {
                        Console.WriteLine("Deleting task: " + t.ToString());
                        context.Tasks.Remove(t);
                    }
                    context.SaveChanges();
```

Now lets show all tasks post delete operation;
```
                    // Show tasks after the 'Delete' operation - there should be 0 tasks
                    Console.WriteLine("\nTasks after delete:");
                    List<Task> tasksAfterDelete = (from t in context.Tasks select t).ToList<Task>();
                    if (tasksAfterDelete.Count == 0)
                    {
                        Console.WriteLine("[None]");
                    }
                    else
                    {
                        foreach (Task t in query)
                        {
                            Console.WriteLine(t.ToString());
                        }
                    }
```


Congratulations! You have completed a multi class crud app with mapped relationships!










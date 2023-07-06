
# CRUD App using C# and SQL Part 1.

## Overview
In this lesson we'll start building our first CRUD app and connect it to a database via an SQL server which we will run from a Docker container.
The Docker container is neccessary due to the fact that we can't run SQL natively on a Mac. The Docker container will effectively provide a compatible 'virtual' environment in which we can.


## Setup and Installations

First we need to install Docker Desktop for Mac. This will allow us to start/stop and view our containers.
https://docs.docker.com/desktop/install/mac-install/

Now that we have Docker desktop, we can install the image for the container to run our SQL server.
https://hub.docker.com/_/microsoft-mssql-server

Download the image using the following command

```
sudo docker pull microsoft/mssql-server-linux:2022-latest

```

Now find the image in the Docker Desktop app and hit play.
Click on environment options and create a field for these;
```
ACCEPT_EULA=Y
MSSQL_SA_PASSWORD=<your_strong_password>
HOMEBREW_NO_ENV_FILTERING=1
```

A note on the meanings of the selected environment variables;

ACCEPT_EULA=Y
Set the ACCEPT_EULA variable to any value to confirm your acceptance of the End-User Licensing Agreement. Required setting for the SQL Server image.

MSSQL_SA_PASSWORD
Set the SA password.

HOMEBREW_NO_ENV_FILTERING
Use system level environment variables over user were available.



Next, if you already have .NET installed, this step can be skipped. If not install from here;
```
https://download.microsoft.com/download/0/F/D/0FD852A4-7EA1-4E2A-983A-0484AC19B92C/dotnet-sdk-2.0.0-osx-gs-x64.pkg
```


Moving over to Visual Studio, create a new console app called 'crudSQL'.
We need to edit some of the project setting (similar to a pom.xml in Java).
To access the relevant file, right click on the project and select edit project file.
This will give us access to the .csproj file.
Replace the contents of the file with the following;
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp7.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Data.SqlClient" Version="4.4.0" />
  </ItemGroup>

</Project>
```

Now that we are setup we can start writing some code to connect to our SQL server. This will then give us the ability to persist instances in the db.
Replace the contents of the Program.cs file with the following;
```

using System.Text;
using System.Data.SqlClient;
using System;


try
            {
                // Build connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "localhost,1433";   (note: comma not colon)
                builder.UserID = "sa";
                builder.Password = "your_password";
                builder.InitialCatalog = "sampleDB";

                // Connect to SQL
                Console.Write("Connecting to SQL Server ... ");
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Done.");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("All done. Press any key to finish...");
            Console.ReadKey(true);
```

It may be the case that the line using System.Data.SqlClient appears to not be in use (i.e its greyed out) and the SqlConnectionStringBuilder class may be unidentified.
If this is the case, right click the project and select Manage NuGet packages.
Search for System.Data.SqlClient and then install.

Run the application and we should be seeing;
```
Connecting to SQL Server ... Done.
All done. Press any key to finish...
```

Here we have created a connection string using the SqlConnectionStringBuilder class.
We then pass this as an argument to the SqlConnection constructor which provides us with a connection which we can use to perform various actions on the db.

## Persisting Items to the DB (Single Class).

Once we have our connection object we can then create a db for our tables;
```
// Create a sample database
Console.Write("Dropping and creating database 'SampleDB' ... ");
String sql = "DROP DATABASE IF EXISTS [SqlTestDb]; CREATE DATABASE [SqlTestDb]";
using (SqlCommand command = new SqlCommand(sql, connection))
{
    command.ExecuteNonQuery();
    Console.WriteLine("Done.");
}
```

Here we create a string SQL command (which drops then re-creates a db called SqlTestDb) and pass it along with the connection object into the SqlCommand constructor. We can then call ExecuteNonQuery() on the command object and execute the SQL.
Now that we have the db, we can create a table and add some data;
```
// Create a Table and insert some sample data
Console.Write("Creating sample table with data, press any key to continue...");
Console.ReadKey(true);
StringBuilder sb = new StringBuilder();
sb.Append("USE SqlTestDb; ");
sb.Append("CREATE TABLE Employees ( ");
sb.Append(" Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, ");
sb.Append(" Name NVARCHAR(50), ");
sb.Append(" Location NVARCHAR(50) ");
sb.Append("); ");
sb.Append("INSERT INTO Employees (Name, Location) VALUES ");
sb.Append("(N'Jared', N'Australia'), ");
sb.Append("(N'Nikita', N'India'), ");
sb.Append("(N'Tom', N'Germany'); ");
sql = sb.ToString();
using (SqlCommand command = new SqlCommand(sql, connection))
{
    command.ExecuteNonQuery();
    Console.WriteLine("Done.");
}
```

Using the stringBuilder object we create an SQL string which creates a table called employees with an Id column (marked as the primary key), a Name column of type NVARCHAR(50) and a Location column of type NVARCHAR(50).
With the table setup we can then add three entries.
(Console.ReadKey(true) pauses the execution till the user presses a key)

### Note.
You may have noticed the string/varchar entries are prefixed with 'N'. This indicates an NVARCHAR as opposed to a VARCHAR.
Varchar stores Non-unicode or English character data types, and it can contain a maximum of 8000 characters. It only supports ASCII values. Nvarchar stores Unicode or Non-English character data types, and it can contain a maximum of 4000 characters. It supports ASCII values as well as special characters.
### Note.




After running this code we canm check the table in terminal to ensure we have three entries.
```
psql -d SqlTestDb
SELECT * FROM employees;
```

Next we will demonstrate how to add a single row.
```
// INSERT demo
Console.Write("Inserting a new row into table, press any key to continue...");
Console.ReadKey(true);
sb.Clear();
sb.Append("INSERT Employees (Name, Location) ");
sb.Append("VALUES (@name, @location);");
sql = sb.ToString();
using (SqlCommand command = new SqlCommand(sql, connection))
{
    command.Parameters.AddWithValue("@name", "Jake");
    command.Parameters.AddWithValue("@location", "United States");
    int rowsAffected = command.ExecuteNonQuery();
    Console.WriteLine(rowsAffected + " row(s) inserted");
}
```
Here we build the SQL string with placeholders for the actual values. This is a measure commonly deployed to combat sql injection. Basically we send the SQL (without the actual values) to the database. The db knows that anything contained in the submission is valid sql. We then plug in the actual values. The db understands that these values are not valid SQL. This way, even if a user (for example) makes the value of their name or location some SQL, the db will not recognise it as such and therefore not execute it.

Next we can demonstrate how to update a row in the db.
```
// UPDATE demo
String userToUpdate = "Nikita";
Console.Write("Updating 'Location' for user '" + userToUpdate + "', press any key to continue...");
Console.ReadKey(true);
sb.Clear();
sb.Append("UPDATE Employees SET Location = N'United States' WHERE Name = @name");
sql = sb.ToString();
using (SqlCommand command = new SqlCommand(sql, connection))
{
    command.Parameters.AddWithValue("@name", userToUpdate);
    int rowsAffected = command.ExecuteNonQuery();
    Console.WriteLine(rowsAffected + " row(s) updated");
}
```


Lets demonstrate how to delete a row.
```
// DELETE demo
String userToDelete = "Jared";
Console.Write("Deleting user '" + userToDelete + "', press any key to continue...");
Console.ReadKey(true);
sb.Clear();
sb.Append("DELETE FROM Employees WHERE Name = @name;");
sql = sb.ToString();
using (SqlCommand command = new SqlCommand(sql, connection))
{
    command.Parameters.AddWithValue("@name", userToDelete);
    int rowsAffected = command.ExecuteNonQuery();
    Console.WriteLine(rowsAffected + " row(s) deleted");
}
```

And finally, reading information out from the db;
```
// READ demo
Console.WriteLine("Reading data from table, press any key to continue...");
Console.ReadKey(true);
sql = "SELECT Id, Name, Location FROM Employees;";
using (SqlCommand command = new SqlCommand(sql, connection))
{
    using (SqlDataReader reader = command.ExecuteReader())
       {
            while (reader.Read())
                {
                    Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                }
        }
}
```

In the next lesson we will implememnt an Object Relational Mapper in our database.








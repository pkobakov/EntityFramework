using CSharpDbDemo.Library;
using Microsoft.Data.SqlClient;

//-------------------------------------------------------------------------------------------------------

  //Using Library

var person = new Person("Ivan", "Ivanov", 55, "ivanov@gmail.com");

//Console.WriteLine(person);



var connectionStringSoftUni = "Server=.; Database= SoftUni;TrustServerCertificate=True; Integrated Security= true;";
using (var firstConnection = new SqlConnection(connectionStringSoftUni))
{
    firstConnection.Open();

    //Execute NonQuery - manipulates th database

    var firstCommand = new SqlCommand("UPDATE Employees SET Salary = Salary + 0.12", firstConnection);
    var rowsAffected = firstCommand.ExecuteNonQuery();
    Console.WriteLine(rowsAffected);

//------------------------------------------------------------------------------------------------------------------
    //Execute Scalar - get data from the database and shows the first row-col that matches to the criteria

    var secondCommand = new SqlCommand("SELECT FirstName FROM Employees WHERE FirstName LIKE 'G%'",connection);
    var result = firstCommand.ExecuteScalar();
    Console.WriteLine(result);

//------------------------------------------------------------------------------------------------------------------
    //Execute ExecuteReader - get data from database

    var thirdCommand = new SqlCommand("SELECT * FROM Employees WHERE FirstName LIKE 'G%'", firstConnection);
    var sqlReader = firstCommand.ExecuteReader();
    while (sqlReader.Read())
    {
        Console.WriteLine(sqlReader["FirstName"]);
    }

};













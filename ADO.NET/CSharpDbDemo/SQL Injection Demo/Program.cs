using Microsoft.Data.SqlClient;

Console.Write("Please enter username: ");
var username = Console.ReadLine();

Console.Write("Please enter password: ");
var password = Console.ReadLine();


var connectionStringService = "Server= .; Database= Service;TrustServerCertificate=True; Integrated Security = true;";


using (var connection = new SqlConnection(connectionStringService))
{

    connection.Open();

    var command = new SqlCommand($"SELECT COUNT(*) FROM Users WHERE Username = '@Username' AND Password = '@Password'", connection);
    command.Parameters.Add(new SqlParameter("@Username", username));
    command.Parameters.Add(new SqlParameter("@Password", password));
    var usersCount = (int)command.ExecuteScalar();
    if (usersCount>0)
    {
        Console.WriteLine("Access granted! Welcome!");
    }

    else
    {
        Console.WriteLine("Access denied!");
    }


};


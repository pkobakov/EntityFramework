using EntityRelations_Lab;
using EntityRelations_Lab.Data;

var db = new ApplicationDbContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();


for (int i = 0; i < 10; i++)
{
    db.Employees.Add(new Employee
    {
        Name = "Niki",
        Lastname = "Kostov",
        Birthday = new DateTime(1974, 12, 09 + i),
        Salary = 100 + i
    });
}

db.SaveChanges();

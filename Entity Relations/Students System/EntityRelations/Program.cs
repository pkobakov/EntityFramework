using EntityRelations.Data;

var db = new ApplicationDbContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();    
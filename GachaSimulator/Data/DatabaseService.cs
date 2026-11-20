using GachaSimulator.Models;
using Microsoft.Data.Sqlite;

namespace GachaSimulator.Data;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source =gacha.db";

    public void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Blindboxes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CategoryName TEXT NOT NULL,
                Cost INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BlindboxId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Rarity INTEGER NOT NULL,
                Description TEXT NOT NULL,
                AttackValue INTEGER NOT NULL,
                DefenseValue INTEGER NOT NULL,
                HealthValue INTEGER NOT NULL,
                MarketValue INTEGER NOT NULL,
                FOREIGN KEY (BlindboxId) REFERENCES Blindboxes(Id)
            );
            CREATE TABLE IF NOT EXISTS Player (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Coins INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS PlayerInventory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Rarity INTEGER NOT NULL,
                Description TEXT NOT NULL,
                AttackValue INTEGER NOT NULL,
                DefenseValue INTEGER NOT NULL,
                HealthValue INTEGER NOT NULL,
                MarketValue INTEGER NOT NULL
            );
        ";
        createTableCmd.ExecuteNonQuery();

        var checkPlayerCmd = connection.CreateCommand();
        checkPlayerCmd.CommandText = "SELECT COUNT(*) FROM Player ";
        long playerCount = (long)checkPlayerCmd.ExecuteScalar()!;

        if (playerCount == 0)
        {
            var insertPlayerCmd = connection.CreateCommand();
            insertPlayerCmd.CommandText = "INSERT INTO Player (Id, Name, Coins) VALUES (1, 'Student', 1000)";
            insertPlayerCmd.ExecuteNonQuery();
        }

        var checkBoxesCmd = connection.CreateCommand();
        checkBoxesCmd.CommandText = "SELECT COUNT(*) FROM Blindboxes ";
        long boxCount = (long)checkBoxesCmd.ExecuteScalar()!;

        if (boxCount == 0)
        {
            SeedInitialData(connection);
        }
    }

    private void SeedInitialData(SqliteConnection connection)
    {
        long faeBoxId = InsertBox(connection, "Pakiet Natury – Duchy Prastarego Lasu", 150);
        InsertItem(connection, faeBoxId, "Błędny Ognik", Rarity.Common,
            "Mały duch bagien, który zwodzi nieostrożnych wędrowców na manowce.", 10, 5, 15, 50);
        InsertItem(connection, faeBoxId, "Leśna Driada", Rarity.Common,
            "Opiekunka starych dębów, potrafiąca leczyć rany szeptem natury.", 10, 5, 15, 50);
        InsertItem(connection, faeBoxId, "Strażnik Kniei", Rarity.Rare,
            "Starożytny byt o korze twardszej niż krasnoludzka stal.", 30, 15, 50, 100);
        InsertItem(connection, faeBoxId, "Królowa Rusałek", Rarity.Epic,
            "Władczyni leśnych jezior, której pieśń usypia całe armie.", 55, 20, 80, 200);
        InsertItem(connection, faeBoxId, "Cernunnos, Pan Dziczy", Rarity.Legendary,
            "Pierwotne bóstwo lasu o porożu sięgającym samych chmur.", 200, 50, 100, 1000);

        long angelBoxId = InsertBox(connection, "Pakiet Niebios – Zastępy Anielskie", 200);
        InsertItem(connection, angelBoxId, "Akolita Światła", Rarity.Common,
            "Młody posłaniec niosący w dłoniach iskierkę niebiańskiego ognia.", 15, 10, 20, 75);
        InsertItem(connection, angelBoxId, "Skrzydlaty Wartownik", Rarity.Common,
            "Strażnik Srebrnych Bram uzbrojony we włócznię pierwszego brzasku.", 15, 10, 20, 75);
        InsertItem(connection, angelBoxId, "Walkiria Jutrzenki", Rarity.Rare,
            "Wojowniczka zstępująca z chmur, by prowadzić poległych bohaterów.", 50, 25, 70, 200);
        InsertItem(connection, angelBoxId, "Archanioł Sprawiedliwości", Rarity.Epic,
            "Dowódca niebiańskiej straży władający mieczem czystego blasku.", 75, 40, 100, 300);
        InsertItem(connection, angelBoxId, "Serafin Sześcioskrzydły", Rarity.Legendary,
            "Najwyższa istota niebios otoczona wiecznym, oślepiającym płomieniem.", 200, 50, 150, 1500);

        long demonBoxId = InsertBox(connection, "Pakiet Otchłani – Legiony Demonów", 250);
        InsertItem(connection, demonBoxId, "Piekielny Chochlik", Rarity.Common,
            "Złośliwy stwór z czeluści, uwielbiający ogniste sztuczki i chaos.", 20, 12, 25, 90);
        InsertItem(connection, demonBoxId, "Ogar Cieni", Rarity.Common,
            "Bestia zrodzona z dymu i popiołu, bezbłędnie tropiąca dusze.", 20, 12, 25, 90);
        InsertItem(connection, demonBoxId, "Sukkub Iluzji", Rarity.Rare,
            "Mistrzyni mrocznej magii, potrafiąca przybrać dowolną postać.", 55, 30, 75, 220);
        InsertItem(connection, demonBoxId, "Książę Magmy", Rarity.Epic,
            "Władca podziemnej cytadeli, w którego żyłach płynie czysta lawa.", 90, 45, 120, 350);
        InsertItem(connection, demonBoxId, "Upadły Arcyksiążę", Rarity.Legendary,
            "Dawny anioł strącony w otchłań, dysponujący mocą gaszącą gwiazdy.", 220, 75, 175, 2000);
    }

    private long InsertBox(SqliteConnection connection, string name, int cost)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText =
            "INSERT INTO BlindBoxes (CategoryName, Cost) VALUES (@name, @cost); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@cost", cost);
        return (long)cmd.ExecuteScalar()!;
    }

    private void InsertItem(SqliteConnection connection, long boxId, string name, Rarity rarity, string desc,
        int attack, int defense, int health, int price)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Items (BlindboxId, Name, Rarity, Description, Attackvalue, Defensevalue, Healthvalue, Marketvalue)
            Values (@boxId, @name, @rarity, @desc, @attack, @defense, @health, @price)";
        cmd.Parameters.AddWithValue("@boxId", boxId);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@rarity", rarity);
        cmd.Parameters.AddWithValue("@desc", desc);
        cmd.Parameters.AddWithValue("@attack", attack);
        cmd.Parameters.AddWithValue("@defense", defense);
        cmd.Parameters.AddWithValue("@health", health);
        cmd.Parameters.AddWithValue("@price", price);
        cmd.ExecuteNonQuery();
    }

    public List<Blindbox> LoadBlindBoxes()
    {
        var boxes = new List<Blindbox>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var boxCmd = connection.CreateCommand();
        boxCmd.CommandText = "SELECT Id, CategoryName, Cost FROM BlindBoxes";

        using var boxReader = boxCmd.ExecuteReader();
        while (boxReader.Read())
        {
            int boxId = boxReader.GetInt32(0);
            string name = boxReader.GetString(1);
            int cost = boxReader.GetInt32(2);

            var items = LoadItemsFromBox(connection, boxId);
            boxes.Add(new Blindbox(name, cost, items) { Id = boxId });
        }

        return boxes;
    }

    private List<Item> LoadItemsFromBox(SqliteConnection connection, int boxId)
    {
        var items = new List<Item>();
        var itemsCmd = connection.CreateCommand();
        itemsCmd.CommandText =
            "SELECT Id, Name, Rarity, Description, AttackValue, DefenseValue, HealthValue, MarketValue FROM Items WHERE BlindboxId = @boxId";
        itemsCmd.Parameters.AddWithValue("@boxId", boxId);

        using var reader = itemsCmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new Item(
                name: reader.GetString(1),
                rarity: (Rarity)reader.GetInt32(2),
                description: reader.GetString(3),
                attack: reader.GetInt32(4),
                defense: reader.GetInt32(5),
                health: reader.GetInt32(6),
                price: reader.GetInt32(7)
            )
            {
                Id = reader.GetInt32(0)
            });
        }

        return items;
    }

    public Player LoadPlayer()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Name, Coins FROM Player WHERE Id  = 1";

        Player player = new Player("Student");
        using (var reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                player.Name = reader.GetString(0);
                player.Coins = reader.GetInt32(1);
            }
        }

        var invcmd = connection.CreateCommand();
        invcmd.CommandText =
            "SELECT Id, Name, Rarity, Description, AttackValue, DefenseValue, HealthValue, MarketValue FROM PlayerInventory";

        using var invReader = invcmd.ExecuteReader();
        while (invReader.Read())
        {
            player.AddItemToInventory(new Item(
                name: invReader.GetString(1),
                rarity: (Rarity)invReader.GetInt32(2),
                description: invReader.GetString(3),
                attack: invReader.GetInt32(4),
                defense: invReader.GetInt32(5),
                health: invReader.GetInt32(6),
                price: invReader.GetInt32(7)
            )
            {
                Id = invReader.GetInt32(0)
            });
        }

        return player;
    }

    public void UpdatePlayerCoins(int coins)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Player SET Coins = @coins WHERE Id = 1";
        cmd.Parameters.AddWithValue("@coins", coins);
        cmd.ExecuteNonQuery();
    }

    public void AddItemToInventory(Item item)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var cmd = connection.CreateCommand();
        cmd.CommandText =
            "INSERT INTO PlayerInventory (Name, Rarity, Description, AttackValue, DefenseValue, HealthValue, MarketValue) VALUES (@name, @rarity, @desc, @attack, @defense, @health, @price); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", item.Name);
        cmd.Parameters.AddWithValue("@rarity", (int)item.ItemRarity);
        cmd.Parameters.AddWithValue("@desc", item.Description);
        cmd.Parameters.AddWithValue("@attack", item.AttackValue);
        cmd.Parameters.AddWithValue("@defense", item.DefenseValue);
        cmd.Parameters.AddWithValue("@health", item.HealthValue);
        cmd.Parameters.AddWithValue("@price", item.MarketValue);
        
        item.Id = Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void RemoveItemFromInventory(int inventoryItemId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM PlayerInventory WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", inventoryItemId);
        cmd.ExecuteNonQuery();
    }

    public void ResetPlayer()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"UPDATE Player SET Coins = 1000 WHERE Id = 1;DELETE FROM PlayerInventory;";
        cmd.ExecuteNonQuery();
    }
}
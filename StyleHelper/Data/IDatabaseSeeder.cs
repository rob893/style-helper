namespace StyleHelper.Data;

public interface IDatabaseSeeder
{
    void SeedDatabase(bool seedData, bool clearCurrentData, bool applyMigrations, bool dropDatabase);
}

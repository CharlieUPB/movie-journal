namespace MovieJournal.Infrastructure.Persistence.Initializer;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}

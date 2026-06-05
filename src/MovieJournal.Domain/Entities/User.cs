using MovieJournal.Domain.Common;

namespace MovieJournal.Domain.Entities;

public class User : AuditableEntity
{
    public string DisplayName {get; private set;}

    public string Email {get; private set;}

    public string Password {get; private set;}
}
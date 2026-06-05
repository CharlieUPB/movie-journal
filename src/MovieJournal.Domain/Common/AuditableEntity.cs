namespace MovieJournal.Domain.Common;
public abstract class AuditableEntity : Entity
{
	public DateTime CreatedAt { get; protected set;} = DateTime.UtcNow;

	public DateTime? UpdatedAt {get; protected set;}

	public bool? IsDeleted { get; protected set; }

	protected void MarkAsUpdated()
	{
		UpdatedAt = DateTime.UtcNow;
	}

	protected void MarkAsDeleted()
	{
		IsDeleted = true;
	}
}

using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Entities;

public interface IEntity
{
    [Key]
    public Guid Id { get; set; }
}

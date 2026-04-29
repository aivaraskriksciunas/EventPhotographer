using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Data.Util;

public interface IEntity
{
    [Key]
    public Guid Id { get; set; }
}

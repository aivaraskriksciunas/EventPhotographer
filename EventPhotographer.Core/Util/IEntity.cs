using System.ComponentModel.DataAnnotations;

namespace EventPhotographer.Core.Util;

public interface IEntity
{
    [Key]
    public Guid Id { get; set; }
}

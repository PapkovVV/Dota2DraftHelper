using System.ComponentModel.DataAnnotations.Schema;

namespace Dota2DraftHelper.Models;

public class Hero
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Faceit {  get; set; }
    public byte[]? ImageData { get; set; } = null;
    [NotMapped]
    public string? WinRate { get; set; } = null;
}

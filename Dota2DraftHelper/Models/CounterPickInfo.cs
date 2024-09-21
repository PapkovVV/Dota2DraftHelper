using System.ComponentModel.DataAnnotations.Schema;

namespace Dota2DraftHelper.Models;

public class CounterPickInfo
{
    public int CounterPickInfoId { get; set; }
    public int PickId { get; set; }
    public int CounterPickId { get; set; }
    public decimal WinRate { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime WinRateDate { get; set; }
    public Hero PickHero { get; set; } 
    public Hero CounterPickHero { get; set; } 
}

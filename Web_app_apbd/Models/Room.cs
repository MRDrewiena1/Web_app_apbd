using System.ComponentModel.DataAnnotations;

namespace Web_app_apbd.Models;

public class Room
{
    [Range(1, int.MaxValue, ErrorMessage = "RoomId musi być większy od zera")]
    public int Id {get;set;}
    
    [Required(ErrorMessage = "Nazwa sali jest wymagana")]
    [StringLength(100, MinimumLength = 1)]
    public string Name {get;set;}
    
    [Required(ErrorMessage = "Kod budynku jest wymagany")]
    [StringLength(10, MinimumLength = 1)]
    public string BuildingCode {get;set;}
    
    [Range(0, 100, ErrorMessage = "Piętro musi być od 0 do 100")]
    public int Floor  {get;set;}
    
    [Range(1, int.MaxValue, ErrorMessage = "Pojemność musi być większa od zera")]
    public int Capacity {get;set;}
    public bool HasProjector {get;set;}
    public bool IsActive {get;set;}
}
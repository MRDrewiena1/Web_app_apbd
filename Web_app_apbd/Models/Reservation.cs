using System.ComponentModel.DataAnnotations;

namespace Web_app_apbd.Models;

public class Reservation
{
    public int Id { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "RoomId musi być większy od zera")]
    public int RoomId { get; set; }
    
    [Required(ErrorMessage = "Nazwa organizatora jest wymagana")]
    [StringLength(100, MinimumLength = 1)]
    public string OrganizerName { get; set; }
    
    [Required(ErrorMessage = "Temat rezerwacji jest wymagany")]
    [StringLength(200, MinimumLength = 1)]
    public string Topic { get; set; }
    
    [Required(ErrorMessage = "Data jest wymagana")]
    public DateOnly Date { get; set; }
    
    [Required(ErrorMessage = "Czas rozpoczęcia jest wymagany")]
    public TimeOnly StartTime { get; set; }
    
    [Required(ErrorMessage = "Czas zakończenia jest wymagany")]
    public TimeOnly EndTime { get; set; }
    
    [Required(ErrorMessage = "Status jest wymagany")]
    [RegularExpression("^(planned|confirmed|cancelled)$", ErrorMessage = "Status musi być: planned, confirmed lub cancelled")]
    public string Status { get; set; }
    
    [CustomValidation(typeof(Reservation), nameof(ValidateEndTime))]
    public DateTime Dummy { get; set; }
 
    public static ValidationResult? ValidateEndTime(DateTime dummy, ValidationContext context)
    {
        var reservation = context.ObjectInstance as Reservation;
        if (reservation?.EndTime <= reservation?.StartTime)
        {
            return new ValidationResult("Czas zakończenia musi być późniejszy niż czas rozpoczęcia");
        }
        return ValidationResult.Success;
    }
}
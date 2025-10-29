namespace Backend.Entities;
using System.ComponentModel.DataAnnotations;

public class Notification
{
    [Key]
    public int NotificationId { get; set; }

    [Required]
    public int RecipientId { get; set; }

    [Required]
    [StringLength(20)]
    public string RecipientType { get; set; } = null!; 

    [Required]
    [StringLength(256)]
    public string Message { get; set; } = null!;

    [Required]
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(20)]
    public string Type { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = "Pending";
}

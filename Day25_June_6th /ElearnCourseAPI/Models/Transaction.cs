using System;

namespace ElearnAPI.Models
{
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PaymentId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public Guid UserId { get; set; }        // Must be Guid
    public Guid CourseId { get; set; } 

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = "Created";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public User? User { get; set; }
    public Course? Course { get; set; }
}

}
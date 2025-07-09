using System;
using System.Collections.Generic;

namespace ElearnAPI.DTOs
{

    public class CreateTransactionDto
{
    public string PaymentId { get; set; }
    public string OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public string PaymentId { get; set; }
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public string CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;

    public string UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}



}
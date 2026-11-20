using System.ComponentModel.DataAnnotations;
using AgriLedger.Api.Enums;

namespace AgriLedger.Api.DTOs.Sales;

public class PaymentStatusRequest
{
    [Required(ErrorMessage = "Η κατάσταση πληρωμής είναι υποχρεωτική.")]
    public PaymentStatus? PaymentStatus { get; set; }
}

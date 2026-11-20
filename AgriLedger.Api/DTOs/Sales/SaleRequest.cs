using System.ComponentModel.DataAnnotations;
using AgriLedger.Api.Enums;

namespace AgriLedger.Api.DTOs.Sales;

public class SaleRequest
{
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Το όνομα πελάτη είναι υποχρεωτικό.")]
    [MaxLength(200, ErrorMessage = "Το όνομα πελάτη μπορεί να έχει έως 200 χαρακτήρες.")]
    public string CustomerName { get; set; } = string.Empty;

    public SaleType SaleType { get; set; }

    public EggSize EggSize { get; set; }

    [Range(1, 1000000, ErrorMessage = "Η ποσότητα θηκών πρέπει να είναι τουλάχιστον 1.")]
    public int CaseQuantity { get; set; }

    [Range(0, 999999999, ErrorMessage = "Η τιμή ανά θήκη δεν μπορεί να είναι αρνητική.")]
    public decimal PricePerCase { get; set; }

    public PaymentStatus PaymentStatus { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace AgriLedger.Api.DTOs.Categories;

public class CategoryRequest
{
    [Required(ErrorMessage = "Ο κωδικός είναι υποχρεωτικός.")]
    [MaxLength(32, ErrorMessage = "Ο κωδικός μπορεί να έχει έως 32 χαρακτήρες.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Το όνομα είναι υποχρεωτικό.")]
    [MaxLength(100, ErrorMessage = "Το όνομα μπορεί να έχει έως 100 χαρακτήρες.")]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Range(0, 10000, ErrorMessage = "Η σειρά εμφάνισης πρέπει να είναι από 0 έως 10000.")]
    public int SortOrder { get; set; }
}

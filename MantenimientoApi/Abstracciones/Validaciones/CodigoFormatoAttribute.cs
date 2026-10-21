using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Abstracciones.Validaciones;

/// <summary>
/// Atributo de validación personalizado para verificar que un código de activo o repuesto
/// cumpla con el formato estándar empresarial (ej. EQ-001, REP-001).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class CodigoFormatoAttribute : ValidationAttribute
{
    private static readonly Regex RegexCodigo = new(@"^[A-Z0-9]+(-[A-Z0-9]+)*$", RegexOptions.Compiled);

    public CodigoFormatoAttribute()
        : base("El código ingresado no cumple con el formato estándar empresarial (debe contener solo letras mayúsculas, números y guiones, sin espacios).")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success; // Se delega la obligatoriedad al atributo [Required]
        }

        string valorString = value.ToString()!.Trim();

        if (!RegexCodigo.IsMatch(valorString))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        return ValidationResult.Success;
    }
}

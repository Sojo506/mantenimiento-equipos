namespace Abstracciones.DTOs.Catalogos;

/// <summary>
/// DTO liviano para transferir elementos de catálogos paramétricos hacia combos y listas desplegables.
/// Evita la exposición de entidades con referencias circulares en el esquema OpenAPI.
/// </summary>
public class ElementoCatalogoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

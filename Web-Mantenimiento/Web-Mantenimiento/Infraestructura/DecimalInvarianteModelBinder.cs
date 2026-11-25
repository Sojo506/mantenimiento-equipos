using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web_Mantenimiento.Infraestructura;

public class DecimalInvarianteModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var resultado = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (resultado == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, resultado);
        var texto = resultado.FirstValue?.Trim();

        if (string.IsNullOrEmpty(texto))
        {
            if (Nullable.GetUnderlyingType(bindingContext.ModelType) != null)
                bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var valor) ||
            decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
        {
            bindingContext.Result = ModelBindingResult.Success(valor);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"El valor '{texto}' no es un número válido.");
        }

        return Task.CompletedTask;
    }
}

public class DecimalInvarianteModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var tipo = Nullable.GetUnderlyingType(context.Metadata.ModelType) ?? context.Metadata.ModelType;
        return tipo == typeof(decimal) ? new DecimalInvarianteModelBinder() : null;
    }
}

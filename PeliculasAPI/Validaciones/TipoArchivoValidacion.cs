using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Validaciones
{
    public class TipoArchivoValidacion : ValidationAttribute
    {
        private readonly string[] _tiposValidos;
        public TipoArchivoValidacion(string[] tiposValidos)
        {
            _tiposValidos = tiposValidos;
        }
        public TipoArchivoValidacion(GrupoTipoArchivo grupoTipoArchivo)
        {
            if (grupoTipoArchivo == GrupoTipoArchivo.Imagen)
            {
                _tiposValidos = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;
            if (formFile is null)
                return ValidationResult.Success;

            if (!_tiposValidos.Contains(formFile.ContentType))
            {
                return new ValidationResult($"El tipo del archivo debe ser uno de los siguientes: {string.Join(", ",_tiposValidos)}");
            }

            return ValidationResult.Success;
        }
    }
}

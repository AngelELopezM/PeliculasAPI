using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Validaciones
{
    public class PesoArchivoValidacion : ValidationAttribute
    {
        private readonly int _pesoMaximoEnMB;

        public PesoArchivoValidacion(int pesoMaximoEnMB)
        {
            _pesoMaximoEnMB = pesoMaximoEnMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;
            if (formFile is null)
                return ValidationResult.Success;

            if (formFile.Length> _pesoMaximoEnMB * 1024 * 1024)
            {
                return new ValidationResult($"El peso del archivo no debe ser mayor a {_pesoMaximoEnMB}");
            }

            return ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class SalaDeCineCercanoFiltroDTO
    {
        [Range(-90, 90)]
        public double Latitud { get; set; }

        [Range(-180, 180)]
        public double Longitud { get; set; }

        private int distanciaEnKms = 10;
        private int distanciaMacimaKms = 50;
        public int DistannciaEnKms
        {
            get
            {
                return distanciaEnKms;
            }
            set
            {
                distanciaEnKms = (value > distanciaMacimaKms) ? distanciaMacimaKms : value;
            }
        }
    }
}

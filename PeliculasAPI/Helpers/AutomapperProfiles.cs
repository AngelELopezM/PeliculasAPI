using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.DTOs;
using PeliculasAPI.DTOs.Usuarios;
using PeliculasAPI.Entidades;
using System.Collections.Immutable;

namespace PeliculasAPI.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles(GeometryFactory geometryFactory)
        {
            #region Generos

            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<Genero, GeneroCreacionDTO>().ReverseMap();

            #endregion

            #region Actores

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<Actor, ActorCreacionDTO>();
            CreateMap<ActorCreacionDTO, Actor>().ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<Actor, ActorPatchDTO>().ReverseMap();

            #endregion

            #region Peliculas

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculasCreacionDTO, Pelicula>()
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));
            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));
            #endregion

            #region Sala de Cine
            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember(x=> x.Latitud, x=> x.MapFrom(y=> y.Ubicacion.Y))
                .ForMember(x => x.Longitud, x => x.MapFrom(y => y.Ubicacion.X));

            ////Aqui es donde vamos a convertir de la ubicacion de longitud y latitud para el point
            //geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember(x=> x.Ubicacion, x=> x.MapFrom(y=> 
                geometryFactory.CreatePoint(new Coordinate(y.Longitud,y.Latitud))));

            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y =>
                geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));
            #endregion

            #region Usuarios

            CreateMap<IdentityUser, UsuarioDTO>().ReverseMap();

            #endregion

            #region Reviews
            CreateMap<Review, ReviewDTO>()
                .ForMember(x => x.NombreUsuario, x => x.MapFrom(y => y.Usuario.UserName));

            CreateMap<ReviewDTO, Review>();
            CreateMap<ReviewCreacionDTO, Review>();
            #endregion
        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();
            if (resultado is null)
                return resultado;

            foreach (var item in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO() { ActorId = item.ActorId, NombrePersona = item.Actor.Nombre, Personaje = item.Personaje });
            }

            return resultado;
        }

        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<GeneroDTO>();
            if (pelicula.PeliculasGeneros is null)
                return resultado;

            foreach (var item in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = item.GeneroId, Nombre = item.Genero.Nombre });
            }
            return resultado;
        }

        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculasCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIds is null)
                return resultado;

            foreach (var id in peliculaCreacionDTO.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculasCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores is null)
                return resultado;

            foreach (var id in peliculaCreacionDTO.GenerosIds)
            {
                resultado.Add(new PeliculasActores() { ActorId = id });
            }

            return resultado;
        }
    }
}

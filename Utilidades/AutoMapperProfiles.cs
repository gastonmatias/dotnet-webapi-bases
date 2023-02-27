using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using webAPIAuthors.DTOs;
using webAPIAuthors.Entidades;

namespace webAPIAuthors.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {   
            // indicar en <> 1) fuente, 2) destino
            
            // para POST nuevo autor, flujo: de DTO a bd
            CreateMap<AutorCreacionDTO, Autor>();
            
            // para POST nuevo libro, flujo: de DTO a bd
            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            
            // para GET autor/es, flujo: de bd a DTO
            CreateMap<Autor, AutorDTO>();
            
            // para GET autor/es, flujo: de bd a DTO
            CreateMap<Libro, LibroDTO>();

            CreateMap<ComentarioCreacionDTO, Comentario>();
            
            // para GET comentario/s, flujo: de bd a DTO
            CreateMap<Comentario,ComentarioDTO>();
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro){
            
            var resultado = new List<AutorLibro>();

            // valida si se esta creando un libro sin autores
            if(libroCreacionDTO.AutoresIds == null){return resultado;}

            foreach (var autorId in libroCreacionDTO.AutoresIds){
                resultado.Add(new AutorLibro(){AutorId = autorId});
            }

            return resultado;
        }
    }
}
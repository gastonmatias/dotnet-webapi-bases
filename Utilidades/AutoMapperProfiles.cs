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
            
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(AutorDTO => AutorDTO.Libros,
                            opciones => opciones.MapFrom(MapAutorDTOLibros));
            
            //#region[blue]
            // para GET autor/es, flujo: de bd a DTO
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(LibroDTO => LibroDTO.Autores,
                           opciones => opciones.MapFrom(MapLibroDTOAutores));
            //endregion

            // CreateMap<LibroPatchDTO, Libro>().ReverseMap();
            CreateMap<LibroPatchDTO,Libro>();
            // CreateMap<Libro,LibroPatchDTO>().ReverseMap();
            CreateMap<Libro,LibroPatchDTO>();

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

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTOConAutores libroDTOConAutores){

            var resultado = new List<AutorDTO>();

            if(libro.AutoresLibros == null ){ return resultado;}

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                    {
                        Id = autorLibro.AutorId,
                        Nombre = autorLibro.Autor.Nombre
                    }
                );
            }

            return resultado;
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTOConLibros autorDTOConLibros){

            var resultado = new List<LibroDTO>();

            if (autor.AutoresLibros == null){return resultado;}

            foreach (var autorLibro in autor.AutoresLibros )
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                }
                );
            }

            return resultado;
        }
    }
}
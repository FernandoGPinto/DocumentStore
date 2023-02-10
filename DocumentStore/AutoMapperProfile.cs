using AutoMapper;
using DocumentStore.Core.Dto;
using DocumentStore.Core.Models;

namespace DocumentStore
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<FileDescriptionDto, FileDescription>();
            CreateMap<FileDescriptionDto, FileDescription>().ReverseMap();

            CreateMap<FileStoreDto, FileStore>();
            CreateMap<FileStoreDto, FileStore>().ReverseMap();
        }
    }
}

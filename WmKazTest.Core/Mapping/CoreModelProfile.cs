using AutoMapper;
using WmKazTest.Core.Model;

namespace WmKazTest.Core.Mapping
{
    public class CoreModelToDataModelProfile : Profile
    {
        public CoreModelToDataModelProfile()
        {
            CreateMap<Data.Model.Observation, Observation>().ReverseMap();
            CreateMap<Data.Model.Sequence, Sequence>().ReverseMap();
            CreateMap<Data.Model.WorkingSection, WorkingSection>().ReverseMap();
        }
    }
}

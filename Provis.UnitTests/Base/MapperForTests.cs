using AutoMapper;
using Provis.Core.Helpers;

namespace Provis.UnitTests.Base
{
    public class MapperForTests
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(x =>
               x.AddProfile(new ApplicationProfile()));

            return mapperConfig.CreateMapper();
        }
    }
}

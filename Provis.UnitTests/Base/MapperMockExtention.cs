using AutoMapper;
using Moq;

namespace Provis.UnitTests.Base
{
    public static class MapperMockExtention
    {
        /// <summary>
        /// Mocks Mapper.Map
        /// </summary>
        /// <typeparam name="TReturn">Type of return object</typeparam>
        /// <param name="obj">Object for mapping. If value is null it is going to use It</param>
        /// <param name="objToReturn">Object that should be returned</param>
        public static void SetupMap<TReturn>(this Mock<IMapper> mapperMock, object obj, TReturn objToReturn)
        {
            mapperMock
                .Setup(x => x.Map<TReturn>(obj ?? It.IsAny<object>()))
                .Returns(objToReturn)
                .Verifiable();
        }
    }
}

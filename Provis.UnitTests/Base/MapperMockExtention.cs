using AutoMapper;
using Moq;
using System.Collections.Generic;

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

        public static void SetupListToListMap<TSource, TDest>(this Mock<IMapper> _mapperMock, List<TSource> sources, List<TDest> dests)
        {
            for (int i = 0; i < sources.Count; i++)
            {
                _mapperMock.SetupMap(sources[i], dests[i]);
            }
        }

        public static void SetupListToDictionaryMap<TFirst, TSecond, TDest>(
                this Mock<IMapper> _mapperMock,
                Dictionary<TFirst,List<TSecond>> sources,
                List<TDest> dests)
        {
            var counter = 0;
            foreach (var task in sources.Keys)
            {
                foreach (var item in sources[task])
                {
                    _mapperMock.SetupMap(dests[counter], item);
                    counter++;
                }
            }
        }
    }
}

using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();

        City GetCity(int id, bool includePointsOfInterest);

        IEnumerable<PointOfInterest> GetPointOfInterests(int cityId);

        PointOfInterest GetPointOfInterest(int cityId, int pofId);

        bool CityExists(int cityId);

        void AddPointOfInterestToCity(int cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        bool Save();
    }
}

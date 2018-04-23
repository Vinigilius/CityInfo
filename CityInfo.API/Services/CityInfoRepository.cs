using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository {

        private CityInfoContext _contex;

        public CityInfoRepository(CityInfoContext context) {
            _contex = context;
        }

        public IEnumerable<City> GetCities() {
            return _contex.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int id, bool includePointsOfInterest) {
            if (includePointsOfInterest) {
                return _contex.Cities.Include(c=>c.PointsOfInterest).Where(c=>c.Id==id).FirstOrDefault();
            }
            return _contex.Cities.Where(c => c.Id == id).FirstOrDefault();
        }

        public PointOfInterest GetPointOfInterest(int cityId, int pofId) {
            return _contex.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == pofId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointOfInterests(int cityId) {
            return _contex.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
        }

        public bool CityExists(int cityId) {
            return _contex.Cities.Where(c => c.Id == cityId).FirstOrDefault() == null ? false : true;
        }

        public void AddPointOfInterestToCity(int cityId, PointOfInterest pointOfInterest) {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool Save() {
            return (_contex.SaveChanges() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest) {
            _contex.PointsOfInterest.Remove(pointOfInterest);
        }
    }
}

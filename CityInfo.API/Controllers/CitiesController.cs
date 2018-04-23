using CityInfo.API.Services;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller {

        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository repository) {
            _cityInfoRepository = repository;
        }

        [HttpGet]
        public IActionResult GetCities() {
            var cityEntities = _cityInfoRepository.GetCities();

            var result = AutoMapper.Mapper.Map<IEnumerable<CityWithoutPointsOfInterestsDto>>(cityEntities);           

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false) {
            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null) {
                return NotFound();
            }

            if (includePointsOfInterest) {
                var cityResult = AutoMapper.Mapper.Map<CityDto>(city);

                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterestsResult = AutoMapper.Mapper.Map<CityWithoutPointsOfInterestsDto>(city);

            return Ok(cityWithoutPointsOfInterestsResult);
        }
    }
}

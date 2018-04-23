using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.Extensions.Logging;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/")]
    public class PointsOfInterestController : Controller {

        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
                                          IMailService mailService,
                                          ICityInfoRepository cityInfoRepository) {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityid}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId) {
            if (!_cityInfoRepository.CityExists(cityId)) {
                _logger.LogInformation($"The city with {cityId} id does not exists.");
                return NotFound();
            }
            var pointsOfInterestForTheCity = _cityInfoRepository.GetPointOfInterests(cityId);
            var pointsOfInterestForTheCityResult = AutoMapper.Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForTheCity);
            
            return Ok(pointsOfInterestForTheCityResult);
        }

        [HttpGet("{cityid}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId, int id) {
            if (!_cityInfoRepository.CityExists(cityId)) {
                _logger.LogInformation($"The city with {cityId} id does not exists.");
                return NotFound();
            }

            var pointOfInterestForTheCity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if(pointOfInterestForTheCity == null) {
                return NotFound();
            }
            var pointOfInterestResult = AutoMapper.Mapper.Map<PointOfInterestDto>(pointOfInterestForTheCity);

            return Ok(pointOfInterestResult);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest) {
            if (pointOfInterest == null) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId)) {
                return NotFound();
            }

            var finalPointOfInterest = AutoMapper.Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestToCity(cityId, finalPointOfInterest);
            if(!_cityInfoRepository.Save()){
                return StatusCode(500, "A problem happend while processing your request!");
            }

            var outputPointOfInterest = AutoMapper.Mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = outputPointOfInterest.Id }, outputPointOfInterest);
        }

        [HttpPut("{cityid}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForUpdateDto newPointOfInterest) {
            if (newPointOfInterest == null) {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId)) {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterest == null) {
                return NotFound();
            }

            AutoMapper.Mapper.Map(newPointOfInterest, pointOfInterest);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem happend while processing your request!");
            }

            return NoContent();
        }

        [HttpPatch("{cityid}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument) {
            if (patchDocument == null) {
                return BadRequest();
            }

            if (!_cityInfoRepository.CityExists(cityId)) {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null) {
                return NotFound();
            }

            var pointofInterestToPatch = AutoMapper.Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointofInterestToPatch, ModelState);

            TryValidateModel(pointofInterestToPatch);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            AutoMapper.Mapper.Map(pointofInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem happend while processing your request!");
            }

            return NoContent();
        }

        [HttpDelete("{cityid}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id) {
            if (!_cityInfoRepository.CityExists(cityId)) {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null) {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            if (!_cityInfoRepository.Save()) {
                return StatusCode(500, "A problem happend while processing your request!");
            }

            return NoContent() ;
        }
    }
}

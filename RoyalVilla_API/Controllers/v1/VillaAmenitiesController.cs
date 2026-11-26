using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla.API.Data;
using RoyalVilla.DTO;
using RoyalVilla.API.Models;
using System.Collections;
using System.ComponentModel;

namespace RoyalVilla.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/villa-amenitie")]
    [ApiVersion("1.0")]
    [ApiController]
    //[Authorize(Roles = "Customer,Admin")]
    public class VillaAmentiesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaAmentiesController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [HttpGet]
        //[Authorize(Roles ="Admin")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaAmenitiesDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaAmenitiesDTO>>>> GetVillaAmenities()
        {
            var villas = await _db.VillaAmenities.ToListAsync();
            var dtoResponseVillaAmenities = _mapper.Map<List<VillaAmenitiesDTO>>(villas);
            var response = APIResponse<IEnumerable<VillaAmenitiesDTO>>.Ok(dtoResponseVillaAmenities, "Villa Amenities retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<VillaAmenitiesDTO>>> GetVillaAmenitiesById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound(APIResponse<object>.NotFound("VillaAmenities ID must be greater than 0"));
                }

                var villaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);
                if (villaAmenities == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"VillaAmenities with ID {id} was not found"));
                }
                return Ok(APIResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(villaAmenities), "Records retrieved successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<VillaAmenitiesDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<VillaAmenitiesDTO>>> CreateVillaAmenities(VillaAmenitiesCreateDTO villaAmentiesDTO)
        {
            try
            {
                if (villaAmentiesDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa Amenities data is required"));
                }


                var villaExists = await _db.Villa.FirstOrDefaultAsync(u => u.Id == villaAmentiesDTO.VillaId);

                if (villaExists == null)
                {
                    return Conflict(APIResponse<object>.Conflict($"Villa with the ID '{villaAmentiesDTO.VillaId}'does not exist."));
                }

                VillaAmenities villaAmenities = _mapper.Map<VillaAmenities>(villaAmentiesDTO);
                villaAmenities.CreatedDate = DateTime.Now;
                await _db.VillaAmenities.AddAsync(villaAmenities);
                await _db.SaveChangesAsync();

                var response = APIResponse<VillaAmenitiesDTO>.CreatedAt(_mapper.Map<VillaAmenitiesDTO>(villaAmenities), "Villa Amenities created successfully");
                return CreatedAtAction(nameof(CreateVillaAmenities), new { id = villaAmenities.Id }, response);

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa amenities:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<VillaAmenitiesDTO>>> UpdateVillaAmenities(int id, VillaAmenitiesUpdateDTO villaAmentiesDTO)
        {
            try
            {
                if (villaAmentiesDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa Amenities data is required"));
                }

                if (id != villaAmentiesDTO.Id)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa Amenities ID in URL does not match VillaAmenities ID in request body"));
                }

                var villaExists = await _db.Villa.FirstOrDefaultAsync(u => u.Id == villaAmentiesDTO.VillaId);

                if (villaExists == null)
                {
                    return Conflict(APIResponse<object>.Conflict($"Villa with the ID '{villaAmentiesDTO.VillaId}'does not exist."));
                }

                var existingVillaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVillaAmenities == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa Amenities with ID {id} was not found"));
                }



                _mapper.Map(villaAmentiesDTO, existingVillaAmenities);
                existingVillaAmenities.UpdatedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                var response = APIResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(existingVillaAmenities), "Villa Amenities updated successfully");
                return Ok(response);

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa amenities:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<object>>> DeleteVillaAmenities(int id)
        {
            try
            {
                var existingVillaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVillaAmenities == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa Amenities with ID {id} was not found"));
                }

                _db.VillaAmenities.Remove(existingVillaAmenities);
                await _db.SaveChangesAsync();

                var response = APIResponse<object>.NoContent("Villa Amenities deleted successfully");
                return Ok(response);

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa amenities:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

    }
}

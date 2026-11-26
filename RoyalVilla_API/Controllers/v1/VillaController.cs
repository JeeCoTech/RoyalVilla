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
    [Route("api/v{version:apiVersion}/villa")]
    [ApiVersion("1.0")]
    [ApiController]
    //[Authorize(Roles = "Customer,Admin")]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [HttpGet]
        //[Authorize(Roles ="Admin")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            var villas = await _db.Villa.ToListAsync();
            var dtoResponseVilla = _mapper.Map<List<VillaDTO>>(villas);
            var response = APIResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVilla, "Villas retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> GetVillaById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound(APIResponse<object>.NotFound("Villa ID must be greater than 0"));
                }

                var villa = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }
                return Ok(APIResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villa), "Records retrieved successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> CreateVilla(VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa data is required"));
                }


                var duplicateVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower());

                if (duplicateVilla != null)
                {
                    return Conflict(APIResponse<object>.Conflict($"A villa with the name '{villaDTO.Name}' already exists"));
                }

                Villa villa = _mapper.Map<Villa>(villaDTO);

                await _db.Villa.AddAsync(villa);
                await _db.SaveChangesAsync();

                var response = APIResponse<VillaDTO>.CreatedAt(_mapper.Map<VillaDTO>(villa), "Villa created successfully");
                return CreatedAtAction(nameof(CreateVilla), new { id = villa.Id }, response);

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> UpdateVilla(int id, VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa data is required"));
                }

                if (id != villaDTO.Id)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID in URL does not match Villa ID in request body"));
                }


                var existingVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVilla == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }

                var duplicateVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower()
                && u.Id != id);

                if (duplicateVilla != null)
                {
                    return Conflict(APIResponse<object>.Conflict($"A villa with the name '{villaDTO.Name}' already exists"));
                }

                _mapper.Map(villaDTO, existingVilla);
                existingVilla.UpdatedDate = DateTime.Now;

                await _db.SaveChangesAsync();
                var response = APIResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villaDTO), "Villa updated successfully");
                return Ok(villaDTO);

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                var existingVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVilla == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }

                _db.Villa.Remove(existingVilla);
                await _db.SaveChangesAsync();

                var response = APIResponse<object>.NoContent("Villa deleted successfully");
                return Ok(response);

            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while creating the villa:", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla.API.Data;
using RoyalVilla.DTO;
using RoyalVilla.API.Models;
using System.Collections;
using System.ComponentModel;

namespace RoyalVilla.API.Controllers
{
    [Route("api/villa")]
    [ApiController]
    //[Authorize (Roles = "Customer, Admin")]
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
        //[Authorize (Roles = "Admin")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<object>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            var villas = await _db.Villa.ToListAsync();
            //return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villas));
            var dtoResponseVilla = _mapper.Map<List<VillaDTO>>(villas);
            var response = APIResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVilla, "Villas retrived successfully");
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> GetVillabyId(int id)
        {
            try
            {
                if(id == 0)
                {
                    return NotFound(APIResponse<object>.NotFound("Valid Id must be greater than 0"));
                    //return new APIResponse<VillaDTO>()
                    //{
                    //    StatusCode = 400,
                    //    Errors = "Villa ID must be greater than 0",
                    //    Success = false,
                    //    Message = "Bad Request"
                    //};
                }

                var villa = await _db.Villa.FirstOrDefaultAsync(u=> u.Id == id);
                if (villa == null)
                {
                    return NotFound(APIResponse<object>.NotFound("Villa with ID {id} was not found"));
                }

                return Ok(APIResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villa), "Records Retrived Successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An Error occured while creting villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost()]
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
                    return BadRequest(APIResponse<object>.BadRequest("Ville data is required"));
                }

                var duplicateVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla != null)
                {
                    return Conflict(APIResponse<object>.Conflict($"Villa with name '{villaDTO.Name}' already exists"));
                }

                Villa villa = _mapper.Map<Villa>(villaDTO);

                await _db.Villa.AddAsync(villa);
                await _db.SaveChangesAsync();

                var response = APIResponse<VillaDTO>.CreatedAt(_mapper.Map<VillaDTO>(villa), "Villa Created Successfully");
                return CreatedAtAction(nameof(CreateVilla) , new {id = villa.Id}, response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An Error occured while creting villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
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

                if(id != villaDTO.Id)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa ID in the URL does not match the ID in the request body"));

                }

                var existingVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Id == id);
                if (existingVilla == null)
                {
                    return NotFound(APIResponse<object>.NotFound("Valid Id must be greater than 0"));
                }

                var duplicateVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower() && v.Id != id);
                if(duplicateVilla != null)
                {
                    return Conflict(APIResponse<object>.Conflict($"Villa with name '{villaDTO.Name}' already exists"));
                }

                Villa villa = _mapper.Map(villaDTO, existingVilla);
                existingVilla.UpdatedDate = DateTime.Now;
                await _db.SaveChangesAsync();

                var response = APIResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villaDTO), "Villa Updated successfully");
                return Ok(villaDTO);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An Error occured while creting villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                var existingVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Id == id);
                if (existingVilla == null)
                {
                    return NotFound(APIResponse<object>.NotFound("Valid Id must be greater than 0"));
                }

                _db.Villa.Remove(existingVilla);
                await _db.SaveChangesAsync();

                var response = APIResponse<object>.NoContent("Villa Deleted Successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An Error occured while creting villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}

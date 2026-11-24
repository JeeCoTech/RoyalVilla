using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.DTO;
using RoyalVilla_API.Models;
using System.Collections;
using System.ComponentModel;

namespace RoyalVilla_API.Controllers
{
    [Route("api/villa")]
    [ApiController]
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
        public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            var villas = await _db.Villa.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villas));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<APIResponse<VillaDTO>>> GetVillabyId(int id)
        {
            try
            {
                if(id == 0)
                {
                    return new APIResponse<VillaDTO>()
                    {
                        StatusCode = 400,
                        Errors = "Villa ID must be greater than 0",
                        Success = false,
                        Message = "Bad Request"
                    };
                }

                var villa = await _db.Villa.FirstOrDefaultAsync(u=> u.Id == id);
                if (villa == null)
                {
                    return NotFound("Villa with ID {id} was not found");
                }

                return new APIResponse<VillaDTO>()
                {
                    StatusCode = 200,
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<VillaDTO>(villa)
                };
                //return Ok(_mapper.Map<VillaDTO>(villa));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while retrieving villa with ID {id}: {ex.Message}");
            }
        }

        [HttpPost()]
        public async Task<ActionResult<VillaDTO>> CreateVilla(VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest("Villa data is required");
                }

                var duplicateVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla != null)
                {
                    return Conflict($"Villa with name '{villaDTO.Name}' already exists");
                }

                Villa villa = _mapper.Map<Villa>(villaDTO);

                await _db.Villa.AddAsync(villa);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateVilla) , new {id = villa.Id}, _mapper.Map<VillaDTO>(villa));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while creting villa: {ex.StackTrace}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<VillaDTO>> UpdateVilla(int id, VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest("Villa data is required");
                }

                if(id != villaDTO.Id)
                {
                    return BadRequest("Villa ID in the URL does not match the ID in the request body");
                }

                var existingVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Id == id);
                if (existingVilla == null)
                {
                    return NotFound($"Villa with ID {id} not found");
                }

                var duplicateVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Name.ToLower() == villaDTO.Name.ToLower() && v.Id != id);
                if(duplicateVilla != null)
                {
                    return Conflict($"Villa with name '{villaDTO.Name}' already exists");
                }

                Villa villa = _mapper.Map(villaDTO, existingVilla);
                existingVilla.UpdatedDate = DateTime.Now;
                await _db.SaveChangesAsync();

                return Ok(villaDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while Updating villa: {ex.StackTrace}");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            try
            {
                var existingVilla = await _db.Villa.FirstOrDefaultAsync(v => v.Id == id);
                if (existingVilla == null)
                {
                    return NotFound($"Villa with ID {id} not found");
                }

                _db.Villa.Remove(existingVilla);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while deleting villa: {ex.StackTrace}");
            }
        }

        [HttpGet("{id:int}/{name}")]
        public string GetVillaByName([FromRoute]int id, [FromRoute]string name)
        {
            return $"Get Villa with Id: {id} and Name: {name}";
        }

        //[HttpGet()]
        //public string GetVillaByName([FromQuery] int id, [FromQuery] string name)
        //{
        //    return $"Get Villa with Id: {id} and Name: {name}";
        //}

        //[HttpGet("{id:int}/{name}")]
        //public string GetVillaByName([FromRoute] int id, [FromHeader] string name)
        //{
        //    return $"Get Villa with Id: {id} and Name: {name}";
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace dotnet_RPG.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        //public static List<Character> Characters = new List<Character>(){
        //    new Character(),
        //    new Character {Id = 1, Name = "Tom"}
        //};
        private readonly DataContext _Context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _Context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private int GetUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.Users = await _Context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            _Context.Characters.Add(character);
            await _Context.SaveChangesAsync();

            serviceResponse.Data = await _Context.Characters
                .Where(c => c.Users!.Id == GetUserId())
                .Select(ch => _mapper.Map<GetCharacterDTO>(ch)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            try{
                var character = await _Context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.Users!.Id == GetUserId());
                if(character is null) { throw new Exception($"Character with Id '{id}' not found"); }

                _Context.Characters.Remove(character);

                await _Context.SaveChangesAsync();

                serviceResponse.Data = await _Context.Characters
                    .Where(c => c.Users!.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();
            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var DBCharacters = await _Context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => c.Users!.Id == GetUserId()).ToListAsync();
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>(){
                Data = DBCharacters.Select(ch => _mapper.Map<GetCharacterDTO>(ch)).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            var DBcharacter = await _Context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(ch => ch.Id == id && ch.Users!.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(DBcharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            try{
                var character = await _Context.Characters
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if(character is null || character.Users!.Id != GetUserId()) { throw new Exception($"Character with Id '{updatedCharacter.Id}' not found"); }

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                await _Context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
            
        }

        public async Task<ServiceResponse<GetCharacterDTO>> AddCharacterSkill(AddCharacterSkillDTO newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var character = await _Context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.Users!.Id == GetUserId());
                
                if(character is null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                var skill = await _Context.Skills
                    .FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);

                if (skill is null)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                character.Skills!.Add(skill);
                await _Context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
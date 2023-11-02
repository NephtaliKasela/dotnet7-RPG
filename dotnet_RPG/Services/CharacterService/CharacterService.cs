using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace dotnet_RPG.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        public static List<Character> Characters = new List<Character>(){
            new Character(),
            new Character {Id = 1, Name = "Tom"}
        };
        private readonly DataContext _Context;

        private readonly IMapper _mapper;
        public CharacterService(IMapper mapper, DataContext context)
        {
            _Context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var character = _mapper.Map<Character>(newCharacter);
            //character.Id = Characters.Max(c => c.Id) + 1;

            _Context.Characters.Add(character);
            await _Context.SaveChangesAsync();

            serviceResponse.Data = await _Context.Characters.Select(ch => _mapper.Map<GetCharacterDTO>(ch)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            try{
                var character = await _Context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if(character is null) { throw new Exception($"Character with Id '{id}' not found"); }

                _Context.Characters.Remove(character);

                await _Context.SaveChangesAsync();

                serviceResponse.Data = await _Context.Characters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();
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
            var DBCharacters = await _Context.Characters.ToListAsync();
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>(){
                Data = DBCharacters.Select(ch => _mapper.Map<GetCharacterDTO>(ch)).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            var DBcharacter = await _Context.Characters.FirstOrDefaultAsync(ch => ch.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(DBcharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            try{
                var character = await _Context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if(character is null) { throw new Exception($"Character with Id '{updatedCharacter.Id}' not found"); }

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
    }
}
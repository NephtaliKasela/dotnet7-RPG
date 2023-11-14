using System.Security.Claims;
using AutoMapper;
using dotnet_RPG.DTOs.Weapon;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace dotnet_RPG.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public IHttpContextAccessor _httpContextAccessor;
        public WeaponService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) 
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        public async Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDTO>();

            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.Users!.Id == GetUserId());
                
                if(character is null)
                {
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                var weapon = new Weapon
                {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };

                //var ConvertedWeapon = _mapper.Map<GetWeaponDTO>(weapon);
                await _context.Weapons.AddAsync(weapon);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                
            }
            return response;
        }
    }
}

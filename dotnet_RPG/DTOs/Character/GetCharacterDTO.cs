using dotnet_RPG.DTOs.Skill;
using dotnet_RPG.DTOs.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_RPG.DTOs.Character
{
    public class GetCharacterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int HitPoints { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Intelligence { get; set; }
        public RpgClass Class { get; set; } = RpgClass.Knigth;
        public GetWeaponDTO? Weapon { get; set; }
        public List<GetSkillDTO>? Skills { get; set; }
    }
}
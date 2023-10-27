using System.Text.Json.Serialization;

namespace dotnet_RPG.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Knigth = 1,
        Mage = 2, 
        Cleric = 3
    }
}
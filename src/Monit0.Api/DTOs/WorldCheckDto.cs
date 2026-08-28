using Monit0.Core.Models.WorldCheck; 


namespace Monit0.Api.DTOs
{
    public class WorldCheckDto
    {
        public string? GlobalStatus { get; set;}
        public DateTime LastDate  { get; set;}

        
    }
}
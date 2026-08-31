


namespace Monit0.Core.Entities
{
    public class Application
    {
        public int Id {get; set;}
        public required string Nom {get; set;}
        public required string Type {get; set;}
        public required string  Environnement {get; set;}
        public required List<Monitoring> Monitorings {get; set;} 
        public DateTime DateDeCréation {get; set;} 
    }    
}

namespace Monit0.Core.Entities
{
    public class Utilisateur
    {
        public int Id {get; set;}
        public required string Nom {get; set;}  
        public required string Email {get; set;} 
        public required string Pays {get; set;}
        public int Age {get; set;}
        public DateTime DateCreation {get; set;}
        
    }
   
}
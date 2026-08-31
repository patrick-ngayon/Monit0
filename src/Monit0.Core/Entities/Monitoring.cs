

namespace Monit0.Core.Entities
{
    public class Monitoring
    {
        public int Id {get; set;} 
        public string Status {get; set;} = string.Empty; 
        public int NbErreurs{get; set;}
        public DateTime DateCheck{get; set;}
        public int ApplicationId {get; set;} 

    } 
}
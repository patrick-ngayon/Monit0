namespace Monit0.Core.Models.WorldCheck

{

    public class WorldCheckMonitoring

    {

        public DateTime LastDate { get; set; }

        public int TotalCount { get; set; }

        public int ErrorCount { get; set; }

        public decimal ErrorPercentage { get; set; }

        public DateTime ExecutionTime { get; set; } = DateTime.Now;

        // Statut global calculé selon la règle métier

        public string GlobalStatus => ErrorCount > 0 ? "KO" : "OK";

        // Compatibilité avec votre ancien code

        public DateTime LAST_DATE

        {

            get => LastDate;

            set => LastDate = value;

        }

        public int NB_TOTAL

        {

            get => TotalCount;

            set => TotalCount = value;

        }

        public int NB_ERR

        {

            get => ErrorCount;

            set => ErrorCount = value;

        }

        public decimal PERCENT_ERR

        {

            get => ErrorPercentage;

            set => ErrorPercentage = value;

        }

        public string StatutGlobal => GlobalStatus;

    }

}

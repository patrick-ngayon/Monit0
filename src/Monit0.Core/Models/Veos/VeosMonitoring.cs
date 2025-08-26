// Monit0.Core/Models/Veos/VeosMonitoring.cs

using System;
using System.Collections.Generic;
using System.Linq;


namespace Monit0.Core.Models.Veos
{
    public class VeosMonitoring
    {
        public List<VeosMonitoringItem> Items { get; set; } = new List<VeosMonitoringItem>();
        public DateTime CheckTime { get; set; } = DateTime.Now;
        // Règle métier: statut global OK si tous les éléments sont OK
        public string GlobalStatus => Items.Any(item => item.Status == "KO") ? "KO" : "OK";
        // Statistiques globales
        public int TotalRecords => Items.Sum(item => item.NbEnregistrements);
        public int TotalErrors => Items.Sum(item => item.NbErr);
    }
}
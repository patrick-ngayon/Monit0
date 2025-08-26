// Monit0.Core/Models/Veos/VeosMonitoringItem.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monit0.Core.Models.Veos

{

    public class VeosMonitoringItem

    {

        public int Position { get; set; }

        public string Libelle { get; set; }

        public DateTime MaxDate { get; set; }

        public int NbEnregistrements { get; set; }

        public int NbErr { get; set; }

        // Règle métier: statut OK si aucune erreur

        public string Status => NbErr == 0 ? "OK" : "KO";

    }

}

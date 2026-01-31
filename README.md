**Surveillance temps réel WorldCheck Oracle - Clean Architecture .NET 9**

🎯 Contexte Aon
15+ applications critiques surveillées à Aon France
WorldCheck Oracle DB - Analyse 24h (10h→10h)
Rapports HTML exécutifs pour direction
Règle métier : ErrorCount > 0 = Statut KO
Dashboard mensuel automatisé

🛠️ Stack technique (Clean Architecture)
✅ .NET 9 | Console Worker Service | Dependency Injection
✅ Monit0.Core → Models + Interfaces (logique métier pure)
✅ Monit0.Infrastructure → DataService + WorldCheckService  
✅ OracleClient | SQL Server | ConnectionFactory multi-DB
✅ HtmlTemplateService → Rapports HTML/CSS autonomes
✅ Repository Pattern | Options Pattern | xUnit Tests
✅ Serilog | appsettings.json | Git versioning

🚀 Flux technique prouvé
Program.cs (DI) → Monit0Worker → WorldCheckService
↓
DataService.ExecuteQueryAsync("OracleDb1", requête custom)
↓ 
WorldCheckMonitoring (ErrorCount=0 → "OK")
↓
./reports/WorldCheck_Report_YYYYMMDD_HHMM.html

📊 Résultats mesurables
Temps détection incidents : 10min → 2min (-80%)
Rapports exécutifs : 100% automatisés
SLA monitoring : 99.5% uptime apps critiques
15+ applications sous surveillance continue

💻 Demo & Installation
$ git clone https://github.com/patrick-p-n/monit0
$ dotnet restore
$ dotnet run --project Monit0.Console

→ ./reports/WorldCheck_Report_*.html généré
→ localhost:8080 (futur dashboard)

📁 Structure projet
Monit0.sln
├── Monit0.Core/          # Modèles + Interfaces
├── Monit0.Infrastructure/# Services implémentés
├── Monit0.Console/       # Worker + Program.cs
├── monit0_guide_complet.md # Architecture détaillée
└── reports/             # HTML générés







# CLAUDE.md — Guide pour Claude Code sur le projet Monit0

Ce fichier est lu automatiquement par Claude Code au démarrage de chaque session.

---

## Présentation du projet

**Monit0** est une application console .NET 9 de monitoring de bases de données de production (environnement AON).
Elle interroge des bases Oracle et SQL Server, calcule des statuts (OK/KO) selon des règles métier, et génère des rapports HTML professionnels.

**Domaine métier :** Surveillance des traitements WorldCheck (conformité réglementaire) et VEOS (système interne AON).

---

## Architecture — Clean Architecture

```
Monit0/
├── src/
│   ├── Monit0.Core/            # Domaine pur (interfaces + modèles)
│   │   ├── Interfaces/
│   │   │   ├── IDataService.cs         # Accès base de données
│   │   │   ├── IWorldCheckService.cs   # Monitoring WorldCheck
│   │   │   ├── IVeosService.cs         # Monitoring VEOS
│   │   │   └── IHtmlTemplateService.cs # Génération rapports
│   │   └── Models/
│   │       ├── WorldCheck/WorldCheckMonitoring.cs
│   │       ├── Veos/VeosMonitoring.cs + VeosMonitoringItem.cs
│   │       └── Database/DatabaseConnection.cs + QueryResult.cs
│   │
│   ├── Monit0.Infrastructure/  # Implémentations concrètes
│   │   └── Services/
│   │       ├── DataService.cs          # Connexions Oracle + SQL Server
│   │       ├── WorldCheckService.cs    # Requête Oracle + rapport HTML
│   │       ├── VeosService.cs          # Requête Oracle ARSV01 + rapport
│   │       ├── HtmlTemplateService.cs  # Templates HTML des rapports
│   │       └── ConnectionTestService.cs
│   │
│   └── Monit0.Console/         # Point d'entrée (Program.cs)
│       ├── Program.cs           # DI + orchestration des services
│       ├── appsettings.json     # ⚠️ CREDENTIALS EN CLAIR — voir sécurité
│       └── reports/             # Rapports HTML générés (ignorés par git)
│
└── tests/
    └── Monit0.Tests.Unit/      # Tests unitaires (à développer)
```

---

## Flux d'exécution

```
Program.cs
  ├── 1. WorldCheckService.GetWorldCheckMonitoringAsync()
  │      └── DataService.ExecuteQueryAsync("OracleDb1", requête SQL Oracle)
  │              → WorldCheckMonitoring { TotalCount, ErrorCount, GlobalStatus }
  │      └── WorldCheckService.SaveReportAsync() → reports/WorldCheck_Report_*.html
  │
  ├── 2. VeosService.GetVeosMonitoringAsync()
  │      └── DataService.ExecuteQueryAsync("OracleDb2", requête SQL Oracle)
  │              → VeosMonitoring { Items[], TotalRecords, TotalErrors, GlobalStatus }
  │      └── VeosService.SaveReportAsync() → reports/VEOS_Report_*.html
  │
  └── 3. HtmlTemplateService.GenerateReportAsync([worldCheck, veos])
             → reports/Combined_Report_*.html
```

---

## Règles métier

| Monitoring | Statut KO si... |
|-----------|-----------------|
| WorldCheck | `ErrorCount > 0` (enregistrements EXPORTED dans la journée) |
| VEOS | Au moins 1 item avec `NbErr > 0` |

---

## Bases de données configurées

| Alias | Type | Base | Usage |
|-------|------|------|-------|
| `OracleDb1` | Oracle | INFO1 Production | WorldCheck |
| `OracleDb2` | Oracle | ARSV01 Production | VEOS |
| `SqlServerDb` | SQL Server | master | Prévu pour extensions futures |

---

## ⚠️ Sécurité — Action requise

**Le fichier `appsettings.json` contient des mots de passe Oracle et SQL Server en clair.**

Actions à faire :
1. Ajouter `appsettings.json` au `.gitignore`
2. Utiliser des variables d'environnement ou `dotnet user-secrets` à la place
3. Faire un `git rm --cached src/Monit0.Console/appsettings.json` si déjà commité

---

## État actuel (avril 2026)

### Fonctionnalités implémentées ✅
- Monitoring WorldCheck (requête Oracle, calcul statut, rapport HTML)
- Monitoring VEOS (requête Oracle, calcul statut par entité, rapport HTML)
- Rapport combiné HTML (WorldCheck + VEOS)
- Injection de dépendances (.NET Generic Host)
- Clean Architecture (Core / Infrastructure / Console)
- Logging via Microsoft.Extensions.Logging

### En cours / À faire selon PLAN_4_SEMAINES.md
- [ ] **Semaine 1 (en cours)** : Async/Await, Generics, LINQ complet, gestion d'erreurs
- [ ] **MockDataService** : faire tourner l'app sans Oracle (pour les tests)
- [ ] **Tests unitaires** : dossier créé, `UnitTest1.cs` vide
- [ ] **Semaine 3+** : Projet `Monit0.Api` (ASP.NET Core, Controllers, DTOs)
- [ ] **Semaine 7+** : Docker, GitHub Actions, CI/CD
- [ ] **Semaine 9** : Déploiement (Railway / Azure / Render)

---

## Objectif de formation

Patrick suit un **plan de 10 semaines** pour maîtriser ASP.NET Core + CI/CD en vue d'entretiens techniques.

Progression :
- Phase 1 (Semaines 1-2) : C# fondamentaux + avancé — **EN COURS**
- Phase 2 (Semaines 3-6) : ASP.NET Core API
- Phase 3 (Semaines 7-9) : Docker + CI/CD + Déploiement
- Phase 4 (Semaine 10+) : Consolidation + simulation d'entretien

Fichiers liés :
- [PLAN_4_SEMAINES.md](PLAN_4_SEMAINES.md) — plan détaillé semaine par semaine
- [ENTRETIEN_PREPARATION.md](ENTRETIEN_PREPARATION.md) — questions/réponses d'entretien
- [docs/GUIDE_ARCHITECTURE_MONIT0.md](docs/GUIDE_ARCHITECTURE_MONIT0.md) — guide architecture
- [ROADMAP_EXPERT_FULLSTACK.md](ROADMAP_EXPERT_FULLSTACK.md) — roadmap long terme

---

## Commandes utiles

```bash
# Build
dotnet build

# Lancer l'application
cd src/Monit0.Console && dotnet run

# Tests
dotnet test

# Voir les rapports générés
ls src/Monit0.Console/reports/
```

---

## Conventions du projet

- Langue du code : **anglais** (noms de classes, méthodes, propriétés)
- Langue des commentaires/docs : **français**
- Pattern : **Clean Architecture** — Core ne dépend jamais d'Infrastructure
- Injection de dépendances : **toujours par interface** (ex: `IWorldCheckService`, pas `WorldCheckService`)
- Nommage interfaces : préfixe `I` (ex: `IDataService`)
- Rapports générés dans : `src/Monit0.Console/reports/`

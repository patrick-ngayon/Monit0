# Préparation Entretiens Développeur .NET/Angular

> Fichier de révision - Mis à jour au fur et à mesure de l'apprentissage
>
> Entretiens prévus : Février 2026

---

## Table des matières

1. [C# Fondamental](#1-c-fondamental)
2. [Programmation Orientée Objet (POO)](#2-programmation-orientée-objet-poo)
3. [Async/Await](#3-asyncawait)
4. [Collections et LINQ](#4-collections-et-linq)
5. [Dependency Injection](#5-dependency-injection)
6. [Clean Architecture](#6-clean-architecture)
7. [ASP.NET Core](#7-aspnet-core)
8. [Angular](#8-angular)
9. [Patterns de conception](#9-patterns-de-conception)
10. [Questions comportementales](#10-questions-comportementales)

---

## 1. C# Fondamental

### Q: Quelle est la différence entre un type valeur et un type référence ?

**Réponse:**
- **Type valeur** (`int`, `bool`, `struct`, `enum`) : stocké directement dans la stack, copié lors de l'assignation
- **Type référence** (`class`, `string`, `object`, `array`) : stocké dans le heap, seule la référence est copiée

```csharp
// Type valeur - copie indépendante
int a = 5;
int b = a;
b = 10;  // a reste 5

// Type référence - même objet
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = list1;
list2.Add(4);  // list1 contient aussi 4 !
```

---

### Q: C'est quoi le mot-clé `string` en C# ? Type valeur ou référence ?

**Réponse:**
`string` est un **type référence**, MAIS il se comporte comme un type valeur car il est **immutable** (non modifiable).

```csharp
string s1 = "Hello";
string s2 = s1;
s2 = "World";  // s1 reste "Hello" (nouvelle instance créée)
```

---

## 2. Programmation Orientée Objet (POO)

### Q: Qu'est-ce qu'une interface et pourquoi l'utiliser ?

**Réponse:**
Une interface est un **contrat** qui définit des méthodes sans les implémenter.

**Avantages:**
1. **Interchangeabilité** : plusieurs classes peuvent implémenter la même interface
2. **Couplage faible** : le code dépend du contrat, pas de l'implémentation
3. **Testabilité** : on peut créer des mocks facilement

**Exemple du projet Monit0:**
```csharp
// Le contrat
public interface IDataService
{
    Task<QueryResult> ExecuteQueryAsync(string databaseName, string query);
}

// Implémentation réelle (Oracle)
public class DataService : IDataService { ... }

// Implémentation mock (sans base de données)
public class MockDataService : IDataService { ... }
```

On peut switcher entre les deux sans modifier le reste du code.

---

### Q: Qu'est-ce que le couplage faible (loose coupling) ?

**Réponse:**
Le couplage faible signifie qu'une classe **dépend d'une abstraction** (interface) plutôt que d'une implémentation concrète.

**Exemple dans Monit0:**
```csharp
// VeosService.cs
public class VeosService : IVeosService
{
    private readonly IDataService _dataService;  // Dépend de l'INTERFACE

    public VeosService(IDataService dataService)  // Reçoit l'interface
    {
        _dataService = dataService;
    }
}
```

`VeosService` ne connaît pas `DataService` ou `MockDataService`. Il connaît seulement le contrat `IDataService`.

**Avantages:**
- Changement d'implémentation sans modifier le code
- Tests facilités (on peut injecter des mocks)
- Code plus maintenable

---

### Q: Quelle est la différence entre une classe abstraite et une interface ?

**Réponse:**

| Aspect | Interface | Classe abstraite |
|--------|-----------|------------------|
| Implémentation | Aucune (avant C# 8) | Peut avoir du code |
| Héritage | Multiple possible | Simple uniquement |
| Constructeur | Non | Oui |
| Champs | Non | Oui |
| Modificateurs d'accès | Public par défaut | Tous possibles |

**Quand utiliser quoi ?**
- **Interface** : définir un contrat (ce que l'objet PEUT FAIRE)
- **Classe abstraite** : partager du code commun (ce que l'objet EST)

---

### Q: Expliquez les 4 piliers de la POO

**Réponse:**

1. **Encapsulation** : cacher les détails internes (private/public)
2. **Héritage** : une classe enfant hérite d'une classe parent
3. **Polymorphisme** : un même type peut avoir plusieurs formes
4. **Abstraction** : exposer uniquement ce qui est nécessaire

---

## 3. Async/Await

### Q: Expliquez async/await en C#

**Réponse:**
`async/await` permet d'écrire du code **asynchrone** de manière lisible.

- `async` : marque une méthode comme asynchrone
- `await` : attend le résultat sans bloquer le thread
- `Task` : représente une opération en cours

```csharp
// SANS async (bloque le thread)
public QueryResult ExecuteQuery()
{
    var result = database.Execute();  // Thread bloqué ici
    return result;
}

// AVEC async (libère le thread)
public async Task<QueryResult> ExecuteQueryAsync()
{
    var result = await database.ExecuteAsync();  // Thread libéré pendant l'attente
    return result;
}
```

**Pourquoi c'est important ?**
- Meilleure scalabilité (le thread peut traiter d'autres requêtes)
- Interface utilisateur non bloquée
- Ressources serveur optimisées

---

### Q: Quelle est la différence entre `Task` et `Task<T>` ?

**Réponse:**
- `Task` : opération asynchrone qui ne retourne rien (équivalent de `void`)
- `Task<T>` : opération asynchrone qui retourne une valeur de type `T`

```csharp
Task SaveAsync();                    // Ne retourne rien
Task<QueryResult> ExecuteAsync();    // Retourne un QueryResult
```

---

## 4. Collections et LINQ

### Q: Quelle est la différence entre `IEnumerable<T>` et `List<T>` ?

**Réponse:**
*(À compléter)*

---

### Q: Qu'est-ce que LINQ et donnez des exemples

**Réponse:**
*(À compléter)*

---

## 5. Dependency Injection

### Q: Qu'est-ce que l'injection de dépendances ?

**Réponse:**
L'injection de dépendances (DI) est un pattern où une classe **reçoit** ses dépendances au lieu de les **créer** elle-même.

**Analogie :** Un chef cuisinier reçoit ses ingrédients d'un livreur au lieu d'aller les chercher lui-même.

**Sans DI (mauvais) :**
```csharp
public class VeosService
{
    private readonly DataService _dataService;

    public VeosService()
    {
        _dataService = new DataService();  // Crée lui-même = couplage FORT
    }
}
```

**Avec DI (bien) :**
```csharp
public class VeosService
{
    private readonly IDataService _dataService;

    public VeosService(IDataService dataService)  // Reçoit de l'extérieur
    {
        _dataService = dataService;
    }
}
```

**Configuration dans Program.cs :**
```csharp
services.AddScoped<IDataService, DataService>();
// "Quand quelqu'un demande IDataService, donne-lui DataService"
```

**Avantages :**
- Couplage faible (dépend de l'interface, pas de l'implémentation)
- Testabilité (on peut injecter des mocks)
- Flexibilité (changer l'implémentation en une ligne)

---

### Q: Expliquez Scoped, Singleton, Transient

**Réponse:**

| Durée de vie | Comportement | Cas d'usage |
|--------------|--------------|-------------|
| **Singleton** | 1 instance pour toute l'app | Cache, Configuration |
| **Scoped** | 1 instance par requête HTTP | DbContext, Services métier |
| **Transient** | Nouvelle instance à chaque demande | Services légers, stateless |

**Exemple :**
```csharp
services.AddSingleton<ICacheService, CacheService>();   // Une seule instance
services.AddScoped<IDataService, DataService>();        // Une par requête
services.AddTransient<IEmailService, EmailService>();   // Nouvelle à chaque fois
```

**Piège d'entretien :** Ne jamais injecter un Scoped dans un Singleton (le Scoped deviendrait un Singleton par erreur).

---

## 6. Clean Architecture

### Q: Qu'est-ce que la Clean Architecture ?

**Réponse:**
*(À compléter)*

---

### Q: Pourquoi séparer en couches ?

**Réponse:**
*(À compléter)*

---

## 7. ASP.NET Core

### Q: Qu'est-ce qu'un Middleware ?

**Réponse:**
*(À compléter)*

---

### Q: Différence entre Controller et Minimal API ?

**Réponse:**
*(À compléter)*

---

## 8. Angular

### Q: Qu'est-ce qu'un Component ?

**Réponse:**
*(À compléter)*

---

### Q: Expliquez les Observables et RxJS

**Réponse:**
*(À compléter)*

---

## 9. Patterns de conception

### Q: Qu'est-ce que le pattern Repository ?

**Réponse:**
*(À compléter)*

---

### Q: Qu'est-ce que le pattern Service Layer ?

**Réponse:**
*(À compléter)*

---

## 10. Questions comportementales

### Q: Parlez-moi d'un projet technique que vous avez réalisé

**Réponse (Monit0):**
*(À personnaliser)*

---

### Q: Comment gérez-vous les deadlines serrées ?

**Réponse:**
*(À personnaliser)*

---

## Notes personnelles

*(Ajoute ici tes propres notes au fur et à mesure)*

---

> Dernière mise à jour : Janvier 2026

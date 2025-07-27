# Wisgenix - Architecture Overview

## High-Level Architecture

```mermaid
graph TB
    subgraph "Presentation Layer"
        API[Content.API<br/>Controllers]
        Swagger[Swagger/OpenAPI]
    end
    
    subgraph "Application Layer"
        Commands[Commands<br/>CQRS Pattern]
        Queries[Queries<br/>CQRS Pattern]
        Handlers[Command/Query Handlers]
        DTOs[Data Transfer Objects]
        Validators[FluentValidation]
        Mappings[AutoMapper Profiles]
    end
    
    subgraph "Domain Layer"
        Entities[Domain Entities<br/>Subject, Topic, Question]
        Events[Domain Events]
        Specs[Specifications]
        Exceptions[Domain Exceptions]
    end
    
    subgraph "Infrastructure Layer"
        EF[Entity Framework Core]
        Configs[EF Configurations]
        Repos[Repository Pattern]
        UoW[Unit of Work]
        DB[(SQL Database)]
    end
    
    subgraph "Cross-Cutting Concerns"
        SharedKernel[Wisgenix.SharedKernel<br/>Base Classes & Patterns]
        Logging[Structured Logging]
    end
    
    API --> Commands
    API --> Queries
    Commands --> Handlers
    Queries --> Handlers
    Handlers --> Entities
    Handlers --> Repos
    Entities --> Events
    Repos --> EF
    EF --> DB
    
    SharedKernel -.-> Entities
    SharedKernel -.-> Handlers
    AI -.-> Commands
```

## Clean Architecture Layers

### 1. **Presentation Layer** (`Content.API`)
- RESTful API controllers
- Request/Response handling
- Authentication & Authorization
- Swagger documentation

### 2. **Application Layer** (`Content.Application`)
- CQRS implementation with MediatR
- Command and Query handlers
- Data validation with FluentValidation
- Object mapping with AutoMapper
- Business orchestration

### 3. **Domain Layer** (`Content.Domain`)
- Rich domain entities with business logic
- Domain events for side effects
- Specifications for complex queries
- Domain exceptions
- Business rules enforcement

### 4. **Infrastructure Layer** (`Content.Infrastructure`)
- Entity Framework Core implementation
- Repository pattern
- Unit of Work pattern
- Database configurations
- External service integrations

## Domain Model

```mermaid
erDiagram
    Subject ||--o{ Topic : contains
    Topic ||--o{ Question : contains
    Question ||--o{ QuestionOption : has
    
    Subject {
        int Id PK
        string SubjectName
        DateTime CreatedDate
        string CreatedBy
        DateTime ModifiedDate
        string ModifiedBy
    }
    
    Topic {
        int Id PK
        string TopicName
        int SubjectId FK
        DateTime CreatedDate
        string CreatedBy
        DateTime ModifiedDate
        string ModifiedBy
    }
    
    Question {
        int Id PK
        string QuestionText
        int TopicId FK
        int DifficultyLevel
        int MaxScore
        QuestionSource GeneratedBy
        string QuestionSourceReference
        DateTime CreatedDate
        string CreatedBy
        DateTime ModifiedDate
        string ModifiedBy
    }
    
    QuestionOption {
        int Id PK
        string OptionText
        int QuestionId FK
        bool IsCorrect
        DateTime CreatedDate
        string CreatedBy
        DateTime ModifiedDate
        string ModifiedBy
    }
```

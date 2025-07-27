# Topic Entity - End-to-End Flow Documentation

## Component Design Overview

```mermaid
graph LR
    subgraph "API Layer"
        Controller[TopicsController]
    end
    
    subgraph "Application Layer"
        Command[CreateTopicCommand]
        Handler[TopicCommandHandler]
        DTO[AddTopicRequest/GetTopicResponse]
        Validator[TopicValidator]
        Mapper[AutoMapper]
    end
    
    subgraph "Domain Layer"
        Entity[Topic Entity]
        Events[TopicCreatedEvent]
        Rules[Business Rules]
    end
    
    subgraph "Infrastructure Layer"
        Repo[TopicRepository]
        Config[TopicConfiguration]
        Context[ContentDbContext]
        DB[(Database)]
    end
    
    Controller --> Command
    Command --> Handler
    Handler --> Validator
    Handler --> Entity
    Handler --> Repo
    Entity --> Events
    Entity --> Rules
    Repo --> Context
    Context --> Config
    Context --> DB
    Handler --> Mapper
    Mapper --> DTO
```

## Detailed Component Breakdown

### 1. **API Controller** (`Content.API/Controllers/TopicsController.cs`)

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Validator
    participant MediatR
    participant Handler
    
    Client->>Controller: POST /api/topics
    Controller->>Validator: ValidateAsync(request)
    Validator-->>Controller: ValidationResult
    Controller->>MediatR: Send(CreateTopicCommand)
    MediatR->>Handler: Handle(command)
    Handler-->>MediatR: GetTopicResponse
    MediatR-->>Controller: Result
    Controller-->>Client: 201 Created + Topic
```

**Responsibilities:**
- HTTP request/response handling
- Input validation
- Command dispatching via MediatR
- HTTP status code management

### 2. **Application Layer Components**

#### Command Pattern
```mermaid
classDiagram
    class CreateTopicCommand {
        +string TopicName
        +int SubjectId
        +CreateTopicCommand(topicName, subjectId)
    }
    
    class TopicCommandHandler {
        -ITopicRepository _repository
        -IUnitOfWork _unitOfWork
        -IMapper _mapper
        -ILogger _logger
        +Handle(CreateTopicCommand) GetTopicResponse
    }
    
    CreateTopicCommand --> TopicCommandHandler : handled by
```

#### Data Flow
```mermaid
flowchart TD
    A[AddTopicRequest] --> B[Validation]
    B --> C[CreateTopicCommand]
    C --> D[TopicCommandHandler]
    D --> E[Topic Entity Creation]
    E --> F[Repository Save]
    F --> G[Unit of Work Commit]
    G --> H[AutoMapper]
    H --> I[GetTopicResponse]
```

### 3. **Domain Entity** (`Content.Domain/Entities/Topic.cs`)

```mermaid
classDiagram
    class Topic {
        -List~Question~ _questions
        +string TopicName
        +int SubjectId
        +IReadOnlyCollection~Question~ Questions
        +Subject Subject
        +Topic(topicName, subjectId)
        +UpdateTopicName(topicName)
        +AddQuestion(...) Question
        +RemoveQuestion(questionId)
        -SetTopicName(topicName)
        -ContainsInvalidCharacters(input) bool
    }
    
    class AuditableEntity {
        +int Id
        +DateTime CreatedDate
        +string CreatedBy
        +DateTime ModifiedDate
        +string ModifiedBy
        +List~BaseDomainEvent~ DomainEvents
        +AddDomainEvent(event)
        +ClearDomainEvents()
    }
    
    Topic --|> AuditableEntity
```

**Business Rules Enforced:**
- Topic name cannot be empty or whitespace
- Topic name cannot exceed 200 characters
- Topic name cannot contain invalid characters (`<`, `>`, `&`, `"`, `'`)
- No duplicate questions within a topic (case-insensitive)

### 4. **Infrastructure Layer**

#### Repository Pattern
```mermaid
classDiagram
    class ITopicRepository {
        <<interface>>
        +GetByIdAsync(id) Task~Topic~
        +GetAllAsync() Task~List~Topic~~
        +AddAsync(topic) Task~Topic~
        +UpdateAsync(topic) Task~Topic~
        +DeleteAsync(id) Task
        +ExistsAsync(id) Task~bool~
    }
    
    class TopicRepository {
        -ContentDbContext _context
        +GetByIdAsync(id) Task~Topic~
        +GetAllAsync() Task~List~Topic~~
        +AddAsync(topic) Task~Topic~
        +UpdateAsync(topic) Task~Topic~
        +DeleteAsync(id) Task
        +ExistsAsync(id) Task~bool~
    }
    
    ITopicRepository <|-- TopicRepository
```

#### Entity Framework Configuration
```mermaid
classDiagram
    class TopicConfiguration {
        +Configure(EntityTypeBuilder~Topic~)
    }
    
    class ContentDbContext {
        +DbSet~Topic~ Topics
        +DbSet~Subject~ Subjects
        +DbSet~Question~ Questions
        +DbSet~QuestionOption~ QuestionOptions
    }
    
    TopicConfiguration --> ContentDbContext : configures
```

## Complete Request Flow Example

### Creating a New Topic

```mermaid
sequenceDiagram
    participant Client
    participant API as TopicsController
    participant Val as FluentValidator
    participant Med as MediatR
    participant Han as CommandHandler
    participant Dom as Topic Entity
    participant Repo as Repository
    participant UoW as Unit of Work
    participant DB as Database
    participant Map as AutoMapper
    
    Client->>API: POST /api/topics<br/>{topicName: "Algebra", subjectId: 1}
    
    API->>Val: ValidateAsync(AddTopicRequest)
    Val-->>API: ValidationResult.IsValid = true
    
    API->>Med: Send(CreateTopicCommand)
    Med->>Han: Handle(CreateTopicCommand)
    
    Han->>Repo: ExistsAsync(subjectId)
    Repo-->>Han: true
    
    Han->>Dom: new Topic("Algebra", 1)
    Dom->>Dom: SetTopicName("Algebra")
    Dom->>Dom: AddDomainEvent(TopicCreatedEvent)
    Dom-->>Han: Topic instance
    
    Han->>Repo: AddAsync(topic)
    Repo-->>Han: Topic with Id
    
    Han->>UoW: SaveChangesAsync()
    UoW->>DB: INSERT INTO Topics...
    DB-->>UoW: Success
    UoW-->>Han: Success
    
    Han->>Map: Map<GetTopicResponse>(topic)
    Map-->>Han: GetTopicResponse
    
    Han-->>Med: GetTopicResponse
    Med-->>API: GetTopicResponse
    API-->>Client: 201 Created + GetTopicResponse
```

## Domain Events Flow

```mermaid
sequenceDiagram
    participant Entity as Topic Entity
    participant Event as TopicCreatedEvent
    participant Handler as Event Handler
    participant Service as External Service
    
    Entity->>Event: AddDomainEvent(TopicCreatedEvent)
    Note over Entity: Business operation completes
    Entity->>Handler: Publish domain events
    Handler->>Service: Notify external systems
    Handler->>Service: Update read models
    Handler->>Service: Send notifications
```

## Error Handling Strategy

```mermaid
flowchart TD
    A[Request] --> B{Validation}
    B -->|Invalid| C[400 Bad Request]
    B -->|Valid| D[Business Logic]
    D --> E{Business Rules}
    E -->|Violation| F[400 Business Rule Violation]
    E -->|Valid| G[Data Access]
    G --> H{Data Operation}
    H -->|Not Found| I[404 Not Found]
    H -->|Duplicate| J[409 Conflict]
    H -->|Success| K[200/201 Success]
    H -->|Error| L[500 Internal Server Error]
```

This architecture ensures:
- **Separation of Concerns**: Each layer has distinct responsibilities
- **Testability**: Dependencies are injected and can be mocked
- **Maintainability**: Clean boundaries between layers
- **Scalability**: CQRS pattern allows independent scaling
- **Domain-Driven Design**: Rich domain models with business logic
- **Event-Driven Architecture**: Domain events for loose coupling
# Wisgenix - Low-Level Design Document

## Component-Level Architecture

### 1. Content.API - Presentation Layer

```mermaid
graph TB
    subgraph "Content.API Structure"
        subgraph "Controllers"
            SC[SubjectsController]
            TC[TopicsController]
            QC[QuestionsController]
        end
        
        subgraph "Middleware"
            GM[GlobalExceptionMiddleware]
            CM[CorrelationMiddleware]
            LM[LoggingMiddleware]
        end
        
        subgraph "Configuration"
            PS[Program.cs<br/>DI Container Setup]
            AS[appsettings.json<br/>Configuration]
        end
        
        subgraph "Models"
            REQ[Request Models]
            RES[Response Models]
            ERR[Error Models]
        end
    end
    
    GM --> SC
    GM --> TC
    GM --> QC
    SC --> REQ
    TC --> REQ
    QC --> REQ
```

#### Controller Design Pattern

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant Validator
    participant MediatR
    participant Handler
    participant Response
    
    Client->>Controller: HTTP Request
    Controller->>Validator: Validate Input
    alt Validation Fails
        Validator-->>Controller: ValidationException
        Controller-->>Client: 400 Bad Request
    else Validation Success
        Controller->>MediatR: Send Command/Query
        MediatR->>Handler: Route to Handler
        Handler-->>MediatR: Result
        MediatR-->>Controller: Response DTO
        Controller->>Response: Map to HTTP Response
        Response-->>Client: HTTP 200/201/204
    end
```

#### Key Responsibilities
- **HTTP Protocol Handling**: Request/response lifecycle management
- **Input Validation**: Basic format and required field validation
- **Command/Query Dispatching**: MediatR integration
- **Response Formatting**: Consistent API response structure
- **Error Handling**: HTTP status code mapping
- **Documentation**: OpenAPI/Swagger integration

### 2. Content.Application - Application Layer

```mermaid
graph TB
    subgraph "Content.Application Structure"
        subgraph "Commands"
            CC[CreateSubjectCommand]
            UC[UpdateSubjectCommand]
            DC[DeleteSubjectCommand]
        end
        
        subgraph "Queries"
            GQ[GetSubjectQuery]
            LQ[ListSubjectsQuery]
            SQ[SearchSubjectsQuery]
        end
        
        subgraph "Handlers"
            CH[CommandHandlers]
            QH[QueryHandlers]
        end
        
        subgraph "DTOs"
            REQ[Request DTOs]
            RES[Response DTOs]
        end
        
        subgraph "Validators"
            CV[Command Validators]
            QV[Query Validators]
        end
        
        subgraph "Mappings"
            MP[AutoMapper Profiles]
        end
    end
    
    CC --> CH
    UC --> CH
    DC --> CH
    GQ --> QH
    LQ --> QH
    SQ --> QH
    CH --> REQ
    QH --> RES
    CV --> CC
    QV --> GQ
    MP --> RES
```

#### CQRS Implementation Pattern

```mermaid
classDiagram
    class IRequest~TResponse~ {
        <<interface>>
    }
    
    class CreateSubjectCommand {
        +string SubjectName
        +string CreatedBy
    }
    
    class CreateSubjectHandler {
        -ISubjectRepository _repository
        -IUnitOfWork _unitOfWork
        -IMapper _mapper
        +Handle(command) GetSubjectResponse
    }
    
    class GetSubjectQuery {
        +int SubjectId
    }
    
    class GetSubjectHandler {
        -ISubjectRepository _repository
        -IMapper _mapper
        +Handle(query) GetSubjectResponse
    }
    
    IRequest~TResponse~ <|-- CreateSubjectCommand
    IRequest~TResponse~ <|-- GetSubjectQuery
    CreateSubjectCommand --> CreateSubjectHandler
    GetSubjectQuery --> GetSubjectHandler
```

#### Key Responsibilities
- **Business Workflow Orchestration**: Coordinate domain operations
- **External Interface Adaptation**: Convert external requests to domain operations
- **Transaction Management**: Unit of Work coordination
- **Validation Pipeline**: FluentValidation integration
- **Object Mapping**: DTO to domain entity transformation
- **Event Handling**: Domain event processing

### 3. Content.Domain - Domain Layer

```mermaid
graph TB
    subgraph "Content.Domain Structure"
        subgraph "Entities"
            SE[Subject Entity]
            TE[Topic Entity]
            QE[Question Entity]
            OE[QuestionOption Entity]
        end
        
        subgraph "Value Objects"
            SN[SubjectName]
            TN[TopicName]
            QT[QuestionText]
            DL[DifficultyLevel]
        end
        
        subgraph "Domain Events"
            SCE[SubjectCreatedEvent]
            TCE[TopicCreatedEvent]
            QCE[QuestionCreatedEvent]
        end
        
        subgraph "Repositories"
            SR[ISubjectRepository]
            TR[ITopicRepository]
            QR[IQuestionRepository]
        end
        
        subgraph "Services"
            DS[Domain Services]
            SP[Specifications]
        end
        
        subgraph "Exceptions"
            DE[Domain Exceptions]
            BR[Business Rule Violations]
        end
    end
    
    SE --> SN
    TE --> TN
    QE --> QT
    SE --> SCE
    TE --> TCE
    QE --> QCE
```

#### Rich Domain Entity Pattern

```mermaid
classDiagram
    class AuditableEntity {
        +int Id
        +DateTime CreatedDate
        +string CreatedBy
        +DateTime ModifiedDate
        +string ModifiedBy
        +List~IDomainEvent~ DomainEvents
        +AddDomainEvent(event)
        +ClearDomainEvents()
    }
    
    class Subject {
        -List~Topic~ _topics
        +SubjectName SubjectName
        +IReadOnlyCollection~Topic~ Topics
        +Subject(subjectName)
        +UpdateSubjectName(name)
        +AddTopic(topic)
        +RemoveTopic(topicId)
        -ValidateTopicUniqueness(topic)
    }
    
    class SubjectName {
        +string Value
        +SubjectName Create(value)$
        -ValidateLength(value)$
        -ValidateCharacters(value)$
    }
    
    AuditableEntity <|-- Subject
    Subject --> SubjectName
```

#### Domain Event Pattern

```mermaid
sequenceDiagram
    participant Entity
    participant DomainEvent
    participant EventHandler
    participant ExternalService
    
    Entity->>Entity: Business Operation
    Entity->>DomainEvent: AddDomainEvent()
    Entity->>Entity: Complete Operation
    Note over Entity: Transaction Boundary
    Entity->>EventHandler: Publish Events
    EventHandler->>ExternalService: Notify External Systems
    EventHandler->>ExternalService: Update Read Models
    EventHandler->>ExternalService: Send Notifications
```

#### Key Responsibilities
- **Business Logic Encapsulation**: Core business rules and invariants
- **Data Integrity**: Entity validation and consistency
- **Domain Events**: Side effect coordination
- **Rich Behavior**: Methods that operate on entity data
- **Invariant Protection**: Prevent invalid state transitions

### 4. Content.Infrastructure - Infrastructure Layer

```mermaid
graph TB
    subgraph "Content.Infrastructure Structure"
        subgraph "Data"
            CTX[ContentDbContext]
            CFG[Entity Configurations]
            MIG[Migrations]
        end
        
        subgraph "Repositories"
            SR[SubjectRepository]
            TR[TopicRepository]
            QR[QuestionRepository]
            UOW[UnitOfWork]
        end
        
        subgraph "Services"
            AI[AzureAIService]
            ES[External Services]
        end
        
        subgraph "Configurations"
            SC[SubjectConfiguration]
            TC[TopicConfiguration]
            QC[QuestionConfiguration]
        end
    end
    
    CTX --> CFG
    CTX --> MIG
    SR --> CTX
    TR --> CTX
    QR --> CTX
    UOW --> CTX
    CFG --> SC
    CFG --> TC
    CFG --> QC
```

#### Repository Implementation Pattern

```mermaid
classDiagram
    class ISubjectRepository {
        <<interface>>
        +GetByIdAsync(id) Task~Subject~
        +GetAllAsync() Task~List~Subject~~
        +AddAsync(subject) Task~Subject~
        +UpdateAsync(subject) Task~Subject~
        +DeleteAsync(id) Task
        +ExistsAsync(id) Task~bool~
        +GetByNameAsync(name) Task~Subject~
    }
    
    class SubjectRepository {
        -ContentDbContext _context
        -ILogger _logger
        +GetByIdAsync(id) Task~Subject~
        +GetAllAsync() Task~List~Subject~~
        +AddAsync(subject) Task~Subject~
        +UpdateAsync(subject) Task~Subject~
        +DeleteAsync(id) Task
        +ExistsAsync(id) Task~bool~
        +GetByNameAsync(name) Task~Subject~
    }
    
    class ContentDbContext {
        +DbSet~Subject~ Subjects
        +DbSet~Topic~ Topics
        +DbSet~Question~ Questions
        +SaveChangesAsync() Task~int~
        +OnModelCreating(builder)
    }
    
    ISubjectRepository <|-- SubjectRepository
    SubjectRepository --> ContentDbContext
```

#### Entity Framework Configuration Pattern

```mermaid
classDiagram
    class IEntityTypeConfiguration~Subject~ {
        <<interface>>
        +Configure(builder)
    }
    
    class SubjectConfiguration {
        +Configure(EntityTypeBuilder~Subject~)
    }
    
    class ContentDbContext {
        +OnModelCreating(ModelBuilder)
        +ApplyConfiguration(IEntityTypeConfiguration)
    }
    
    IEntityTypeConfiguration~Subject~ <|-- SubjectConfiguration
    ContentDbContext --> SubjectConfiguration
```

#### Key Responsibilities
- **Data Persistence**: Entity Framework Core integration
- **Query Optimization**: Efficient data retrieval
- **Transaction Management**: Unit of Work implementation
- **External Service Integration**: Azure AI, third-party APIs
- **Database Schema Management**: Migrations and configurations

## Inter-Component Communication

### Request Flow Architecture

```mermaid
sequenceDiagram
    participant API as Content.API
    participant App as Content.Application
    participant Dom as Content.Domain
    participant Infra as Content.Infrastructure
    participant DB as Database
    
    API->>App: Command/Query
    App->>App: Validate Request
    App->>Dom: Create/Retrieve Entity
    Dom->>Dom: Apply Business Rules
    Dom->>Dom: Raise Domain Events
    App->>Infra: Repository Operation
    Infra->>DB: SQL Query/Command
    DB-->>Infra: Result Set
    Infra-->>App: Domain Entity
    App->>App: Process Domain Events
    App->>App: Map to DTO
    App-->>API: Response DTO
```

### Dependency Flow

```mermaid
graph TD
    API[Content.API] --> App[Content.Application]
    App --> Dom[Content.Domain]
    App --> Infra[Content.Infrastructure]
    Infra --> Dom
    
    API --> SK[Wisgenix.SharedKernel]
    App --> SK
    Dom --> SK
    Infra --> SK
    
    style Dom fill:#e1f5fe
    style SK fill:#f3e5f5
```

## Error Handling Strategy

```mermaid
flowchart TD
    A[Request] --> B{Input Validation}
    B -->|Invalid| C[400 Bad Request<br/>ValidationException]
    B -->|Valid| D[Business Logic]
    D --> E{Business Rules}
    E -->|Violation| F[400 Bad Request<br/>BusinessRuleViolationException]
    E -->|Valid| G[Data Access]
    G --> H{Data Operation}
    H -->|Not Found| I[404 Not Found<br/>EntityNotFoundException]
    H -->|Conflict| J[409 Conflict<br/>DuplicateEntityException]
    H -->|Success| K[200/201 Success]
    H -->|Database Error| L[500 Internal Server Error<br/>DatabaseException]
    H -->|External Service Error| M[502 Bad Gateway<br/>ExternalServiceException]
```

## Performance Considerations

### Query Optimization
- **Eager Loading**: Include related entities when needed
- **Projection**: Select only required fields for DTOs
- **Pagination**: Implement efficient paging for large datasets
- **Caching**: Repository-level caching for frequently accessed data

### Memory Management
- **Dispose Pattern**: Proper resource cleanup
- **Async/Await**: Non-blocking I/O operations
- **Connection Pooling**: Efficient database connection usage

This low-level design provides detailed implementation guidance for each component while maintaining clean architecture principles and ensuring proper separation of concerns.

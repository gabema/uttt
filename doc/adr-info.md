## Architecture Decision Record (ADR)

Think of ADRs as the **“why” behind your architecture**. They’re not just documentation—they’re a living log of reasoning, alternatives, and consequences that future developers (or even *future you*) will thank you for.

### 🔑 **Core System Structure**
- **Architecture style**: monolith vs. microservices, layered vs. hexagonal, event-driven vs. request/response.  
- **Deployment model**: cloud provider (AWS, Azure, GCP), hybrid, or on-premises.  
- **Service boundaries**: how domains are split into services or modules.  

---

### 📊 **Data & Storage**
- **Database technology**: SQL vs. NoSQL, PostgreSQL vs. MongoDB.  
- **Data partitioning/replication**: sharding, multi-region strategies.  
- **Persistence approach**: event sourcing vs. CRUD.  

---

### 🔒 **Security & Compliance**
- **Authentication/authorization**: OAuth2, OpenID Connect, custom solutions.  
- **Encryption standards**: TLS versions, at-rest encryption choices.  
- **Compliance-driven design**: HIPAA, GDPR, PCI-DSS requirements.  

---

### ⚡ **Integration & Communication**
- **API style**: REST, GraphQL, gRPC, messaging.  
- **Inter-service communication**: synchronous vs. asynchronous.  
- **External dependencies**: third-party services, SaaS integrations.  

---

### 🛠 **Technology & Frameworks**
- **Programming language/platform**: .NET, Java, Node.js.  
- **Frameworks/libraries**: Angular vs. React, EF Core vs. custom ORM.  
- **Infrastructure tooling**: Kubernetes vs. serverless, Terraform vs. Bicep.  

---

### 📈 **Cross-Cutting Concerns**
- **Scalability strategies**: horizontal scaling, caching layers, CDN usage.  
- **Observability**: logging, metrics, tracing standards.  
- **Resilience**: retry policies, circuit breakers, failover strategies.  

---

### 🚦 **When to Write an ADR**
Capture a decision when:
- It **affects multiple teams** or long-term maintainability.  
- It involves **trade-offs** (performance vs. cost, flexibility vs. simplicity).  
- It’s **hard to reverse** once implemented.  
- It’s **governance-related** (compliance, security, auditability).  

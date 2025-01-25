# Sistema de Gestão de Portfólio de Investimentos

## Descrição do Projeto

Este é um sistema de gestão de portfólio de investimentos desenvolvido para atender às necessidades de uma empresa de gestão financeira. Este sistema permite que o time de operação gerencie produtos financeiros e que os clientes realizem operações de compra, venda e consulta de investimentos.

O sistema é implementado em **C#**, seguindo os padrões de **CQRS** e **Event Sourcing**, utilizando **Kafka** para gerenciamento de eventos.&#x20;

---

## Requisitos do Sistema

### Funcionalidades do Time de Operação:

- Gestão de produtos financeiros (inserção, atualização, remoção).
- Disparo de e-mails diários para notificar produtos com vencimento próximo.

### Funcionalidades para Clientes:

- Compra e venda de produtos financeiros.
- Consulta de investimentos.
- Extrato detalhado das operações realizadas.

### Requisitos de Desempenho:

- Consultas de produtos e extratos devem suportar **grande volume de requisições** com tempo de resposta abaixo de **100ms**.

---

## Tecnologias Utilizadas

### Backend:

- **.NET** (C#)
- **CQRS** e **Event Sourcing**
- **Mediator** (para command, query e event handlers)
- **MongoDB** (Repositório)
- **Redis** (Cache)
- **Kafka** (Mensageria e eventos)
- **AutoMapper** (Mapeamento de objetos)
- **Ardalis.EnumExtensions** (Para enums)

### Testes:

- **xUnit** (Testes unitários)\
  Inluir teste funcional com Moq

- **NBomber** (Load tests)

### Infraestrutura:

- **Docker** (Execução do ambiente completo)

### Estrutura do Projeto:

&#x20;(o projeto está organizado como um monorepo e padrão de CQRS event sourcing)

- **Customers** (Módulo de clientes)
- **User** (Módulo de usuários)
- **Infrastructure** (Configurações de infraestrutura)
- **Products** (Gestão de produtos financeiros)
- **Portfolio** (Gestão de portfólio de investimentos)
- **Statement** (Registro e extrato de operações do portfólio)

---

## Como Executar a Aplicação

1. **Clone o Repositório**:

   ```bash
   https://github.com/otaviotursi/InvestmentsPortfolio.git
   cd investments
   ```

2. **Suba o Ambiente com Docker**:

   ```bash
   docker-compose up
   ```

3. **Configuração do Kafka**: Execute os comandos abaixo para criar os tópicos necessários:

   ```bash
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic investment-purchased --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic investment-sold --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic product-inserted --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic product-updated --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic product-deleted --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic product-expiry-notification --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic insert-customer-portfolio-statement --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic update-customer-portfolio --partitions 3 --replication-factor 1
   docker exec kafka kafka-topics --bootstrap-server localhost:9092 --create --topic delete-customer-portfolio --partitions 3 --replication-factor 1
   ```

4. **Execução do Projeto**: Navegue até a pasta principal do projeto e execute:

   ```bash
   dotnet run --project src/Investments/Investments.sln
   ```

---

## Documentação da API

As rotas e as operações disponíveis estão descritas no Swagger.

1. Suba o projeto e acesse o Swagger pelo link:
   ```
   https://localhost:44359/swagger
   ```



````
2. Confira os endpoints para:
   - **Gestão de Produtos**
     - *** Adicionar Produto***
       POST /product 
     - *** Atualizar Produto***
       PUT /product 
     - *** Delete Produto***
       DELETE /product/{id} 
     - *** Buscar Produto***
       GET /product
         -param: productName
         -param: productId
     - *** Buscar Extrato de alterações de Produto***
       GET /product/statement
         -param: productName
         -param: productId
         -param: userId
         -param: expirationDate
   - **Gestão de Usuários**
     - *** Adicionar Usuário***
       POST /user 
     - *** Atualizar Usuário***
       PUT /user 
     - *** Delete Usuário***
       DELETE /user/{id} 
     - *** Buscar Usuário***
       GET /user
         -param: user
         -param: id
         -param: fullName
   - **Gestão de Clientes**
     - *** Adicionar Cliente***
       POST /customer 
     - *** Atualizar Cliente***
       PUT /customer 
     - *** Delete Cliente***
       DELETE /customer/{id} 
     - *** Buscar Cliente***
       GET /customer
         -param: user
         -param: id
         -param: fullName
   - **Gestão de Portfolio do cliente**
     - *** Adicionar Operação***
       POST /portfolio 
     - *** Busca portfolio do cliente***
       GET /portfolio
         -param: customerId
     - *** Busca histório de portfolio do cliente***
       GET /portfolio/statement
         -param: customerId


```

## Testes

### Testes Unitários:
Execute os testes unitários com o comando:
```bash
dotnet test
````



### Testes de Carga:

Execute os testes de carga com **NBomber** para verificar o desempenho:

```bash
cd tests/LoadTests
nbomber run
```

---

## Observações

- O sistema é preparado para lidar com alto volume de dados, garantindo escalabilidade e baixa latência.
- Para ajustes ou personalizações, acesse os arquivos de configuração na pasta **Infrastructure**.
- O projeto segue os padrões de Clean Architecture, garantindo separação clara de responsabilidades e manutenção facilitada.

---

## Contribuição

Contribuições são bem-vindas! Por favor, envie um pull request ou abra uma issue com sugestões ou melhorias.

---

## Autor

Desenvolvido por Otávio com fins de estudar tecnologias que não estão no meu cotidiano

---

##

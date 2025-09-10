## Funcionalidade 30%

Avalie se o projeto atende a todos os requisitos funcionais definidos.
* Será revisado na avalição final.

## Qualidade do Código 20%

Considere clareza, organização e uso de padrões de codificação.

### Services
* Pontos positivos
  - Boa organização em pastas por domínio
  - Clara separação de contextos delimitados
  - Boa separação de responsabilidades por projetos
  - Uso de CQRS

* Recomendações opcionais
  - Crie um _handler_ para cada _command_ e _query_, ao invés de agrupar vários em um único _handler_. Isso melhora a clareza, facilita a manutenção, os testes, e não quebre o princípio de responsabilidade única (SRP, SOLID Principles).
  - Cuidado com o uso excessivo de `AsNoTracking()`, não o use como recurso padrão.
  - Em `PaymentManagement`, parece que `PaymentCommandHandler` e `PaymentService` são redundantes. Considere adotar ou serviços ou _handlers_ para regras de negócio.

### Testes 
* Pontos positivos
  - Vários testes já criados
  - Navegação dos testes é fácil via _Test Explorer_
  - Mais de 80% de cobertura de código em toda a solução

* Comentários 
  - O teste `Register_ValidModel_ShouldReturnOkWithModel` não está testando nada. Recomendações para o _assert_:
    - Se `_userManager.CreateAsync()` foi chamado com os parâmetros corretos
    - Se `_userManager.AddToRoleAsync()` foi chamado com os parâmetros corretos
    - Se `_mediator.Send()` foi chamado com o comando correto
    - Se `_userManager.FindByEmailAsync()` foi chamado com o email correto
    - Se `_userManager.GetClaimsAsync()` foi chamado com o usuário correto
    - Se `_notifier.GetNotifications()` foi ou não foi chamado
  - Melhorar a organização dos arquivos de testes, está fácil navegar pelo _Test Explorer_, mas difícil localizar os arquivos no _Solution Explorer_.
    - Não é obrigação, apenas uma recomendação para de estrutura de testes, acompanhando a estrutura de pastas da solução, e reduzindo o tamanho das classes de teste e os nomes dos métodos.
A estrutura sugerida é:

```bash
tests/
├── ServicesTests/
│   ├── CourseManagementTests/
│   │   ├── CommandTests/
│   │   │   ├── AddCourseCommandTests.cs
│   │   │   ├── UpdateProgressCommandTests.cs
│   .   .   .
│   │   ├── HandlersTests/
│   │   │   ├── AddCourseHandlerTests.cs
│   │   │   ├── UpdateProgressHandlerTests.cs
│   .   .   .
│   │   ├── QueriesTests/
│   │   │   ├── CourseQueriesTests/
│   │   │   │   ├── GetAllTests.cs
│   │   │   │   ├── GetByIdTests.cs
│   .   .   .   .
├── ApiTests/
│   ├── AppUserServiceTests/
│   │   ├── IsAuthenticatedTests.cs
│   │   ├── GetIsTests.cs
│   │   └── IsAdminTests.cs
.   .   
``` 
Exemplo de classe de teste, com escopo e foco em um caso de uso específico, mas vários casos de teste, seguindo o modelo `[Objeto > Estado > Espectativa]`:

Teste: `tests/ServicesTests/CoursesManagement/QueriesTests/GetAllTests.cs`
```csharp
namespace Tests.ServicesTests.CourseManagementTests.QueriesTests;

public partial class CourseQueriesTests
{
    public class GetAllTests
    {
        [Fact]
        public async Task NoCourse_ReturnEmptyList() { ... }    

        [Fact]
        public async Task ExistingCourses_ReturnListSortedByName() { ... }
}
```

### Domain
* Pontos positivos
  - Uso de _Value Objects_ e entidades
  - Uso de _Data Annotations_ para validação
  - Início de adoção de comportamentos para entidades
  - Entidades e _Value Objects_ estão com boa granularidade
  - Boa separação de contextos delimitados

* Comentários e recomendações:
  - Para "preço", o tipo ideal é `decimal`, não `double`, para evitar problemas de precisão com valores monetários.
  - Em `EProgressLesson.cs`, remover o prefixo `E` do enum. Use `LessonProgress`.
  - No domínio de cursos, considere usar `{get; init;}` para propriedades que não devem ser alteradas após a criação do objeto.
  - Não é necessário um construtor sem parâmetros em entidades e _value objects_ por causa do EF. Vide https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
    
### API
* Pontos positivos
  - Boa organização em classes por domínio
  - Uso de filtros para tratamento de erros e notificações
  - Uso de _DTOs_ e _ViewModels_ para comunicação com o cliente
  - Uso de autenticação e autorização com políticas baseadas em funções (Aluno e Admin)

* Comentários e recomendações:
  - Movam as configurações dos serviços e EF de contextos delimitados para dentro de `Application`, isso vai efitar que a API tenha dependência à camadas não relevantes como à de dados. A API deve apenas referênciar à `Application`.
  - Evitem o uso de _magic strings_ para nomes de políticas de autorização. Usem constantes.
  - A _controller_ `AuthController` está com muitas regras em seus métodos, considerem criar um `AuthService` para encapsular a lógica de autenticação e registro.
  - Em `LessonsController`:
    - o _endpoint_ `/lessons/get-by-courseId` não é um caminho RESTful. Considere usar `/courses/{courseId}/lessons` para obter as lições de um curso específico.
    - o _endpoint_ `/lessons/get-progress` não é um caminho RESTful. Considere usar `/courses/{courseId}/progress` para obter o progresso do curso do aluno autenticado.
    - o _endpoint_ `/lessons/start-class` deveria ser apenas `/lessons/{lessonId}/start` para iniciar uma aula específica. Adota apenas um nome para "aula", seja "lesson" ou "class".
    - Em `StartClass()`, se a aula está completa, retorne um erro 400 (Bad Request) ao invés de 404 (Not Found), pois o recurso existe, mas a ação não é permitida.
  - A camada de API deve ser o mais fina possível, delegando a maior parte da lógica para a camada de aplicação. Evitem colocar muita lógica na API.

### Geral
* Nomenclatura em inglês dos serviços está investida, o correto é:
  - `CourseManagement`, `PaymentManagement`, `LessonProgress`
  - Dica: Use ChatGPT ou Copilote para ajudar na tradução.
* Remover códigos comentados
* Remover `using` não utilizados
* Consistência na versão do C#. Ex: usar `namespace X;` ao invés de `namespace X { ... }`, adotar contrutor primário, etc.
* Evite redundância em nomes de métodos. Ex: Na classe `CourseRepository`, o método `CourseExists()` pode ser apenas `Exists()`, pois o contexto já é de cursos.
* O comentário para "coisas à se fazer" é `// TODO: <descrição>`, para que ferramentas possam identificar.
* Considerem adotar "Code Review" para as revisões de código entre os membros do time.
* Considerem consistência entre os projetos, se adotam o nomes `Domain` ou `Business`.
* Nomes de parâmetros não devem começar com `_`, isso é apenas para campos privados.
* A documentação dos _endpoints_ pode melhorar. Ex: 
```csharp 
/// <summary>
/// Retorna todas as aulas associadas do curso
/// </summary>
/// <param name="courseId">Identificação do curso</param>
/// <returns><see cref="IEnumerable{LessonViewModel}"/>Lista de aulas e informações detalhadas</returns>
```

## Eficiência e Desempenho 20%

Avalie o desempenho e a eficiência das soluções implementadas.
* Pontos positivos
  - Uso de `AsNoTracking()` em consultas de leitura para melhorar o desempenho.
  - Use de recursos 

* Tente eliminar os _Warnings_ dos projetos quando se faz o _build_.

## Inovação e Diferenciais 10%

Considere a criatividade e inovação na solução proposta.
* Será revisado na avalição final.

## Documentação e Organização 10%

Verifique a qualidade e completude da documentação, incluindo README.md.
* Antes de submeterem para avaliação final, atualizem o README.md e repitam o processo de execução do projeto, para garantir que todas as instruções estão corretas e atualizadas.
* Será revisado na avalição final.

## Resolução de Feedbacks 10%

Avalie a resolução dos problemas apontados na primeira avaliação de frontend
* Será revisado na avalição final.
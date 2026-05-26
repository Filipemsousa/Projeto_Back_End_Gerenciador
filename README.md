# Remember Apply

## 1. Descrição do Projeto

O **Remember Apply** é uma plataforma web desenvolvida para centralizar, monitorar e gerenciar candidaturas em processos seletivos. O sistema oferece uma interface intuitiva onde o usuário pode cadastrar novas vagas, alterar o status de cada oportunidade e acompanhar seu progresso através de um Dashboard dinâmico composto por gráficos estatísticos de pizza/rosca e barras. A aplicação foi projetada focando em segurança de dados (conformidade com a LGPD e senhas) e resiliência visual de layout.

---

## 2. Tecnologias Utilizadas
* **Linguagem:** C# (Back-End) e JavaScript / HTML/CSS (Front-End)
* **Framework:** ASP.NET Core (Web API) com Entity Framework Core para persistência de dados
* **Banco de Dados:** MySQL
* **Segurança:** Autenticação e Autorização baseadas em tokens JWT (*JSON Web Tokens*) e criptografia de senhas
* **Documentação de API:** Swagger (OpenAPI UI) com suporte a cabeçalhos de autorização *Bearer*

---

## 3. Instruções de Execução
Para rodar o projeto localmente, siga os passos abaixo:

1. **Clonar o Repositório:**
   ```bash
git clone [https://github.com/seu-usuario/organiza-emprego.git](https://github.com/seu-usuario/organiza-emprego.git)
cd organiza-emprego

    2. Configurar o Banco de Dados (Back-End):
Acesse o arquivo appsettings.json na pasta do servidor e ajuste a sua DefaultConnection com as credenciais locais do seu banco de dados.

    3. Executar as Migrations e Iniciar a API:
Abra o terminal na raiz do projeto do Back-End e execute os comandos para criar as tabelas e subir o servidor:
Bash
dotnet ef database update
dotnet run
A API estará ativa e rodando por padrão em http://localhost:5000 (ou na porta configurada no seu ambiente).

    4. Configurar a URL da API no Front-End:
Certifique-se de que a constante API_URL no seu arquivo JavaScript (cadastro.js, login.js, etc.) está apontando corretamente para o endereço local da sua API (ex: http://localhost:5000/api).

    5. Executar a Aplicação Front-End:
Como a interface é construída com HTML5/CSS3/JavaScript puro, basta abrir o arquivo login.html diretamente no seu navegador de preferência ou utilizar a extensão Live Server no VS Code para rodar o ambiente local.

## 4. Endpoints da API
Abaixo estão os principais endpoints disponíveis no sistema, protegidos ou documentados via Swagger:
🔐 Autenticação (/api/Auth)
    • POST /api/Auth/register - Realiza o cadastro de um novo usuário (Valida match de senhas e consentimento LGPD).
    • POST /api/Auth/login - Valida as credenciais e retorna o Token JWT e dados do usuário.


💼 Gerenciamento de Vagas (/api/Vagas)
    • GET /api/Vagas - Lista todas as candidaturas do usuário autenticado (Suporta parâmetros de filtro por empresa/vaga).
    • POST /api/Vagas - Cadastra uma nova candidatura no banco de dados.
    • PUT /api/Vagas/{id} - Atualiza as informações ou o status de uma candidatura existente.
    • DELETE /api/Vagas/{id} - Remove uma vaga do histórico do usuário.
    • 
Nota de Desenvolvimento: Para testar os endpoints protegidos diretamente pela interface técnica, acesse http://localhost:PORTA/swagger, clique no botão Authorize no topo direito e insira o token JWT gerado no endpoint de Login utilizando o formato Bearer SEU_TOKEN_AQUI.



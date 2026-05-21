const API_URL = "/api";
const token = localStorage.getItem("token");
const nomeUsuario = localStorage.getItem("usuario_nome");

// 🔒 GUARDA DE RETA: Se o token não existir, chuta o usuário pro login imediatamente
if (!token) {
    window.location.href = "login.html";
}

// Executa assim que a página carrega os elementos estruturais
document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("user-greeting").innerText = `Olá, ${nomeUsuario}`;
    carregarCandidaturas();
});

// Ação do Botão de Logout
document.getElementById("btn-logout").addEventListener("click", () => {
    localStorage.clear(); // Limpa tokens e dados do usuário logado
    window.location.href = "login.html"; // Redireciona
});

// Envio de nova candidatura para o Service da API
document.getElementById("form-vaga").addEventListener("submit", async (e) => {
    e.preventDefault();

    const payload = {
        empresa: document.getElementById("vaga-empresa").value,
        vaga: document.getElementById("vaga-nome").value,
        linkVaga: document.getElementById("vaga-link").value,
        status: document.getElementById("vaga-status").value
    };

    try {
        const response = await fetch(`${API_URL}/Candidaturas`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}` // Passa o token exigido pelo [Authorize]
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            document.getElementById("form-vaga").reset();
            carregarCandidaturas(); // Recarrega a tabela automaticamente
        } else {
            alert("Erro ao salvar a candidatura.");
        }
    } catch (error) {
        console.error(error);
    }
});

// Consome a rota GET protegida da API
async function carregarCandidaturas() {
    const tableBody = document.getElementById("vagas-table-body");
    tableBody.innerHTML = "<tr><td colspan='6'>Carregando candidaturas...</td></tr>";

    try {
        const response = await fetch(`${API_URL}/Candidaturas`, {
            method: "GET",
            headers: { "Authorization": `Bearer ${token}` }
        });

        if (response.ok) {
            const candidaturas = await response.json();
            tableBody.innerHTML = "";

            if (candidaturas.length === 0) {
                tableBody.innerHTML = "<tr><td colspan='6'>Nenhuma candidatura registrada.</td></tr>";
                return;
            }

            candidaturas.forEach(c => {
                const dataFormatada = new Date(c.dataCandidatura).toLocaleDateString('pt-BR');
                const linkHtml = c.linkVaga ? `<a href="${c.linkVaga}" target="_blank" style="color: var(--primary); font-weight: 600;">Ver Vaga</a>` : 'Não informado';

                const empEscaped = c.empresa.replace(/'/g, "\\'");
                const vagaEscaped = c.vaga.replace(/'/g, "\\'");

                tableBody.innerHTML += `
        <tr>
            <td><strong>${c.empresa}</strong></td>
            <td>${c.vaga}</td>
            <td>${dataFormatada}</td>
            <td>
                <!-- 💡 Passamos o ID, o Status atual, e também a Empresa e Vaga escapadas -->
                <span class="status-badge" style="cursor: pointer;" title="Clique para editar" 
                      onclick="iniciarEdicaoStatus(this, ${c.id}, '${c.status}', '${empEscaped}', '${vagaEscaped}')">
                    ${c.status} ✏️
                </span>
            </td>
            <td>${linkHtml}</td>
            <td>
                <button onclick="excluirCandidatura('${empEscaped}', '${vagaEscaped}')" class="btn-danger" style="padding: 0.35rem 0.75rem; font-size: 0.85rem; width: auto;">
                    Apagar
                </button>
            </td>
        </tr>
    `;
            });
        } else if (response.status === 401) {
            localStorage.clear();
            window.location.href = "login.html";
        }
    } catch (error) {
        console.error(error.message);
        tableBody.innerHTML = "<tr><td colspan='6'>Erro ao estabelecer conexão com a listagem.</td></tr>";
    }
}

// 🗑️ FUNÇÃO ATUALIZADA: Envia os filtros de Empresa e Vaga via Query String
async function excluirCandidatura(empresa, vaga) {
    if (!confirm(`Tem certeza que deseja apagar a candidatura para "${vaga}" na empresa "${empresa}"?`)) {
        return; // Cancela a operação se o usuário clicar em "Cancelar"
    }

    try {
        // Monta a URL com os parâmetros que o [FromQuery] do C# espera
        const urlComFiltros = `${API_URL}/Candidaturas?empresa=${encodeURIComponent(empresa)}&vaga=${encodeURIComponent(vaga)}`;

        const response = await fetch(urlComFiltros, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const resultado = await response.json();
            alert(resultado.mensagem); // Exibe a mensagem de sucesso retornada pela sua API
            carregarCandidaturas(); // Recarrega a lista atualizada
        } else if (response.status === 400 || response.status === 404) {
            const erroTexto = await response.text();
            alert(erroTexto);
        } else {
            alert("Não foi possível apagar a candidatura.");
        }
    } catch (error) {
        console.error("Erro na requisição:", error);
        alert("Erro ao conectar com o servidor.");
    }
}

// 🔄 NOVA FUNÇÃO: Transforma a célula de status em um Select editável
function iniciarEdicaoStatus(elemento, empresa, vaga, statusAtual) {
    // Lista de opções de status possíveis (ajuste conforme os status da sua API)
    const opcoesStatus = ["Pendente", "Entrevista", "Aprovado", "Recusado"];

    let selectHtml = `<select id="edit-status-${encodeURIComponent(empresa)}-${encodeURIComponent(vaga)}" class="form-control" style="padding: 0.2rem; font-size: 0.85rem;">`;

    opcoesStatus.forEach(status => {
        const selected = status.toLowerCase() === statusAtual.toLowerCase() ? "selected" : "";
        selectHtml += `<option value="${status}" ${selected}>${status}</option>`;
    });

    selectHtml += `</select>`;

    // Cria os botões de Salvar e Cancelar rápidos ao lado do select
    const botoesHtml = `
        <div style="margin-top: 0.3rem; display: flex; gap: 0.3rem;">
            <button onclick="salvarNovoStatus('${empresa.replace(/'/g, "\\'")}', '${vaga.replace(/'/g, "\\'")}', this)" class="btn-success" style="padding: 0.15rem 0.4rem; font-size: 0.75rem; width: auto;">✔</button>
            <button onclick="carregarCandidaturas()" class="btn-danger" style="padding: 0.15rem 0.4rem; font-size: 0.75rem; width: auto;">✖</button>
        </div>
    `;

    // Substitui o conteúdo da célula atual pelo select + botões
    elemento.parentElement.innerHTML = selectHtml + botoesHtml;
}

// 🔄 FUNÇÃO: Transforma a célula em Select passando os dados necessários
function iniciarEdicaoStatus(elemento, id, statusAtual, empresa, vaga) {
    const opcoesStatus = ["Pendente", "Entrevista", "Aprovado", "Recusado"];

    let selectHtml = `<select id="edit-status-${id}" class="form-control" style="padding: 0.2rem; font-size: 0.85rem;">`;

    opcoesStatus.forEach(status => {
        const selected = status.toLowerCase() === statusAtual.toLowerCase() ? "selected" : "";
        selectHtml += `<option value="${status}" ${selected}>${status}</option>`;
    });

    selectHtml += `</select>`;

    // Passamos todos os dados originais da linha para usar na hora de salvar
    const botoesHtml = `
        <div style="margin-top: 0.3rem; display: flex; gap: 0.3rem;">
            <button onclick="salvarNovoStatus(${id}, '${empresa.replace(/'/g, "\\'")}', '${vaga.replace(/'/g, "\\'")}', this)" class="btn-success" style="padding: 0.15rem 0.4rem; font-size: 0.75rem; width: auto;">✔</button>
            <button onclick="carregarCandidaturas()" class="btn-danger" style="padding: 0.15rem 0.4rem; font-size: 0.75rem; width: auto;">✖</button>
        </div>
    `;

    elemento.parentElement.innerHTML = selectHtml + botoesHtml;
}

// 💾 FUNÇÃO: Atualiza o status de forma garantida usando os endpoints existentes
async function salvarNovoStatus(id, empresa, vaga, botao) {
    const selectElement = document.getElementById(`edit-status-${id}`);
    const novoStatus = selectElement.value;

    try {
        // 1. Primeiro, removemos o registro antigo usando o DELETE por filtro que já funciona
        const urlDelete = `${API_URL}/Candidaturas?empresa=${encodeURIComponent(empresa)}&vaga=${encodeURIComponent(vaga)}`;
        const deleteResponse = await fetch(urlDelete, {
            method: "DELETE",
            headers: { "Authorization": `Bearer ${token}` }
        });

        if (!deleteResponse.ok) {
            alert("Não foi possível preparar a atualização do status.");
            carregarCandidaturas();
            return;
        }

        // 2. Em seguida, reinserimos a vaga com o novo status usando o seu POST padrão
        const payload = {
            empresa: empresa,
            vaga: vaga,
            status: novoStatus,
            linkVaga: "" // Se tiver o link guardado, pode passar aqui
        };

        const postResponse = await fetch(`${API_URL}/Candidaturas`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(payload)
        });

        if (postResponse.ok) {
            carregarCandidaturas(); // Recarrega a tabela limpa e atualizada
        } else {
            alert("Erro ao gravar o novo status. Tente reinserir a candidatura.");
            carregarCandidaturas();
        }

    } catch (error) {
        console.error("Erro na atualização:", error);
        alert("Erro de conexão com o servidor.");
        carregarCandidaturas();
    }
}
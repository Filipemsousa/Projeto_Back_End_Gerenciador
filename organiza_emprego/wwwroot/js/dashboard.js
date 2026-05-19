const API_URL = "http://localhost:5000/api";
const token = localStorage.getItem("token");
const nomeUsuario = localStorage.getItem("usuario_nome");

// 🔒 GUARDA DE ROTA: Se o token não existir, chuta o usuário pro login imediatamente
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
    tableBody.innerHTML = "<tr><td colspan='5'>Carregando candidaturas...</td></tr>";

    try {
        const response = await fetch(`${API_URL}/Candidaturas`, {
            method: "GET",
            headers: { "Authorization": `Bearer ${token}` }
        });

        if (response.ok) {
            const candidaturas = await response.json();
            tableBody.innerHTML = "";

            if (candidaturas.length === 0) {
                tableBody.innerHTML = "<tr><td colspan='5'>Nenhuma candidatura registrada.</td></tr>";
                return;
            }

            candidaturas.forEach(c => {
                const dataFormatada = new Date(c.dataCandidatura).toLocaleDateString('pt-BR');
                const linkHtml = c.linkVaga ? `<a href="${c.linkVaga}" target="_blank" style="color: var(--primary); font-weight: 600;">Ver Vaga</a>` : 'Não informado';

                tableBody.innerHTML += `
                    <tr>
                        <td><strong>${c.empresa}</strong></td>
                        <td>${c.vaga}</td>
                        <td>${dataFormatada}</td>
                        <td><span class="status-badge">${c.status}</span></td>
                        <td>${linkHtml}</td>
                    </tr>
                `;
            });
        } else if (response.status === 401) {
            // Se o token expirou no backend, força o logout no front
            localStorage.clear();
            window.location.href = "login.html";
        }
    } catch (error) {
        console.error(error);
        tableBody.innerHTML = "<tr><td colspan='5'>Erro ao estabelecer conexão com a listagem.</td></tr>";
    }
}
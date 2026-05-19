const API_URL = "http://localhost:5000/api";

// 🔒 Guarda de Rota invertida: se já estiver logado, pula direto pro painel
if (localStorage.getItem("token")) {
    window.location.href = "index.html";
}

document.getElementById("form-login").addEventListener("submit", async (e) => {
    e.preventDefault();

    const payload = {
        email: document.getElementById("login-email").value,
        senha: document.getElementById("login-senha").value
    };

    try {
        const response = await fetch(`${API_URL}/Auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            const dados = await response.json();

            // Salva os dados retornados pela nossa API sem BCrypt
            localStorage.setItem("token", dados.token);
            localStorage.setItem("usuario_nome", dados.nome || "Candidato");

            // Navega de verdade para a página do painel
            window.location.href = "index.html";
        } else {
            alert("E-mail ou senha incorretos.");
        }
    } catch (error) {
        console.error(error);
        alert("Não foi possível conectar ao servidor da API.");
    }
});
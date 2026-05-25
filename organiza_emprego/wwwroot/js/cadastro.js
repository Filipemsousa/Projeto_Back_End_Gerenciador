const API_URL = "/api";


document.getElementById("form-cadastro").addEventListener("submit", async (e) => {
    e.preventDefault();

    // 🛡️ ADICIONADO: Validação do Checkbox da LGPD
    const checkboxTermos = document.getElementById("aceitar-termos");
    if (!checkboxTermos.checked) {
        alert("Para prosseguir com o cadastro, você deve ler e aceitar os termos da LGPD.");
        return; // Força a parada do código e não envia nada para a API
    }

    // Seu código original continua perfeitamente intacto aqui embaixo 👇
    const payload = {
        nome: document.getElementById("cad-nome").value,
        email: document.getElementById("cad-email").value,
        senha: document.getElementById("cad-senha").value
    };

    try {
        const response = await fetch(`${API_URL}/Auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            alert("Cadastro realizado com sucesso!");
            window.location.href = "login.html"; // Redireciona para o login
        } else {
            const erroTxt = await response.text();
            alert(`Erro no cadastro: ${erroTxt}`);
        }
    } catch (error) {
        console.error(error.message);
        alert("Erro ao tentar conectar à API.");
    }
});
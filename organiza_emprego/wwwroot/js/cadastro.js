const API_URL = "/api";


document.getElementById("form-cadastro").addEventListener("submit", async (e) => {
    e.preventDefault();

    // 1. 🛡️ Validação do Checkbox da LGPD (Mantida)
    const checkboxTermos = document.getElementById("aceitar-termos");
    if (!checkboxTermos.checked) {
        alert("Para prosseguir com o cadastro, você deve ler e aceitar os termos da LGPD.");
        return;
    }

    // 2. 🔑 ADICIONADO: Validação de Igualdade das Senhas
    const senha = document.getElementById("cad-senha").value;
    const confirmarSenha = document.getElementById("cad-confirmar-senha").value;

    if (senha !== confirmarSenha) {
        alert("As senhas digitadas não coincidem. Por favor, verifique e tente novamente.");
        return; // Interrompe a execução antes de montar o payload ou chamar a API
    }

    // Seu código original de envio continua perfeitamente intocado daqui para baixo 👇
    const payload = {
        nome: document.getElementById("cad-nome").value,
        email: document.getElementById("cad-email").value,
        senha: senha // Utiliza a variável tratada acima
    };

    try {
        const response = await fetch(`${API_URL}/Auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            alert("Cadastro realizado com sucesso!");
            window.location.href = "login.html";
        } else {
            const erroTxt = await response.text();
            alert(`Erro no cadastro: ${erroTxt}`);
        }
    } catch (error) {
        console.error(error.message);
        alert("Erro ao tentar conectar à API.");
    }
});
const url = 'https://localhost:7221/Auth/login';

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('login-form');

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        try {
            const loginRequest = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({email, password}),
            });

            const loginResponse = await loginRequest.json();

            if (loginRequest.status !== 200) {
                console.log("Erro:");
                console.log(loginResponse.status);
                console.log(loginResponse.message);
                return;
            }

            const userTokens = {
                token: loginResponse.data.token,
                refreshToken: loginResponse.data.refreshToken
            };

            localStorage.setItem('token', userTokens.token);
            localStorage.setItem('refreshToken', userTokens.refreshToken);

            window.location.href = '/src/Pages/index.html'

        } catch (error) {
            alert('Erro ao conectar com a API: ' + error.message);
        }
    });
});

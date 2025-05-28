const url = 'https://localhost:7221/Auth/login';

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('login-form');

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            if (!response.ok) {

                const errorData = await response.json();
                alert('Erro no login: ' + (errorData.message || response.statusText));
                return;
            }

            const data = await response.json();
            const { token, refreshToken } = data.data;

            console.log(token);
            console.log(refreshToken);

        } catch (error) {
            alert('Erro ao conectar com a API: ' + error.message);
        }
    });
});

const url = "https://localhost:7221/Auth/verify-email";
const params = new URLSearchParams(window.location.search);
const email = params.get('email');
const userEmail = document.getElementById('user-email');

userEmail.innerText = email;

const confirmForm = document.getElementById('confirm-email-form');

confirmForm.addEventListener('submit', async function(event) {
    event.preventDefault();

    const emailCode = document.getElementById('email-code').value;

    const user = {
        email: email,
        emailCode: emailCode,
    }

    await verifyEmail(user);
});

async function verifyEmail(user) {
    const verifyEmailRequest = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(user)
    });

    const verifyEmailResponse = await verifyEmailRequest.json();

    if (verifyEmailRequest.status !== 200) {
        console.log(verifyEmailResponse.status)
        console.log(verifyEmailResponse.message);
        return;
    }

    console.log('Successo!!');
    console.log(verifyEmailResponse.status);
    console.log(verifyEmailResponse.message)

    const userIsLogged = await logUser()

    if (userIsLogged === true) {
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userPassword');
        window.location.href = '/src/Pages/index.html';
    }
}

async function logUser(){
    const loginURL = 'https://localhost:7221/Auth/login'

    const email = localStorage.getItem('userEmail');
    const password = localStorage.getItem('userPassword');

    const loginRequest = await fetch(loginURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({email, password}),
    });

    const loginResponse = await loginRequest.json();

    const userTokens = {
        token: loginResponse.data.token,
        refreshToken: loginResponse.data.refreshToken
    };

    localStorage.setItem('token', userTokens.token);
    localStorage.setItem('refreshToken', userTokens.refreshToken);

    return loginRequest.status === 200;
}


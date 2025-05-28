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
    console.log(user.email);
    console.log(user.emailCode);

    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(user)  // Não precisa { user }, já está como objeto.
    });

    const data = await response.json();

    console.log(data);
    console.log(data.data);
    console.log(response.statusText);
}

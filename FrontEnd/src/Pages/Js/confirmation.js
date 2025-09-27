document.addEventListener("DOMContentLoaded", async() =>{
    const confirmationItem = localStorage.getItem('confirmation-item')
    const message = document.getElementById('item-message');
    const phone = localStorage.getItem('user-phone');
    const email = localStorage.getItem('user-email');
    const confirmForm = document.getElementById('confirm-item-form');

    switch (confirmationItem) {
        case 'phone-content':
            message.innerHTML = `<p id="item-message">
                                Enviamos um código de confirmação para o número <strong id="user-item">${phone}</strong>
                                . Coloque ele logo abaixo:
                             </p>`;

            const sendPhoneConfirmationRequest = await sendPhoneVerification(email)
            console.log(sendPhoneConfirmationRequest)

            confirmForm.addEventListener('submit', async function(event) {
                event.preventDefault();

                const phoneCode = document.getElementById('item-code').value;

                const user = {
                    email: email,
                    emailCode: phoneCode,
                }

                const verifyEmailCodeResponse = await verifyPhoneCode(user);
                console.log(verifyEmailCodeResponse)
            });

            break;

        case 'email-content':
            message.innerHTML = `<p id="item-message">
                                Enviamos um código de confirmação para o email <strong id="user-item">${email}</strong>
                                . Coloque ele logo abaixo:
                             </p>`;

            const sendEmailVerificationResponse = await sendEmailVerification(email);
            console.log(sendEmailVerificationResponse)

            confirmForm.addEventListener('submit', async function(event) {
                event.preventDefault();

                const emailCode = document.getElementById('item-code').value;

                const user = {
                    email: email,
                    emailCode: emailCode,
                }

                const verifyEmailCodeResponse = await verifyEmailCode(user);
                console.log(verifyEmailCodeResponse)
            });

            break;
    }
})

async function sendEmailVerification(email) {
    const emailConfirmationURL = 'https://localhost:7221/Auth/email-confirmation';

    const sendEmailVerificationRequest = await fetch(emailConfirmationURL,{
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(email)
    })

    return await sendEmailVerificationRequest.json();
}

async function verifyEmailCode(user){
    const verifyEmailCodeURL = `https://localhost:7221/Auth/verify-email`

    const verifyEmailRequest = await fetch(verifyEmailCodeURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(user)
    });

    return await verifyEmailRequest.json();
}

async function sendPhoneVerification(email){
    const sendPhoneCodeURL = `https://localhost:7221/Auth/phone-confirmation`

    const sendPhoneVerificationRequest = await fetch(sendPhoneCodeURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(email)
    })

    return await sendPhoneVerificationRequest.json()
}

async function verifyPhoneCode(user){
    const verifyPhoneCodeURL = `https://localhost:7221/Auth/verify-phone`

    const verifyPhoneRequest = await fetch(verifyPhoneCodeURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(user)
    })

    return await verifyPhoneRequest.json();
}
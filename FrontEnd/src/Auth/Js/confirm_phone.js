const url = "https://localhost:7221/Auth/verify-phone";
const params = new URLSearchParams(window.location.search);
const phone = params.get('phone');
const userPhone = document.getElementById('user-phone');

userPhone.innerText = phone;

const confirmForm = document.getElementById('confirm-phone-form');

confirmForm.addEventListener('submit', async function(event) {
    event.preventDefault();

    const phoneCode = document.getElementById('phone-code').value;

    const user = {
        email: email,
        phoneCode: phoneCode,
    }

    await verifyPhone(user);
});

async function verifyPhone(user) {
    const verifyPhoneRequest = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(user)
    });

    const verifyPhoneResponse = await verifyPhoneRequest.json();

    if (verifyPhoneResponse.status !== 200) {
        console.log(verifyPhoneResponse.status)
        console.log(verifyPhoneResponse.message);
        return;
    }

    console.log('Successo!!');
    console.log(verifyPhoneResponse.status);
    console.log(verifyPhoneResponse.message)
}

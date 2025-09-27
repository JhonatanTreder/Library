document.addEventListener('DOMContentLoaded', async () => {
    try {
        const token = localStorage.getItem('token');

        if (!token) {
            setTimeout(() => {
                alert('dfs')
                window.location.href = '/src/Auth/login.html';
            }, 2000);
            return;
        }

        const getUserResponse = await getUserRequest(token);

        if (getUserResponse.status === "Ok") {

            const spans = document.querySelectorAll('.info-content');

            const userInfo = {
                name: getUserResponse.data.name,
                email: getUserResponse.data.email,
                phoneNumber: getUserResponse.data.phoneNumber,
                userType: getUserResponse.data.userType,
            }

            spans[0].innerText = userInfo.name;
            spans[1].innerText = userInfo.email;
            spans[3].innerText = userInfo.phoneNumber;
            spans[4].innerText = userInfo.userType;

            await getUserDashBoard(token);
            await getPendingValidations(token);

            const saveButton = document.getElementById('save-button');

            saveButton.addEventListener('click', async () => {

                await updateUserRequest(token);
            })
        }

    } catch (e) {
        console.error(e);
        console.log('O servidor está offline ou inacessível, tente novamente mais tarde.');
    }
});

function parseJWT(token) {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
        atob(base64)
            .split('')
            .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
    );
    return JSON.parse(jsonPayload);
}

async function getUserRequest(token) {

    const tokenData = parseJWT(token);
    const userEmail = tokenData['email']

    const getUserURL = `https://localhost:7221/User/email/${encodeURIComponent(userEmail)}`;

    const getUserRequest = await fetch(getUserURL, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        }
    })

    if (getUserRequest.status === 404) {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('userEmail');

        window.location.href = '/src/Auth/register.html';
    }

    if (getUserRequest.status === 401) {
        window.location.href = '/src/Auth/register.html';
    }

    return await getUserRequest.json();
}

async function updateUserRequest(token) {

    const spans = document.querySelectorAll('.info-content');

    const userName = spans[0].innerText;
    const userEmail = spans[1].innerText;
    const userPassword = spans[2].innerText;
    const userPhoneNumber = spans[3].innerText;

    const userUpdateDTO = {
        name: userName,
        email: userEmail,
        phoneNumber: userPhoneNumber
    }

    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$/

    if (passwordRegex.test(userPassword) === true) {
        userUpdateDTO.password = userPassword;
    }

    console.log(userUpdateDTO)

    const payload = await parseJWT(token);
    const userId = payload['nameid'];

    const updateUserURL = `https://localhost:7221/User/${encodeURIComponent(userId)}`;

    const putUserInfoResponse = await fetch(updateUserURL, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(userUpdateDTO)
    })

    console.log(putUserInfoResponse)
}

async function getPendingValidations(token) {

    const payload = await parseJWT(token);

    const userId = payload['nameid'];

    const pendingValidationsURL = `https://localhost:7221/User/${encodeURIComponent(userId)}/pending-validations`;

    const getPendingValidationsResponse = await fetch(pendingValidationsURL, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        }
    })

    const response = await getPendingValidationsResponse.json();

    const pendingValidations = {
        email: response.data.email,
        phoneNumber: response.data.phoneNumber,
    }

    await showPendingValidations(pendingValidations);
}

async function showPendingValidations(pendingValidations) {
    const verifications = document.getElementById('pending-verifications');

    if (!pendingValidations || (pendingValidations.email == null && pendingValidations.phoneNumber == null)) {
        verifications.style.display = 'none';
        return;
    }

    verifications.style.display = '';

    if (pendingValidations.email != null) {
        const emailContent = document.createElement('div');

        emailContent.id = 'email-content';
        emailContent.classList.add('verify-item');
        emailContent.innerHTML = `

            <i class="material-symbols-outlined">Email</i>
            <h5 class="info-title">Email:</h5>
            <span class="info-content">${pendingValidations.email}</span>
            <button class="verify-button">Verificar</button>
        `;

        verifications.appendChild(emailContent);
    }

    if (pendingValidations.phoneNumber != null) {
        const phoneContent = document.createElement('div');

        phoneContent.id = 'phone-content';
        phoneContent.classList.add('verify-item');
        phoneContent.innerHTML = `

            <i class="material-symbols-outlined">Phone</i>
            <h5 class="info-title">Número de telefone:</h5>
            <span class="info-content">${pendingValidations.phoneNumber}</span>
            <button class="verify-button">Verificar</button>
        `;

        verifications.appendChild(phoneContent);

        await delegateVerificationType()
    }
}

async function delegateVerificationType() {
    const verifyItems = document.getElementsByClassName('verify-item');

    Array.from(verifyItems).forEach((item) => {
        const verifyButton = item.querySelector('.verify-button');
        const infoContent = item.querySelector('.info-content').innerText;

        verifyButton.addEventListener('click', () => {

            switch (item.id){
                case 'phone-content':
                    const email = document.getElementById('email-section').innerText;
                    localStorage.setItem('user-phone', infoContent);
                    localStorage.setItem('user-email', email);
                    break;


                case 'email-content':
                    localStorage.setItem('info-content', infoContent);
                    break;
            }

            localStorage.setItem('confirmation-item', item.id);
            window.location.href = '/src/Pages/confirmation.html';
        })
    })

    console.log(verifyItems)
}

async function getUserDashBoard(token) {

    const payload = await parseJWT(token);

    const userId = payload['nameid'];

    const userDashboardURL = `https://localhost:7221/User/${encodeURIComponent(userId)}/user-dashboard`;

    const getUserDashboardResponse = await fetch(userDashboardURL, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        }
    });

    const dashboardResponseData = await getUserDashboardResponse.json();

    const cardContents = document.querySelectorAll('.card-content');

    cardContents[0].innerText = dashboardResponseData.data['totalLoans']
    cardContents[1].innerText = dashboardResponseData.data['totalFines']
    cardContents[2].innerText = dashboardResponseData.data['favoriteBooks'].value ?? 0
    cardContents[3].innerText = dashboardResponseData.data['totalLoans'].value ?? 0
    cardContents[4].innerText = dashboardResponseData.data['entryDate']
    cardContents[5].innerText = dashboardResponseData.data['eventsHeld'].value ?? 0

    return dashboardResponseData;
}
document.addEventListener('DOMContentLoaded', async () => {
    try {
        const token = localStorage.getItem('token');

        if (!token){
            return window.location.href = '/src/Auth/login.html';
        }

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

        const getUserResponse = await getUserRequest.json();

        if (getUserRequest.ok) {
            const userInfo = {
                name: getUserResponse.data.name,
                email: getUserResponse.data.email,
                phoneNumber: getUserResponse.data.phoneNumber,
                userType: getUserResponse.data.userType,
            }

            const spans = document.querySelectorAll('.info-content');

            spans[0].innerText = userInfo.name;
            spans[1].innerText = userInfo.email;
            spans[3].innerText = userInfo.phoneNumber;
            spans[4].innerText = userInfo.userType;

            if (token) {
                const payload = await parseJWT(token);

                const userId = payload['nameid'];

                const saveButton = document.getElementById('save-button');

                saveButton.addEventListener('click', async (e) => {

                    const userUpdateDTO = {
                        name: spans[0].innerText,
                        email: spans[1].innerText,
                        phoneNumber: spans[3].innerText,
                    }

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
                })

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
            }
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


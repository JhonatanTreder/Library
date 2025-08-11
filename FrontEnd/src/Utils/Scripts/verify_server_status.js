const serverStatusURL = `https://localhost:7221/Health/server-status`

document.addEventListener('DOMContentLoaded', async () => {
    try {

        const serverStatusResponse = await fetch(serverStatusURL,{
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });

        const responseData = await serverStatusResponse.json()

        if (serverStatusResponse.ok) {
            document.body.style.display = 'block';
            console.log(responseData.status);
            console.log(responseData.message);
        }
    }
    catch (error) {
        console.error(error);
        window.location.href = '/src/Pages/Errors/server_down.html';
    }
})
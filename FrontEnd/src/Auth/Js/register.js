const url = 'https://localhost:7221/Auth/register';
const emailConfirmationUrl = 'https://localhost:7221/Auth/email-confirmation';

document.getElementById('button').addEventListener('click', callAPI);

async function callAPI(event) {
    event.preventDefault();

    const newUser = {
        name: document.getElementById('name').value,
        email: document.getElementById('email').value,
        password: document.getElementById('password').value
    };

    const isValidUser = await userIsValid(newUser)

    alert('O usuário é válido? ' + isValidUser)

    if (!isValidUser) return;

    try {

        const registerRequest = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newUser)
        });

        const registerResponse = await registerRequest.json();

        if (registerRequest.status !== 200) {

            alert(registerResponse.status);
            alert(registerResponse.message);
            clearInputs()

            return;
        }

        alert(registerResponse.status);
        alert(registerResponse.message);

        clearInputs()

        const sendEmailRequest = await fetch(emailConfirmationUrl,{
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newUser.email)
        })

        const sendEmailResponse = await sendEmailRequest.json();

        if (sendEmailRequest.status !== 200) {

            alert(sendEmailResponse.status);
            alert(sendEmailResponse.message);
        }

        const loginCredentials = {
            email: newUser.email,
            password: newUser.password,
        }

        localStorage.setItem('userEmail', loginCredentials.email);
        localStorage.setItem('userPassword', loginCredentials.password)
        window.location.href = `confirm_email.html?email=${encodeURIComponent(newUser.email)}`;

    } catch (error) {

        console.error('Erro na requisição:', error);
    }
}

async function userIsValid(newUser){

    const nameRegex = /^[-a-zA-Z0-9._]{3,45}$/;
    const passwordRegex = /^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])[A-Za-z0-9!@#$%^&*]{6,12}$/;
    const errorList = []

    //Validação do nome
    if (!nameRegex.test(newUser.name)){
        alert('O nome de usuário deve ter entre 3 e 45 caracteres e conter apenas letras, números, ponto, underline ou hífen')
        errorList.push('name error')
    }

    if (newUser.name.length < 3 || newUser.name.length > 45) {
        alert('O nome de usuário deve estar entre 3 a 45 caracteress')
        errorList.push('name error')
    }

    if (newUser.name === ''){
        alert('O nome de usuário está sem caracteres')
        errorList.push('name error')
    }

    if (newUser.name.toString().includes(' ')){
        alert(`O nome de usuário "${newUser.name}" não pode conter espaços em branco`)
        errorList.push('name error')
    }

    //Validação do Email
    if (newUser.email.length < 6 || newUser.email.length > 256){
        alert('O tamanho do email deve conter entre 6 a 256 caracteres')
        errorList.push('email error')
    }

    if (newUser.email === '') {
        alert('O email é obrigatório e não pode ser vazio')
        errorList.push('email error')
    }

    if (newUser.email.toString().includes(' ')){
        alert('O email não pode conter espaços em branco')
        errorList.push('email error')
    }

    //Validação da senha
    if (!passwordRegex.test(newUser.password)) {
        alert('A senha deve ter entre 6 e 12 caracteres, contendo pelo menos uma letra maiúscula, um número e um símbolo especial (!@#$%^&*).')
        errorList.push('password error')
    }

    if (newUser.password === ''){
        alert('A senha é obrigatória e não pode ser vazia')
        errorList.push('password error')
    }

    if (newUser.password.toString().includes(' ')){
        alert('A senha não pode conter espaços em branco')
        errorList.push('password error')
    }

    return errorList.length === 0;
}

function clearInputs(){
    document.getElementById('name').value = '';
    document.getElementById('email').value = '';
    document.getElementById('password').value = '';
}

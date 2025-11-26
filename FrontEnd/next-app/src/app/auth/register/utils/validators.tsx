export function validateUserName(name: string): boolean {
    return  /^(?=.*[A-Za-z]{3})[A-Za-z](?:[A-Za-z ]{1,43}[A-Za-z])?$/.test(name)
}

export function validateEmail(email: string): boolean {
    return /^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?$/.test(email);
}

export function validatePassword(email: string): boolean {
    return /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=\[{\]};:'",<.>/?\\|`~])[A-Za-z\d!@#$%^&*()_\-+=\[{\]};:'",<.>/?\\|`~]{6,45}$/.test(email)
}

export function validateMatriculates(matriculates:string): boolean {
    return /^[0-9]{15}$/.test(matriculates)
}
export function validateEmail(email: string): boolean {
    return /^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?$/.test(email);
}

export function validatePhoneNumber(phoneNumber: string): boolean {
    if (phoneNumber === '') {
        return true
    }
    
    return /^\+55(11|12|13|14|15|16|17|18|19|21|22|24|27|28|31|32|33|34|35|37|38|41|42|43|44|45|46|47|48|49|51|53|54|55|61|62|63|64|65|66|67|68|69|71|73|74|75|77|79|81|82|83|84|85|86|87|88|89|91|92|93|94|95|96|97|98|99)(9[0-9]{8}|[2-9][0-9]{7})$/.test(phoneNumber);
}

export function validateUserName(userName: string): boolean {
    return  /^(?=.*[A-Za-z]{3})[A-Za-z](?:[A-Za-z ]{1,43}[A-Za-z])?$/.test(userName)
}

export function validateMatriculates(matriculates:string): boolean {
    return /^[0-9]{15}$/.test(matriculates)
}
import { HttpClient } from "@angular/common/http";
import { LoginDTO } from "../../../interfaces/Auth";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.development";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private httpClient = inject(HttpClient);
   
    login(loginCredentials: LoginDTO) {
        return this.httpClient.post(`${environment.urls.localHost}/Auth/login`, loginCredentials, {
            observe: 'response',
            headers: { 'Content-Type': 'application/json' }
        }).subscribe({
            next: (loginResponse: any) => {
                console.log(loginResponse.body.data)
            },
            error: (apiError) => {
                console.log(apiError.error)
            }
        })
    }

    isAuthenticated() {
        const token = sessionStorage.getItem('token');
    }

    parseJWT(token: string) {
        const payload = token.split('.')[1];
        const decoded: any = atob(payload);

        console.log(decoded)
    }
}
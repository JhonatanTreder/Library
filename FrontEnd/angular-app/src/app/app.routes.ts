import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';

export const routes: Routes = [
    {
        path: '', loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent),
        title: 'Home - Library'
    },
    {
        path: 'login', loadComponent: () => import('./pages/auth/login/login').then(m => m.LoginComponent),
        title: 'Login - Library'
    }
];

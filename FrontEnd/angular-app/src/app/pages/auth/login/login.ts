import { LoginDTO } from '../../../../interfaces/Auth';
import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { isValidEmail, isValidPassword } from '../../../../utils/formValidator';
import { AnimateOnScrollDirective } from '../../../../utils/animation/animateOnScroll';
import { generate } from 'rxjs';

@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    AnimateOnScrollDirective
  ],
  standalone: true,
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent implements OnInit {

  private authService = inject(AuthService)

  platoIcon = 'assets/images/platao-icon.webp'
  aristotelesIcon = 'assets/images/aristoteles-icon.webp'
  isValidEmail: boolean | undefined = undefined
  isValidPassword: boolean | undefined = undefined
  formLoading: boolean = false

  ngOnInit(): void {
  }

  loginUser() {

  }

  validateEmail(email: string) {
    this.isValidEmail = isValidEmail(email)
  }

  validatePassword(password: string) {
    this.isValidPassword = isValidPassword(password)
  }
}

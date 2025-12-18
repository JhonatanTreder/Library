"use client";

import Image from 'next/image';
import React, { useEffect, useState } from 'react';

// Hooks para validação do formulário
import { useValidation } from '@/app/auth/login/hooks/useValidation';
import { validateEmail, validatePassword } from '@/app/auth/login/utils/validators';

import useAnimateOnScroll from '@/app/utils/Animations';
import animationStyles from '@/app/utils/styles/animations.module.css';

import leftContentStyles from './styles/leftContent.module.css';
import platoIcon from '@/app/auth/register/images/platao-icon.webp';
import aristotleIcon from '@/app/auth/register/images/aristoteles-icon.webp';

import formStyles from './styles/loginForm.module.css';
import { useLogin } from '@/app/auth/login/hooks/useLogin';

export default function LoginForm() {
  const { loginUser, loading, error } = useLogin();

  useEffect(() => {
    useAnimateOnScroll(`.toRightAnimation`, `${animationStyles.fadeInRight}`);
    useAnimateOnScroll(`.toLeftAnimation`, `${animationStyles.fadeInLeft}`);
  }, []);

  const emailField = useValidation(validateEmail);
  const passwordField = useValidation(validatePassword);

  async function handleSubmit(f: React.FormEvent<HTMLFormElement>) {
    console.log('teste')
    f.preventDefault();

    if (!emailField.isValid || !passwordField.isValid) {
      console.log("Não foi possível realizar a requisição");
      return;
    }

    const form = f.currentTarget;

    const formData = {
      email: (form.elements.namedItem('email') as HTMLInputElement)?.value,
      password: (form.elements.namedItem('password') as HTMLInputElement)?.value,
    };

    console.log(formData)
    const response = await loginUser(formData);

    console.log(response)
  }

  return (
    <section className={formStyles.loginContainer}>
      <section className={leftContentStyles.citationsContainer}>
        <div className={leftContentStyles.citations}>
          <div className={leftContentStyles.citation}>
            <div className={`${leftContentStyles.imgWrapper} toRightAnimation`}>
              <Image
                src={platoIcon}
                className={leftContentStyles.citationImg}
                priority
                alt="Platão"
                width="250"
                height="200"
              />
            </div>
            <p className={`${leftContentStyles.citationTitle} toLeftAnimation`}>
              "A educação é a arma mais poderosa que você pode usar para mudar o mundo." — Platão
            </p>
          </div>

          <div className={leftContentStyles.citation}>
            <p className={`${leftContentStyles.citationTitle} toRightAnimation`}>
              "A esperança é o sonho do homem acordado." — Aristóteles
            </p>
            <div className={`${leftContentStyles.imgWrapper} toLeftAnimation`}>
              <Image
                src={aristotleIcon}
                className={leftContentStyles.citationImg}
                priority
                alt="Aristotle"
                width="250"
                height="200"
              />
            </div>
          </div>
        </div>
      </section>

      <form className={formStyles.loginForm} onSubmit={handleSubmit}>
        <h1 className={formStyles.loginTitle}>Seja Bem-vindo</h1>

        <p className={formStyles.divisionLine}></p>

        <section className={formStyles.inputsSection}>
          <section className={formStyles.inputArea}>
            <p className={formStyles.inputText}>Email</p>
            <input
              className={`${formStyles.inputBox} ${
                emailField.isValid === false
                  ? formStyles.invalidInput
                  : emailField.isValid === true
                  ? formStyles.validInput
                  : ''
              }`}
              required
              placeholder="Email"
              value={emailField.value}
              onChange={emailField.handleChange}
              name="email"
            />
            <p
              className={`${formStyles.inputDescription} ${
                emailField.isValid === false
                  ? formStyles.invalidText
                  : emailField.isValid === true
                  ? formStyles.validText
                  : ''
              }`}
            >
              Digite o seu email cadastrado.
            </p>
          </section>

          <section className={formStyles.inputArea}>
            <p className={formStyles.inputText}>Senha</p>
            <input
              className={`${formStyles.inputBox} ${
                passwordField.isValid === false
                  ? formStyles.invalidInput
                  : passwordField.isValid === true
                  ? formStyles.validInput
                  : ''
              }`}
              required
              type="password"
              value={passwordField.value}
              onChange={passwordField.handleChange}
              placeholder="Senha"
              name="password"
            />
            <p
              className={`${formStyles.inputDescription} ${
                passwordField.isValid === false
                  ? formStyles.invalidText
                  : passwordField.isValid === true
                  ? formStyles.validText
                  : ''
              }`}
            >
              Digite a sua senha.
            </p>
          </section>
        </section>

        <button 
          className={formStyles.loginButton} 
          type="submit"
          disabled={loading}
        >
          {loading ? 'Entrando...' : 'Entrar'}
        </button>
        
        <section className={formStyles.registerRef}>
          <p className={formStyles.registerRefTitle}>Não possui uma conta?</p>
          <a href="/auth/register" className={formStyles.registerRefLink}>
            Registrar-se
          </a>
        </section>
      </form>
    </section>
  );
}
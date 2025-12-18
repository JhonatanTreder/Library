"use client"

import Image from 'next/image';

import React, { useEffect, useState } from 'react';

//Hooks Para validação do formulário
import { useValidation } from './hooks/useValidation';
import { validateUserName, validateEmail, validatePassword, validateMatriculates } from './utils/validators';
//-----------------------------------------------------------------------------------------

import useAnimateOnScroll from '@/app/utils/Animations'
import animationStyles from '@/app/utils/styles/animations.module.css'

import leftContentStyles from '@/app/auth/register/styles/leftContent.module.css'
import platoIcon from '@/app/auth/register/images/platao-icon.webp'
import aristotleIcon from '@/app/auth/register/images/aristoteles-icon.webp'

import formStyles from '@/app/auth/register/styles/registerForm.module.css'

import 'material-icons/iconfont/material-icons.css';
import { useUserRegister } from './hooks/userRegister';

export default function RegisterForm(){

    const { registerUser, loading, error, success } = useUserRegister()

    useEffect(() =>{
        useAnimateOnScroll(`.toRightAnimation`, `${animationStyles.fadeInRight}`)
        useAnimateOnScroll(`.toLeftAnimation`, `${animationStyles.fadeInLeft}`)
    }, []);

    const nameField = useValidation(validateUserName)
    const emailField = useValidation(validateEmail)
    const passwordField = useValidation(validatePassword)
    const matriculatesField = useValidation(validateMatriculates)

    const [confirmPassword, setConfirmPassword] = useState("")
    const [isConfirmPasswordValid, setIsConfirmPasswordValid] = useState<boolean | null>(null)

    useEffect(() => {
        if (confirmPassword === "") {
            setIsConfirmPasswordValid(null);
            return;
        }

        setIsConfirmPasswordValid(confirmPassword === passwordField.value);
    }, [passwordField.value]);

    const validateConfirmPassword = (confirmPassword: string) =>{
        return confirmPassword === passwordField.value
    }

    const handleConfirmPasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.value

        setConfirmPassword(value)

        if (value === ""){
            setIsConfirmPasswordValid(null)
            return
        }

        setIsConfirmPasswordValid(validateConfirmPassword(value))
    }

    async function handleSubmit(f: React.FormEvent<HTMLFormElement>){

        f.preventDefault()
        
        const validInputs = {
            validName: nameField.isValid,
            validEmail: emailField.isValid,
            validPassword: passwordField.isValid,
            validPasswordConfirmation: isConfirmPasswordValid,
            validMatriculates: matriculatesField.isValid
        }

        if (Object.values(validInputs).some(value => value === false)){
            console.log("Não foi possível realizar a requisição")
            return
        }

        const form = f.currentTarget

        const formData = {
            name: (form.elements.namedItem('name') as HTMLInputElement)?.value,
            email: (form.elements.namedItem('email') as HTMLInputElement)?.value,
            password: (form.elements.namedItem('password') as HTMLInputElement)?.value,
            matriculates: (form.elements.namedItem('matriculates') as HTMLInputElement)?.value
        }
        
        await registerUser(formData)
    }

    return(
        <section className={formStyles.registerContainer}>

            <section className={leftContentStyles.citationsContainer}>
                <div className={leftContentStyles.citations}>

                    <div className={leftContentStyles.citation}>

                        <div className={`${leftContentStyles.imgWrapper} toRightAnimation`}>
                            <Image
                                src={platoIcon}
                                className={leftContentStyles.citationImg}
                                priority
                                alt='Platão'
                                width='250'
                                height='200'>                            
                            </Image>
                        </div>

                        <p className={`${leftContentStyles.citationTitle} toLeftAnimation`}>
                            “O livro é um mestre que fala, mas que não responde.” — Platão
                        </p>
                    </div>

                    <div className={leftContentStyles.citation}>

                        <p className={`${leftContentStyles.citationTitle} toRightAnimation`}>
                            “A leitura é o caminho mais curto para o conhecimento.” — Aristóteles
                        </p>

                        <div className={`${leftContentStyles.imgWrapper} toLeftAnimation`}>
                            <Image
                                src={aristotleIcon}
                                className={leftContentStyles.citationImg}
                                priority
                                alt='Aristotle'
                                width='250'
                                height='200'>                            
                            </Image>
                        </div>
                    </div>

                </div>
            </section>
            
            <form
                className={formStyles.registerForm}
                onSubmit={handleSubmit}>

                <h1 className={formStyles.registerTitle}>
                    Registre-se
                </h1>

                <p className={formStyles.divisionLine}></p>

                <section className={formStyles.inputsSection}>
                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Nome</p>
                       <input
                            className={`${formStyles.inputBox} ${
                                nameField.isValid === false
                                ? formStyles.invalidInput
                                : nameField.isValid === true
                                ? formStyles.validInput
                                : ""
                            }`}

                            value={nameField.value}
                            onChange={nameField.handleChange}
                            name="name"
                            placeholder="Nome de usuário"
                            required
                        />

                        <p className={`${formStyles.inputDescription} ${
                                nameField.isValid === false
                                ? formStyles.invalidText
                                : nameField.isValid === true
                                ? formStyles.validText
                                : ""
                            }`}>
                            O nome completo deve conter de 3 a 45 caracteres.
                        </p>
                    </section>

                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Email</p>

                        <input
                            className={`${formStyles.inputBox} ${
                                emailField.isValid === false
                                ? formStyles.invalidInput
                                : emailField.isValid === true
                                ? formStyles.validInput
                                : ""
                            }`}

                            required placeholder='Email'
                            value={emailField.value}
                            onChange={emailField.handleChange}
                            name='email'>
                        </input>

                        <p className={`${formStyles.inputDescription} ${
                                emailField.isValid === false
                                ? formStyles.invalidText
                                : emailField.isValid === true
                                ? formStyles.validText
                                : ""
                            }`}>
                            O Email deve estar em um formato válido.
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
                                : ""
                            }`}

                            required type='password'
                            value={passwordField.value}
                            onChange={passwordField.handleChange}
                            placeholder='Senha'
                            name='password'>
                        </input>

                       <p className={`${formStyles.inputDescription} ${
                                passwordField.isValid === false
                                ? formStyles.invalidText
                                : passwordField.isValid === true
                                ? formStyles.validText
                                : ""
                            }`}>
                            A senha deve conter pelo menos 6 caracteres.
                            Tendo pelo menos uma letra maiúscula, um caractere
                            especial e um número.
                        </p>
                    </section>

                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Confirmar Senha</p>
                        <input
                            className={`${formStyles.inputBox} ${
                                isConfirmPasswordValid === false
                                ? formStyles.invalidInput
                                : isConfirmPasswordValid === true
                                ? formStyles.validInput
                                : ""
                            }`}

                            required
                            type='password'
                            placeholder='Confirmar senha'
                            value={confirmPassword}
                            onChange={handleConfirmPasswordChange}
                            name='confirm-password'>
                        </input>
                        
                        <p className={`${formStyles.inputDescription} ${

                                isConfirmPasswordValid === false
                                ? formStyles.invalidText
                                : isConfirmPasswordValid === true
                                ? formStyles.validText
                                : ""
                            }`}>
                            A sua senha deve ser igual à anterior.
                        </p>

                    </section>

                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Número de Matrícula</p>
                        <input
                            className={`${formStyles.inputBox} ${
                                matriculatesField.isValid === false
                                ? formStyles.invalidInput
                                : matriculatesField.isValid === true
                                ? formStyles.validInput
                                : ""
                            }`}

                            required type='text' 
                            placeholder='Matrícula'
                            value={matriculatesField.value}
                            onChange={matriculatesField.handleChange}
                            name='matriculates'>
                        </input>

                         <p className={`${formStyles.inputDescription} ${
                                matriculatesField.isValid === false
                                ? formStyles.invalidText
                                : matriculatesField.isValid === true
                                ? formStyles.validText
                                : ""
                            }`}>
                           O número da matrícula deve conter ao todo 15 caracteres.
                        </p>
                    </section>
                </section>

                <button className={formStyles.registerButton}> Criar conta </button>

                <section className={formStyles.loginRef}>
                    <p className={formStyles.loginRefTitle}>Já possui uma conta?</p>
                    <a href='/auth/login' className={formStyles.loginRefLink}>Entrar</a>
                </section>
            </form>
        </section>
    );
}

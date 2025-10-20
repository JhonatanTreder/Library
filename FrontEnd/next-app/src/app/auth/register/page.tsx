"use client"

import Image from 'next/image';

import React, { useEffect } from 'react';
import useAnimateOnScroll from '@/app/utils/Animations'
import animationStyles from '@/app/utils/styles/animations.module.css'

import leftContentStyles from '@/app/auth/register/styles/leftContent.module.css'
import platoIcon from '@/app/auth/register/images/platao-icon.webp'
import aristotleIcon from '@/app/auth/register/images/aristoteles-icon.webp'

import formStyles from '@/app/auth/register/styles/registerForm.module.css'

import 'material-icons/iconfont/material-icons.css';
import { userRegister } from './hooks/userRegister';

export default function RegisterForm(){

    const { registerUser, loading, error, success } = userRegister()

    useEffect(() =>{
        useAnimateOnScroll(`.toRightAnimation`, `${animationStyles.fadeInRight}`)
        useAnimateOnScroll(`.toLeftAnimation`, `${animationStyles.fadeInLeft}`)
    }, []);

    async function handleSubmit(f: React.FormEvent<HTMLFormElement>){
        f.preventDefault()

        const form = f.currentTarget

        const formData = {
            name: (form.elements.namedItem('name') as HTMLInputElement)?.value,
            email: (form.elements.namedItem('email') as HTMLInputElement)?.value,
            password: (form.elements.namedItem('password') as HTMLInputElement)?.value,
            matriculates: (form.elements.namedItem('matriculates') as HTMLInputElement)?.value
        }

        //"Conflict State" - Console Log 
        const response = await registerUser(formData)
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
                            “A leitura é o caminho mais curto para o conhecimento”. Aristóteles — 
                        </p>

                        <div className={`${leftContentStyles.imgWrapper} toLeftAnimation`}>
                            <Image
                                src={aristotleIcon}
                                className={leftContentStyles.citationImg}
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
                            className={formStyles.inputBox} required placeholder='Nome de usuário'
                            name='name'>
                        </input>
                    </section>

                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Email</p>
                        <input 
                            className={formStyles.inputBox} required placeholder='Email'
                            name='email'>
                        </input>
                    </section>

                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Senha</p>
                        <input 
                            className={formStyles.inputBox} 
                            required type='password' 
                            placeholder='Senha'
                            name='password'>
                        </input>
                    </section>

                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Confirmar Senha</p>
                        <input 
                            className={formStyles.inputBox} 
                            required type='password' 
                            placeholder='Confirmar senha'
                            name='confirm-password'>
                        </input>
                    </section>

                    <section className={formStyles.inputArea}>
                        <p className={formStyles.inputText}>Número de Matrícula</p>
                        <input 
                            className={formStyles.inputBox} 
                            required type='text' 
                            placeholder='Matrícula'
                            name='matriculates'>
                        </input>
                    </section>
                </section>

                <button className={formStyles.registerButton}> Criar conta </button>

                <section className={formStyles.loginRef}>
                    <p className={formStyles.loginRefTitle}>Já possui uma conta?</p>
                    <a className={formStyles.loginRefLink}>Entrar</a>
                </section>
            </form>
        </section>
    );
}

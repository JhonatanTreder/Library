import Image from 'next/image';

import leftContentStyles from '@/app/auth/register/styles/leftContent.module.css'
import platoIcon from '@/app/auth/register/images/platao-icon.webp'
import aristotleIcon from '@/app/auth/register/images/aristoteles-icon.webp'

import formStyles from '@/app/auth/register/styles/registerForm.module.css'
import googleIcon from '@/app/auth/register/images/google-icon-logo.svg'
import facebookIcon from '@/app/auth/register/images/facebook-icon-logo.svg'
import microsoftIcon from '@/app/auth/register/images/microsoft-icon-logo.svg'
import 'material-icons/iconfont/material-icons.css';

export default function RegisterForm(){
    return(
        <section className={formStyles.registerContainer}>

            <section className={leftContentStyles.citationsContainer}>
                <div className={leftContentStyles.citations}>

                    <div className={leftContentStyles.citation}>
                        <Image
                            src={platoIcon}
                            className={leftContentStyles.citationImg}
                            alt='Platão'
                            width='250'
                            height='200'>                            
                        </Image>

                        <p className={leftContentStyles.citationTitle}>
                            “O livro é um mestre que fala, mas que não responde.” — Platão
                        </p>
                    </div>

                    <div className={leftContentStyles.citation}>

                        <p className={leftContentStyles.citationTitle}>
                            “A leitura é o caminho mais curto para o conhecimento”. — Aristóteles
                        </p>

                        <Image
                            src={aristotleIcon}
                            className={leftContentStyles.citationImg}
                            alt='Platão'    
                            width='250'
                            height='200'>                              
                        </Image>
                    </div>

                </div>
            </section>
            
            <form className={formStyles.registerForm}>
                <h1 className={formStyles.registerTitle}>
                    Bem-Vindo!!
                </h1>

                <p className={formStyles.registerSubTitle}>
                    Por favor, realize o registro logo abaixo.
                </p>

                <p className={formStyles.divisionLine}></p>

                <section className={formStyles.inputArea}>
                    <p className={formStyles.inputText}>Nome</p>
                    <input className={formStyles.inputBox} required placeholder='Nome de usuário'></input>
                </section>

                <section className={formStyles.inputArea}>
                    <p className={formStyles.inputText}>Email</p>
                    <input className={formStyles.inputBox} required placeholder='Email'></input>
                </section>

                <section className={formStyles.inputArea}>
                    <p className={formStyles.inputText}>Senha</p>
                    <input className={formStyles.inputBox} required type='password' placeholder='Senha'></input>
                </section>

                <section className={formStyles.inputArea}>
                    <p className={formStyles.inputText}>Confirmar Senha</p>
                    <input className={formStyles.inputBox} required type='password' placeholder='Confirmar senha'></input>
                </section>

                <button className={formStyles.registerButton}>Criar conta</button>

                <p className={formStyles.authTypesText}>Ou, se registrar com:</p>

                <section className={formStyles.authTypes}>

                    <Image 
                        className={`material-icons ${formStyles.authSvgIcon}`}
                        alt='Google Icon'
                        src={googleIcon}
                        width='24'
                        height='24'>
                    </Image>

                    <Image 
                        className={`material-icons ${formStyles.authSvgIcon}`}
                        alt='Facebook Icon'
                        src={facebookIcon}
                        width='24'
                        height='24'>
                    </Image>

                    <Image 
                        className={`material-icons ${formStyles.authSvgIcon}`}
                        alt='Microsoft Icon'
                        src={microsoftIcon}
                        width='24'
                        height='24'>
                    </Image>

                </section>

                <section className={formStyles.loginRef}>
                    <p className={formStyles.loginRefTitle}>Já possui uma conta?</p>
                    <a className={formStyles.loginRefLink}>Entrar</a>
                </section>
            </form>
        </section>
    );
}

import Image from 'next/image';
import styles from '@/app/auth/register/styles/register.module.css'
import googleIcon from '@/app/auth/register/images/google-icon-logo.svg'
import facebookIcon from '@/app/auth/register/images/facebook-icon-logo.svg'
import microsoftIcon from '@/app/auth/register/images/microsoft-icon-logo.svg'
import 'material-icons/iconfont/material-icons.css';

export default function Register(){
    return(
        <section className={styles.registerContainer}>
            <form className={styles.registerForm}>

                <h1 className={styles.registerTitle}>
                    Bem-Vindo!!
                </h1>

                <p className={styles.registerSubTitle}>
                    Por favor, realize o registro logo abaixo.
                </p>

                <p className={styles.divisionLine}></p>

                <section className={styles.inputArea}>
                    <p className={styles.inputText}>Nome</p>
                    <input className={styles.inputBox} required placeholder='Nome de usuário'></input>
                </section>

                <section className={styles.inputArea}>
                    <p className={styles.inputText}>Email</p>
                    <input className={styles.inputBox} required placeholder='Email'></input>
                </section>

                <section className={styles.inputArea}>
                    <p className={styles.inputText}>Senha</p>
                    <input className={styles.inputBox} required type='password' placeholder='Senha'></input>
                </section>

                <section className={styles.inputArea}>
                    <p className={styles.inputText}>Confirmar Senha</p>
                    <input className={styles.inputBox} required type='password' placeholder='Confirmar senha'></input>
                </section>

                <button className={styles.registerButton}>Registrar</button>

                <p className={styles.authTypesText}>Ou, se registrar com:</p>

                <section className={styles.authTypes}>

                    <Image 
                        className={`material-icons ${styles.authSvgIcon}`}
                        alt='Google Icon'
                        src={googleIcon}
                        width='24'
                        height='24'>
                    </Image>

                    <Image 
                        className={`material-icons ${styles.authSvgIcon}`}
                        alt='Facebook Icon'
                        src={facebookIcon}
                        width='24'
                        height='24'>
                    </Image>

                    <Image 
                        className={`material-icons ${styles.authSvgIcon}`}
                        alt='Microsoft Icon'
                        src={microsoftIcon}
                        width='24'
                        height='24'>
                    </Image>

                </section>

                <section className={styles.loginRef}>
                    <p className={styles.loginRefTitle}>Já possui uma conta?</p>
                    <a className={styles.loginRefLink}>Entrar</a>
                </section>
            </form>
        </section>
    );
}
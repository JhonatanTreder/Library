"use client"

import { useState, useRef, useEffect, KeyboardEvent, ClipboardEvent, ChangeEvent } from 'react';
import validationStyles from '@/app/auth/validation/styles/validationScreen.module.css';
import { useRouter } from 'next/navigation';

export default function ShowValidationScreen() {

    const userEmail = sessionStorage.getItem('user-email')

    const [code, setCode] = useState<string[]>(Array(6).fill(''));
    const inputRefs = useRef<(HTMLInputElement | null)[]>([]);

    const router = useRouter()

    useEffect(() => {
        if (inputRefs.current[0]) {
            inputRefs.current[0].focus();
        }
    }, []);

    const handleChange = (index: number, value: string) => {
        if (!/^\d*$/.test(value)) return;

        const newCode = [...code];
        newCode[index] = value;
        setCode(newCode);

        if (value !== '' && index < 5) {
            inputRefs.current[index + 1]?.focus();
        }
    };

    const handleKeyDown = (index: number, e: KeyboardEvent<HTMLInputElement>) => {
        if (e.key === 'Backspace' && code[index] === '' && index > 0) {
            inputRefs.current[index - 1]?.focus();
        }

        if (e.key === 'ArrowLeft' && index > 0) {
            e.preventDefault();
            inputRefs.current[index - 1]?.focus();
        }
        if (e.key === 'ArrowRight' && index < 5) {
            e.preventDefault();
            inputRefs.current[index + 1]?.focus();
        }
    };

    const handlePaste = (e: ClipboardEvent<HTMLInputElement>) => {
        e.preventDefault();
        const pastedData = e.clipboardData.getData('text');
        
        if (/^\d+$/.test(pastedData)) {
            const digits = pastedData.split('').slice(0, 6);
            const newCode = [...code];
            
            digits.forEach((digit, index) => {
                if (index < 6) {
                    newCode[index] = digit;
                }
            });
            
            setCode(newCode);
            
            const nextIndex = Math.min(digits.length, 5);
            inputRefs.current[nextIndex]?.focus();
        }
    };

    const handleFocus = (index: number) => {
        inputRefs.current[index]?.select();
    };

    const isCodeComplete = code.every(digit => digit !== '');
    
    const handleVerifyCode = async () => {
        if (isCodeComplete) {
            const verificationCode = code.join('');
            console.log('Código para verificar:', verificationCode);

            const verifyEmailCodeDTO = {
                email: sessionStorage.getItem('user-email') ?? '',
                emailCode: verificationCode
            }

            const verifyEmailCodeURL = `https://localhost:7221/Auth/verify-email`

            const verifyEmailCodeRequest = await fetch(verifyEmailCodeURL, {
                method: 'POST',
                headers:{
                    'Content-Type' : 'application/json'
                },
                body: JSON.stringify(verifyEmailCodeDTO)
            })

            const verifyEmailResponse = await verifyEmailCodeRequest.json()
            const responseStatus = verifyEmailResponse['status']
            const responseMessage = verifyEmailResponse['message']

            if (responseStatus !== 'Ok'){
                console.log('Error:' + responseStatus)
            }

            else {
                console.log('Success:' + responseStatus)
                sessionStorage.removeItem('user-email')

                //Logar o usuário ao invés de simplesmente redirecionar ele para outra tela
                router.push('/pages/home')
            }

            console.log('Message: ' + responseMessage)
        }
    };

    const handleResendCode = () => {
        console.log('Reenviando código...');
        setCode(Array(6).fill(''));
        inputRefs.current[0]?.focus();
    };

    return (
        <section className={validationStyles.validationContainer}>

            <div className={validationStyles.validationBox}>  

                <section className={validationStyles.validationSection}>

                    <h1 className={validationStyles.validationBoxTitle}>Verificação de Código</h1>

                    <div className={validationStyles.validationDescriptionSection}>

                        <p className={validationStyles.validationBoxDescription}>

                            Coloque abaixo o código de verificação que enviamos para o email:&nbsp;

                            "<strong className={validationStyles.userEmailReference}>
                                {userEmail}
                            </strong>"
                        </p>

                    </div>

                    <section className={validationStyles.codeParts}>
                        {code.map((digit, index) => (
                            <input
                                key={index}
                                ref={(el) => {
                                    inputRefs.current[index] = el;
                                }}
                                className={validationStyles.codePart}
                                type="text"
                                inputMode="numeric"
                                pattern="[0-9]*"
                                maxLength={1}
                                value={digit}
                                onChange={(e: ChangeEvent<HTMLInputElement>) => 
                                    handleChange(index, e.target.value)
                                }
                                onKeyDown={(e: KeyboardEvent<HTMLInputElement>) => 
                                    handleKeyDown(index, e)
                                }
                                onPaste={handlePaste}
                                onFocus={() => handleFocus(index)}
                                autoComplete="one-time-code">

                            </input>
                        ))}
                    </section>

                    <section className={validationStyles.resendCodeSection}>

                        <p className={validationStyles.resendCodeDescription}>
                            Não recebeu o seu código?
                        </p>

                        <a 
                            className={validationStyles.resendCodeLink} 
                            onClick={handleResendCode}
                            style={{ cursor: 'pointer' }}>
                            Reenviar
                        </a>
                    </section>

                    <div className={validationStyles.divisionLine}></div>

                    <button 
                        className={validationStyles.verifyCodeButton}
                        onClick={handleVerifyCode}
                        disabled={!isCodeComplete}>
                            
                        Verificar Código
                    </button>
                </section>
            </div>
        </section>
    );
}
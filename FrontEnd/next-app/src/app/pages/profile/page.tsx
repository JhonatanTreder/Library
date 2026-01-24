"use client"

import profileStyles from '@/app/pages/profile/profile.module.css'

import ShowNavbar from "@/app/components/Navbar"
import { useRouter } from "next/navigation"
import { useEffect, useState } from "react"
import { useProfile } from '@/app/components/profile/hooks/useProfile'

import PasswordIcon from '@mui/icons-material/Password'
import EmailIcon from '@mui/icons-material/Email'
import UsernameIcon from '@mui/icons-material/Assignment'
import MatriculatesIcon from '@mui/icons-material/Badge'
import PhoneIcon from '@mui/icons-material/Phone'

export default function Profile() {
    const router = useRouter()
    const {
        dashboardData,
        profileData,
        editingField,
        showEmailModal,
        showPasswordModal,
        hasChanges,
        isLoading,
        error,
        handleEdit,
        handleSave,
        handleCancel,
        handleOpenEmailModal,
        handleCloseEmailModal,
        handleOpenPasswordModal,
        handleClosePasswordModal,
        handleSaveAllChanges,
        handleDiscardChanges,
        handleRequestEmailChange,
        handleConfirmEmailChange,
        handleRequestPasswordChange,
        handleConfirmPasswordChange,
        inputRef
    } = useProfile()

    const [tempValue, setTempValue] = useState("")

    // Estados do modal de email
    const [emailCurrentPassword, setEmailCurrentPassword] = useState("")
    const [newEmail, setNewEmail] = useState("")
    const [emailConfirmationCode, setEmailConfirmationCode] = useState("")
    const [showEmailConfirmationStep, setShowEmailConfirmationStep] = useState(false)
    const [emailModalError, setEmailModalError] = useState<string | null>(null)
    const [emailModalSuccess, setEmailModalSuccess] = useState<string | null>(null)

    // Estados do modal de senha
    const [passwordCurrentPassword, setPasswordCurrentPassword] = useState("")
    const [newPassword, setNewPassword] = useState("")
    const [confirmNewPassword, setConfirmNewPassword] = useState("")
    const [passwordConfirmationCode, setPasswordConfirmationCode] = useState("")
    const [showPasswordConfirmationStep, setShowPasswordConfirmationStep] = useState(false)
    const [passwordModalError, setPasswordModalError] = useState<string | null>(null)
    const [passwordModalSuccess, setPasswordModalSuccess] = useState<string | null>(null)

    useEffect(() => {
        const token = localStorage.getItem('token')

        if (token === null || token === undefined) {
            localStorage.removeItem('token')
            localStorage.removeItem('refresh-token')
            localStorage.removeItem('token-expiration-time')

            router.push('/auth/login')
        }
    }, [router])

    const handleEditClick = (fieldName: keyof typeof profileData) => {
        setTempValue(profileData[fieldName])
        handleEdit(fieldName)
    }

    const handleSaveClick = (fieldName: keyof typeof profileData) => {
        handleSave(fieldName, tempValue)
    }

    const handleCancelClick = () => {
        setTempValue("")
        handleCancel()
    }

    const handleKeyDown = (e: React.KeyboardEvent, fieldName: keyof typeof profileData) => {
        if (e.key === 'Enter') {
            handleSaveClick(fieldName)
        } else if (e.key === 'Escape') {
            handleCancelClick()
        }
    }

    const handleCloseEmailModalAndReset = () => {
        handleCloseEmailModal()
        setEmailCurrentPassword("")
        setNewEmail("")
        setEmailConfirmationCode("")
        setShowEmailConfirmationStep(false)
        setEmailModalError(null)
        setEmailModalSuccess(null)
    }

    const handleRequestEmailChangeClick = async () => {
        setEmailModalError(null)
        setEmailModalSuccess(null)

        if (!emailCurrentPassword.trim()) {
            setEmailModalError('A senha atual não pode estar vazia')
            return
        }

        if (!newEmail.trim()) {
            setEmailModalError('O novo email não pode estar vazio')
            return
        }

        if (!newEmail.includes('@') || !newEmail.includes('.')) {
            setEmailModalError('Email inválido! Use o formato: exemplo@dominio.com')
            return
        }

        if (newEmail === profileData.email) {
            setEmailModalError('O novo email não pode ser igual ao atual')
            return
        }

        const result = await handleRequestEmailChange(emailCurrentPassword, newEmail)

        if (result.success) {
            setEmailModalSuccess(result.message)
            setShowEmailConfirmationStep(true)
            setEmailCurrentPassword("")
        } else {
            setEmailModalError(result.message)
        }
    }

    const handleConfirmEmailChangeClick = async () => {
        setEmailModalError(null)
        setEmailModalSuccess(null)

        if (!emailConfirmationCode.trim()) {
            setEmailModalError('O código de confirmação não pode estar vazio')
            return
        }

        if (emailConfirmationCode.length !== 6) {
            setEmailModalError('O código deve ter 6 dígitos')
            return
        }

        const result = await handleConfirmEmailChange(emailConfirmationCode)

        if (result.success) {
            setEmailModalSuccess(result.message)

            setTimeout(() => {
                handleCloseEmailModalAndReset()
            }, 2000)
        } else {
            setEmailModalError(result.message)
        }
    }

    const handleClosePasswordModalAndReset = () => {
        handleClosePasswordModal()
        setPasswordCurrentPassword("")
        setNewPassword("")
        setConfirmNewPassword("")
        setPasswordConfirmationCode("")
        setShowPasswordConfirmationStep(false)
        setPasswordModalError(null)
        setPasswordModalSuccess(null)
    }

    const handleRequestPasswordChangeClick = async () => {
        setPasswordModalError(null)
        setPasswordModalSuccess(null)

        if (!passwordCurrentPassword.trim()) {
            setPasswordModalError('A senha atual não pode estar vazia')
            return
        }

        if (!newPassword.trim()) {
            setPasswordModalError('A nova senha não pode estar vazia')
            return
        }

        if (newPassword.length < 6) {
            setPasswordModalError('A nova senha deve ter pelo menos 6 caracteres')
            return
        }

        if (newPassword !== confirmNewPassword) {
            setPasswordModalError('As senhas não coincidem')
            return
        }

        if (newPassword === passwordCurrentPassword) {
            setPasswordModalError('A nova senha não pode ser igual à senha atual')
            return
        }

        const result = await handleRequestPasswordChange(passwordCurrentPassword, newPassword)

        if (result.success) {
            setPasswordModalSuccess(result.message)
            setShowPasswordConfirmationStep(true)
            setPasswordCurrentPassword("")
            setNewPassword("")
            setConfirmNewPassword("")
        }

        else {
            setPasswordModalError(result.message)
        }
    }

    const handleConfirmPasswordChangeClick = async () => {
        setPasswordModalError(null)
        setPasswordModalSuccess(null)

        if (!passwordConfirmationCode.trim()) {
            setPasswordModalError('O código de confirmação não pode estar vazio')
            return
        }

        if (passwordConfirmationCode.length !== 6) {
            setPasswordModalError('O código deve ter 6 dígitos')
            return
        }

        const result = await handleConfirmPasswordChange(passwordConfirmationCode)

        if (result.success) {
            setPasswordModalSuccess(result.message)

        }
        
        else {
            setPasswordModalError(result.message)
        }
    }

    const renderField = (fieldName: keyof typeof profileData) => {
        const isEditing = editingField === fieldName
        const value = profileData[fieldName]

        return (
            <div className={profileStyles.editBox}>
                {isEditing ? (
                    <input
                        ref={inputRef}
                        type={fieldName === 'senha' ? 'password' : 'text'}
                        className={profileStyles.inputBox}
                        value={tempValue}
                        onChange={(e) => setTempValue(e.target.value)}
                        onKeyDown={(e) => handleKeyDown(e, fieldName)}
                        disabled={isLoading}
                    />
                ) : (
                    <p className={profileStyles.inputBox}>{value}</p>
                )}
                {isEditing ? (
                    <div style={{ display: 'flex', gap: '0.5rem' }}>
                        <button
                            className={profileStyles.editButton}
                            onClick={() => handleSaveClick(fieldName)}
                            disabled={isLoading}
                        >
                            Salvar
                        </button>
                        <button
                            className={profileStyles.editButton}
                            onClick={handleCancelClick}
                            style={{ background: '#6B7280' }}
                            disabled={isLoading}
                        >
                            Cancelar
                        </button>
                    </div>
                ) : (
                    <button
                        className={profileStyles.editButton}
                        onClick={() => handleEditClick(fieldName)}
                        disabled={isLoading}
                    >
                        Editar
                    </button>
                )}
            </div>
        )
    }

    return (
        <section className={profileStyles.profileContainer}>
            <ShowNavbar></ShowNavbar>
            <section className={profileStyles.profileSection}>
                <div className={profileStyles.profileCard}>
                    {isLoading && (
                        <div className={profileStyles.loadingOverlay}>
                            <div className={profileStyles.spinner}></div>
                            <p>Carregando...</p>
                        </div>
                    )}

                    {error && (
                        <div className={profileStyles.errorBanner}>
                            <p>{error}</p>
                        </div>
                    )}

                    <div className={profileStyles.editContainer}>
                        <div className={profileStyles.editItem}>
                            <div className={profileStyles.editHeader}>
                                <p className={profileStyles.editTitle}>
                                    Nome Completo
                                </p>
                                <UsernameIcon></UsernameIcon>
                            </div>
                            {renderField('nomeCompleto')}
                        </div>

                        <div className={profileStyles.editItem}>
                            <div className={profileStyles.editHeader}>
                                <p className={profileStyles.editTitle}>
                                    Matrícula
                                </p>
                                <MatriculatesIcon></MatriculatesIcon>
                            </div>
                            {renderField('matricula')}
                        </div>

                        <div className={profileStyles.editItem}>
                            <div className={profileStyles.editHeader}>
                                <p className={profileStyles.editTitle}>
                                    Numero de Telefone
                                </p>
                                <PhoneIcon></PhoneIcon>
                            </div>
                            {renderField('telefone')}
                        </div>

                        <div className={profileStyles.editItem}>
                            <div className={profileStyles.editHeader}>
                                <p className={profileStyles.editTitle}>
                                    E-mail
                                </p>
                                <EmailIcon></EmailIcon>
                            </div>
                            <div className={profileStyles.editBox}>
                                <p className={profileStyles.inputBox}>{profileData.email}</p>
                                <button
                                    className={profileStyles.editButton}
                                    onClick={handleOpenEmailModal}
                                    disabled={isLoading}
                                >
                                    Editar
                                </button>
                            </div>
                        </div>

                        <div className={profileStyles.editItem}>
                            <div className={profileStyles.editHeader}>
                                <p className={profileStyles.editTitle}>
                                    Senha
                                </p>
                                <PasswordIcon></PasswordIcon>
                            </div>
                            <div className={profileStyles.editBox}>
                                <p className={profileStyles.inputBox}>{profileData.senha}</p>
                                <button
                                    className={profileStyles.editButton}
                                    onClick={handleOpenPasswordModal}
                                    disabled={isLoading}
                                >
                                    Editar
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <div className={profileStyles.dashboardSection}>
                    <h2 className={profileStyles.dashboardTitle}>Seu Dashboard</h2>
                    <div className={profileStyles.dashboardItems}>
                        <div className={profileStyles.dashboardCard}>
                            <p className={profileStyles.dashboardCardTitle}>Total de Empréstimos Realizados</p>

                            <p className={profileStyles.dashboardCardValue}>
                                {dashboardData.totalLoans}
                            </p>
                        </div>

                        <div className={profileStyles.dashboardCard}>
                            <p className={profileStyles.dashboardCardTitle}>Total de Eventos Realizados</p>

                            <p className={profileStyles.dashboardCardValue}>
                                {dashboardData.eventsHeld.length}
                            </p>
                        </div>

                        <div className={profileStyles.dashboardCard}>
                            <p className={profileStyles.dashboardCardTitle}>Livros Favoritados</p>

                            <p className={profileStyles.dashboardCardValue}>
                                {dashboardData.favoriteBooks.length}
                            </p>
                        </div>

                        <div className={profileStyles.dashboardCard}>
                            <p className={profileStyles.dashboardCardTitle}>Total de Multas</p>

                            <p className={profileStyles.dashboardCardValue}>
                                {dashboardData.totalFines}
                            </p>
                        </div>

                        <div className={profileStyles.dashboardCard}>
                            <p className={profileStyles.dashboardCardTitle}>Data de Entrada</p>

                            <p className={profileStyles.dashboardCardValue}>
                                {dashboardData.entryDate?.toString()}
                            </p>
                        </div>
                    </div>
                </div>

                {hasChanges && !isLoading && (
                    <div className={profileStyles.saveBar}>
                        <div className={profileStyles.saveBarContent}>
                            <p className={profileStyles.saveBarText}>
                                Você tem alterações não salvas!
                            </p>
                            <div className={profileStyles.saveBarButtons}>
                                <button
                                    className={profileStyles.discardButton}
                                    onClick={handleDiscardChanges}
                                    disabled={isLoading}
                                >
                                    Descartar
                                </button>
                                <button
                                    className={profileStyles.saveButton}
                                    onClick={handleSaveAllChanges}
                                    disabled={isLoading}
                                >
                                    {isLoading ? 'Salvando...' : 'Salvar Alterações'}
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </section>

            {/* EMAIL MODAL */}
            {showEmailModal && (
                <div className={profileStyles.modalOverlay} onClick={handleCloseEmailModalAndReset}>
                    <div className={profileStyles.modalContent} onClick={(e) => e.stopPropagation()}>
                        <div className={profileStyles.modalHeader}>
                            <h2>Alterar E-mail</h2>
                            <button
                                className={profileStyles.closeButton}
                                onClick={handleCloseEmailModalAndReset}
                                disabled={isLoading}
                            >
                                ✕
                            </button>
                        </div>

                        <div className={profileStyles.modalBody}>
                            {emailModalError && (
                                <div className={profileStyles.modalError}>
                                    {emailModalError}
                                </div>
                            )}

                            {emailModalSuccess && (
                                <div className={profileStyles.modalSuccess}>
                                    {emailModalSuccess}
                                </div>
                            )}

                            {!showEmailConfirmationStep && (
                                <>
                                    <label className={profileStyles.modalLabel}>
                                        E-mail atual:
                                    </label>
                                    <p className={profileStyles.currentValue}>{profileData.email}</p>

                                    <label className={profileStyles.modalLabel} style={{ marginTop: '1.5rem' }}>
                                        Senha atual:
                                    </label>
                                    <input
                                        type="password"
                                        className={profileStyles.modalInput}
                                        placeholder="Digite sua senha atual"
                                        value={emailCurrentPassword}
                                        onChange={(e) => {
                                            setEmailCurrentPassword(e.target.value)
                                            setEmailModalError(null)
                                        }}
                                        disabled={isLoading}
                                        autoFocus
                                    />

                                    <label className={profileStyles.modalLabel} style={{ marginTop: '1.5rem' }}>
                                        Novo e-mail:
                                    </label>
                                    <input
                                        type="email"
                                        className={profileStyles.modalInput}
                                        placeholder="Digite o novo e-mail"
                                        value={newEmail}
                                        onChange={(e) => {
                                            setNewEmail(e.target.value)
                                            setEmailModalError(null)
                                        }}
                                        disabled={isLoading}
                                    />

                                    <p className={profileStyles.passwordHint}>
                                        Um código de confirmação será enviado para o novo e-mail
                                    </p>
                                </>
                            )}

                            {showEmailConfirmationStep && (
                                <>
                                    <label className={profileStyles.modalLabel}>
                                        Código de confirmação:
                                    </label>
                                    <input
                                        type="text"
                                        className={profileStyles.modalInput}
                                        placeholder="Digite o código de 6 dígitos"
                                        value={emailConfirmationCode}
                                        onChange={(e) => {
                                            const value = e.target.value.replace(/\D/g, '').slice(0, 6)
                                            setEmailConfirmationCode(value)
                                            setEmailModalError(null)
                                        }}
                                        disabled={isLoading}
                                        maxLength={6}
                                        autoFocus
                                    />

                                    <p className={profileStyles.passwordHint}>
                                        Digite o código de 6 dígitos enviado para: <strong>{newEmail}</strong>
                                    </p>
                                    <p className={profileStyles.passwordHint}>
                                        O código expira em 5 minutos
                                    </p>
                                </>
                            )}
                        </div>

                        <div className={profileStyles.modalFooter}>
                            <button
                                className={profileStyles.modalButtonCancel}
                                onClick={handleCloseEmailModalAndReset}
                                disabled={isLoading}
                            >
                                Cancelar
                            </button>

                            {!showEmailConfirmationStep ? (
                                <button
                                    className={profileStyles.modalButtonSave}
                                    onClick={handleRequestEmailChangeClick}
                                    disabled={isLoading}
                                >
                                    {isLoading ? 'Enviando...' : 'Trocar Email'}
                                </button>
                            ) : (
                                <button
                                    className={profileStyles.modalButtonSave}
                                    onClick={handleConfirmEmailChangeClick}
                                    disabled={isLoading}
                                >
                                    {isLoading ? 'Confirmando...' : 'Confirmar'}
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            )}

            {/* PASSWORD MODAL */}
            {showPasswordModal && (
                <div className={profileStyles.modalOverlay} onClick={handleClosePasswordModalAndReset}>
                    <div className={profileStyles.modalContent} onClick={(e) => e.stopPropagation()}>
                        <div className={profileStyles.modalHeader}>
                            <h2>Alterar Senha</h2>
                            <button
                                className={profileStyles.closeButton}
                                onClick={handleClosePasswordModalAndReset}
                                disabled={isLoading}
                            >
                                ✕
                            </button>
                        </div>

                        <div className={profileStyles.modalBody}>
                            {passwordModalError && (
                                <div className={profileStyles.modalError}>
                                    {passwordModalError}
                                </div>
                            )}

                            {passwordModalSuccess && (
                                <div className={profileStyles.modalSuccess}>
                                    {passwordModalSuccess}
                                </div>
                            )}

                            {!showPasswordConfirmationStep && (
                                <>
                                    <label className={profileStyles.modalLabel}>
                                        Senha atual:
                                    </label>
                                    <input
                                        type="password"
                                        className={profileStyles.modalInput}
                                        placeholder="Digite sua senha atual"
                                        value={passwordCurrentPassword}
                                        onChange={(e) => {
                                            setPasswordCurrentPassword(e.target.value)
                                            setPasswordModalError(null)
                                        }}
                                        disabled={isLoading}
                                        autoFocus
                                    />

                                    <label className={profileStyles.modalLabel} style={{ marginTop: '1.5rem' }}>
                                        Nova senha:
                                    </label>
                                    <input
                                        type="password"
                                        className={profileStyles.modalInput}
                                        placeholder="Digite a nova senha"
                                        value={newPassword}
                                        onChange={(e) => {
                                            setNewPassword(e.target.value)
                                            setPasswordModalError(null)
                                        }}
                                        disabled={isLoading}
                                    />

                                    <label className={profileStyles.modalLabel} style={{ marginTop: '1.5rem' }}>
                                        Confirme a nova senha:
                                    </label>
                                    <input
                                        type="password"
                                        className={profileStyles.modalInput}
                                        placeholder="Confirme a nova senha"
                                        value={confirmNewPassword}
                                        onChange={(e) => {
                                            setConfirmNewPassword(e.target.value)
                                            setPasswordModalError(null)
                                        }}
                                        disabled={isLoading}
                                    />

                                    <p className={profileStyles.passwordHint}>
                                        A senha deve ter pelo menos 6 caracteres
                                    </p>
                                    <p className={profileStyles.passwordHint}>
                                        Um código de confirmação será enviado para seu email
                                    </p>
                                </>
                            )}

                            {showPasswordConfirmationStep && (
                                <>
                                    <label className={profileStyles.modalLabel}>
                                        Código de confirmação:
                                    </label>
                                    <input
                                        type="text"
                                        className={profileStyles.modalInput}
                                        placeholder="Digite o código de 6 dígitos"
                                        value={passwordConfirmationCode}
                                        onChange={(e) => {
                                            const value = e.target.value.replace(/\D/g, '').slice(0, 6)
                                            setPasswordConfirmationCode(value)
                                            setPasswordModalError(null)
                                        }}
                                        disabled={isLoading}
                                        maxLength={6}
                                        autoFocus
                                    />

                                    <p className={profileStyles.passwordHint}>
                                        Digite o código de 6 dígitos enviado para: <strong>{profileData.email}</strong>
                                    </p>
                                    <p className={profileStyles.passwordHint}>
                                        O código expira em 5 minutos
                                    </p>
                                    <p className={profileStyles.passwordHint}>
                                        Após confirmar, você será redirecionado para fazer login novamente
                                    </p>
                                </>
                            )}
                        </div>

                        <div className={profileStyles.modalFooter}>
                            <button
                                className={profileStyles.modalButtonCancel}
                                onClick={handleClosePasswordModalAndReset}
                                disabled={isLoading}
                            >
                                Cancelar
                            </button>

                            {!showPasswordConfirmationStep ? (
                                <button
                                    className={profileStyles.modalButtonSave}
                                    onClick={handleRequestPasswordChangeClick}
                                    disabled={isLoading}
                                >
                                    {isLoading ? 'Enviando...' : 'Trocar Senha'}
                                </button>
                            ) : (
                                <button
                                    className={profileStyles.modalButtonSave}
                                    onClick={handleConfirmPasswordChangeClick}
                                    disabled={isLoading}
                                >
                                    {isLoading ? 'Confirmando...' : 'Confirmar'}
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            )}
        </section>
    )
}
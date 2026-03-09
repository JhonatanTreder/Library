"use client"

import { useRouter } from "next/navigation"
import { useEffect, useState, useRef } from "react"
import { ProfileData, UseProfileProps } from "@/app/interfaces/profile/UseProfileProps"
import { UserDashboardData } from "@/app/interfaces/endpoints/Dashboard"
import { PendingValidations } from "@/app/interfaces/endpoints/PendingValidations"
import { validateEmail, validateMatriculates, validatePhoneNumber, validateUserName } from "@/app/utils/FormatValidator"

// Configuração centralizada da API
const API_CONFIG = {
    baseURL: 'https://localhost:7221',
    endpoints: {
        me: '/User/me',
        user: (id: string) => `/User/${id}`,
        requestUserGeneralInfo: (id: string) => `/User/${id}/dashboard`,
        pendingUserValidations: (id: string) => `/User/${id}/pending-validations`,
        requestEmailChange: '/Auth/request-email-change',
        confirmEmailChange: '/Auth/confirm-email-change',
        cancelEmailChange: '/Auth/cancel-email-change',
        confirmPasswordChange: '/Auth/confirm-password-change',
        requestPasswordChange: '/Auth/request-password-change'
    }
}

export const useProfile = (): UseProfileProps => {
    const [profileData, setProfileData] = useState<ProfileData>({
        nomeCompleto: "",
        matricula: "",
        telefone: "",
        email: "",
        senha: "••••••••"
    })

    const [originalData, setOriginalData] = useState<ProfileData>({
        nomeCompleto: "",
        matricula: "",
        telefone: "",
        email: "",
        senha: "••••••••"
    })

    const [dashboardData, setDashboardData] = useState<UserDashboardData>({
        totalLoans: 0,
        eventsHeld: [],
        favoriteBooks: [],
        entryDate: null,
        totalFines: 0
    })

    const [pendingValidations, setPendingValidations] = useState<PendingValidations>({
        emailIsPending: null,
        phoneNumberIsPending: null
    })

    const [editingField, setEditingField] = useState<string | null>(null)
    const [showEmailModal, setShowEmailModal] = useState(false)
    const [showPasswordModal, setShowPasswordModal] = useState(false)
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)

    const inputRef = useRef<HTMLInputElement>(null)
    const router = useRouter()

    const hasChanges =
        profileData.nomeCompleto !== originalData.nomeCompleto ||
        profileData.matricula !== originalData.matricula ||
        profileData.telefone !== originalData.telefone

    const getAuthToken = (): string | null => {
        const token = localStorage.getItem('token')

        if (!token) {
            clearAuthData()
            router.push('/auth/login')
            return null
        }

        return token
    }

    const clearAuthData = (): void => {
        localStorage.removeItem('token')
        localStorage.removeItem('refresh-token')
        localStorage.removeItem('token-expiration-time')
    }

    const getAuthHeaders = (token: string): HeadersInit => ({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    })

    const fetchUserId = async (token: string): Promise<string | null> => {
        try {
            const response = await fetch(`${API_CONFIG.baseURL}${API_CONFIG.endpoints.me}`, {
                method: 'GET',
                headers: getAuthHeaders(token)
            })

            if (!response.ok) {
                throw new Error(`Erro ao buscar ID do usuário: ${response.status}`)
            }

            const data = await response.json()

            if (!data || !data.data) {
                throw new Error('Resposta da API inválida')
            }

            return data.data
        } catch (error) {
            console.error('Erro em fetchUserId:', error)
            return null
        }
    }

    const fetchUserGeneralInfo = async (userId: string, token: string): Promise<any | null> => {
        try {
            const response = await fetch(`${API_CONFIG.baseURL}${API_CONFIG.endpoints.requestUserGeneralInfo(userId)}`, {
                method: 'GET',
                headers: getAuthHeaders(token)
            })

            if (!response.ok) {
                throw new Error(`Erro ao buscar pelas informações de dashboard do usuário: ${response.status}`)
            }

            const data = await response.json()

            if (!data || !data.data) {
                throw new Error('Resposta da API inválida')
            }

            return data['data']
        }
        catch (error) {
            console.error('Erro em fetchUserGeneralInfo:', error)
            return null
        }
    }

    const fetchUserData = async (userId: string, token: string): Promise<any | null> => {
        try {
            const response = await fetch(
                `${API_CONFIG.baseURL}${API_CONFIG.endpoints.user(userId)}`,
                {
                    method: 'GET',
                    headers: getAuthHeaders(token)
                }
            )

            if (!response.ok) {
                if (response.status === 401 || response.status === 403) {

                    clearAuthData()

                    router.push('/auth/login')

                    return null
                }
                throw new Error(`Erro ao buscar dados do usuário: ${response.status}`)
            }

            const data = await response.json()

            if (!data || !data.data) {
                throw new Error('Dados do usuário não encontrados na resposta')
            }

            return data.data
        } catch (error) {
            console.error('Erro em fetchUserData:', error)
            return null
        }
    }

    const fetchPendingUserValidations = async (userId: string, token: string): Promise<any | null> => {
        try {
            const pendingUserValidationsURL = `${API_CONFIG.baseURL}${API_CONFIG.endpoints.pendingUserValidations(userId)}`

            const pendingValidationsRequest = await fetch(pendingUserValidationsURL, {
                method: 'GET',
                headers: getAuthHeaders(token)
            }
            )

            console.log(pendingValidationsRequest)

            const pendingValidationsData = await pendingValidationsRequest.json()

            return pendingValidationsData['data']
        }

        catch (error) {
            console.log('Erro em fetchPendingUserValidations:', error)
            return null
        }
    }

    const transformApiDataToProfile = (apiData: any): ProfileData => {
        return {
            nomeCompleto: apiData.name || "",
            matricula: apiData.matriculates || "",
            telefone: apiData.phoneNumber || "--",
            email: apiData.email || "",
            senha: "••••••••"
        }
    }

    const transformProfileToApiData = (data: ProfileData) => {
        return {
            name: data.nomeCompleto,
            userMatriculates: data.matricula,
            phoneNumber: data.telefone === "Sem número de telefone" ? "" : data.telefone
        }
    }

    useEffect(() => {
        const loadProfileData = async () => {
            setIsLoading(true)
            setError(null)

            try {
                const token = getAuthToken()
                if (!token) return

                const userId = await fetchUserId(token)
                if (!userId) {
                    setError('Não foi possível identificar o usuário')
                    clearAuthData()
                    router.push('/auth/login')
                    return
                }

                const userData = await fetchUserData(userId, token)
                if (!userData) {
                    setError('Não foi possível carregar os dados do perfil')
                    return
                }

                const transformedData = transformApiDataToProfile(userData)
                setProfileData(transformedData)
                setOriginalData(transformedData)

                const userDashboardData = await fetchUserGeneralInfo(userId, token)

                if (!userDashboardData) {
                    throw new Error('Não foi possível carregar o dashboard do perfil')
                }

                setDashboardData(userDashboardData)

                const userPendingValidations = await fetchPendingUserValidations(userId, token)

                if (!userPendingValidations) {
                    throw new Error('Não foi possível buscar pelas validações pendentes do usuário')
                }

                setPendingValidations(userPendingValidations)

                console.log('Dados do perfil carregados com sucesso: ', transformedData)
                console.log('Dados do dashboard carregados com sucesso: ', userDashboardData)
                console.log('Dados de validações pendentes: ', userPendingValidations)

            }

            catch (error) {
                console.error('Erro ao carregar dados do perfil:', error)

                setError('Erro ao carregar perfil. Tente novamente.')
            }

            finally {
                setIsLoading(false)
            }
        }

        loadProfileData()
    }, [router])

    useEffect(() => {
        if (editingField && inputRef.current) {
            inputRef.current.focus()
        }
    }, [editingField])

    const handleEdit = (fieldName: keyof ProfileData) => {
        setEditingField(fieldName)
    }

    const handleSave = async (fieldName: keyof ProfileData, value: string) => {
        setProfileData(prev => ({
            ...prev,
            [fieldName]: value
        }))
        setEditingField(null)
    }

    const handleCancel = () => {
        setEditingField(null)
    }

    const handleOpenEmailModal = () => {
        setShowEmailModal(true)
    }

    const handleCloseEmailModal = () => {
        setShowEmailModal(false)
    }

    const handleOpenPasswordModal = () => {
        setShowPasswordModal(true)
    }

    const handleClosePasswordModal = () => {
        setShowPasswordModal(false)
    }

    const handleValidateProfileValue = (userProfileData: ProfileData) => {

        const profileUpdateStatus = {
            field: '',
            message: '',
            isValid: true
        }

        if (validateEmail(userProfileData.email) !== true) {
            profileUpdateStatus.field = 'email'
            profileUpdateStatus.message = 'O e-mail está em um formato inválido'
            profileUpdateStatus.isValid = false

            return profileUpdateStatus
        }

        if (validateUserName(userProfileData.nomeCompleto) !== true) {

            profileUpdateStatus.field = 'userName'
            profileUpdateStatus.message = 'O nome do usuário está em um formato inválido'
            profileUpdateStatus.isValid = false

            return profileUpdateStatus
        }

        if (validateMatriculates(userProfileData.matricula) !== true) {

            profileUpdateStatus.field = 'matriculates'
            profileUpdateStatus.message = 'A matricula do usuário está em um formato inválido'
            profileUpdateStatus.isValid = false

            return profileUpdateStatus
        }

        if (validatePhoneNumber(userProfileData.telefone) !== true) {

            profileUpdateStatus.field = 'phoneNumber'
            profileUpdateStatus.message = 'O número de telefone do usuário está em um formato inválido'
            profileUpdateStatus.isValid = false

            return profileUpdateStatus
        }


        profileUpdateStatus.message = 'Usuário atualizado com sucesso'
        profileUpdateStatus.isValid = true

        return profileUpdateStatus
    }

    const handleSaveAllChanges = async () => {
        setIsLoading(true)

        const profileValueValidation = handleValidateProfileValue(profileData)

        if (profileValueValidation.field === 'phoneNumber' && profileValueValidation.isValid) {
            pendingValidations.phoneNumberIsPending = false
        }

        console.log('------------------------------------')
        console.log('Validação dos novos dados')
        console.log(profileValueValidation.message)
        console.log(profileValueValidation.isValid)
        console.log('------------------------------------')
        
        setError(null)

        try {

            const token = getAuthToken()

            if (!token) return

            const userId = await fetchUserId(token)
            if (!userId) {
                setError('Não foi possível identificar o usuário')
                return
            }

            const updatedData = transformProfileToApiData(profileData)

            const response = await fetch(
                `${API_CONFIG.baseURL}${API_CONFIG.endpoints.user(userId)}`,
                {
                    method: 'PATCH',
                    headers: getAuthHeaders(token),
                    body: JSON.stringify(updatedData)
                }
            )

            if (!response.ok) {

                if (response.status === 401 || response.status === 403) {

                    clearAuthData()
                    router.push('/auth/login')
                    setError('Sessão expirada. Faça login novamente.')

                    return
                }

                throw new Error(`Erro ao atualizar perfil: ${response.status}`)
            }

            setOriginalData({
                ...profileData,
                senha: "••••••••"
            })

            console.log('Alterações salvas com sucesso!')
            setError(null)

        }

        catch (error) {

            console.error('Erro ao salvar alterações:', error)

            setError('Erro ao salvar alterações. Tente novamente.')
            handleDiscardChanges()
        }

        finally {
            setIsLoading(false)
        }
    }

    const handleDiscardChanges = () => {

        setProfileData(originalData)
        setEditingField(null)
        setError(null)
    }

    const handleRequestEmailChange = async (

        currentPassword: string,
        newEmail: string

    ): Promise<{ success: boolean; message: string }> => {

        setIsLoading(true)
        setError(null)

        try {
            const token = getAuthToken()

            if (!token) {

                return { success: false, message: 'Token de autenticação não encontrado' }
            }

            const userId = await fetchUserId(token)

            if (!userId) {

                return { success: false, message: 'Não foi possível identificar o usuário' }
            }

            const response = await fetch(`${API_CONFIG.baseURL}${API_CONFIG.endpoints.requestEmailChange}`,
                {
                    method: 'POST',
                    headers: getAuthHeaders(token),
                    body: JSON.stringify({
                        userId: userId,
                        userPassword: currentPassword,
                        newEmail: newEmail
                    })
                }
            )

            if (!response.ok) {

                if (response.status === 401 || response.status === 403) {

                    clearAuthData()
                    router.push('/auth/login')
                    return { success: false, message: 'Sessão expirada' }

                }

                const errorData = await response.json().catch(() => null)

                if (response.status === 400) {
                    const errorMessage = errorData?.message || 'Erro ao solicitar mudança de email'

                    return { success: false, message: errorMessage }
                }

                throw new Error(`Erro ao solicitar mudança de email: ${response.status}`)
            }

            console.log('Solicitação de mudança de email enviada com sucesso!')

            return {
                success: true,
                message: 'Código de confirmação enviado para o novo email. Válido por 5 minutos.'
            }

        }

        catch (error) {

            console.error('Erro ao solicitar mudança de email:', error)

            return {

                success: false,
                message: 'Erro ao solicitar mudança de email. Tente novamente.'
            }
        }

        finally {
            setIsLoading(false)
        }
    }

    const handleConfirmEmailChange = async (

        confirmationCode: string

    ): Promise<{ success: boolean; message: string }> => {

        setIsLoading(true)
        setError(null)

        try {

            const token = getAuthToken()

            if (!token) {

                return { success: false, message: 'Token de autenticação não encontrado' }
            }

            const userId = await fetchUserId(token)

            if (!userId) {

                return { success: false, message: 'Não foi possível identificar o usuário' }
            }

            const response = await fetch(`${API_CONFIG.baseURL}${API_CONFIG.endpoints.confirmEmailChange}`,
                {
                    method: 'POST',
                    headers: getAuthHeaders(token),
                    body: JSON.stringify({
                        userId: userId,
                        emailCode: confirmationCode
                    })
                }
            )

            if (!response.ok) {

                if (response.status === 401 || response.status === 403) {

                    clearAuthData()
                    router.push('/auth/login')
                    return { success: false, message: 'Sessão expirada' }
                }

                const errorData = await response.json().catch(() => null)

                if (response.status === 400) {

                    const errorMessage = errorData?.message || 'Código inválido ou expirado'
                    return { success: false, message: errorMessage }
                }

                throw new Error(`Erro ao confirmar mudança de email: ${response.status}`)
            }

            const userData = await fetchUserData(userId, token)

            if (userData) {

                const transformedData = transformApiDataToProfile(userData)

                setProfileData(transformedData)
                setOriginalData(transformedData)
            }

            console.log('Email alterado com sucesso!')

            return {

                success: true,
                message: 'Email alterado com sucesso!'
            }

        }

        catch (error) {

            console.error('Erro ao confirmar mudança de email:', error)

            return {
                success: false,
                message: 'Erro ao confirmar mudança de email. Tente novamente.'
            }
        }

        finally {
            setIsLoading(false)
        }
    }


    const handleRequestPasswordChange = async (

        currentPassword: string,
        newPassword: string

    ): Promise<{ success: boolean; message: string }> => {

        setIsLoading(true)
        setError(null)

        try {

            const token = getAuthToken()

            if (!token) {
                return { success: false, message: 'Token de autenticação não encontrado' }
            }

            const userId = await fetchUserId(token)

            if (!userId) {
                return { success: false, message: 'Não foi possível identificar o usuário' }
            }

            const response = await fetch(`${API_CONFIG.baseURL}${API_CONFIG.endpoints.requestPasswordChange}`,
                {
                    method: 'POST',
                    headers: getAuthHeaders(token),
                    body: JSON.stringify({
                        userId: userId,
                        currentPassword: currentPassword,
                        newPassword: newPassword
                    })
                }
            )

            if (!response.ok) {

                if (response.status === 401 || response.status === 403) {

                    clearAuthData()
                    router.push('/auth/login')
                    return { success: false, message: 'Sessão expirada' }
                }

                const errorData = await response.json().catch(() => null)

                if (response.status === 400) {

                    const errorMessage = errorData?.message || 'Erro ao solicitar mudança de senha'

                    return { success: false, message: errorMessage }
                }

                throw new Error(`Erro ao solicitar mudança de senha: ${response.status}`)
            }

            console.log('Solicitação de mudança de senha enviada com sucesso!')

            return {

                success: true,
                message: 'Código de confirmação enviado para seu email. Válido por 5 minutos.'
            }

        }

        catch (error) {

            console.error('Erro ao solicitar mudança de senha:', error)

            return {
                success: false,
                message: 'Erro ao solicitar mudança de senha. Tente novamente.'
            }
        }

        finally {
            setIsLoading(false)
        }
    }

    const handleConfirmPasswordChange = async (

        confirmationCode: string

    ): Promise<{ success: boolean; message: string }> => {

        setIsLoading(true)
        setError(null)

        try {

            const token = getAuthToken()

            if (!token) {
                return { success: false, message: 'Token de autenticação não encontrado' }
            }

            const userId = await fetchUserId(token)

            if (!userId) {
                return { success: false, message: 'Não foi possível identificar o usuário' }
            }

            const response = await fetch(`${API_CONFIG.baseURL}${API_CONFIG.endpoints.confirmPasswordChange}`,
                {
                    method: 'POST',
                    headers: getAuthHeaders(token),
                    body: JSON.stringify({
                        userId: userId,
                        passwordCode: confirmationCode
                    })
                }
            )

            if (!response.ok) {

                if (response.status === 401 || response.status === 403) {

                    clearAuthData()
                    router.push('/auth/login')
                    return { success: false, message: 'Sessão expirada' }
                }

                const errorData = await response.json().catch(() => null)

                if (response.status === 400) {

                    const errorMessage = errorData?.message || 'Código inválido ou expirado'
                    return { success: false, message: errorMessage }
                }

                throw new Error(`Erro ao confirmar mudança de senha: ${response.status}`)
            }

            console.log('Senha alterada com sucesso!')

            setTimeout(() => {

                clearAuthData()
                router.push('/auth/login')
            }, 2000)

            return {
                success: true,
                message: 'Senha alterada com sucesso! Redirecionando para login...'
            }

        }

        catch (error) {

            console.error('Erro ao confirmar mudança de senha:', error)

            return {
                success: false,
                message: 'Erro ao confirmar mudança de senha. Tente novamente.'
            }

        }

        finally {
            setIsLoading(false)
        }
    }

    const handleUpdatePassword = async (newPassword: string): Promise<boolean> => {

        console.warn('handleUpdatePassword está deprecated. Use handleRequestPasswordChange')

        return false
    }

    return {
        pendingValidations,
        dashboardData,
        profileData,
        originalData,
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
        handleUpdatePassword,
        inputRef
    }
}
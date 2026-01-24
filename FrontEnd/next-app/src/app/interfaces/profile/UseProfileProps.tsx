import { RefObject } from "react"
import { UserDashboardData } from '@/app/interfaces/entities/Events'

export interface ProfileData {
    nomeCompleto: string
    matricula: string
    telefone: string
    email: string
    senha: string
}

export interface UseProfileProps {
    profileData: ProfileData
    originalData: ProfileData
    dashboardData: UserDashboardData
    editingField: string | null
    showEmailModal: boolean
    showPasswordModal: boolean
    hasChanges: boolean
    isLoading: boolean
    error: string | null
    handleEdit: (fieldName: keyof ProfileData) => void
    handleSave: (fieldName: keyof ProfileData, value: string) => void
    handleCancel: () => void
    handleOpenEmailModal: () => void
    handleCloseEmailModal: () => void
    handleOpenPasswordModal: () => void
    handleClosePasswordModal: () => void
    handleSaveAllChanges: () => Promise<void>
    handleDiscardChanges: () => void
    handleRequestEmailChange: (currentPassword: string, newEmail: string) => Promise<{ success: boolean; message: string }>
    handleConfirmEmailChange: (confirmationCode: string) => Promise<{ success: boolean; message: string }>
    handleRequestPasswordChange: (currentPassword: string, newPassword: string) => Promise<{ success: boolean; message: string }>
    handleConfirmPasswordChange: (confirmationCode: string) => Promise<{ success: boolean; message: string }>
    handleUpdatePassword: (newPassword: string) => Promise<boolean>
    inputRef: RefObject<HTMLInputElement | null>
}
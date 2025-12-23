import headerStyles from '@/app/components/Styles/home/headerSection.module.css'
import ArrowIcon from '@mui/icons-material/ArrowForward'
import { useRouter } from 'next/navigation'

export default function ShowHeaderSection(){

    const router = useRouter()

    const handleViewBookPage = () => {
        router.push('/pages/books')
    }
    
    return(
        <div className={headerStyles.headerContainer}>
            <div className={headerStyles.welcomeSection}>
                
                <h1 className={headerStyles.welcomeTitle}>
                    Seja Bem-Vindo a Biblioteca
                </h1>

                <p className={headerStyles.welcomeDescription}>
                    Uma forma mais eficiente de lidar com o nosso acervo de livros!!
                </p>

                <div 
                    className={`${headerStyles.IconWrapper}`}
                    onClick={handleViewBookPage}>
                    Conferir Catálogo
                    <ArrowIcon className={headerStyles.linkIcon} sx={{fontSize: 18}}/>
                </div>

            </div>
        </div>
    )
}
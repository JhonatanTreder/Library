import headerStyles from '@/app/components/Styles/home/headerSection.module.css'
import ArrowIcon from '@mui/icons-material/ArrowForward'

export default function ShowHeaderSection(){
    return(
        <div className={headerStyles.headerContainer}>
            <div className={headerStyles.welcomeSection}>
                
                <h1 className={headerStyles.welcomeTitle}>
                    Seja Bem-Vindo a Biblioteca
                </h1>

                <p className={headerStyles.welcomeDescription}>
                    Uma forma mais eficiente de lidar com o nosso acervo de livros!!
                </p>

                <div className={`${headerStyles.IconWrapper}`}>
                    Conferir Catálogo
                    <ArrowIcon className={headerStyles.linkIcon} sx={{fontSize: 18}}/>
                </div>

            </div>
        </div>
    )
}
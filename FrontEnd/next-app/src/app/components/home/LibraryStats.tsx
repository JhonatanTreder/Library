import libraryStatsStyles from '@/app/components/Styles/home/libraryStats.module.css'

//Icons
import UserIcon from '@mui/icons-material/Person'
import BookIcon from '@mui/icons-material/AutoStories'
import BorrowedBooks from '@mui/icons-material/AssignmentReturn'
import DelayedLoans from '@mui/icons-material/AssignmentLate'
import EventsIcon from '@mui/icons-material/Event'
import ArrowIcon from '@mui/icons-material/ArrowForward'

export default function ShowLibraryStats(){
    return(
        <div className={libraryStatsStyles.libraryContainer}>
            <h1 className={libraryStatsStyles.containerTitle}>
                Estatísticas da Biblioteca
            </h1>

            <section className={libraryStatsStyles.statsSection}>
                
                <div className={libraryStatsStyles.statsCard}>

                    <UserIcon className={libraryStatsStyles.cardIcon} sx={{fontSize: 40}}/>
                    <h2 className={libraryStatsStyles.cardTitle}>Usuários Ativos</h2>
                    <p className={libraryStatsStyles.cardValue}>157</p>

                    <div className={`${libraryStatsStyles.IconWrapper}`}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{fontSize: 18}}/>
                    </div>
                </div>

                <div className={libraryStatsStyles.statsCard}>

                    <BookIcon className={libraryStatsStyles.cardIcon} sx={{fontSize: 40}}/>
                    <h2 className={libraryStatsStyles.cardTitle}>Total de Livros</h2>
                    <p className={libraryStatsStyles.cardValue}>19.579</p>

                    <div className={`${libraryStatsStyles.IconWrapper}`}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{fontSize: 18}}/>
                    </div>
                </div>

                <div className={libraryStatsStyles.statsCard}>

                    <BorrowedBooks className={libraryStatsStyles.cardIcon} sx={{fontSize: 40}}/>
                    <h2 className={libraryStatsStyles.cardTitle}>Livros Emprestados</h2>
                    <p className={libraryStatsStyles.cardValue}>312</p>

                    <div className={`${libraryStatsStyles.IconWrapper}`}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{fontSize: 18}}/>
                    </div>
                </div>

                <div className={libraryStatsStyles.statsCard}>

                    <DelayedLoans className={libraryStatsStyles.cardIcon} sx={{fontSize: 40}}/>
                    <h2 className={libraryStatsStyles.cardTitle}>Empréstimos Atrasados</h2>
                    <p className={libraryStatsStyles.cardValue}>24</p>

                    <div className={`${libraryStatsStyles.IconWrapper}`}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{fontSize: 18}}/>
                    </div>
                </div>

                <div className={libraryStatsStyles.statsCard}>

                    <EventsIcon className={libraryStatsStyles.cardIcon} sx={{fontSize: 40}}/>
                    <h2 className={libraryStatsStyles.cardTitle}>Eventos Ativos</h2>
                    <p className={libraryStatsStyles.cardValue}>3</p>

                    <div className={`${libraryStatsStyles.IconWrapper}`}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{fontSize: 18}}/>
                    </div>
                </div>

            </section>
        </div>
    )
}

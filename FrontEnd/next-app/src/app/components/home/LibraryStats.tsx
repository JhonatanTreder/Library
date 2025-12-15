import libraryStatsStyles from '@/app/components/Styles/home/libraryStats.module.css'

//Icons
import NewBooks from '@mui/icons-material/LibraryAdd'
import BookIcon from '@mui/icons-material/AutoStories'
import UnavailableBooks from '@mui/icons-material/LayersClear'
import BorrowedBooks from '@mui/icons-material/AssignmentReturn'
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

                    <NewBooks className={libraryStatsStyles.cardIcon} sx={{fontSize: 40}}/>
                    <h2 className={libraryStatsStyles.cardTitle}>Novos Livros</h2>
                    <p className={libraryStatsStyles.cardValue}>30</p>

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

                    <UnavailableBooks className={libraryStatsStyles.cardIcon} sx={{fontSize: 40}}/>
                    <h2 className={libraryStatsStyles.cardTitle}>Livros Indisponíveis</h2>
                    <p className={libraryStatsStyles.cardValue}>5</p>

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

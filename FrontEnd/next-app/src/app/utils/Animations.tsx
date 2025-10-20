import animationStyles from '@/app/utils/styles/animations.module.css'

export default function useAnimateOnScroll(elementClass: string, animationType: string){

    if (typeof window === "undefined") {
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if(entry.isIntersecting){
                entry.target.classList.remove(`${animationStyles.hidden}`)
                entry.target.classList.add(animationType)
            }
        })
    }, {})

    const elements = document.querySelectorAll(elementClass);

    elements.forEach(element => observer.observe(element));
}
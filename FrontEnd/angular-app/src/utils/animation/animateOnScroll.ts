import { isPlatformBrowser } from "@angular/common";
import {
    OnDestroy,
    OnInit,
    Directive,
    ElementRef,
    Renderer2,
    Input,
    inject,
    PLATFORM_ID,
} from "@angular/core";

@Directive({
    selector: '[appAnimateOnScroll]',
    standalone: true,
})
export class AnimateOnScrollDirective implements OnInit, OnDestroy {
    private platformId = inject(PLATFORM_ID);
    private elementRef = inject(ElementRef);
    private renderer = inject(Renderer2);
    private observer!: IntersectionObserver;

    @Input() animationClass: string = '';
    @Input() hiddenClass: string = 'hidden';

    ngOnInit(): void {
        
        if (!isPlatformBrowser(this.platformId)) return;

        this.observer = new IntersectionObserver(([entry]) => {
            if (entry.isIntersecting) {
                this.renderer.removeClass(this.elementRef.nativeElement, this.hiddenClass);
                this.renderer.addClass(this.elementRef.nativeElement, this.animationClass);

                this.observer.unobserve(this.elementRef.nativeElement);
            }
        });

        this.observer.observe(this.elementRef.nativeElement);
    }

    ngOnDestroy(): void {
        if (!isPlatformBrowser(this.platformId)) return;

        this.observer.disconnect();
    }
}
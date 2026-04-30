import { filter } from 'rxjs/operators';
import { Router, NavigationEnd } from '@angular/router';
import { Component, OnInit, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-navbar',
  imports: [MatIconModule],
  standalone: true,
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})

export class NavbarComponent implements OnInit {
  private router = inject(Router);
  private currentPage: string = '';

  ngOnInit(): void {
  this.router.events
    .pipe(filter(event => event instanceof NavigationEnd))
    .subscribe((event: NavigationEnd) => {
      this.currentPage = event.urlAfterRedirects;

      console.log('Current page:', this.currentPage);
    }
  );
}

  activeItem: string = '';

  viewHomePage() {
    this.activeItem = 'home';
    this.router.navigate(['/home']);
  }

  viewBooksPage() {
    this.activeItem = 'books';
    this.router.navigate(['/books']);
  }

  viewEventsPage() {
    this.activeItem = 'events';
    this.router.navigate(['/events']);
  }

  viewLoansPage() {
    this.activeItem = 'loans';
    this.router.navigate(['/loans']);
  }

  viewFavoritesPage() {
    this.activeItem = 'favorites';
    this.router.navigate(['/favorites']);
  }

  viewProfilePage() {
    this.activeItem = 'profile';
    this.router.navigate(['/profile']);
  }

  viewNotificationsPage() {
    this.activeItem = 'notifications';
    this.router.navigate(['/notifications']);
  }
}

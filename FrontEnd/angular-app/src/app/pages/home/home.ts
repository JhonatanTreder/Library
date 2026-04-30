import { Component, OnInit, OnChanges, inject, SimpleChange, AfterViewInit, AfterViewChecked, ChangeDetectionStrategy } from '@angular/core';
import { NavbarComponent } from "../../components/navbar/navbar";
import { HeaderSection } from "../../components/home/header-section/header-section";
import { LibraryStats } from "../../components/home/library-stats/library-stats";
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [NavbarComponent, HeaderSection, LibraryStats, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class HomeComponent implements OnInit {
  form = {
    nome: '',
    email: '',
    senha: '',
    idade: null,
    genero: '',
    bio: ''
  };

  ngOnInit() {
    
  }

  onSubmit(tipo: string) {
    console.log(tipo); // 'A'
    console.log(this.form); // todos os dados do formulário 
  }

  verifyOnExit (value: string) {
    console.log(value);
  }
}

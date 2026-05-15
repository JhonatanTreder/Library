import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from "../../components/navbar/navbar";
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [NavbarComponent, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class HomeComponent implements OnInit {

  ngOnInit(): void {

  }

}

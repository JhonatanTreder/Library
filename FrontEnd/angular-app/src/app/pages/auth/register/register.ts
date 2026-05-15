import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-register',
  imports: [],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register implements OnInit {
  ngOnInit(): void {
    this.loadForm();
  }

  loadForm() {
    
  }
}

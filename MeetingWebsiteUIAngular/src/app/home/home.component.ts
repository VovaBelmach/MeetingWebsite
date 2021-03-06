import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../shared/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: []
})
export class HomeComponent implements OnInit {

  constructor(private router: Router, private service: UserService) { }

   ngOnInit() {  }

   onProfile() {
    this.router.navigate(['/home/profile']);
  }

  onLogout() {
    localStorage.removeItem('token');
    this.router.navigate(['/user/login']);
  }

  onOpenSearch(){
    this.router.navigate(['/home/search']);
  }
}

import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { error } from '@angular/compiler/src/util';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.model.username = '';
      this.model.password = '';
      console.log('Login in successfully');
    }, error => {
      console.log(error);
    });
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logOut() {
    const token = localStorage.removeItem('token');
    console.log('Loged Out');
  }

}

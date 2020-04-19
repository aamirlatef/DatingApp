import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { error } from '@angular/compiler/src/util';
import { AlertifyServiceService } from '../_services/AlertifyService.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(public authService: AuthService, private alertify: AlertifyServiceService,
              private router: Router) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.model.username = '';
      this.model.password = '';
      this.alertify.success('Login Successfully');
    // tslint:disable-next-line: no-shadowed-variable
    }, error => {
      this.alertify.error(error);
    }, () => { // complete handler
      this.router.navigate(['/members']);
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logOut() {
    const token = localStorage.removeItem('token');
    this.alertify.success('Loged Out Successfully');
    this.router.navigate(['/home']);
  }

}

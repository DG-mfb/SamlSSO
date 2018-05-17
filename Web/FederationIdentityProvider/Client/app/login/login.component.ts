import { Component, OnInit, Inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AlertService, AuthenticationService } from '../_services/index';
import { UserService } from "../_services/user.service";
import { User } from "../_models/user";
import { DOCUMENT } from '@angular/platform-browser';

@Component({
    moduleId: module.id,
    templateUrl: 'login.component.html'
})

export class LoginComponent implements OnInit {
    model: any = {};
    loading = false;
	returnUrl: string;
	state: string;

	constructor( @Inject(DOCUMENT) private document: any,
		private route: ActivatedRoute,
		private userService: UserService,
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService) { }

    ngOnInit() {
        // reset login status
		this.authenticationService.logout();
		//var testUser = new User();
		//testUser.username = "d@domain.com";
		//testUser.password = "111";
		//testUser.firstName = "John";
		//testUser.lastName = "Dow";
		//this.userService.create(testUser)
        // get return url from route parameters or default to '/'
		this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
		this.state = this.route.snapshot.queryParams['state'] || '/';
    }

    login() {
        this.loading = true;
		this.authenticationService.login(this.model.username, this.model.password)
			.subscribe(
			data => {
				this.authenticationService.ssologin(data.access_token, this.returnUrl, this.state)
					.subscribe(
					d => {
						//this.document.location.href = 'http://localhost:60879/api/Account/SSOLogon';
						this.document.location.href = this.returnUrl + "?state=" + this.state;
						//this.router.navigate(['this.returnUrl']);
					})
				//this.router.navigate([this.returnUrl]);
			},
			error => {
				this.alertService.error(error);
				this.loading = false;
			});
    }
}

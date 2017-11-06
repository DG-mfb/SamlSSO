import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { AuthenticationService } from '../services/AuthenticationService';

@Component({
    moduleId: module.id,
    selector: 'login',
	templateUrl: 'login.component.html',
	providers: [AuthenticationService],
    styleUrls: ['login.component.css']
})
export class LoginComponent implements OnInit
{
    public showWarningMessage = false;
    public showErrorMessage = false;
	loading = false;
	model: any = {};
	returnUrl: string;
	constructor(
		private route: ActivatedRoute,
        private router: Router,
		private authenticationService: AuthenticationService)
	{
    }
	ngOnInit(): void {
		this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
	}

    login(): void {

        this.clearMessages();

		var req = this.authenticationService.login(this.model.username, this.model.password);
		req.subscribe(
			data =>
			{
				this.router.navigate([this.returnUrl]);
			},
			error => {
				//this.alertService.error(error);
				this.loading = false;
			});
        //this.userService.currentUser$
        //    .subscribe(user => {
        //        if (user != null) {
        //            this.clearMessages();
        //            this.router.navigate(['/']);   
        //        }
        //        else {
        //            this.showErrorMessage = false;
        //            this.showWarningMessage = true;
        //        }                    
        //    },
        //    error => {
        //        this.showErrorMessage = true;
        //        this.showWarningMessage = false;
        //    });
    }

    private clearMessages(): void {
        this.showWarningMessage = false;
        this.showErrorMessage = false;
    }
}
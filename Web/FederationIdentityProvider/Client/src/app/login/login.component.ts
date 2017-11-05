import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { AuthenticationService } from '../services/AuthenticationService';

@Component({
    moduleId: module.id,
    selector: 'login',
	templateUrl: 'login.component.html',
	providers: [AuthenticationService],
    //styleUrls: ['login.component.scss']
})
export class LoginComponent {

    public showWarningMessage = false;
    public showErrorMessage = false;

    email:string;
    password: string;

    constructor(
        private router: Router,
		private authenticationService: AuthenticationService)
	{
    }


    onSubmit(): void {

        this.clearMessages();

		this.authenticationService.login(this.email, this.password);

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
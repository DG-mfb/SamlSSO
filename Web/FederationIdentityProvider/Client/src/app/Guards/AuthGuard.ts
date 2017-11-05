import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, ActivatedRoute } from '@angular/router';

@Injectable()
export class AuthGuard implements CanActivate {

	constructor(private router: Router, private route: ActivatedRoute) { }

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
		//if (localStorage.getItem('currentUser')) {
		//	// logged in so return true
		//	return true;
		//}

		// not logged in so redirect to login page with the return url
		//var uri = state.queryParams['returnUrl'] || '/'
		//this.router.navigate(['/login'], { queryParams: { returnUrl: uri } });
		this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
		return false;
	}
}